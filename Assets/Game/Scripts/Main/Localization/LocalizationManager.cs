using System;
using UnityEngine;

public class LocalizationManager
{
    public Action<LocalizationType> onLocalization;

    public LocalizationType currentType = LocalizationType.English;

    public void Localize(LocalizationType type)
    {
        if (currentType != type)
        {
            onLocalization?.Invoke(type);
            currentType = type;
        }
    }
}

public enum LocalizationType
{
    English,
    Russian
}
