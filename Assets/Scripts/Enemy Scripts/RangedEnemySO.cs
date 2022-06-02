using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Additional variables for ranged enemies
[CreateAssetMenu(menuName = "Enemies/Ranged")]
public class RangedEnemySO : EnemySO
{
    public bool isMovingRandomly;
    [Range(0,1)] public float movementChance;
    public Vector3 maxMovement;
    public GameObject projectilePrefab;
}

