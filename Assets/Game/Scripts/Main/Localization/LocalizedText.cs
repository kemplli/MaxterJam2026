using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField]
    private string russianText;

    private string englishText;

    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();

        englishText = text.text;
    }

    private void OnEnable()
    {
        G.localization.onLocalization += Localize;
    }

    private void OnDisable()
    {
        G.localization.onLocalization -= Localize;
    }

    private void Localize(LocalizationType type)
    {
        if (type == LocalizationType.Russian)
        {
            text.text = russianText;
        }
        else
        {
            text.text = englishText;
        }
    }
}