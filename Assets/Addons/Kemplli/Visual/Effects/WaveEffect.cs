using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class WaveEffect : IExtraEffect
{
    [NonSerialized]
    public float frequency;

    [NonSerialized]
    public float amplitude;

    public TextEffects id { get; set; } = TextEffects.Waving;
    public string linkID { get; set; } = "wave";

    public void Update(float currentTime,TMP_TextInfo textInfo)
    {
        if (textInfo == null || textInfo.textComponent == null) return;

        for (int l = 0; l < textInfo.linkCount; l++)
        {
            TMP_LinkInfo linkInfo = textInfo.linkInfo[l];
            string currentLinkID = linkInfo.GetLinkID();

            bool isOurEffect = currentLinkID.Contains("wave_amplitude");

            if (isOurEffect)
            {
                float currentFreq = TMPTagParser.GetFloat(currentLinkID, "wave_frequency", this.frequency);
                float currentAmp = TMPTagParser.GetFloat(currentLinkID, "wave_amplitude", this.amplitude);

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

                    float wave = Mathf.Sin(i + currentTime * currentFreq) * currentAmp;

                    vertices[vertexIndex + 0].y += wave;
                    vertices[vertexIndex + 1].y += wave;
                    vertices[vertexIndex + 2].y += wave;
                    vertices[vertexIndex + 3].y += wave;
                }
            }
        }
    }
}

[Serializable]
public class FadeInEffect
{
    [NonSerialized] public float fadeDuration = 0.25f;

    [NonSerialized] private Dictionary<int, float> _appearanceTimers = new Dictionary<int, float>();
    [NonSerialized] private TMP_Text _lastTextComponent;

    public void Update(float currentTime, TMP_TextInfo textInfo)
    {
        if (textInfo == null || textInfo.textComponent == null) return;

        if (_lastTextComponent != textInfo.textComponent)
        {
            _appearanceTimers.Clear();
            _lastTextComponent = textInfo.textComponent;
        }

        int characterCount = textInfo.characterCount;

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) continue;

            int uniqueCharKey = HashCode.Combine(
                Mathf.RoundToInt(charInfo.bottomLeft.x * 100f),
                Mathf.RoundToInt(charInfo.bottomLeft.y * 100f)
            );

            if (!_appearanceTimers.ContainsKey(uniqueCharKey))
            {
                _appearanceTimers[uniqueCharKey] = currentTime;
            }

            float startTime = _appearanceTimers[uniqueCharKey];
            float elapsed = currentTime - startTime;
            float progress = Mathf.Clamp01(elapsed / fadeDuration);

            if (progress >= 1f) continue;

            AnimateCharacter(textInfo, i, progress);
        }
    }

    public void ResetTimers()
    {
        _appearanceTimers?.Clear();
    }

    private void AnimateCharacter(TMP_TextInfo textInfo, int charIndex, float progress)
    {
        TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];
        int materialIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;

        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
        Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

        Vector3 origV0 = charInfo.bottomLeft;  // Левый нижний
        Vector3 origV1 = charInfo.topLeft;     // Левый верхний
        Vector3 origV2 = charInfo.topRight;    // Правый верхний
        Vector3 origV3 = charInfo.bottomRight; // Правый нижний

        Vector3 bottomRightAnchor = origV3;

        Vector3 shakeWaveOffset0 = vertices[vertexIndex + 0] - origV0;
        Vector3 shakeWaveOffset1 = vertices[vertexIndex + 1] - origV1;
        Vector3 shakeWaveOffset2 = vertices[vertexIndex + 2] - origV2;
        Vector3 shakeWaveOffset3 = vertices[vertexIndex + 3] - origV3;

        float easeProgress = 1f - Mathf.Pow(1f - progress, 4f);

        vertices[vertexIndex + 0] = Vector3.Lerp(bottomRightAnchor, origV0, easeProgress) + (shakeWaveOffset0 * easeProgress);
        vertices[vertexIndex + 1] = Vector3.Lerp(bottomRightAnchor, origV1, easeProgress) + (shakeWaveOffset1 * easeProgress);
        vertices[vertexIndex + 2] = Vector3.Lerp(bottomRightAnchor, origV2, easeProgress) + (shakeWaveOffset2 * easeProgress);
        vertices[vertexIndex + 3] = bottomRightAnchor + (shakeWaveOffset3 * easeProgress);

        byte alphaByte = (byte)(progress * 255);
        colors[vertexIndex + 0].a = alphaByte;
        colors[vertexIndex + 1].a = alphaByte;
        colors[vertexIndex + 2].a = alphaByte;
        colors[vertexIndex + 3].a = alphaByte;
    }
}