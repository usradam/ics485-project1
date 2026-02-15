using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] CanvasGroup gameOverCanvasGroup;
    [SerializeField] SphereCollisionDetector collisionDetector;
    [SerializeField] float fadeInDuration = 1f;

    void Start()
    {
        // Hide game over UI at start
        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.interactable = false;
            gameOverCanvasGroup.blocksRaycasts = false;
            gameOverCanvasGroup.alpha = 0;
        }
    }

    void OnEnable()
    {
        // Subscribe to crash event
        if (collisionDetector != null)
        {
            collisionDetector.OnPlayerCrashed += HandlePlayerCrashed;
        }
    }

    void OnDisable()
    {
        // Unsubscribe from crash event
        if (collisionDetector != null)
        {
            collisionDetector.OnPlayerCrashed -= HandlePlayerCrashed;
        }
    }

    private void HandlePlayerCrashed(SphereCollisionDetector detector)
    {
        // Show game over UI
        ShowGameOver();
    }

    private void ShowGameOver()
    {
        if (gameOverCanvasGroup != null)
        {
            // Fade in the game over screen
            StartCoroutine(FadeInGameOver());
        }
    }

    private System.Collections.IEnumerator FadeInGameOver()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            gameOverCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
            yield return null;
        }
        
        gameOverCanvasGroup.alpha = 1;
        gameOverCanvasGroup.interactable = true;
        gameOverCanvasGroup.blocksRaycasts = true;
    }

    // Called from button onClick in inspector
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
