using UnityEngine;

public class ObstacleD : MonoBehaviour
{
    public float radius = 1f; 
    public float speed = 2f;  
    
    private float t = 0f;
    private Vector3 startPos;

    void Start()
    {
        
        startPos = transform.position;
    }

    void Update()
    {
        t += Time.deltaTime * speed;

       
        float x = radius * (t - Mathf.Sin(t));
        float y = radius * (1 - Mathf.Cos(t));

        
        Vector3 newPos = startPos + new Vector3(x, y, 0);

        
        Vector3 deltaVector = newPos - transform.position;
        transform.Translate(deltaVector, Space.World);
    }
}