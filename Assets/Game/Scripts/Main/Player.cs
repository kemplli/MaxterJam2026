using UnityEngine;
public class Player : ManagedBehaviour
{
    public PlayerDynamicData data;

    protected override void ManagedInitialize()
    {
        G.player = this;
        data = G.d_data.GetPlayerDynamicData();
    }

    public void TakeDamage(int damage)
    {
        data.health -= damage;
    }

    public void Heal(int healing)
    {
        data.health += healing;
    }

    public override void ManagedUpdate()
    {
        G.ui.healthSlider.value = (float)data.health / (float)data.startHealth;
        G.ui.healthText.text = data.health.ToString() + "/" + data.startHealth.ToString();

        data.health = Mathf.Clamp(data.health,0,15);
    }
}

