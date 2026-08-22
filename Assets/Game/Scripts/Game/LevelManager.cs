using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class LevelManager : ManagedBehaviour
{
    public List<Level> levels = new List<Level>();

    private List<GameObject> currentEnemies = new List<GameObject>();

    public int currentEnemyCount;

    public int currentLevel;

    protected override void ManagedInitialize()
    {
        G.levelManager = this;
    }

    public void LoadLevel(int index)
    {
        G.lever.currentMovingProgress = 0;

        var level = levels[index];

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

        currentEnemyCount = currentEnemies.Count;

        currentLevel++;
    }
}

[Serializable]
public class Level
{
    public List<GameObject> enemies = new List<GameObject>();
    public List<Vector2> enemiesPositions = new List<Vector2>();
}