using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    private List<GameObject> cardPrefabs = new List<GameObject>();

    public bool won;

    public bool enemyTurn;

    private int sceneIndex;

    private void Awake()
    {
        cardPrefabs = Resources.LoadAll<GameObject>("Prefabs/Cards").ToList();

        Debug.Log("AWAKE");

        G.events.actionsLeft += GetDamageFromAllEnemies;

        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.activeSceneChanged += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, Scene arg1)
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
        won = false;
    }

    private int i = 0;
    private void Update()
    {
        if (sceneIndex == 0 || sceneIndex == 3 || sceneIndex == 4) return;

#if UNITY_EDITOR        

        if (Input.GetKeyDown(KeyCode.J))
        {
            G.localization.Localize(G.localization.currentType == LocalizationType.English ? LocalizationType.Russian : LocalizationType.English);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            G.SetPaused(!G.IsPaused);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (i == 4) i = 0;
            var inst = Instantiate(cardPrefabs[i],Vector3.zero,Quaternion.identity);
            G.area.Claim(inst.GetComponent<CardViewInteractable>());
            i++;
        }
#endif

        if (G.levelManager.currentEnemyCount == 0 && G.levelManager.deadBodies.Count == 0)
        {
            if (G.levelManager.dynamicData.currentLevel >= G.levelManager.levels.Count && !won)
            {
                StartCoroutine(Win());
                won = true;
                return;
            }
            bool manager = false;
            foreach (var a in G.levelManager.managerLevels)
            {
                if (G.levelManager.dynamicData.currentLevel == a)
                {
                    if (!won)
                    {
                        G.tooltip.Hide();
                        StartCoroutine(Win());
                        G.levelManager.dynamicData.currentLevel -= 1;
                        won = true;
                        return;
                    }
                }

            }
            G.levelManager.LoadLevel(G.levelManager.dynamicData.currentLevel);
            if (G.levelManager.dynamicData.currentLevel - 1 != 1)
            G.events.onTurnEnd?.Invoke();
        }
    }

    private IEnumerator Win()
    {
        yield return StartCoroutine(G.synergy.CheckSynergies());
        if (G.levelManager.dynamicData.currentLevel == G.levelManager.levels.Count)
        {
            G.transition.TransitioN(() => SceneManager.LoadScene(3));
            yield break;
        }
        G.events.onWin.Invoke();
    }

    private IEnumerator GetDamageFromAllEnemiesCoroutine()
    {
        enemyTurn = true;

        yield return new WaitForSeconds(1f);
        
        foreach (var enemy in G.levelManager.currentEnemies)
        {
            if(enemy == null) continue;

            yield return enemy.GetComponent<Enemy>().Attack();
        }

        enemyTurn = false;
        EndTurn();
    }

    private void EndTurn()
    {
        G.events.onTurnEnd?.Invoke();
    }

    public void GetDamageFromAllEnemies()
    {
        if (enemyTurn || G.levelManager.currentEnemyCount == 0) return;
        StartCoroutine(GetDamageFromAllEnemiesCoroutine());
    }
}
