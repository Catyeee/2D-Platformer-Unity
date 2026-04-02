using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private bool startMovingRight = true;
    [SerializeField] private bool useRigidbody = true;

    private Vector2 startPosition;
    private Vector2 targetA;
    private Vector2 targetB;
    private Vector2 currentTarget;
    private Rigidbody2D rb;

    private void Awake()
    {
        startPosition = transform.position;
        targetA = startPosition + Vector2.left * patrolDistance;
        targetB = startPosition + Vector2.right * patrolDistance;
        currentTarget = startMovingRight ? targetB : targetA;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var nextPosition = Vector2.MoveTowards(CurrentPosition, currentTarget, speed * Time.fixedDeltaTime);
        MoveTo(nextPosition);

        if (Vector2.Distance(CurrentPosition, currentTarget) <= 0.01f)
        {
            currentTarget = currentTarget == targetA ? targetB : targetA;
        }
    }

    private Vector2 CurrentPosition => useRigidbody && rb != null ? rb.position : (Vector2)transform.position;

    private void MoveTo(Vector2 position)
    {
        if (useRigidbody && rb != null)
        {
            rb.MovePosition(position);
        }
        else
        {
            transform.position = position;
        }
    }

    private void OnDrawGizmosSelected()
    {
        var pos = (Vector2)transform.position;
        var left = pos + Vector2.left * patrolDistance;
        var right = pos + Vector2.right * patrolDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(left, right);
        Gizmos.DrawSphere(left, 0.05f);
        Gizmos.DrawSphere(right, 0.05f);
    }
}
