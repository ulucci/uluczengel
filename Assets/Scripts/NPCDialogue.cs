using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] private DialogueNode introNode;
    [SerializeField] private DialogueNode taskNode;
    [SerializeField] private DialogueNode deliveryNode;
    [SerializeField] private GameObject exclamationMark;
    [SerializeField] private int requiredPhase = 1;
    [SerializeField] private bool notifiesPhase1Done;
    [SerializeField] private bool notifiesDorisUnlock;

    [Header("Purchase")]
    [SerializeField] private bool requiresPurchase;
    [SerializeField] private string itemName;
    [SerializeField] private int itemCost;
    [SerializeField] private Sprite itemSprite;

    public string ItemName  => itemName;
    public int    ItemCost  => itemCost;
    public Sprite ItemSprite => itemSprite;
    public bool WaitingForPurchase { get; private set; }
    public bool ItemReady          { get; private set; }

    private bool introSeen;
    private bool taskPlayed;
    private bool taskDone;
    private bool playerInRange;
    private NPCMovement npcMovement;

    private void Start()
    {
        npcMovement = GetComponent<NPCMovement>();
    }

    private void Update()
    {
        bool phaseOk    = StoryManager.Instance != null && StoryManager.Instance.Phase >= requiredPhase;
        bool canInteract = !taskDone && phaseOk && !WaitingForPurchase;

        if (exclamationMark != null)
            exclamationMark.SetActive(canInteract);

        if (!canInteract || !playerInRange || DialogueManager.IsOpen || IntroManager.IsActive) return;
        if (Input.GetKeyDown(KeyCode.E)) TriggerDialogue();
    }

    private void TriggerDialogue()
    {
        DialogueNode node;
        if      (!introSeen)                          node = introNode;
        else if (!taskPlayed)                         node = taskNode;
        else if (ItemReady && deliveryNode != null)   node = deliveryNode;
        else return;

        if (node == null) return;
        npcMovement?.PauseForDialogue(GameObject.FindWithTag("Player").transform);
        DialogueManager.Instance.StartDialogue(node, this);
    }

    public void OnDialogueComplete()
    {
        if (!introSeen)
        {
            introSeen = true;
            if (taskNode == null) taskDone = true;
        }
        else if (!taskPlayed)
        {
            taskPlayed = true;
            if (requiresPurchase)
                WaitingForPurchase = true;
            else
            {
                taskDone = true;
                NotifyCompletion();
            }
        }
        else if (ItemReady)
        {
            taskDone = true;
            NotifyCompletion();
        }

        npcMovement?.ResumeFromDialogue();
    }

    public void SetItemReady()
    {
        WaitingForPurchase = false;
        ItemReady = true;
    }

    private void NotifyCompletion()
    {
        if (StoryManager.Instance == null) return;
        if (notifiesPhase1Done)  StoryManager.Instance.OnPhase1TaskDone();
        if (notifiesDorisUnlock) StoryManager.Instance.OnDoloresDelivered();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}
