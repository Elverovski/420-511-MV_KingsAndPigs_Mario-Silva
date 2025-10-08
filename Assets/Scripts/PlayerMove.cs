using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private AudioClip sfxJump;
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 300f;

    private float x;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;
    private bool jump = false;
    private bool isGrounded = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        x = Input.GetAxis("Horizontal");
        animator.SetFloat("x", Mathf.Abs(x));

        if (x > 0f) spriteRenderer.flipX = false;
        if (x < 0f) spriteRenderer.flipX = true;

        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {
            jump = true;
            audioSource.PlayOneShot(sfxJump);
        }

        animator.SetBool("Attack", Input.GetKey(KeyCode.Space));
        animator.SetBool("isRunning", x != 0);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        if (jump)
        {
            jump = false;
            rb.AddForce(Vector2.up * jumpForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    public void ActivateHitbox()
    {
        attackHitbox.SetActive(true);
        StartCoroutine(DisableHitboxAfterDelay(0.1f));
    }

    private IEnumerator DisableHitboxAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        attackHitbox.SetActive(false);
    }
}
