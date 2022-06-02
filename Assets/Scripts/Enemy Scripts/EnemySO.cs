using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Core of each enemy
public class EnemySO : ScriptableObject
{
    public GameObject model;
    public int health;
    [Tooltip("Times per second")] public float attackSpeed;
    public float movementSpeed;
    public int coins;
}
