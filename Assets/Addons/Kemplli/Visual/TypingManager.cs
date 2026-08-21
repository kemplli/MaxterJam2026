using System.Collections;
using TMPro;
using EasyButtons;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;

//TODO: Make text wrapping
public class TypingManager : MonoBehaviour
{
    [Header("Main")]
    [NonSerialized]
    public TMP_Text textHolder;

    public List<TextContainer> textContainers = new List<TextContainer>();

    public int delayMs;
    private int startDelay;

    [SerializeField]
    private float autoMovingDelay;

    [SerializeField]
    private string typeSound;

    [Header("Effects and colors")]
    private List<IExtraEffect> extraEffects = new List<IExtraEffect>();

    [SerializeField]
    private Color customColor;

    [SerializeField]
    private int operatedTextIndexEDITOR;

    private int currentMonologue;

    private TextEventManager textEventManager;

    private FadeInEffect fadeInEffect;

    private string currentText;

    private bool movingMonologue;
    private bool movingMonologueAuto;

    private bool isTyping;

    private void Awake()
    {
        G.typing = this;

        startDelay = delayMs;

        textEventManager = new TextEventManager(this);
        fadeInEffect = new FadeInEffect();

        var all = ReflectionUtil.FindAllSublacesWithInterfaces<IExtraEffect>().ToList();
        foreach (var effect in all)
        {
            extraEffects.Add(Activator.CreateInstance(effect) as IExtraEffect);
        }
    }

    public void StartTyping()
    {
        delayMs = startDelay;

        isTyping = true;

        currentText = textContainers[0].Text;

        fadeInEffect.ResetTimers();
        currentMonologue = 0;
        movingMonologue = false;
        movingMonologueAuto = false;
        textHolder.alpha = 1f;
        textHolder.enableWordWrapping = true;

        FormatText(currentText);
        currentText = FixTextJumping(currentText);

        StartCoroutine(Type(currentText));
    }

    public void StopTyping()
    {
        if (isTyping)
        {
            isTyping = false;
            StopAllCoroutines();
            textHolder.DOFade(0, 0.5f).OnComplete(() => StopAllCoroutines());
        }
    }

    private void LateUpdate()
    {
        if (!isTyping) return;

        textHolder.ForceMeshUpdate();
        TMP_TextInfo textInfo = textHolder.textInfo;

        foreach (var effect in extraEffects)
        {
            effect.Update(Time.time, textInfo);
        }

        fadeInEffect.Update(Time.time, textInfo);

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            textHolder.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }

        textHolder.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private void Update()
    {
        if (!isTyping) return;

        if (Input.GetKeyDown(KeyCode.W) && !movingMonologue)
        {
            if (textHolder.text != currentText)
            {
                Skip();
            }
            else
            {
                MoveMonologue();
            }
        }

        if (textHolder.text == currentText && !movingMonologueAuto)
        {
            StartCoroutine(MoveMonologueAuto());
        }
    }

    private void MoveMonologue()
    {
        if (currentMonologue != textContainers.Count - 1)
        {
            movingMonologue = true;
            StopAllCoroutines();
            textHolder.DOFade(0, 0.5f).OnComplete(() =>
            {
                currentMonologue++;
                currentText = textContainers[currentMonologue].Text;

                CleanTextHolder();
                fadeInEffect.ResetTimers();

                textHolder.alpha = 1f;
                FormatText(currentText);
                currentText = FixTextJumping(currentText);
                delayMs = startDelay;
                StartCoroutine(Type(currentText));
            });
            CustomCoroutine.WaitThenExecute(0.5f, () => { movingMonologue = false; movingMonologueAuto = false; });
        }
        else
        {
            isTyping = false;
            textHolder.DOFade(0, 0.5f).OnComplete(() => StopAllCoroutines());
        }
    }

    private IEnumerator MoveMonologueAuto()
    {
        movingMonologueAuto = true;

        yield return new WaitForSeconds(autoMovingDelay);

        if (currentMonologue != textContainers.Count - 1)
        {
            movingMonologue = true;
            textHolder.DOFade(0, 0.5f).OnComplete(() =>
            {
                currentMonologue++;
                currentText = textContainers[currentMonologue].Text;

                CleanTextHolder();
                fadeInEffect.ResetTimers();

                textHolder.alpha = 1f;
                FormatText(currentText);
                currentText = FixTextJumping(currentText);
                delayMs = startDelay;
                StartCoroutine(Type(currentText));
            });
            yield return new WaitForSeconds(0.5f);
            movingMonologue = false;
            movingMonologueAuto = false;
        }
        else
        {
            isTyping = false;
            textHolder.DOFade(0, 0.5f).OnComplete(() => StopAllCoroutines());
        }
    }

