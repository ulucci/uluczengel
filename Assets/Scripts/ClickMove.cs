using UnityEngine;

public class ClickMove : MonoBehaviour
{
    public Transform targetPoint;

    private void OnMouseDown()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            player.transform.position =
                targetPoint.position;
        }
    }
}