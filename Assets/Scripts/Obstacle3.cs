using UnityEngine;

public class Obstacle3 : MonoBehaviour
{
    public float speed = 2f;      
    public float maxAngle = 75f;  

    void Update()
    {
       
        float currentAngle = maxAngle * Mathf.Sin(Time.time * speed);

      
        
        transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
    }
}