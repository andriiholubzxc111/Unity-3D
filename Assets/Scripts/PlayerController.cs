using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Рух ")]
    public float forwardSpeed = 5f;      
    public float horizontalSpeed = 5f;   
    
    [Header("Стрибок")]
    public float jumpForce = 7f;         
    private bool isGrounded = true;     

    [Header("Прискорення")]
    public float sprintMultiplier = 2f;  
    public float maxSprintTime = 3f;     
    private float currentSprintTime;
    private bool isSprinting = false;

    [Header("Ями та Респаун ")]
    public float fallThreshold = -2f;    
    private Vector3 startPosition;       
    private Rigidbody rb;
    private bool gameIsFinished = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position; 
        currentSprintTime = maxSprintTime;  
    }

    void Update()
    {
        if (gameIsFinished || (GameManager.Instance != null && !GameManager.Instance.isGameActive)) return; 

    HandleJump();
    HandleSprint();
    CheckFall();
}

    void FixedUpdate()
{
    
    if (gameIsFinished || (GameManager.Instance != null && !GameManager.Instance.isGameActive)) 
    {
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); // Зупиняємо рух по осях X та Z
        return;
    }
    HandleMovement();
}

    private void HandleMovement()
    {
      
        float currentSpeed = isSprinting ? forwardSpeed * sprintMultiplier : forwardSpeed;

        
        float horizontalInput = Input.GetAxis("Horizontal");

       
        Vector3 forwardMove = transform.forward * currentSpeed;
        Vector3 horizontalMove = transform.right * horizontalInput * horizontalSpeed;

  
        Vector3 movement = forwardMove + horizontalMove;
        movement.y = rb.linearVelocity.y; 

        rb.linearVelocity = movement;
    }

    private void HandleJump()
    {
        
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; 
        }
    }

    private void HandleSprint()
    {
        
        if (Input.GetKey(KeyCode.LeftShift) && currentSprintTime > 0)
        {
            isSprinting = true;
            currentSprintTime -= Time.deltaTime; 
        }
        else
        {
            isSprinting = false;
      
            if (currentSprintTime < maxSprintTime)
            {
                currentSprintTime += Time.deltaTime;
            }
        }
    }

    private void CheckFall()
    {

        if (transform.position.y < fallThreshold)
        {
            transform.position = startPosition; 
            rb.linearVelocity = Vector3.zero;         
        }
    }

   
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            Debug.Log("Фініш! Рівень пройдено.");
            gameIsFinished = true; 
            rb.linearVelocity = Vector3.zero;

            // НОВЕ: Повідомляємо GameManager про перемогу
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LevelComplete();
            }
        }
    }
}