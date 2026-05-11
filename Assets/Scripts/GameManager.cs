using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class Record
{
    public string playerName; // Додано: зберігання імені гравця (Пункт 8)
    public int maxCoins;
    public float timeOfAttempt;
}

[Serializable]
public class SaveData
{
    public int totalCollisions;
    public List<Record> highScores = new List<Record>();
}

public class GameManager : MonoBehaviour
{
    // 3. Забезпечення існування єдиного екземпляру (Singleton)
    public static GameManager Instance { get; private set; }

    // 4. Подія програшу
    public event Action OnLoseEvent;

    // Подія перемоги
    public event Action OnWinEvent;

    [Header("Налаштування рівня")]
    public int startingLives = 3;
    public float levelTimeLimit = 60f; 

    // Додано для UI (Лаб 4)
    [Header("Стан гри")]
    public string currentPlayerName; 
    public bool isGameActive = false; // Блокує таймер і фізику до логіну

    // 2. Глобальне сховище параметрів
    [HideInInspector] public int currentLives;
    [HideInInspector] public int collisions; 
    [HideInInspector] public float timeSinceStart; 
    [HideInInspector] public int collectedCoins; 
    
    private SaveData data = new SaveData();
    private bool isGameOver = false;
    private string saveFilePath;

    void Awake()
    {
        // Реалізація патерну Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        saveFilePath = Application.persistentDataPath + "/gamedata.json";
        
        OnLoseEvent += HandleLoseEvent;
        LoadData(); 
        
        // Зверни увагу: я прибрав звідси ResetLevelData(). 
        // Тепер гра скидається і починається ТІЛЬКИ після введення логіну (метод StartGame).
    }

    void Update()
    {
        // Таймер працює тільки тоді, коли гра активна і не програна
        if (!isGameActive || isGameOver) return;

        timeSinceStart += Time.deltaTime;

        if (timeSinceStart >= levelTimeLimit)
        {
            TriggerGameOver("Час вийшов!");
        }
    }

    // НОВИЙ МЕТОД: Викликається з UI після введення імені та натискання "Старт" (Пункт 7)
    public void StartGame(string playerName)
    {
        currentPlayerName = playerName;
        ResetLevelData();
        isGameActive = true;
        Debug.Log($"Гру розпочато! Гравець: {currentPlayerName}");
    }

    public void AddCoin()
    {
        if (!isGameActive || isGameOver) return;
        
        collectedCoins++;
        Debug.Log($"Монету зібрано! Всього: {collectedCoins}");
    }

    public void LoseLife()
    {
        if (!isGameActive || isGameOver) return;
        
        currentLives--;
        collisions++;
        data.totalCollisions = collisions;
        Debug.Log($"Втрачено життя! Залишилось: {currentLives}");

        if (currentLives <= 0)
        {
            TriggerGameOver("Закінчилися життя!");
        }
    }

    // Метод тепер публічний, щоб UIManager міг його викликати при фініші
    public void TriggerGameOver(string reason)
    {
        if (isGameOver) return;
        isGameOver = true;
        isGameActive = false; // Зупиняємо гру
        Debug.Log($"Причина завершення: {reason}");
        
        // Оновлено: додаємо ім'я гравця до рекорду (Пункт 8)
        Record newRecord = new Record { 
            playerName = currentPlayerName, 
            maxCoins = collectedCoins, 
            timeOfAttempt = timeSinceStart 
        };
        data.highScores.Add(newRecord);
        
        OnLoseEvent?.Invoke(); 
        SaveData(); 
    }

    private void HandleLoseEvent()
    {
        Debug.Log(">>> ГРУ ЗАВЕРШЕНО. Подію оброблено. <<<");
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Дані збережено у файл: {saveFilePath}");
    }

    private void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            data = JsonUtility.FromJson<SaveData>(json);
            collisions = data.totalCollisions;
            Debug.Log("Попередні дані успішно завантажено з JSON.");
        }
    }

    private void ResetLevelData()
    {
        currentLives = startingLives;
        collisions = 0;
        collectedCoins = 0;
        timeSinceStart = 0f;
        isGameOver = false;
    }

    // НОВИЙ МЕТОД: Повертає список рекордів для відображення в UI (Пункти 5 і 6)
    public List<Record> GetHighScores()
    {
        return data.highScores;
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }
    // Викликається при успішному завершенні рівня
    public void LevelComplete()
    {
        if (isGameOver || !isGameActive) return;
        
        isGameOver = true;
        isGameActive = false;
        Debug.Log("Рівень пройдено успішно!");

        // Зберігаємо рекорд
        Record newRecord = new Record { 
            playerName = currentPlayerName, 
            maxCoins = collectedCoins, 
            timeOfAttempt = timeSinceStart 
        };
        data.highScores.Add(newRecord);
        SaveData(); 

        // Викликаємо подію перемоги
        OnWinEvent?.Invoke();
    }
}