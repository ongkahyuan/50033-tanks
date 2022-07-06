using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDescriptor", menuName = "ScriptableObjects/Level", order = 5)]
public class LevelDescriptor : ScriptableObject
{
    // #if UNITY_EDITOR
    //     [Multiline]
    //     public string DeveloperDescription = "";
    // #endif
    public float speed = 1f;
    private int counter = 0;
    public float attackRange = 1f;
    public float attackForce = 15f;
    public int concurrentTankNumber = 2;
    public int totalTankNumber = 2;
    public float speedIncrement = 1f;
    public float attackRangeIncrement = 0.5f;
    public float attackForceIncrement = 1f;
    public int concurrentTankNumberIncrement = 1;
    public int totalTankNumberIncrement = 1;
    public int maxConcurrentTankNumber = 20;
    public EnemyStats stats;

    public void advanceLevel()
    {
        counter += 1;
        if (counter == 1)
        {
            Debug.Log("Total tank count: " + totalTankNumber);
            totalTankNumber += totalTankNumberIncrement;
            Debug.Log("Total tank count set to " + totalTankNumber);
            if (speed < 12f)
            {
                Debug.Log("Speed: " + speed);
                speed += speedIncrement;
                stats.setSpeed(speed);
                Debug.Log("Speed set to " + speed);
            }
        }
        if (counter == 2)
        {
            if (speed < 12f)
            {
                Debug.Log("Speed: " + speed);
                speed += speedIncrement;
                stats.setSpeed(speed);
                Debug.Log("Speed set to " + speed);
            }
            if (concurrentTankNumber < maxConcurrentTankNumber)
            {
                Debug.Log("Concurrent Tank Number: " + concurrentTankNumber);
                concurrentTankNumber += concurrentTankNumberIncrement;
                Debug.Log("Concurrent Tank Number set to: " + concurrentTankNumber);
            }
            counter = 0;
        }
    }

    public void reset()
    {
        totalTankNumber = 1;
        concurrentTankNumber = 1;
        speed = 3.5f;
        stats.resetStats();
        counter = 0;
    }
}
