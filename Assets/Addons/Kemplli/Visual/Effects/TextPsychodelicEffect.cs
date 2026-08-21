using System;
using TMPro;
using UnityEngine;

[Serializable]
public class TextPsychodelicEffect : IExtraEffect
{
    [NonSerialized]
    public float speed = 5f;

    [NonSerialized]
    public float thickness = 3f;

    [NonSerialized]
    public float waveSpacing = 0.1f;

    public string linkID { get; set; } = "outline";
    public TextEffects id { get; set; } = TextEffects.PsychedelicOutline;


    public void Update(float currentTime, TMP_TextInfo textInfo)
    {
        if (textInfo == null || textInfo.textComponent == null) return;

        for (int l = 0; l < textInfo.linkCount; l++)
        {
            TMP_LinkInfo linkInfo = textInfo.linkInfo[l];
            if (!linkInfo.GetLinkID().Contains(linkID)) continue;

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
                Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

                // Базовый огонь-таймер
                float fireWave = Mathf.PingPong(currentTime * speed - (i * 0.3f), 1f);

                // Если фаза горения активна, деформируем нижние вершины буквы вверх (эффект сжатия в пепел)
                float dissolveUp = fireWave * 15f;
                vertices[vertexIndex + 0].y += dissolveUp; // Левый низ летит вверх
                vertices[vertexIndex + 3].y += dissolveUp; // Правый низ летит вверх

                // Плавное окрашивание в цвет тления (Оранжевый -> Черный -> Прозрачный)
                Color32 burnColor = Color32.Lerp(new Color32(255, 100, 0, 255), new Color32(30, 30, 30, 0), fireWave);

                // Задаем градиент: верх еще держится, низ уже сгорел
                colors[vertexIndex + 0] = burnColor;
                colors[vertexIndex + 3] = burnColor;
            }
        }
    }
}