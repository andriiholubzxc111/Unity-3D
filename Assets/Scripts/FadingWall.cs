using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class FadingWall : MonoBehaviour
{
    [Header("Налаштування часу (в секундах)")]
    public float solidDuration = 2f;       
    public float transparentDuration = 2f; 

    [Header("Матеріали")]
    public Material solidMaterial;        
    public Material transparentMaterial;   

    private BoxCollider wallCollider;
    private MeshRenderer meshRenderer;
    private float timer;
    private bool isSolid = true; 

    void Start()
    {
        wallCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        
        
        timer = solidDuration;
        UpdateWallState();
    }

    void Update()
    {
        
        timer -= Time.deltaTime;

        
        if (timer <= 0f)
        {
            isSolid = !isSolid; 
            
            
            timer = isSolid ? solidDuration : transparentDuration; 
            
            UpdateWallState();
        }
    }

    private void UpdateWallState()
    {
        if (isSolid)
        {
            
            wallCollider.isTrigger = false;
            meshRenderer.material = solidMaterial;
        }
        else
        {
            
            wallCollider.isTrigger = true;
            meshRenderer.material = transparentMaterial;
        }
    }
}