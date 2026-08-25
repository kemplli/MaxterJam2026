using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class TMP_LinkShaker : MonoBehaviour
{
    [Header("Идентификатор тега (<link=\"shake\">)")]
    public string linkID = "shake";

    [Header("Настройки дефолтной тряски")]
    public float speed = 5f;
    public float magnitude = 2f;
    public float stretch = 1.5f;

    private TMP_Text textComponent;
    private TMP_TextInfo textInfo;
    private MeshInfoBackup[] meshInfoBackups;
    private string lastText;

    private struct MeshInfoBackup
    {
        public Vector3[] vertices;
    }

    void Awake()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    void Start()
    {
        CacheOriginalVertices();
    }

    void CacheOriginalVertices()
    {
        textComponent.ForceMeshUpdate();
        textInfo = textComponent.textInfo;
        meshInfoBackups = new MeshInfoBackup[textInfo.meshInfo.Length];

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            meshInfoBackups[i].vertices = (Vector3[])textInfo.meshInfo[i].vertices.Clone();
        }

        lastText = textComponent.text;
    }

    void Update()
    {
        // Проверяем, не изменился ли текст напрямую, вместо тяжелого TEXT_CHANGED_EVENT
        if (textComponent.text != lastText)
        {
            CacheOriginalVertices();
        }

        if (textInfo == null || textInfo.characterCount == 0 || textInfo.linkCount == 0) return;

        float currentTime = Time.time;

        // Сброс меша к оригиналу
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            System.Array.Copy(meshInfoBackups[i].vertices, textInfo.meshInfo[i].vertices, meshInfoBackups[i].vertices.Length);
        }

        // Пробегаемся по всем ссылкам в тексте
        for (int l = 0; l < textInfo.linkCount; l++)
        {
            TMP_LinkInfo linkInfo = textInfo.linkInfo[l];
            string currentLinkID = linkInfo.GetLinkID();

            // Проверяем, наш ли это эффект
            if (currentLinkID.Contains(linkID))
            {
                // Если у вас есть парсер параметров из ID ссылки, можно раскомментировать строки ниже:
                // float currentSpeed = TMPTagParser.GetFloat(currentLinkID, "shake_speed", this.speed);
                // float currentMagnitude = TMPTagParser.GetFloat(currentLinkID, "shake_magnitude", this.magnitude);
                // float currentStretch = TMPTagParser.GetFloat(currentLinkID, "shake_stretch", this.stretch);

                float currentSpeed = this.speed;
                float currentMagnitude = this.magnitude;
                float currentStretch = this.stretch;

                int firstCharIndex = linkInfo.linkTextfirstCharacterIndex;
                int linkLength = linkInfo.linkTextLength;

                // Трясем только символы внутри линка
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

        // Обновляем геометрию
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}