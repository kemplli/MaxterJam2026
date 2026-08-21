using System;
using TMPro;
using UnityEngine;

[Serializable]
public class ShakeEffect : IExtraEffect
{
    [NonSerialized] 
    public float speed;

    [NonSerialized]
    public float magnitude;

    public string linkID { get; set; } = "shake";
    public TextEffects id { get; set; } = TextEffects.Shaking;

    public void Update(float currentTime, TMP_TextInfo textInfo)
    {
        if (textInfo == null || textInfo.textComponent == null) return;

        string fullRawText = textInfo.textComponent.text;

        for (int l = 0; l < textInfo.linkCount; l++)
        {
            TMP_LinkInfo linkInfo = textInfo.linkInfo[l];
            string currentLinkID = linkInfo.GetLinkID();

            bool isOurEffect = currentLinkID.Contains(linkID);

            if (isOurEffect)
            {
                float currentSpeed = TMPTagParser.GetFloat(currentLinkID, "shake_speed", this.speed);
                float currentMagnitude = TMPTagParser.GetFloat(currentLinkID, "shake_magnitude", this.magnitude);

                float currentStretch = TMPTagParser.GetFloat(currentLinkID, "shake_stretch", 1.5f);

                int firstCharIndex = linkInfo.linkTextfirstCharacterIndex;
                int linkLength = linkInfo.linkTextLength;

                for (int i = firstCharIndex; i < firstCharIndex + linkLength; i++)
                {
                    if (i < 0 || i >= textInfo.characterCount) continue;

                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible) continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                    float globalSeedX = (currentTime * currentSpeed) + (i * 100f);
                    float globalSeedY = (currentTime * currentSpeed) + (i * 100f) + 50f;
                    float globalShakeX = (Mathf.PerlinNoise(globalSeedX, 0f) * 2f - 1f) * currentMagnitude;
                    float globalShakeY = (Mathf.PerlinNoise(0f, globalSeedY) * 2f - 1f) * currentMagnitude;
                    Vector3 globalOffset = new Vector3(globalShakeX, globalShakeY, 0f);

                    for (int v = 0; v < 4; v++)
                    {
                        float vertexSeedX = (currentTime * currentSpeed * 1.8f) + (i * 150f) + (v * 250f);
                        float vertexSeedY = (currentTime * currentSpeed * 1.8f) + (i * 150f) + (v * 250f) + 80f;

                        float vertexShakeX = (Mathf.PerlinNoise(vertexSeedX, 0f) * 2f - 1f) * currentStretch;
                        float vertexShakeY = (Mathf.PerlinNoise(0f, vertexSeedY) * 2f - 1f) * currentStretch;
                        Vector3 vertexOffset = new Vector3(vertexShakeX, vertexShakeY, 0f);

                        vertices[vertexIndex + v] += globalOffset + vertexOffset;
                    }
                }
            }
        }
    }
}
