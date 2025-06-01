using System.Collections.Generic;
using UnityEngine;

public class CutoutConeController : MonoBehaviour
{
    public Transform coneOrigin;      // The tip of the cone
    public Transform coneTransform;   // The cone object (for direction)
    public float coneAngle = -10f;    // Cone angle in degrees
    public float coneRange = 64f;

    private List<Material> affectedMaterials = new List<Material>();
    private Shader targetShader; // Assign this to your custom cutout shader
    private Shader targetShader2; // Assign this to your custom cutout shader

    void Start()
    {
        // You must set this to match your shader name exactly
        targetShader = Shader.Find("Shader Graphs/Cutout Shader BOOLEAN");
        targetShader2 = Shader.Find("Shader Graphs/Cutout Shader BOOLEAN 2");
        if (targetShader == null || targetShader2 == null)
        {
            Debug.LogError("Cutout shader not found!");
            return;
        }

        // Find all renderers in the scene
        Renderer[] renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);

        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (mat.shader == targetShader || mat.shader == targetShader2)
                {
                    // Make sure we get a unique instance of the material (not sharedMaterial)
                    Material runtimeMat = rend.material;
                    affectedMaterials.Add(runtimeMat);
                }
            }
        }
    }

    void Update()
    {
        Vector3 origin = coneOrigin.position;
        Vector3 direction = coneOrigin.forward.normalized;

        foreach (Material mat in affectedMaterials)
        {
            mat.SetVector("_ConeOrigin", origin);
            mat.SetVector("_ConeDirection", direction);
            mat.SetFloat("_ConeAngle", coneAngle);
            mat.SetFloat("_ConeRange", coneRange);
        }
    }
}
