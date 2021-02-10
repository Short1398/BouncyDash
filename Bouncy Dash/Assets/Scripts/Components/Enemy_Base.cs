using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : MonoBehaviour
{
    protected enum EnemyTypes
    {
        TURRET, 
        GRUNT
    }

    [SerializeField]
    protected EnemyTypes m_enemyType;
    [Header("Damage")]
    [SerializeField]
    protected float damageDealt;

    protected const string PLAYER_TAG = "Player";
}
