using UnityEngine;

public class ScreenSpaceBooleanController : MonoBehaviour
{
    public Renderer targetRenderer; // The Renderer of the cube (make sure it is assigned)
    
    private Material runtimeMaterial;

    private void Start()
    {
        // Create a runtime instance of the material
        runtimeMaterial = targetRenderer.material;
    }

    private void Update()
    {
        if (runtimeMaterial != null)
        {
            runtimeMaterial.SetVector("_MaskPosition", transform.position);
            Debug.Log("Mask Position: " + transform.position);
        }
    }
}
