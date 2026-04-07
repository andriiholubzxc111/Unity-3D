using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

// Клас для таблиці рекордів
[Serializable]
public class Record
{
    public int maxCoins;
    public float timeOfAttempt;
}

// Клас для збереження даних у JSON
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

    [Header("Налаштування рівня")]
    public int startingLives = 3;
    public float levelTimeLimit = 60f; 

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
            DontDestroyOnLoad(gameObject); // Зберігаємо об'єкт при перезавантаженні сцен
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Шлях для збереження JSON файлу
        saveFilePath = Application.persistentDataPath + "/gamedata.json";
        
        // 5. Підписка обробника події
        OnLoseEvent += HandleLoseEvent;

        LoadData(); // 7. Завантаження даних при запуску
        ResetLevelData();
    }

    void Update()
    {
        if (isGameOver) return;

        // Оновлюємо час від запуску рівня
        timeSinceStart += Time.deltaTime;

        // 6. Перевірка обмеження часу
        if (timeSinceStart >= levelTimeLimit)
        {
            TriggerGameOver("Час вийшов!");
        }
    }

    public void AddCoin()
    {
        if (isGameOver) return;
        collectedCoins++;
        Debug.Log($"Монету зібрано! Всього: {collectedCoins}");
    }

    public void LoseLife()
    {
        if (isGameOver) return;
        
        currentLives--;
        collisions++;
        data.totalCollisions = collisions;
        Debug.Log($"Втрачено життя! Залишилось: {currentLives}");

       
        if (currentLives <= 0)
        {
            TriggerGameOver("Закінчилися життя!");
        }
    }

    private void TriggerGameOver(string reason)
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log($"Причина програшу: {reason}");
        
        // Додаємо спробу до таблиці рекордів
        Record newRecord = new Record { maxCoins = collectedCoins, timeOfAttempt = timeSinceStart };
        data.highScores.Add(newRecord);
        
        // Викликаємо подію програшу
        OnLoseEvent?.Invoke(); 
        
        // 7. Збереження після запису результатів
        SaveData(); 
    }

    // 5. Обробник події, який виводить повідомлення в консоль
    private void HandleLoseEvent()
    {
        Debug.Log(">>> ПРОГРАШ! Подію оброблено. Гру завершено. <<<");
    }

    // 7. Збереження даних у JSON
    private void SaveData()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log($"Дані збережено у файл: {saveFilePath}");
    }

    // 7. Завантаження даних з JSON
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
        collectedCoins = 0;
        timeSinceStart = 0f;
        isGameOver = false;
    }

    // Збереження даних при штатному закритті гри
    private void OnApplicationQuit()
    {
        SaveData();
    }
}