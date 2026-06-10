using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform destination;

    private static int playerInsideCount;

    private void OnEnable() => playerInsideCount = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInsideCount++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerInsideCount--;
    }

    private void OnMouseDown()
    {
        if (playerInsideCount <= 0) return;

        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        player.transform.position = destination.position;
    }
}
