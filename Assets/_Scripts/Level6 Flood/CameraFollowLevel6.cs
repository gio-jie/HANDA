using UnityEngine;

public class CameraFollowLevel6 : MonoBehaviour
{
    public Transform player;          // assign your Player GameObject
    public float smoothSpeed = 5f;    // higher = faster follow
    public Vector3 offset;            // camera offset from player

    private bool followPlayer = false;

    public void StartFollowing()
    {
        followPlayer = true;
    }

    void LateUpdate()
    {
        if(!player) return;

        Vector3 targetPos = new Vector3(
            transform.position.x,
            player.position.y + offset.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed);
    }
}
