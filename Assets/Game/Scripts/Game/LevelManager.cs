using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class LevelManager : ManagedBehaviour
{
    public List<Level> levels = new List<Level>();

    [NonSerialized]
    public List<GameObject> currentEnemies = new List<GameObject>();

    public int currentEnemyCount;

    public LevelDynamicData dynamicData;

    protected override void ManagedInitialize()
    {
        G.levelManager = this;

        dynamicData = G.d_data.GetLevelDynamicData();
    }

    public void LoadLevel(int index)
    {
        G.events.onLoadingLevel?.Invoke();

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

        dynamicData.currentLevel++;
    }
}

[Serializable]
public class Level
{
    public List<GameObject> enemies = new List<GameObject>();
    public List<Vector2> enemiesPositions = new List<Vector2>();
}