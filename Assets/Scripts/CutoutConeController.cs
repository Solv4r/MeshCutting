using System.Collections.Generic;
using UnityEngine;

public class CutoutConeController : MonoBehaviour
{
    public Transform coneOrigin;        // The tip of the cone
    public Transform coneTransform;     // The cone object (for direction)
    public float coneAngle = -10f;      // Cone angle in degrees
    public float coneRange = 64f;       // Cone range in world units

    private List<Material> affectedMaterials = new List<Material>();
    private Shader targetShader; // Boolean Shader
    private Shader targetShader2; // Boolean Shader 1
    private Shader targetShader3; // Boolean Shader 2
    private Shader targetShader4; // Shader REVERSED
    private Shader targetShader5; // Shader 1 REVERSED
    private Shader targetShader6; // Shader 2 REVERSED

    void Start()
    {
        // You must set this to match your shader name exactly
        targetShader = Shader.Find("Shader Graphs/Cutout Shader BOOLEAN");
        targetShader2 = Shader.Find("Shader Graphs/Cutout Shader BOOLEAN 1");
        targetShader3 = Shader.Find("Shader Graphs/Cutout Shader BOOLEAN 2");
        targetShader4 = Shader.Find("Shader Graphs/Cutout Shader REVERSED");
        targetShader5 = Shader.Find("Shader Graphs/Cutout Shader 1 REVERSED");
        targetShader6 = Shader.Find("Shader Graphs/Cutout Shader 2 REVERSED");
        if (targetShader == null || targetShader2 == null || targetShader3 == null || targetShader4 == null || targetShader5 == null || targetShader6 == null)
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
                if (mat.shader == targetShader || mat.shader == targetShader2 || mat.shader == targetShader3 || mat.shader == targetShader4 || mat.shader == targetShader5 || mat.shader == targetShader6)
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

    public void RefreshMaterials()
    {
        affectedMaterials.Clear();

        Renderer[] renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (mat.shader == targetShader || mat.shader == targetShader2 || mat.shader == targetShader3 || mat.shader == targetShader4 || mat.shader == targetShader5 || mat.shader == targetShader6)
                {
                    // Make sure we get a unique instance of the material (not sharedMaterial)
                    Material runtimeMat = rend.material;
                    affectedMaterials.Add(runtimeMat);
                }
            }
        }
    }
}
