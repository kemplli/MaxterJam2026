using System;

public class Events
{
    public Action<int> onRelease;
    public Action onWin;
    public Action onLoadingLevel;
    public Action actionsLeft;
    public Action onTurnEnd;
}
