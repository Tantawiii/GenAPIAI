using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BattleGame.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Over UI")]
        public GameObject gameOverPanel;
        public Button restartButton;
        public Button quitButton;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Debug.Log("GameManager initialized");
            }
            else
            {
                Destroy(gameObject);
            }

            // Verify UI components
            if (gameOverPanel == null)
                Debug.LogError("GameOverPanel not assigned in GameManager!");
            if (restartButton == null)
                Debug.LogError("RestartButton not assigned in GameManager!");
            if (quitButton == null)
                Debug.LogError("QuitButton not assigned in GameManager!");
        }

        private void Start()
        {
            // Hide game over panel initially
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }

            // Set up button listeners
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(QuitGame);
            }

            // Make sure time scale is set correctly at start
            Time.timeScale = 1f;
        }

        public void GameOver()
        {
            Debug.Log("Game Over triggered!");
            if (gameOverPanel != null)
            {
                // Destroy all bullets in the scene
                GameObject[] bullets = GameObject.FindGameObjectsWithTag("PlayerBullet");
                foreach (GameObject bullet in bullets)
                {
                    Destroy(bullet);
                }
                
                gameOverPanel.SetActive(true);
                Time.timeScale = 0f; // Pause the game
            }
            else
            {
                Debug.LogError("GameOverPanel is null!");
            }
        }

        public void RestartGame()
        {
            Time.timeScale = 1f; // Reset time scale
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
} 