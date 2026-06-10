using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ClickMove : MonoBehaviour
{
    [SerializeField] private Transform targetPoint;
    [SerializeField] private LayerMask wallLayer;

    private void OnMouseDown()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null || targetPoint == null) return;

        Vector3 from = player.transform.position;
        Vector3 to = targetPoint.position;

        if (!Physics.Linecast(from, to, wallLayer))
            player.transform.position = to;
    }
}
