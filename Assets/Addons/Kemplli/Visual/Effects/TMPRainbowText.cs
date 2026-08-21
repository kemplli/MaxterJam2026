using System;
using TMPro;
using UnityEngine;

[Serializable]
public class TMPRainbowText : IExtraEffect
{
    [NonSerialized]
    public float speed;

    [NonSerialized]
    public float waveSpacing;

    public string linkID { get; set; } = "rainbow";
    public TextEffects id { get; set; } = TextEffects.Rainbow;

    public void Update(float currentTime, TMP_TextInfo textInfo)
    {
        if (textInfo == null || textInfo.textComponent == null) return;

        for (int l = 0; l < textInfo.linkCount; l++)
        {
            TMP_LinkInfo linkInfo = textInfo.linkInfo[l];
            string currentLinkID = linkInfo.GetLinkID();

            bool isOurEffect = currentLinkID.Contains("rainbow_speed");


            if (isOurEffect)
            {
                float currentSpeed = TMPTagParser.GetFloat(currentLinkID, "rainbow_speed", this.speed);
                float currentSpacing = TMPTagParser.GetFloat(currentLinkID, "rainbow_waveSpacing", this.waveSpacing);

                int firstCharIndex = linkInfo.linkTextfirstCharacterIndex;
                int linkLength = linkInfo.linkTextLength;

                for (int i = firstCharIndex; i < firstCharIndex + linkLength; i++)
                {
                    if (i < 0 || i >= textInfo.characterCount) continue;

                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible) continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;
                    Color32[] destinationColors = textInfo.meshInfo[materialIndex].colors32;

                    float hue = (currentTime * currentSpeed + i * currentSpacing) % 1f;
                    Color32 rainbowColor = Color.HSVToRGB(hue, 1f, 1f);
                    rainbowColor.a = (byte)(textInfo.textComponent.alpha * 255);

                    destinationColors[vertexIndex + 0] = rainbowColor;
                    destinationColors[vertexIndex + 1] = rainbowColor;
                    destinationColors[vertexIndex + 2] = rainbowColor;
                    destinationColors[vertexIndex + 3] = rainbowColor;
                }
            }
        }
    }
}