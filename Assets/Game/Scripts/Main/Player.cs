using DG.Tweening;
using System;
using UnityEditor.Embree;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Player : ManagedBehaviour
{
    public PlayerDynamicData data;

    protected override void ManagedInitialize()
    {
        G.player = this;
        data = G.d_data.GetPlayerDynamicData();

        G.events.onTurnEnd += TakePoisonDamage;
    }

    private int turnsWithPoison;
    private void TakePoisonDamage()
    {
        if (turnsWithPoison != 0)
        {
            data.health -= data.poison;
            G.ui.SpawnFloatingMessage("-1 Здоровье (Яд)", G.darkGreen);
            G.gVolCon.VignetteFlash(0f, 0.26f, G.darkGreen);
            G.feel.ShakeLight();
            data.poison = 0;
            turnsWithPoison = 0;
            return;
        }
        turnsWithPoison++;
    }

    public void TakeDamage(int damage)
    {
        data.health -= damage;
        if(data.health < 0) data.health = 0;
    }

    public void Heal(int healing)
    {
        data.health += healing;
    }

    public void TakePoison(int poison)
    {
        data.poison += poison;
    }

    public override void ManagedUpdate()
    {
        G.ui.healthSlider.value = (float)data.health / (float)data.startHealth;
        G.ui.healthText.text = data.health.ToString() + "/" + data.startHealth.ToString();
        G.ui.poisonText.text = "Яд: " + data.poison.ToString();

        G.ui.poisonText.gameObject.SetActive(data.poison > 0);

        data.health = Mathf.Clamp(data.health,0,15);

        if(data.health == 0) Death();
    }

    public void Death()
    {
        G.events.onTurnEnd -= TakePoisonDamage;

        G.ui.defeatImage.gameObject.SetActive(true);
        G.ui.defeatImage.DOFade(0.7f, 1f);
        G.ui.WTo.DOFade(1, 1f);
        G.ui.defeat.DOFade(1, 1f);
    }
}
