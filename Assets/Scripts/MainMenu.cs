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
    
    [Header("Enemy Counter")]
    [SerializeField] GameObject enemyCounterPanel;
    [SerializeField] TextMeshProUGUI enemyCounterText;
    
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
        
        // Hide enemy counter initially
        if (enemyCounterPanel != null)
        {
            enemyCounterPanel.SetActive(false);
        }
        
        // Count total enemies in the scene
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        Debug.Log($"Total enemies found: {totalEnemies}");
        
        // Only show counter if we're in the game scene (not main menu which is scene 0)
        if (SceneManager.GetActiveScene().buildIndex > 0 && enemyCounterPanel != null)
        {
            enemyCounterPanel.SetActive(true);
            UpdateEnemyCounter();
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

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("User quit the game.");
    }
    
    public void OnEnemyKilled()
    {
        enemiesKilled++;
        Debug.Log($"Enemy killed! {enemiesKilled}/{totalEnemies}");
        
        // Update the counter display
        UpdateEnemyCounter();
        
        if (enemiesKilled >= totalEnemies)
        {
            ShowWinScreen();
        }
    }
    
    private void UpdateEnemyCounter()
    {
        if (enemyCounterText != null)
        {
            enemyCounterText.text = $"Enemies Killed: {enemiesKilled}/{totalEnemies}";
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
