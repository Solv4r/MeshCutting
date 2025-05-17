using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light directionalLight; // Dein Sonnen-Licht
    public float rotationSpeed = 1.0f; // Grad pro Sekunde

    void Update()
    {
        if (directionalLight != null)
        {
            directionalLight.transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
            float dot = Vector3.Dot(directionalLight.transform.forward, Vector3.down);
            directionalLight.intensity = Mathf.Clamp01(dot);
        }
    }
}
