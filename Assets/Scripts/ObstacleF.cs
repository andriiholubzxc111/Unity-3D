using UnityEngine;

public class ObstacleF : MonoBehaviour
{
    public float R = 3f;     
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

        
        float x = R * Mathf.Pow(Mathf.Cos(t), 3);
        float y = R * Mathf.Pow(Mathf.Sin(t), 3);

        Vector3 newPos = startPos + new Vector3(x, 0, y); 

        
        Vector3 deltaVector = newPos - transform.position;
        transform.Translate(deltaVector, Space.World);
    }
}