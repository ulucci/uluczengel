using UnityEngine;

public class ClickTeleport : MonoBehaviour
{
    public Camera cam;
    public Transform player;

    public Transform floor1Target;
    public Transform floor2Target;
    public Transform floor3Target;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos =
                cam.ScreenToWorldPoint(Input.mousePosition);

            Collider2D hit =
                Physics2D.OverlapPoint(mousePos);

            if (hit == null) return;

            switch (hit.gameObject.name)
            {
                case "Door_1":
                    player.position = floor1Target.position;
                    break;

                case "Door_2":
                    player.position = floor2Target.position;
                    break;

                case "Door_3":
                    player.position = floor3Target.position;
                    break;
            }
        }
    }
}