    public void Skip()
    {
        StopAllCoroutines();

        if (currentText.Contains("[S]"))
        {
            StartCoroutine(textEventManager.CheckEvent("S"));
        }
        if (currentText.Contains("[Sound=Boom]"))
        {
            StartCoroutine(textEventManager.CheckEvent("Sound=Boom"));
        }

        ClearTextFromEvents();
        textHolder.text = currentText;
    }

    private void ClearTextFromEvents()
    {
        if (string.IsNullOrEmpty(currentText)) return;

        currentText = Regex.Replace(currentText, @"\[[^\]]*\]", "");
    }

    private IEnumerator Type(string text)
    {
        CleanTextHolder();

        StringBuilder currentDisplayingText = new StringBuilder();
        List<string> openedTagsStack = new List<string>();

        int i = 0;
        textHolder.maxVisibleCharacters = int.MaxValue;

        List<char> eventLink = new List<char>();
        while (i < text.Length)
        {
            if(!isTyping) yield break;

            if (text[i] == '<')
            {
                int tagEndIndex = text.IndexOf('>', i);
                if (tagEndIndex != -1)
                {
                    string tag = text.Substring(i, tagEndIndex - i + 1);

                    if (tag.StartsWith("</"))
                    {
                        if (openedTagsStack.Count > 0)
                            openedTagsStack.RemoveAt(openedTagsStack.Count - 1);
                    }
                    else
                    {
                        if (tag.StartsWith("<link")) openedTagsStack.Add("</link>");
                        else if (tag.StartsWith("<color")) openedTagsStack.Add("</color>");
                        else if (tag.StartsWith("<b")) openedTagsStack.Add("</b>");
                        else if (tag.StartsWith("<i")) openedTagsStack.Add("</i>");
                    }

                    currentDisplayingText.Append(tag);
                    i = tagEndIndex + 1;
                    continue;
                }
            }

            if (eventLink.Count > 0)
            {
                if (text[i] == ']')
                {
                    eventLink.Add(text[i]);
                    string link = new string(eventLink.ToArray());
                    yield return textEventManager.CheckEvent(link);

                    eventLink.Clear();
                    i++;
                    continue;
                }
                else
                {
                    eventLink.Add(text[i]);
                    i++;
                    continue;
                }

            }

            if (text[i] == '[')
            {
                eventLink.Add(text[i]);
                i++;
                continue;
            }

            char character = text[i];
            currentDisplayingText.Append(character);

            StringBuilder tempText = new StringBuilder(currentDisplayingText.ToString());
            for (int j = openedTagsStack.Count - 1; j >= 0; j--)
            {
                tempText.Append(openedTagsStack[j]);
            }

            textHolder.text = tempText.ToString();



            if (!char.IsWhiteSpace(character) && !string.IsNullOrEmpty(typeSound) && character != ' ')
            {
                G.audio.PlaySound(typeSound, 1f, transform.position);
            }

            i++;
            if(character != ' ')
            yield return new WaitForSeconds(delayMs / 1000f);
        }

        ClearTextFromEvents();
        textHolder.text = currentText;
    }

    private void CleanTextHolder()
    {
        textHolder.text = string.Empty;
    }

