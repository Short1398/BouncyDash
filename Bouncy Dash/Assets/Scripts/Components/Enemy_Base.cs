using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Base : MonoBehaviour
{
    protected enum EnemyTypes
    {
        TURRET, 
        GRUNT,
        FLYING
    }

    [SerializeField]
    protected EnemyTypes m_enemyType;
    [Header("Damage")]
    [SerializeField]
    protected float m_damageDealt;

    [Header("Mobility")]
    [SerializeField]
    protected float m_accTime;
    [SerializeField]
    protected float m_decTime;
    [SerializeField]
    protected float m_maxHorizontalSpeed;
    [SerializeField]
    protected float m_maxVerticalSpeed;

    //Movement tracking 
    protected float m_currentHorizontalSpeed;
    protected float m_currentVerticalSpeed;

    //Find player
    protected const string PLAYER_TAG = "Player";
}
