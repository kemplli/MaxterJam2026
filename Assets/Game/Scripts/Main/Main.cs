using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

public class Main : MonoBehaviour
{
    private List<GameObject> cardPrefabs = new List<GameObject>();

    private bool winned;

    private void Awake()
    {
        cardPrefabs = Resources.LoadAll<GameObject>("Prefabs/Cards").ToList();

        G.events.actionsLeft += GetDamageFromAllEnemies;
    }

    private int i = 0;
    private void Update()
    {
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

        if (G.levelManager.currentEnemyCount == 0)
        {
            if (G.levelManager.dynamicData.currentLevel == 0)
            {
                G.levelManager.LoadLevel(G.levelManager.dynamicData.currentLevel);
                return;
            }

            if (!winned)
            {
                Win();
                winned = true;
            }
        }
    }

    private void Win()
    {
        G.events.onWin.Invoke();
    }

    private IEnumerator GetDamageFromAllEnemiesCoroutine()
    {
        yield return new WaitForSeconds(1f);

        foreach (var enemy in G.levelManager.currentEnemies)
        {
            if(enemy == null) continue;

            yield return enemy.GetComponent<Enemy>().Attack();
        }

        EndTurn();
    }

    private void EndTurn()
    {
        G.events.onTurnEnd?.Invoke();
    }

    public void GetDamageFromAllEnemies()
    {
        StartCoroutine(GetDamageFromAllEnemiesCoroutine());
    }

    private void OnDestroy()
    {
        G.events.actionsLeft -= GetDamageFromAllEnemies;
    }
}
