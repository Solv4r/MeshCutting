using System.Collections.Generic;
using UnityEngine;

public class ShiftTime : MonoBehaviour
{

    private bool isNear = false;
    private Transform player;
    private bool inPast = false;
    private bool inPresent = true;
    private bool isReady = true;
    public GameObject PresentLight;
    public GameObject PastLight;
    public Shader PastShader;
    public Shader PastShaderBrick;
    public Shader PastShaderObject;
    public Shader PresentShader;
    public Shader PresentShaderBrick;
    public Shader PresentShaderObject;
    public List<GameObject> PastBoxObjects; // List of box objects (Boolean Shader & Reverse)
    public List<GameObject> PastBrickObjects; // List of objects with brick material (Boolean Shader 2 & Reverse 2)
    public List<GameObject> PastObjects; // List of other objects (Boolean 1 & Reverse 1)
    public CutoutConeController coneController;

    void Start()
    {
        // Enable present light, disable past light
        if (PresentLight != null) PresentLight.SetActive(true); // set lighting for present to true
        if (PastLight != null) PastLight.SetActive(false); // set lighting for past to false
        ApplyShaderToPresentBoxObjects(); // Apply present shader to all objects in the present
        ApplyShaderToPresentBoxObjects(); // Apply present shader to all brick objects in the present
        ApplyShaderToPresentObjects(); // Apply present shader to all other objects in the present
    }

    void Update()
    {
        if (isNear && inPresent && !inPast && isReady && Input.GetKeyDown(KeyCode.F))
        {
            ShiftTimeToPast();
        }

        if (isNear && inPast && !inPresent && isReady && Input.GetKeyDown(KeyCode.F))
        {
            ShiftTimeToPresent();
        }

    }
    void ApplyShaderToPastBoxObjects()
    {
        foreach (GameObject obj in PastBoxObjects)
        {
            if (obj != null)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.shader = PastShader; // Apply the past shader
                }
            }
        }
    }
    void ApplyShaderToPastBrickObjects()
    {
        foreach (GameObject obj in PastBrickObjects)
        {
            if (obj != null)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.shader = PastShaderBrick;
                }
            }
        }
    }
    void ApplyShaderToPastObjects()
    {
        foreach (GameObject obj in PastObjects)
        {
            if (obj != null)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.shader = PastShaderObject;
                }
            }
        }
    }
    void ApplyShaderToPresentBoxObjects()
    {
        foreach (GameObject obj in PastBoxObjects)
        {
            if (obj != null)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.shader = PresentShader; // Apply shader
                }
            }
        }
    }
    void ApplyShaderToPresentBricks()
    {
        foreach (GameObject obj in PastBrickObjects)
        {
            if (obj != null)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.shader = PresentShaderBrick;
                }
            }
        }
    }
    void ApplyShaderToPresentObjects()
    {
        foreach (GameObject obj in PastObjects)
        {
            if (obj != null)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.shader = PresentShaderObject;
                }
            }
        }
    }

    void ShiftTimeToPast()
    {
        inPast = true;
        inPresent = false;
        isReady = false;
        // switch lights
        if (PresentLight != null) PresentLight.SetActive(false); // set directional light for present to false
        if (PastLight != null) PastLight.SetActive(true); // set area light for past to true
        ApplyShaderToPastBoxObjects(); // Apply past shader to all objects in the past
        ApplyShaderToPastBrickObjects(); // Apply past shader to all brick objects in the past
        ApplyShaderToPastObjects(); // Apply past shader to all other objects in the past
        coneController.RefreshMaterials(); // Refresh materials in the cutout cone controller
        Invoke("ResetReady", 2f); // Reset ready state after 2 second
        Debug.Log("Time shifted to the past.");
    }
    void ShiftTimeToPresent()
    {
        inPast = false;
        inPresent = true;
        isReady = false;
        // switch lights
        if (PresentLight != null) PresentLight.SetActive(true); // set directional light for present to true
        if (PastLight != null) PastLight.SetActive(false); // set area light for past to false
        ApplyShaderToPresentBoxObjects(); // Apply present shader to all objects in the past
        ApplyShaderToPresentBricks(); // Apply present shader to all brick objects in the past
        ApplyShaderToPresentObjects(); // Apply present shader to all other objects in the past
        coneController.RefreshMaterials(); // Refresh materials in the cutout cone controller
        Invoke("ResetReady", 2f); // Reset ready state after 2 second
        Debug.Log("Time shifted to the present.");
    }
    void ResetReady()
    {
        isReady = true;
        Debug.Log("ShiftTime is ready again.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger area of TimeShiftBook.");
            isNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the trigger area TimeShiftBook.");
            isNear = false;
        }
    }

    public bool GetPresent()
    {
        return inPresent;
    }
    public bool GetPast()
    {
        return inPast;
    }
}
