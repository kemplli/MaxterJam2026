using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Linq;
using UnityEngineInternal;
using JetBrains.Annotations;

public class Main : MonoBehaviour
{
    private GameObject cardPrefab;

    private void Awake()
    {
        cardPrefab = Resources.Load<GameObject>("Prefabs/Card");
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

        if (Input.GetMouseButtonDown(1))
        {
            var inst = Instantiate(cardPrefab,Vector3.zero,Quaternion.identity);
            G.area.Claim(inst.GetComponent<CardViewInteractable>());
        }

      //  if (Input.GetMouseButtonDown(0))
        //{
          //  G.transition.TransitioN(() => { Debug.Log("FUCK"); });
        //}
    }

}
