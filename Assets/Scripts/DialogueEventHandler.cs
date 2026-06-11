using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueEventEntry
{
    public string key;
    public UnityEvent onTrigger;
    public DialogueNode chainDialogue;
}

public class DialogueEventHandler : MonoBehaviour
{
    public static DialogueEventHandler Instance { get; private set; }

    [SerializeField] private List<DialogueEventEntry> events;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Trigger(string key)
    {
        if (string.IsNullOrEmpty(key)) return;
        var entry = events.Find(e => e.key == key);
        if (entry == null) return;
        entry.onTrigger?.Invoke();
        if (entry.chainDialogue != null)
            DialogueManager.Instance.StartDialogue(entry.chainDialogue);
    }
}
