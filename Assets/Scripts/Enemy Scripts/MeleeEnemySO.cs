using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Additional variables for melee enemies
[CreateAssetMenu(menuName = "Enemies/Melee")]
public class MeleeEnemySO : EnemySO
{
    public int meleeDamage;
    public float dashDistance;
    public bool isFollowing;
}
