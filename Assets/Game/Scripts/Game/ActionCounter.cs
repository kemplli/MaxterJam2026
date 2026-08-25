using UnityEngine;

public class ActionCounter : ManagedBehaviour
{
    public int counter;

    public int maxActions; 

    protected override void ManagedInitialize()
    {
        G.actions = this;

        G.events.onRelease += OnRelease;
        G.events.onLoadingLevel += ReloadCounter;
        G.events.onTurnEnd += ReloadCounter;
        G.events.onHealing += UpdateCounter;

        counter = maxActions;
    }

    private void ReloadCounter()
    {
        counter = maxActions;
    }

    private void OnRelease(int damage)
    {
        UpdateCounter();
    }

    private void UpdateCounter()
    {
        counter--;


        if (counter == 0)
        {
            G.events.actionsLeft?.Invoke();
        }
    }

    public override void ManagedUpdate()
    {
        G.ui.actionsLeft.text = "Осталось действий: " + counter;
    }
}
