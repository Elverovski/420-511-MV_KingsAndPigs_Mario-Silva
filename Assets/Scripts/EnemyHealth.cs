using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public Color damageColor = Color.red;
    public Color originalColor = Color.white;

    private SpriteRenderer sr;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage()
    {
        if (isDead) return;

        StartCoroutine(FlashColor());
        Die();
    }

    private IEnumerator FlashColor()
    {
        if (sr != null)
        {
            sr.color = damageColor;
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;
        }
    }

    public void Die()
    {
        isDead = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        if (animator != null)
            animator.SetTrigger("Dead");

        Destroy(gameObject, 1.5f);
    }
}
