using UnityEngine;

[ExecuteInEditMode]
public class SlicerPlane : MonoBehaviour
{
    [Header("Visualisierung")]
    public float planeSize = 1f;
    public Color planeColor = new Color(1f, 0f, 0f, 0.2f); // halbtransparentes Rot

    private void OnDrawGizmos()
    {
        Gizmos.color = planeColor;

        // Lokale Ausrichtung der Plane: normal ist transform.up
        Vector3 center = transform.position;
        Vector3 normal = transform.up;

        // Eckpunkte einer Fläche zeichnen
        Vector3 right = transform.right * planeSize;
        Vector3 forward = transform.forward * planeSize;

        Vector3 p1 = center + right + forward;
        Vector3 p2 = center + right - forward;
        Vector3 p3 = center - right - forward;
        Vector3 p4 = center - right + forward;

        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        // Fläche ausfüllen
        Gizmos.DrawLine(center, center + normal); // Normal zeigen
    }

    /// <summary>
    /// Liefert eine mathematische Ebene zurück, die zum Schneiden verwendet werden kann
    /// </summary>
    public Plane GetPlane()
    {
        return new Plane(transform.up, transform.position);
    }
}
