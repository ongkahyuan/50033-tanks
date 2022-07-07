using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDescriptor", menuName = "ScriptableObjects/Level", order = 5)]
public class LevelDescriptor : ScriptableObject
{
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
    public int maxConcurrentTankNumber = 10;
    public float healthPackProbability = 0.5f;
    public EnemyStats stats;

    public void advanceLevel()
    {
        // Describes how levels increase
        counter += 1;
        if (counter == 1)
        {
            // Increase concurrent tank number and speed
            if (speed < 9f)
            {
                speed += speedIncrement;
                stats.setSpeed(speed);
            }
            if (concurrentTankNumber < maxConcurrentTankNumber) concurrentTankNumber += concurrentTankNumberIncrement;
            if (concurrentTankNumber > totalTankNumber) totalTankNumber = concurrentTankNumber;
        }
        if (counter == 2)
        {
            // Increase total tank number and speed
            totalTankNumber += totalTankNumberIncrement;
            if (speed < 12f)
            {
                speed += speedIncrement;
                stats.setSpeed(speed);
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
