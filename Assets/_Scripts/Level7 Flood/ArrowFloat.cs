using UnityEngine;

public class ArrowFloat : MonoBehaviour
{
    public enum Direction { Left, Right }
    public Direction arrowDirection = Direction.Right;

    [Header("Movement Settings")]
    public float moveDistance = 10f;
    public float moveSpeed = 2f;

    private Vector3 startPos;
    private Vector3 moveDir;

    void Start()
    {
        startPos = transform.localPosition;
        moveDir = (arrowDirection == Direction.Right) ? Vector3.right : Vector3.left;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.localPosition = startPos + moveDir * offset;
    }
}