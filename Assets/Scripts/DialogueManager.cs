using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public static bool IsOpen { get; private set; }
    public static event System.Action OnDialogueClosed;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text choiceTextA;
    [SerializeField] private TMP_Text choiceTextB;
    [SerializeField] private float typeSpeed = 0.04f;

    private DialogueNode currentNode;
    private NPCDialogue currentNPC;
    private System.Action onCompleteCallback;
    private bool isTyping;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        dialoguePanel.SetActive(false);
        choiceTextA.gameObject.SetActive(false);
        choiceTextB.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!IsOpen) return;

        if (HasChoices())
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectChoice(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) SelectChoice(1);
            else if (Input.GetMouseButtonDown(0))
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(choiceTextA.rectTransform, Input.mousePosition))
                    SelectChoice(0);
                else if (choiceTextB.gameObject.activeSelf &&
                         RectTransformUtility.RectangleContainsScreenPoint(choiceTextB.rectTransform, Input.mousePosition))
                    SelectChoice(1);
            }
            return;
        }

        if (Input.GetMouseButtonDown(0)) HandleClick();
    }

    public void StartDialogue(DialogueNode startNode, NPCDialogue npc = null, System.Action onComplete = null)
    {
        if (IsOpen || startNode == null) return;
        IsOpen = true;
        currentNPC = npc;
        onCompleteCallback = onComplete;
        dialoguePanel.SetActive(true);
        ShowNode(startNode);
    }

    private void ShowNode(DialogueNode node)
    {
        currentNode = node;
        speakerText.text = node.speakerName;
        choiceTextA.gameObject.SetActive(false);
        choiceTextB.gameObject.SetActive(false);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(node.dialogueText));
    }

    private void HandleClick()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentNode.dialogueText;
            isTyping = false;
            ShowChoices();
        }
        else
        {
            CompleteNode();
        }
    }

    private void CompleteNode()
    {
        if (currentNode.moneyDelta != 0)
            GameManager.Instance.AddMoney(currentNode.moneyDelta);

        if (!string.IsNullOrEmpty(currentNode.eventKey))
            DialogueEventHandler.Instance.Trigger(currentNode.eventKey);

        if (currentNode.nextNode != null) ShowNode(currentNode.nextNode);
        else CloseDialogue();
    }

    public void SelectChoice(int index)
    {
        var choice = currentNode.choices[index];

        if (choice.moneyDelta != 0)
            GameManager.Instance.AddMoney(choice.moneyDelta);

        if (!string.IsNullOrEmpty(choice.eventKey))
            DialogueEventHandler.Instance.Trigger(choice.eventKey);

        if (choice.nextNode != null) ShowNode(choice.nextNode);
        else CloseDialogue();
    }

    private void ShowChoices()
    {
        if (!HasChoices()) return;

        choiceTextA.gameObject.SetActive(true);
        choiceTextA.text = currentNode.choices[0].choiceText;

        if (currentNode.choices.Length > 1)
        {
            choiceTextB.gameObject.SetActive(true);
            choiceTextB.text = currentNode.choices[1].choiceText;
        }
    }

    private void CloseDialogue()
    {
        IsOpen = false;
        dialoguePanel.SetActive(false);
        currentNPC?.OnDialogueComplete();
        currentNPC = null;
        currentNode = null;
        var cb = onCompleteCallback;
        onCompleteCallback = null;
        cb?.Invoke();
        OnDialogueClosed?.Invoke();
    }

    private bool HasChoices() =>
        currentNode != null && currentNode.choices != null && currentNode.choices.Length > 0;

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        ShowChoices();
    }
}
