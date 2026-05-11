using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 
using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("UI Панелі")]
    public GameObject loginPanel;
    public GameObject mainMenuPanel;       // НОВЕ: Головне меню
    public GameObject hudPanel;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;  // НОВЕ: Панель перемоги
    public GameObject leaderboardPanel;

    [Header("Елементи логіну")]
    public TMP_InputField nameInputField;
    
    [Header("Тексти HUD (Під час гри)")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI collisionsText;
    
    [Header("Тексти меню та рекордів")]
    public TextMeshProUGUI gameOverMessageText;
    public TextMeshProUGUI leaderboardText;

    void Start()
    {
        ShowPanel(loginPanel);
        Time.timeScale = 0f; 

        if (GameManager.Instance != null)
        {
            // Підписуємося на програш і на перемогу
            GameManager.Instance.OnLoseEvent += () => ShowGameOver("Гру завершено! Життя вичерпано.");
            GameManager.Instance.OnWinEvent += ShowWinScreen;
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameActive)
        {
            timerText.text = $"Час: {GameManager.Instance.timeSinceStart:F1} с";
            collisionsText.text = $"Зіткнення: {GameManager.Instance.collisions}";
        }
    }

    public void OnStartClicked()
    {
        if (!string.IsNullOrEmpty(nameInputField.text))
        {
            GameManager.Instance.StartGame(nameInputField.text);
            ShowPanel(hudPanel);
            Time.timeScale = 1f; 
        }
    }

    // --- МЕТОДИ СТАНІВ ГРИ ---

    public void ShowGameOver(string message)
    {
        GameManager.Instance.isGameActive = false;
        ShowPanel(gameOverPanel);
        if(gameOverMessageText != null) gameOverMessageText.text = message;
        Time.timeScale = 0f; 
    }

    // НОВЕ: Метод для перемоги
    public void ShowWinScreen()
    {
        GameManager.Instance.isGameActive = false;
        ShowPanel(levelCompletePanel);
        Time.timeScale = 0f; 
    }

    // НОВЕ: Метод для Головного меню
    public void OpenMainMenu()
    {
        ShowPanel(mainMenuPanel);
    }

    // --- ТАБЛИЦЯ РЕКОРДІВ ---

    public void ShowLeaderboard()
    {
        leaderboardPanel.SetActive(true); 
        leaderboardText.text = "ТАБЛИЦЯ РЕКОРДІВ\n\n";

        if (GameManager.Instance != null)
        {
            var records = GameManager.Instance.GetHighScores(); 
            var top3Records = records
                .OrderByDescending(r => r.maxCoins)
                .ThenBy(r => r.timeOfAttempt)
                .Take(3);

            int place = 1; 
            foreach (var rec in top3Records)
            {
                leaderboardText.text += $"{place}. {rec.playerName} | {rec.maxCoins} монет | {rec.timeOfAttempt:F1}с\n";
                place++;
            }
        }
    }

    public void CloseLeaderboard()
    {
        leaderboardPanel.SetActive(false);
    }

    // --- КНОПКИ УПРАВЛІННЯ (Пункт 1) ---

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Debug.Log("Вихід з гри...");
        Application.Quit();
    }

    // Універсальний метод перемикання (оновлено)
    private void ShowPanel(GameObject panel)
    {
        if(loginPanel != null) loginPanel.SetActive(panel == loginPanel);
        if(mainMenuPanel != null) mainMenuPanel.SetActive(panel == mainMenuPanel);
        if(hudPanel != null) hudPanel.SetActive(panel == hudPanel);
        if(gameOverPanel != null) gameOverPanel.SetActive(panel == gameOverPanel);
        if(levelCompletePanel != null) levelCompletePanel.SetActive(panel == levelCompletePanel);
        if(leaderboardPanel != null) leaderboardPanel.SetActive(false); 
    }
}