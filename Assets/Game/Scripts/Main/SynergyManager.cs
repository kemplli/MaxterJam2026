using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SynergyManager : ManagedBehaviour
{
    public List<Synergy> synergies = new List<Synergy>();

    private AllSynergiesData data;

    protected override void ManagedInitialize()
    {
        G.synergy = this;

        data = G.s_data.GetAllSynergiesData();
        Debug.Log(data);
    }

    public IEnumerator CheckSynergies()
    {
        foreach (var synergy in synergies)
        {
            bool hasFirst = false; bool hasSecond = false;
            SynergyTypes type = SynergyTypes.None;
            foreach (var element in data.synergies)
            {
                if (synergy.first == element.first) hasFirst = true;
                if (synergy.last == element.last) { hasSecond = true; type = element.type; }
            }
            if (hasFirst && hasSecond)
            {
                yield return ExecuteSynergy(type);
            }
        }
    }

    private IEnumerator ExecuteSynergy(SynergyTypes type)
    {
        var sData = G.s_data.GetSynergyData(type);

        Debug.Log(sData.name);

        yield break;
    }
}
