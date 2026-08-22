using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : ManagedBehaviour
{
    public List<Level> levels = new List<Level>();

    private List<GameObject> currentEnemies = new List<GameObject>();

    protected override void ManagedInitialize()
    {
        LoadLevel(levels.First());
    }

    public void LoadLevel(Level level)
    {
        currentEnemies.Clear();
        for (int i = 0; i < level.enemies.Count; i++)
        {
            var enemy = level.enemies[i];
            var pos = level.enemiesPositions[i];
            pos = new Vector2(pos.x + 10, pos.y);

            var inst = Instantiate(enemy, pos, Quaternion.identity);
            currentEnemies.Add(inst);
        }
        G.lever.FullWithEnemies(currentEnemies);
        G.lever.FullWithDestinations(level.enemiesPositions);
    }
}

[Serializable]
public class Level
{
    public List<GameObject> enemies = new List<GameObject>();
    public List<Vector2> enemiesPositions = new List<Vector2>();
}