using System.Collections.Generic;
using UnityEngine;

public class ReceptionCounter : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform itemGrid;

    private bool playerInRange;
    private bool isOpen;
    private readonly List<NPCDialogue> currentItems = new List<NPCDialogue>();
    private readonly List<GameObject> spawnedSlots  = new List<GameObject>();

    private void Update()
    {
        if (DialogueManager.IsOpen || IntroManager.IsActive) return;
        if (!isOpen)
        {
            if (playerInRange && Input.GetKeyDown(KeyCode.E)) Open();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) Close();
    }

    private void Open()
    {
        currentItems.Clear();
        foreach (var npc in FindObjectsOfType<NPCDialogue>())
            if (npc.WaitingForPurchase) currentItems.Add(npc);

        shopPanel.SetActive(true);
        isOpen = true;
        RebuildSlots();
    }

    private void RebuildSlots()
    {
        foreach (var s in spawnedSlots) Destroy(s);
        spawnedSlots.Clear();
        foreach (var npc in currentItems)
            spawnedSlots.Add(BuildSlot(npc));
    }

    private GameObject BuildSlot(NPCDialogue npc)
    {
        var slot = new GameObject("Slot_" + npc.ItemName, typeof(RectTransform));
        slot.transform.SetParent(itemGrid, false);
        slot.AddComponent<ShopSlot>().Init(npc, this);
        return slot;
    }

    public void CompletePurchase(NPCDialogue npc, GameObject slotGO)
    {
        if (npc.ItemCost > 0)
            GameManager.Instance.AddMoney(-npc.ItemCost);

        npc.SetItemReady();
        currentItems.Remove(npc);
        spawnedSlots.Remove(slotGO);
        Destroy(slotGO);

        if (currentItems.Count == 0) Close();
    }

    public void Close()
    {
        foreach (var s in spawnedSlots) Destroy(s);
        spawnedSlots.Clear();
        shopPanel.SetActive(false);
        isOpen = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { playerInRange = false; Close(); }
    }
}
