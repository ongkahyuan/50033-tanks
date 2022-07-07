using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;



public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public float m_EndGameDelay = 9f;
    public CameraControl m_CameraControl;
    public Text m_MessageText;
    public GameObject[] spawnPoints;
    public GameObject[] m_TankPrefabs;
    public TankManager[] m_Tanks;
    public List<Transform> wayPointsForAI;
    // public int levelDescriptor.concurrentTankNumber;

    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private WaitForSeconds m_EndGameWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    public UnityEvent RoundEnd;
    private int m_KillNumber;
    private bool playerAlive = true;
    public bool GodMode = false;
    public LevelDescriptor levelDescriptor;
    private int remainingSpawns;
    public Score score;

    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);
        m_EndGameWait = new WaitForSeconds(m_EndGameDelay);
        // Initialise tank managers for maximum number of concurrent tanks
        m_Tanks = new TankManager[levelDescriptor.maxConcurrentTankNumber];
        for (int i = 0; i < levelDescriptor.maxConcurrentTankNumber; i++)
        {
            m_Tanks[i] = new TankManager();
        }

        StartCoroutine(GameLoop());
    }


    private void SpawnAllTanks()
    {
        remainingSpawns = levelDescriptor.totalTankNumber;
        // Set up player tank
        if (m_Tanks[0].m_Instance == null)
        {
            m_Tanks[0].m_Instance =
                Instantiate(m_TankPrefabs[0], spawnPoints[0].transform.position, spawnPoints[0].transform.rotation) as GameObject;
            m_Tanks[0].m_PlayerNumber = 1;
            m_Tanks[0].SetupPlayerTank();
            m_Tanks[0].SetSpawn(spawnPoints[0].transform);
        }

        spawnEnemies();
    }

    private void spawnEnemies()
    {
        // Sets up AI tank
        GameObject m_TankPrefab;
        for (int i = 1; i < levelDescriptor.concurrentTankNumber + 1; i++)
        {
            if (remainingSpawns % 2 == 0)
            {
                m_TankPrefab = m_TankPrefabs[1];
            }
            else
            {
                m_TankPrefab = m_TankPrefabs[2];
            }
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, spawnPoints[i].transform.position, spawnPoints[i].transform.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].SetType(m_TankPrefab.tag);
            m_Tanks[i].SetupAI(wayPointsForAI);
            m_Tanks[i].SetSpawn(spawnPoints[i].transform);
            remainingSpawns -= 1;
        }
    }
    private void respawnEnemy()
    {
        // Finds a single inactive tank that has been initialised, and reactivates it
        foreach (TankManager m_Tank in m_Tanks)
        {
            if (!m_Tank.m_Instance.activeInHierarchy & m_Tank.m_SpawnPoint != null)
            {
                m_Tank.Reset();
                remainingSpawns -= 1;
                Debug.Log("Respawned Tank");
                break;
            }
        }
    }


    private void SetCameraTargets()
    {
        // Dynamically set the number of targets depending on number of concurrent tanks
        Transform[] targets = new Transform[levelDescriptor.concurrentTankNumber + 1];

        for (int i = 0; i < targets.Length; i++)
            targets[i] = m_Tanks[i].m_Instance.transform;

        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {
        // In each loop, spawn required number of tanks, set targets, start game
        SpawnAllTanks();
        SetCameraTargets();
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        // Game continues as long as player is still alive
        if (!playerAlive) SceneManager.LoadScene(0);
        else StartCoroutine(GameLoop());
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = $"ROUND {m_RoundNumber}";

        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();

        m_MessageText.text = string.Empty;

        while (!OneTankLeft() & playerAlive) yield return null;
    }


    private IEnumerator RoundEnding()
    {
        RoundEnd.Invoke();
        DisableTankControl();
        if (playerAlive)
        {
            string message = EndMessage();
            m_MessageText.text = message;
            levelDescriptor.advanceLevel();

            yield return m_EndWait;
        }
        else
        {
            // Display different message if game ends
            string message = EndGameMessage();
            m_MessageText.text = message;

            yield return m_EndGameWait;
        }
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;
        for (int i = 0; i < levelDescriptor.concurrentTankNumber + 1; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf) numTanksLeft++;
        }

        return numTanksLeft <= 1 & remainingSpawns == 0;
    }

    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }

    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        var sb = new StringBuilder();

        sb.Append($"ROUND {m_RoundNumber} OVER!");
        sb.Append("\n\n");
        sb.Append($"Total kills: {m_KillNumber}");

        return sb.ToString();
    }

    private string EndGameMessage()
    {
        // Updates score scriptable object if new high score is reached
        bool newHighScore = score.updateScore(m_KillNumber);
        int highScore = score.highScore;

        // Special message if high score reached
        if (newHighScore)
        {
            var sb = new StringBuilder();

            sb.Append($"GAME OVER!");
            sb.Append("\n");
            sb.Append($"Total rounds: {m_RoundNumber}\n");
            sb.Append($"Total kills: {m_KillNumber}\n");
            sb.Append($"NEW HIGH SCORE!");
            return sb.ToString();
        }
        else
        {
            var sb = new StringBuilder();

            sb.Append($"GAME OVER!");
            sb.Append("\n");
            sb.Append($"Total rounds: {m_RoundNumber}\n");
            sb.Append($"Total kills: {m_KillNumber}\n");
            sb.Append($"High Score: {highScore}");
            return sb.ToString();

        }
    }

    private void ResetAllTanks()
    {
        for (int i = 0; i < levelDescriptor.concurrentTankNumber + 1; i++) m_Tanks[i].Reset();
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < levelDescriptor.concurrentTankNumber + 1; i++) m_Tanks[i].EnableControl();
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < levelDescriptor.concurrentTankNumber + 1; i++) m_Tanks[i].DisableControl();
    }

    public void addKill()
    // Triggered by EnemyDeath event listener
    {
        // Updating the kill score
        m_KillNumber += 1;
        // Respawn enemies if there are still spawns available
        if (remainingSpawns > 0)
        {
            respawnEnemy();
        }
    }

    public void KillPlayer()
    // Triggered by PlayerDeath event listener
    {
        playerAlive = false;
        // Resets the level increments
        levelDescriptor.reset();
    }

    private void OnApplicationQuit()
    {
        // Resets the level increments
        levelDescriptor.reset();
    }
}