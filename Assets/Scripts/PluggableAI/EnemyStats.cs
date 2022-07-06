using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/EnemyStats")]
public class EnemyStats : ScriptableObject
{

    public float moveSpeed = 1;
    public float lookRange = 40f;
    public float lookSphereCastRadius = 1f;

    public float attackRange = 1f;
    public float attackRate = 1f;
    public float attackForce = 15f;
    public int attackDamage = 50;

    public float searchDuration = 4f;
    public float searchingTurnSpeed = 120f;

    public void increaseSpeed(float increment)
    {
        moveSpeed += increment;
    }
    public void setSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void increaseAttackRange(float incrementRange, float incrementForce)
    {
        attackRange += incrementRange;
        attackForce += incrementForce;
    }

    public void resetStats()
    {
        moveSpeed = 3.5f;
        lookRange = 40f;
    }
}