using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOut2D : MonoBehaviour
{
    public float fadeDuration = 2f;     
    public bool destroyAfter = true;     
    private SpriteRenderer sr;
    private bool fading = false;
    private float t = 0f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!fading) return;

        t += Time.deltaTime;
        float alpha = Mathf.Clamp01(1f - (t / fadeDuration));
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

        if (t >= fadeDuration)
        {
            SceneManager.LoadScene("Victory");

            if (destroyAfter) Destroy(gameObject);
            else gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Door"))
        {
            fading = true;
        }
    }
}
