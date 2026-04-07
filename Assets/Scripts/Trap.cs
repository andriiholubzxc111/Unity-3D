using UnityEngine;

public class Trap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.LoseLife();
            // Знищуємо об'єкт пастки, щоб вона не зняла всі життя за одну секунду
            Destroy(gameObject); 
        }
    }
}