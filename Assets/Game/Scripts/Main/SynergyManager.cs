using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class SynergyManager : ManagedBehaviour
{
    public List<Synergy> synergies = new List<Synergy>();

    private AllSynergiesData data;

    private List<Sack> sacks = new List<Sack>();

    protected override void ManagedInitialize()
    {
        G.synergy = this;

        data = G.s_data.GetAllSynergiesData();
        Debug.Log(data);
    }

    public IEnumerator CheckSynergies()
    {
        foreach (var currentSynergy in synergies)
        {
            if (currentSynergy.sack == null) continue;

            HashSet<Enemies> sackElements = new HashSet<Enemies>();
            if (currentSynergy.first != Enemies.None) sackElements.Add(currentSynergy.first);
            if (currentSynergy.second != Enemies.None) sackElements.Add(currentSynergy.second);
            if (currentSynergy.last != Enemies.None) sackElements.Add(currentSynergy.last);

            if (sackElements.Count < 2) continue;

            foreach (var recipe in data.synergies)
            {
                // Собираем элементы текущего рецепта из базы данных
                HashSet<Enemies> recipeElements = new HashSet<Enemies>();
                if (recipe.first != Enemies.None) recipeElements.Add(recipe.first);
                if (recipe.second != Enemies.None) recipeElements.Add(recipe.second);
                if (recipe.last != Enemies.None) recipeElements.Add(recipe.last);

                if (recipeElements.Count < 2) continue;

                if(!sacks.Contains(currentSynergy.sack))
                sacks.Add(currentSynergy.sack);
                if (recipeElements.IsSubsetOf(sackElements))
                {
                    currentSynergy.sack.synergyTypes = recipe.type;

                    //yield return ExecuteSynergy(recipe.type);

                    break;
                }
            }
        }
        yield return StartCoroutine(SackAnimations());
    }

    private IEnumerator ExecuteSynergy(SynergyTypes type)
    {
        var sData = G.s_data.GetSynergyData(type);

        Debug.Log(sData.name);

        yield break;
    }

    private IEnumerator SackAnimations()
    {
        Debug.Log(sacks.Count);
        foreach (var sack in sacks)
        {
            if (sack.synergyTypes == SynergyTypes.None)
            {
                sack.enabled = false;

                Sequence sackSeq = DOTween.Sequence();

                sackSeq.AppendInterval(0.5f);

                Vector3 startPos = sack.transform.position;
                Vector3 baseScale = sack.transform.localScale;

                sackSeq.Append(sack.transform.DOMoveY(startPos.y - 0.2f, 0.12f).SetEase(Ease.OutQuad))
                       .Append(sack.transform.DOMoveY(startPos.y + 3f, 0.5f).SetEase(Ease.OutCubic))
                       .Join(sack.transform.DOScale(new Vector3(baseScale.x * 0.6f, baseScale.y * 1.3f, baseScale.z), 0.12f).SetEase(Ease.OutQuad))
                       .Append(sack.transform.DOScale(baseScale, 0.25f).SetEase(Ease.InOutSine));

                sackSeq.InsertCallback(0.62f, () => sack.transform.GetChild(0).gameObject.SetActive(false));

                sackSeq.AppendInterval(0.05f)
                       .Append(sack.transform.DORotate(new Vector3(0, 0, -25f), 0.2f).SetEase(Ease.InOutSine))
                       .Append(sack.transform.DORotate(new Vector3(0, 0, 25f), 0.3f).SetEase(Ease.InOutSine))
                       .Append(sack.transform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.InOutSine));

                sackSeq.AppendInterval(0.1f)
                       .Append(sack.transform.DOMoveY(startPos.y, 0.17f).SetEase(Ease.InCubic))
                       .AppendCallback(() =>
                       {
                           G.ui.SpawnFloatingMessage("Без эффекта", Color.red);
                           G.feel.ShakeLight();
                           G.gVolCon.VignetteFlash(0f, 0.26f, Color.red);
                       })
                       .Append(sack.transform.DOScale(new Vector3(baseScale.x * 1.1f, baseScale.y * 0.9f, baseScale.z), 0.06f).SetEase(Ease.OutQuad))
                       .Append(sack.transform.DOScale(baseScale, 0.1f).SetEase(Ease.OutBack));

                yield return sackSeq.WaitForCompletion();
            }
            else
            {
                var data = G.s_data.GetSynergyData(sack.synergyTypes);

                sack.enabled = false;

                sack.transform.DOScale(sack.transform.localScale/2,1.5f).SetEase(Ease.OutQuad);

                yield return new WaitForSeconds(1.5f);

                sack.transform.DOScale(sack.transform.localScale * 10, 0.05f).OnComplete(() => Destroy(sack.gameObject));

                yield return new WaitForSeconds(0.025f);

                G.feel.ShakeLight();
                G.ui.SpawnFloatingMessage(data.name, Color.red);
            }
        }

        yield return new WaitForSeconds(3f);
        yield break;
    }
}
