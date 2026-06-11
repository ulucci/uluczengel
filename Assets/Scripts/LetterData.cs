using UnityEngine;

[CreateAssetMenu(fileName = "LetterData", menuName = "Dialogue/Letter Data")]
public class LetterData : ScriptableObject
{
    [TextArea(2, 6)]
    public string[] lines;
}
