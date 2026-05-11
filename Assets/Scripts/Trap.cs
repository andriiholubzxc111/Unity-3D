using UnityEngine;

public class Trap : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
{
    
    Debug.Log("До пастки доторкнувся: " + other.name); 

    if (other.CompareTag("Player"))
    {
        GameManager.Instance.LoseLife();
        Destroy(gameObject); 
    }
}
}