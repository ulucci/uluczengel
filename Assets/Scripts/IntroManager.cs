using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class IntroManager : MonoBehaviour
{
    public static bool IsActive { get; private set; }

    [SerializeField] private GameObject letterPanel;
    [SerializeField] private TMP_Text letterText;
    [SerializeField] private LetterData letterData;
    [SerializeField] private LetterData refuseLetterData;
    [SerializeField] private LetterData acceptLetterData;
    [SerializeField] private DialogueNode patronStartNode;
    [SerializeField] private float typeSpeed = 0.03f;
    [SerializeField] private float fadeDuration = 1.5f;

    [Space] [SerializeField] private UnityEvent onFadeOutStart;

    private LetterData _activeData;
    private bool _isFollowUp;

    private string[] Lines => (_activeData ?? letterData).lines;

    private int currentLine;
    private bool isTyping;
    private bool isFading;
    private string shownText = "";
    private CanvasGroup canvasGroup;
    private Coroutine typingCoroutine;

    [SerializeField] private float linePause = 0.6f;

    private void Start()
    {
        IsActive = true;
        canvasGroup = letterPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = letterPanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        letterText.text = "";
        letterPanel.SetActive(true);
        typingCoroutine = StartCoroutine(TypeLine(Lines[0]));
    }

    private void Update()
    {
        if (!IsActive || isFading) return;
        if (Input.GetMouseButtonDown(0) && isTyping) SkipTyping();
    }

    private void SkipTyping()
    {
        StopCoroutine(typingCoroutine);
        isTyping = false;

        string line = Lines[currentLine];
        shownText += (line.StartsWith("<") ? line : BuildWrapped(line)) + "\n";
        letterText.text = shownText;
        typingCoroutine = StartCoroutine(AdvanceAfterPause());
    }

    private string BuildWrapped(string line)
    {
        string[] words = line.Split(' ');
        string result = "";
        for (int w = 0; w < words.Length; w++)
        {
            if (w > 0)
                result += WordFitsOnLine(shownText + result, words[w]) ? " " : "\n";
            result += words[w];
        }

        return result;
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;

        if (line.StartsWith("<"))
        {
            shownText += line + "\n";
            letterText.text = shownText;
            isTyping = false;
            typingCoroutine = StartCoroutine(AdvanceAfterPause());
            yield break;
        }

        string[] words = line.Split(' ');
        string buf = "";

        for (int w = 0; w < words.Length; w++)
        {
            string word = words[w];

            if (w > 0)
            {
                string sep = WordFitsOnLine(shownText + buf, word) ? " " : "\n";
                buf += sep;
                letterText.text = shownText + buf;
            }

            foreach (char c in word)
            {
                buf += c;
                letterText.text = shownText + buf;
                yield return new WaitForSeconds(typeSpeed);
            }
        }

        shownText += buf + "\n";
        letterText.text = shownText;
        isTyping = false;
        typingCoroutine = StartCoroutine(AdvanceAfterPause());
    }

    private bool WordFitsOnLine(string existing, string word)
    {
        letterText.text = existing + " " + word;
        letterText.ForceMeshUpdate();
        int spaceIdx = existing.Length;
        int wordIdx = spaceIdx + 1;
        if (letterText.textInfo.characterCount <= wordIdx) return true;
        return letterText.textInfo.characterInfo[spaceIdx].lineNumber ==
               letterText.textInfo.characterInfo[wordIdx].lineNumber;
    }

    private IEnumerator AdvanceAfterPause()
    {
        yield return new WaitForSeconds(linePause);
        currentLine++;
        if (currentLine < Lines.Length)
            typingCoroutine = StartCoroutine(TypeLine(Lines[currentLine]));
        else
            StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        isFading = true;
        onFadeOutStart?.Invoke();

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - (t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        letterPanel.SetActive(false);
        IsActive = false;

        if (_isFollowUp) yield break;

        if (DialogueManager.Instance == null || StoryManager.Instance == null || patronStartNode == null)
        {
            Debug.LogError($"[IntroManager] FadeOut: DialogueManager={DialogueManager.Instance}, StoryManager={StoryManager.Instance}, patronStartNode={patronStartNode}");
            yield break;
        }
        DialogueManager.Instance.StartDialogue(patronStartNode, onComplete: StoryManager.Instance.OnPatronIntroDone);
    }

    public void ShowRefuseLetter() => StartCoroutine(ShowLetterRoutine(refuseLetterData));
    public void ShowAcceptLetter() => StartCoroutine(ShowLetterRoutine(acceptLetterData));

    private IEnumerator ShowLetterRoutine(LetterData data)
    {
        if (data == null) yield break;
        yield return null; // diyalog kapandıktan sonra aç
        _activeData  = data;
        _isFollowUp  = true;
        currentLine  = 0;
        shownText    = "";
        isTyping     = false;
        isFading     = false;
        canvasGroup.alpha = 1f;
        letterText.text   = "";
        letterPanel.SetActive(true);
        IsActive = true;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(Lines[0]));
    }
}