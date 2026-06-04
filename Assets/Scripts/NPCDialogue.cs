using UnityEngine;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    public GameObject dialoguePanel;

    public TMP_Text dialogueText;

    [TextArea]
    public string[] lines;

    private bool playerInRange;
    private int lineIndex;

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!dialoguePanel.activeSelf)
            {
                dialoguePanel.SetActive(true);

                lineIndex = 0;

                dialogueText.text = lines[lineIndex];
            }
            else
            {
                lineIndex++;

                if (lineIndex >= lines.Length)
                {
                    dialoguePanel.SetActive(false);
                }
                else
                {
                    dialogueText.text = lines[lineIndex];
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            dialoguePanel.SetActive(false);
        }
    }
}