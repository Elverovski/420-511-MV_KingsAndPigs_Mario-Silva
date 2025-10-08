using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f; 
        Debug.Log("Game over");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f; 
    }

    public void OnRestartButton()
    {
        Time.timeScale = 1f;

        if (playerHealth != null)
            playerHealth.CallRespawnOrReload();
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Start");
    }
}
