using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Score")]
public class Score : ScriptableObject
{

    public int highScore = 0;

    public bool updateScore(int score)
    // Save high score if exceeds previous high score
    {
        if (score > highScore)
        {
            highScore = score;
            return true;
        }
        return false;
    }

}