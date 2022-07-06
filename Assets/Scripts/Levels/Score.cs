using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Score")]
public class Score : ScriptableObject
{

    public int highScore = 0;

    public bool updateScore(int score)
    {
        if (score > highScore)
        {
            highScore = score;
            return true;
        }
        return false;
    }

}