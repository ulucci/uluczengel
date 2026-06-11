using UnityEngine;

[System.Serializable]
public class Choice
{
    public string choiceText;
    public DialogueNode nextNode;
    public int moneyDelta;
    public string eventKey;
}

[CreateAssetMenu(fileName = "NewNode", menuName = "Dialogue/Node")]
public class DialogueNode : ScriptableObject
{
    public string speakerName;
    [TextArea(2, 5)] public string dialogueText;
    public int moneyDelta;
    public string eventKey;
    public DialogueNode nextNode;
    public Choice[] choices;
}