    public void FormatText(string rawDialogue)
    {
        if (string.IsNullOrEmpty(rawDialogue)) return;

        string processed = Regex.Replace(rawDialogue, @"\[\s*(.*?)\s*\]", match =>
        {
            string content = match.Groups[1].Value;
            content = Regex.Replace(content, @"\s*=\s*", "=");
            content = Regex.Replace(content, @"\s+", " ");
            return $"[{content.Trim()}]";
        });

        processed = Regex.Replace(processed, @"\[size=(\d+%)\]", "<size=$1>");
        processed = processed.Replace("[/size]", "</size>");

        HashSet<string> visualEffects = new HashSet<string> { "wave", "rainbow", "shake", "outline" };

        string tagPattern = @"\[(\/?)([a-zA-Z0-9_]+)(?:\s+([^\]]*))?\]";

        StringBuilder result = new StringBuilder();
        int lastIndex = 0;

        List<string> activeEffects = new List<string>();

        foreach (Match match in Regex.Matches(processed, tagPattern))
        {
            string textBefore = processed.Substring(lastIndex, match.Index - lastIndex);
            result.Append(textBefore);

            bool isClosing = match.Groups[1].Value == "/";
            string tagName = match.Groups[2].Value;
            string rawArgs = match.Groups[3].Value.Trim();

            if (visualEffects.Contains(tagName))
            {
                if (isClosing)
                {
                    if (activeEffects.Count > 0) result.Append("</link>");

                    for (int i = activeEffects.Count - 1; i >= 0; i--)
                    {
                        if (activeEffects[i].StartsWith(tagName + "_") || activeEffects[i] == tagName + "_default")
                        {
                            activeEffects.RemoveAt(i);
                        }
                    }

                    if (activeEffects.Count > 0)
                    {
                        result.Append($"<link=\"effects:{string.Join(";", activeEffects)}\">");
                    }
                }
                else
                {
                    if (activeEffects.Count > 0) result.Append("</link>");

                    if (!string.IsNullOrEmpty(rawArgs))
                    {
                        string[] paramsArray = rawArgs.Split(' ');
                        foreach (string p in paramsArray)
                        {
                            if (!string.IsNullOrEmpty(p))
                            {
                                string fullParam = $"{tagName}_{p}";
                                if (!activeEffects.Contains(fullParam))
                                {
                                    activeEffects.Add(fullParam);
                                }
                            }
                        }
                    }
                    else
                    {
                        activeEffects.Add($"{tagName}_default");
                    }

                    result.Append($"<link=\"effects:{string.Join(";", activeEffects)}\">");
                }
            }
            else
            {
                result.Append(match.Value);
            }

            lastIndex = match.Index + match.Length;
        }

        result.Append(processed.Substring(lastIndex));
        currentText = result.ToString();
    }

    public string FixTextJumping(string processedText)
    {
        var allSizes = Regex.Matches(processedText, @"<size=(\d+)%>");
        int maxPercent = 100; 

        if (allSizes.Count > 0)
        {
            foreach (Match m in allSizes)
            {
                if (int.TryParse(m.Groups[1].Value, out int percent))
                {
                    if (percent > maxPercent) maxPercent = percent;
                }
            }
        }

        if (maxPercent > 100)
        {
            processedText = processedText.Insert(0, $"<size={maxPercent}%>\u200B</size>");
        }

        return processedText;
    }

    #region editor help
#if UNITY_EDITOR
    [Button]
    public void MakeRed()
    {
        string clipboardText = GUIUtility.systemCopyBuffer;

        if (!string.IsNullOrEmpty(clipboardText))
        {
            Debug.Log(clipboardText);
            MakeSelectedTextColorful(Color.red,clipboardText);
        }
        else
        {
            Debug.LogWarning("Буфер обмена пуст! Сначала выдели текст и нажми Ctrl+C.");
        }
    }

    [Button]
    public void MakeBlue()
    {
        string clipboardText = GUIUtility.systemCopyBuffer;

        if (!string.IsNullOrEmpty(clipboardText))
        {
            Debug.Log(clipboardText);
            MakeSelectedTextColorful(Color.blue, clipboardText);
        }
        else
        {
            Debug.LogWarning("Буфер обмена пуст! Сначала выдели текст и нажми Ctrl+C.");
        }
    }

    [Button]
    public void MakeGreen()
    {
        string clipboardText = GUIUtility.systemCopyBuffer;

        if (!string.IsNullOrEmpty(clipboardText))
        {
            Debug.Log(clipboardText);
            MakeSelectedTextColorful(Color.green, clipboardText);
        }
        else
        {
            Debug.LogWarning("Буфер обмена пуст! Сначала выдели текст и нажми Ctrl+C.");
        }
    }

    [Button]
    public void MakeCustomColor()
    {
        string clipboardText = GUIUtility.systemCopyBuffer;

        if (!string.IsNullOrEmpty(clipboardText))
        {
            Debug.Log(clipboardText);
            MakeSelectedTextColorful(customColor, clipboardText);
        }
        else
        {
            Debug.LogWarning("Буфер обмена пуст! Сначала выдели текст и нажми Ctrl+C.");
        }
    }

