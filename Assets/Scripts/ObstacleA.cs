using UnityEngine;

public class ObstacleA : MonoBehaviour
{
    public Vector3 pointA; 
    public Vector3 pointB; 
    public float speed = 2f;
    
    private Vector3 target;

    void Start()
    {
        
        target = pointB;
        transform.position = pointA; 
    }

    void Update()
    {
        
        Vector3 direction = (target - transform.position).normalized;
        float distanceThisFrame = speed * Time.deltaTime;

        
        if (Vector3.Distance(transform.position, target) <= distanceThisFrame)
        {
            
            transform.position = target;
            target = target == pointA ? pointB : pointA;
        }
        else
        {
            
            transform.Translate(direction * distanceThisFrame, Space.World);
        }
    }
}