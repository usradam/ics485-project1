using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Game Over")]
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] CanvasGroup gameOverCanvasGroup;
    [SerializeField] SphereCollisionDetector collisionDetector;
    [SerializeField] float fadeInDuration = 1f;
    
    [Header("Win Condition")]
    [SerializeField] CanvasGroup winCanvasGroup;
    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] CarMovement carMovement;
    [SerializeField] Rigidbody sphereRigidbody;
    
    private int totalEnemies;
    private int enemiesKilled = 0;

    void Start()
    {
        // Hide game over UI at start
        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.interactable = false;
            gameOverCanvasGroup.blocksRaycasts = false;
            gameOverCanvasGroup.alpha = 0;
        }
        
        // Hide win UI at start
        if (winCanvasGroup != null)
        {
            winCanvasGroup.interactable = false;
            winCanvasGroup.blocksRaycasts = false;
            winCanvasGroup.alpha = 0;
        }
        
        // Count total enemies in the scene
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
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
    
    public void OnEnemyKilled()
    {
        enemiesKilled++;
        
        if (enemiesKilled >= totalEnemies)
        {
            ShowWinScreen();
        }
    }
    
    private void ShowWinScreen()
    {
        // Stop the car from moving
        if (carMovement != null)
        {
            carMovement.SetExploded();
        }
        
        // Stop the sphere's momentum
        if (sphereRigidbody != null)
        {
            sphereRigidbody.linearVelocity = Vector3.zero;
            sphereRigidbody.angularVelocity = Vector3.zero;
        }
        
        if (winCanvasGroup != null)
        {
            StartCoroutine(FadeInWinScreen());
        }
    }
    
    private System.Collections.IEnumerator FadeInWinScreen()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            winCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeInDuration);
            yield return null;
        }
        
        winCanvasGroup.alpha = 1;
        winCanvasGroup.interactable = true;
        winCanvasGroup.blocksRaycasts = true;
    }
}
