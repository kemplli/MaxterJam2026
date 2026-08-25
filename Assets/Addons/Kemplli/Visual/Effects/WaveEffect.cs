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
    [NonSerialized]
    public float fadeDuration = 0.25f;

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

            int uniqueCharKey = charInfo.index;

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

        Vector3 bottomRightAnchor = charInfo.bottomRight;

        float easeProgress = 1f - Mathf.Pow(1f - progress, 4f);

        vertices[vertexIndex + 0] = Vector3.Lerp(bottomRightAnchor, vertices[vertexIndex + 0], easeProgress);
        vertices[vertexIndex + 1] = Vector3.Lerp(bottomRightAnchor, vertices[vertexIndex + 1], easeProgress);
        vertices[vertexIndex + 2] = Vector3.Lerp(bottomRightAnchor, vertices[vertexIndex + 2], easeProgress);
        vertices[vertexIndex + 3] = Vector3.Lerp(bottomRightAnchor, vertices[vertexIndex + 3], easeProgress);

        byte alphaByte = (byte)(progress * 255);
        colors[vertexIndex + 0].a = alphaByte;
        colors[vertexIndex + 1].a = alphaByte;
        colors[vertexIndex + 2].a = alphaByte;
        colors[vertexIndex + 3].a = alphaByte;
    }
}