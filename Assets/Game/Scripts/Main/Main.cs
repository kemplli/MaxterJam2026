using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngineInternal;

public class Main : MonoBehaviour
{
    private GameObject prefab;

    private void Awake()
    {
        prefab = Resources.Load<GameObject>("Prefabs/A");
    }

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

        if (Input.GetMouseButtonDown(0))
        {
            G.transition.TransitioN(() => { Debug.Log("FUCK"); });
        }
    }

}
