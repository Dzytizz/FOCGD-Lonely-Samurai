using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoulData
{
    public bool soulExists;
    public int coinsCollected;
    public int level;
    public int stage;
    public float[] position;

    public SoulData(bool soulExists, int coinsCollected, int level, int stage, float[] position)
    {
        this.soulExists = soulExists;
        this.coinsCollected = coinsCollected;
        this.level = level;
        this.stage = stage;
        this.position = position;
    }
}