    [Button]
    public void GetIndexes()
    {
        string clipboardText = GUIUtility.systemCopyBuffer;

        if (!string.IsNullOrEmpty(clipboardText))
        {
            var startIndex = textContainers[operatedTextIndexEDITOR].Text.IndexOf(clipboardText);
            Debug.Log(startIndex + " " + (clipboardText.Length + startIndex));
        }
        else
        {
            Debug.LogWarning("Буфер обмена пуст! Сначала выдели текст и нажми Ctrl+C.");
        }
    }

    private void MakeSelectedTextColorful(Color color,string selected)
    {
        if (string.IsNullOrEmpty(textContainers[operatedTextIndexEDITOR].Text) || string.IsNullOrEmpty(selected)) return;

        int startIndex = textContainers[operatedTextIndexEDITOR].Text.IndexOf(selected);

        if (startIndex == -1)
        {
            Debug.LogWarning($"Фраза '{selected}' не найдена в основном тексте!");
            return;
        }

        string hexColor = "#" + UnityEngine.ColorUtility.ToHtmlStringRGB(color);

        string startTag = $"<color={hexColor}>";
        string endTag = "</color>";

        textContainers[operatedTextIndexEDITOR].Text = textContainers[operatedTextIndexEDITOR].Text.Insert(startIndex + selected.Length, endTag);
        textContainers[operatedTextIndexEDITOR].Text = textContainers[operatedTextIndexEDITOR].Text.Insert(startIndex, startTag);
    }

    [Button()]
    public void MakeRainbow(float speed = 2f,float waveSpacing = 0.1f)
    {
        string clipboardText = GUIUtility.systemCopyBuffer;

        if (!string.IsNullOrEmpty(clipboardText))
        {
            Debug.Log(clipboardText);
            MakeExtraEffect(TextEffects.Rainbow,clipboardText,new Dictionary<string, float> { { "speed", speed }, { "waveSpacing",waveSpacing} });
        }
        else
        {
            Debug.LogWarning("Буфер обмена пуст! Сначала выдели текст и нажми Ctrl+C.");
        }
    }

    [Button()]
    public void MakeWave(float frequency = 5f,float amplitude = 5f)
    {
        string clipboardText = GUIUtility.systemCopyBuffer;

        if (!string.IsNullOrEmpty(clipboardText))
        {
            Debug.Log(clipboardText);
            MakeExtraEffect(TextEffects.Waving, clipboardText,new Dictionary<string, float> { { "frequency",frequency }, { "amplitude", amplitude} });
        }
        else
        {
            Debug.LogWarning("Буфер обмена пуст! Сначала выдели текст и нажми Ctrl+C.");
        }
    }

    [Button()]
    public void MakeShake(float speed = 10f,float magnitude = 5f)
    {
        string clipboardText = GUIUtility.systemCopyBuffer;

        if (!string.IsNullOrEmpty(clipboardText))
        {
            Debug.Log(clipboardText);
            MakeExtraEffect(TextEffects.Shaking, clipboardText,new Dictionary<string, float>() { { "speed", speed }, { "magnitude",magnitude} });
        }
        else
        {
            Debug.LogWarning("Буфер обмена пуст! Сначала выдели текст и нажми Ctrl+C.");
        }
    }


    [Button()]
    public void MakeSize(float sizePercentage)
    {
        string clipboardText = GUIUtility.systemCopyBuffer;

        if (!string.IsNullOrEmpty(clipboardText))
        {
            Debug.Log(clipboardText);
            int startIndex = textContainers[operatedTextIndexEDITOR].Text.IndexOf(clipboardText);

            if (startIndex == -1)
            {
                Debug.LogWarning($"Фраза '{clipboardText}' не найдена в основном тексте!");
                return;
            }

            string startTag = $"<size={sizePercentage}%>";
            string endTag = "</size>";

            textContainers[operatedTextIndexEDITOR].Text = textContainers[operatedTextIndexEDITOR].Text.Insert(startIndex + clipboardText.Length, endTag);
            textContainers[operatedTextIndexEDITOR].Text = textContainers[operatedTextIndexEDITOR].Text.Insert(startIndex, startTag);
        }
        else
        {
            Debug.LogWarning("Буфер обмена пуст! Сначала выдели текст и нажми Ctrl+C.");
        }
    }



