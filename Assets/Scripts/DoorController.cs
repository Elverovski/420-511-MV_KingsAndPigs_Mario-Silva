using UnityEngine;
using UnityEngine.SceneManagement;
public class DoorController : MonoBehaviour
{
    [Header("Tag de la porte")]
    public string doorTag;

    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    private void OnMouseDown()
    {
        if (doorTag == "Start")
        {
            SceneManager.LoadScene("Play");
        }
        else if (doorTag == "Quit")
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    private void OnMouseEnter() => sr.color = Color.orange;
    private void OnMouseExit() => sr.color = originalColor;
}
