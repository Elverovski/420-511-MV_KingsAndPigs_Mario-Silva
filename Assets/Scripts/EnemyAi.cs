using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public enum State { Patrol, Chase, Attack }
    public State currentState = State.Patrol;

    public Transform leftPoint;
    public Transform rightPoint;

    public Transform player;
    public PlayerHealth playerHealth;
    public Animator animator;

    public float speed = 2f;
    public float chaseSpeed = 3.2f;
    public float stopDistance = 0.9f;

    public float detectionRadius = 6f;
    public float lostRadius = 8f;

    public float attackRange = 1.1f;
    public float attackCooldown = 1.2f;
    public string attackTrigger = "Attack";

    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;

    private Rigidbody2D rb;
    private bool toRight = true;
    private float lastAttackTime;
    private float originalScaleX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScaleX = Mathf.Abs(transform.localScale.x);
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                if (PlayerInDetection()) currentState = State.Chase;
                break;
            case State.Chase:
                if (!player || Vector2.Distance(transform.position, player.position) > lostRadius)
                    currentState = State.Patrol;
                else
                    Chase();
                break;
            case State.Attack:
                Attack();
                break;
        }

        if (animator)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("IsChasing", currentState == State.Chase);
        }
    }

    private void Patrol()
    {
        if (!leftPoint || !rightPoint) return;
        if (!IsGrounded())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float moveDir = toRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(moveDir * speed, rb.linearVelocity.y);
        FlipTowards(moveDir);

        if (toRight && transform.position.x >= rightPoint.position.x - 0.05f)
            toRight = false;
        else if (!toRight && transform.position.x <= leftPoint.position.x + 0.05f)
            toRight = true;
    }

    private void Chase()
    {
        if (!player) return;

        float dirX = player.position.x - transform.position.x;
        float distance = Mathf.Abs(dirX);

        if (distance > stopDistance)
        {
            float moveDir = Mathf.Sign(dirX);
            rb.linearVelocity = new Vector2(moveDir * chaseSpeed, rb.linearVelocity.y);
            FlipTowards(dirX);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            currentState = State.Attack;
        }
    }

    private void Attack()
    {
        rb.linearVelocity = Vector2.zero;
        if (!playerHealth) { currentState = State.Patrol; return; }

        float distance = Vector2.Distance(transform.position, playerHealth.transform.position);

        if (distance <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                if (animator != null && !string.IsNullOrEmpty(attackTrigger))
                    animator.SetTrigger(attackTrigger);

                lastAttackTime = Time.time;
            }
        }
        else
        {
            currentState = State.Chase;
        }

        FlipTowards(player.position.x - transform.position.x);
    }

    public void DealDamage()
    {
        if (playerHealth == null) return;
        float distanceToPlayer = Vector2.Distance(transform.position, playerHealth.transform.position);
        if (distanceToPlayer <= attackRange)
            playerHealth.TakeDamage(1);
    }

    private bool PlayerInDetection()
    {
        if (!player) return false;
        return Vector2.Distance(transform.position, player.position) <= detectionRadius;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void FlipTowards(float dirX)
    {
        if (dirX == 0) return;
        Vector3 scale = transform.localScale;
        scale.x = originalScaleX * Mathf.Sign(dirX);
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, lostRadius);
        if (groundCheck)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
