using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("UI - Cœurs")]
    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;

    [SerializeField] private PlayerRespawn respawnSystem;
    [SerializeField] private GameOverScript gameOverScript;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("Vie du joueur")]
    public int maxHealth = 3;
    public int currentHealth;

    private Animator animator;
    private SpriteRenderer sr;
    private Color originalColor;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (Time.timeScale == 0f)
        {
            Debug.Log(" Time.timeScale = 1");
            Time.timeScale = 1f;
        }

        if (gameOverScript == null)
            gameOverScript = FindFirstObjectByType<GameOverScript>();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        UpdateHearts();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"Le joueur récupère {amount} HP. HP = {currentHealth}");
        UpdateHearts();
    }

    public void TakeDamage(int dmg)
    {
        currentHealth = Mathf.Clamp(currentHealth - dmg, 0, maxHealth);
        Debug.Log($"Le joueur prend {dmg} dégâts. HP restants = {currentHealth}");

        if (sr != null)
            StartCoroutine(FlashRed());

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float knockbackForceX = 5f;
            float knockbackForceY = 7f;
            float direction = GetComponent<SpriteRenderer>().flipX ? 1f : -1f;

            if (IsGrounded())
                rb.linearVelocity = new Vector2(direction * knockbackForceX, knockbackForceY);
            else
                rb.linearVelocity = new Vector2(direction * knockbackForceX * 0.5f, rb.linearVelocity.y);
        }

        UpdateHearts();

        if (currentHealth <= 0)
            Die();
    }

    private System.Collections.IEnumerator FlashRed()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    private async void Die()
    {
        Debug.Log("Le joueur est mort !");
        GetComponent<PlayerMove>().enabled = false;
        animator.SetTrigger("Dead");

        await Task.Delay(3000);

        if (this == null) return;

        if (gameOverScript != null)
            gameOverScript.Show();
    }

  
    public void CallRespawnOrReload()
    {
        if (gameOverScript != null)
            gameOverScript.Hide();

        if (respawnSystem != null && respawnSystem.HasCheckpoint())
        {
            respawnSystem.Respawn();
            currentHealth = maxHealth;
            animator.ResetTrigger("Dead");
            animator.Play("Idle");
            GetComponent<PlayerMove>().enabled = true;
            UpdateHearts();

            foreach (HealingItem item in FindObjectsByType<HealingItem>(FindObjectsSortMode.None))
                item.ResetItem();

            Debug.Log(" Respawn effectue !");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


    private void UpdateHearts()
    {
        if (heart1 != null) heart1.SetActive(currentHealth >= 1);
        if (heart2 != null) heart2.SetActive(currentHealth >= 2);
        if (heart3 != null) heart3.SetActive(currentHealth >= 3);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Healing"))
        {
            HealingItem heal = collision.GetComponent<HealingItem>();
            if (heal != null && heal.CanHeal())
            {
                Heal(1);
                heal.Use();
            }
        }
        else if (collision.CompareTag("Trap"))
        {
            TakeDamage(1);
            if (currentHealth <= 0)
                Die();
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }
}
