using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TypewriterEffect : MonoBehaviour
{
    [Header("Text Target")]
    public TMP_Text tmpText; // Auto-filled if left null

    [Header("Typing Speed (Characters per Second)")]
    [Range(1f, 100f)]
    public float charactersPerSecond = 40f; // Base typing speed

    [Header("Timing Options")]
    public bool useUnscaledTime = false; // Ignores timeScale if true
    public bool skipWhitespaceDelay = true; // Skip delay for spaces/newlines
    public float punctuationPauseMultiplier = 3f; // Pause for . , ! ?

    [Header("Rich Text Support")]
    public bool supportRichTextTags = true; // Don’t type <b></b> one char at a time

    [Header("Auto Start")]
    public bool playOnAwake = false;
    [TextArea] public string startText;

    [Header("Events")]
    public UnityEvent onStartTyping;
    public UnityEvent onComplete;

    private Coroutine typeRoutine;

    void Awake()
    {
        if (!tmpText)
            tmpText = GetComponent<TMP_Text>();

        if (playOnAwake && !string.IsNullOrEmpty(startText))
            StartTypewriter(startText);
    }

    public void StartTypewriter(string message)
    {
        if (typeRoutine != null)
            StopCoroutine(typeRoutine);

        typeRoutine = StartCoroutine(TypeText(message ?? ""));
        onStartTyping?.Invoke();
    }

    public void ShowFullText(string message)
    {
        if (typeRoutine != null)
            StopCoroutine(typeRoutine);

        if (tmpText)
            tmpText.text = message ?? "";

        onComplete?.Invoke();
    }

    IEnumerator TypeText(string message)
    {
        if (!tmpText)
            yield break;

        tmpText.text = "";
        StringBuilder visible = new StringBuilder();
        int i = 0;

        float baseDelay = 1f / Mathf.Max(1f, charactersPerSecond);

        while (i < message.Length)
        {
            char c = message[i];

            // 🔹 Handle "\n" escape sequence as a real newline
            if (c == '\\' && i < message.Length - 1 && message[i + 1] == 'n')
            {
                visible.Append('\n');
                tmpText.text = visible.ToString();
                i += 2; // skip both '\' and 'n'

                // Delay logic for newline
                float nlDelay = 0f;
                if (!(skipWhitespaceDelay && char.IsWhiteSpace('\n')))
                {
                    nlDelay = baseDelay;
                }

                if (nlDelay > 0f)
                {
                    if (useUnscaledTime) yield return new WaitForSecondsRealtime(nlDelay);
                    else yield return new WaitForSeconds(nlDelay);
                }
                else
                {
                    yield return null;
                }

                continue;
            }

            // 🔹 Handle <rich text> tags
            if (supportRichTextTags && c == '<')
            {
                int tagEnd = message.IndexOf('>', i);
                if (tagEnd >= 0)
                {
                    visible.Append(message, i, tagEnd - i + 1);
                    tmpText.text = visible.ToString();
                    i = tagEnd + 1;
                    continue;
                }
            }

            // 🔹 Normal character
            visible.Append(c);
            tmpText.text = visible.ToString();

            // 🔹 Delay calculation
            float delay = 0f;
            if (!(skipWhitespaceDelay && char.IsWhiteSpace(c)))
            {
                bool isPunct = (c == '.' || c == '!' || c == '?' || c == ',');
                delay = baseDelay * (isPunct ? punctuationPauseMultiplier : 1f);
            }

            if (delay > 0f)
            {
                if (useUnscaledTime) yield return new WaitForSecondsRealtime(delay);
                else yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }

            i++;
        }

        typeRoutine = null;
        onComplete?.Invoke();
    }
}