    private void MakeExtraEffect(TextEffects id,string selected, Dictionary<string,float> values)
    {
        if (string.IsNullOrEmpty(textContainers[operatedTextIndexEDITOR].Text) || string.IsNullOrEmpty(selected)) return;

        int startIndex = textContainers[operatedTextIndexEDITOR].Text.IndexOf(selected);

        if (startIndex == -1)
        {
            Debug.LogWarning($"Фраза '{selected}' не найдена в основном тексте!");
            return;
        }

        string linkID = string.Empty;
        try
        {
            linkID = extraEffects.Find(x => x.id == id).linkID;
        }
        catch (NullReferenceException)
        {
            var all = ReflectionUtil.FindAllSublacesWithInterfaces<IExtraEffect>();
            IExtraEffect effect;
            foreach (var item in all)
            {
                effect = Activator.CreateInstance(item) as IExtraEffect;
                if (effect.id == id)
                {
                    extraEffects.Add(effect);
                    linkID = effect.linkID;
                    break;
                }
            }
        }

        string extraValues = string.Empty;
        foreach (var (key, value) in values)
        {
            extraValues =  extraValues.Insert(0,key + " = " + value.ToString() + " ");
        }

        string startTag = $"[{linkID} {extraValues}]";
        string endTag = $"[/{linkID}]";

        textContainers[operatedTextIndexEDITOR].Text = textContainers[operatedTextIndexEDITOR].Text.Insert(startIndex + selected.Length, endTag);
        textContainers[operatedTextIndexEDITOR].Text = textContainers[operatedTextIndexEDITOR].Text.Insert(startIndex, startTag);
    }
#endif
    #endregion
}

public interface IExtraEffect
{
    public TextEffects id { get; set; }
    public string linkID {  get; set; }
    public void Update(float currentTime,TMP_TextInfo textInfo);
}

public enum TextEffects
{
    Shaking,
    Rainbow,
    Waving,
    Scaling,
    PsychedelicOutline,
    Burn
}

public class TextEventManager
{
    private TypingManager typingManager;
    public TextEventManager(TypingManager typingManager) {
        this.typingManager = typingManager;
    }
    public IEnumerator CheckEvent(string link)
    {
        string cleanLink = link.Replace('[', ' ').Replace(']', ' ').Trim();

        string eventName = cleanLink;
        string eventValue = string.Empty;

        if (cleanLink.Contains("="))
        {
            string[] parts = cleanLink.Split('=');
            eventName = parts[0].Trim();
            eventValue = parts[1].Trim();
        }

        switch (eventName)
        {
            case "T":
                float delay = 0.5f;
                if (!string.IsNullOrEmpty(eventValue))
                {
                    float.TryParse(eventValue, NumberStyles.Any, CultureInfo.InvariantCulture, out delay);
                }

                yield return new WaitForSecondsRealtime(delay);
                break;

            case "S":
                G.feel.ShakeHard();
                break;
            case "D":
                int typeDelay = 0;
                if (!string.IsNullOrEmpty(eventValue))
                {
                    int.TryParse(eventValue, NumberStyles.Any, CultureInfo.InvariantCulture, out typeDelay);
                }
                typingManager.delayMs = typeDelay;
                break;
            case "Sound":
                if (!string.IsNullOrEmpty(eventValue))
                {
                    G.audio.PlaySound(eventValue, 1f, Vector3.zero);
                }
                break;
        }

        yield return null;

    }
}

public static class TMPTagParser
{
    public static float GetFloat(string linkID, string paramName, float defaultValue)
    {
        if (string.IsNullOrEmpty(linkID) || !linkID.Contains(":")) return defaultValue;

        string[] parts = linkID.Split(':');
        if (parts.Length < 2) return defaultValue;

        string[] parameters = parts[1].Split(new char[] { ';', ',' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string param in parameters)
        {
            string[] keyValue = param.Split('=');
            if (keyValue.Length == 2)
            {
                string key = keyValue[0].Trim().ToLower();
                string value = keyValue[1].Trim();

                if (key == paramName.ToLower())
                {
                    if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out float result))
                    {
                        return result;
                    }
                }
            }
        }

        return defaultValue;
    }

}

[Serializable]
public class TextContainer
{
    public string elementName;

    [TextArea(10,20)]
    public string Text;

    [TextArea(10, 20)]
    public string Draft;
}

public class TypingText : ManagedBehaviour
{
    [SerializeField]
    private List<TextContainer> textContainers = new List<TextContainer>();

    [SerializeField]
    private TMP_Text textHolder;

    protected void StartTyping()
    {
        G.typing.textHolder = textHolder;
        GameMath.FullListWithOther(textContainers,ref G.typing.textContainers);
        G.typing.StartTyping();
    }
}
