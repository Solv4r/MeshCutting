using Unity.VisualScripting;
using UnityEngine;

public class TimeRayCone : MonoBehaviour
{
    public float coneAngle = 45f;
    public float coneRadius = 4f; // Must match the Sphere Collider radius
    public Transform rayOrigin;

    void OnTriggerStay(Collider other)
    {
        TimeStateObject tso = other.GetComponent<TimeStateObject>();
        if (tso == null || tso.visualObject == null) return;

        Bounds bounds = other.bounds;
        Vector3[] corners = GetBoundingBoxCorners(bounds);

        bool fullyInside = true;

        foreach (Vector3 corner in corners)
        {
            Vector3 directionToCorner = corner - rayOrigin.position;
            float angle = Vector3.Angle(rayOrigin.forward, directionToCorner);
            Vector3 dirNormalized = directionToCorner.normalized;
            float dot = Vector3.Dot(rayOrigin.forward, dirNormalized);
            float cosThreshold = Mathf.Cos(coneAngle * Mathf.Deg2Rad);
            
            Color lineColor = Color.green;
            if (dot < cosThreshold || directionToCorner.magnitude > coneRadius)
                lineColor = Color.red;

            Debug.DrawLine(rayOrigin.position, corner, lineColor);
            Debug.Log($"Corner {corner}: dot = {dot:F3}, cosThreshold = {cosThreshold:F3}, distance = {directionToCorner.magnitude:F2}");

            if (dot < cosThreshold || directionToCorner.magnitude > coneRadius)
            {
                fullyInside = false;
                break;
            }
        }

        tso.SetInFutureZone(fullyInside);
    }

    void OnTriggerExit(Collider other)
    {
        TimeStateObject tso = other.GetComponent<TimeStateObject>();
        if (tso != null)
            tso.SetInFutureZone(false);
    }

    /*
    void OnDrawGizmos()
    {
        if (rayOrigin == null) return;

        Gizmos.color = Color.yellow;

        Vector3 origin = rayOrigin.position;
        Vector3 forward = rayOrigin.forward;

        // Draw central line
        Gizmos.DrawLine(origin, origin + forward * coneRadius);

        // Draw cone circle base in 3D space
        int segments = 24;
        float angleStep = 360f / segments;

        Vector3[] circlePoints = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            Quaternion rot = Quaternion.AngleAxis(angle, forward); // rotate around forward axis

            Vector3 direction = rot * rayOrigin.up;
            Vector3 radialDir = Quaternion.AngleAxis(coneAngle, direction) * forward;
            Vector3 point = origin + radialDir.normalized * coneRadius;
            circlePoints[i] = point;

            Gizmos.DrawLine(origin, point); // draw cone edge line
        }

        // Draw base circle of cone
        for (int i = 0; i < segments; i++)
        {
            Vector3 current = circlePoints[i];
            Vector3 next = circlePoints[(i + 1) % segments];
            Gizmos.DrawLine(current, next);
        }
    }
    */



    Vector3[] GetBoundingBoxCorners(Bounds bounds)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        return new Vector3[]
        {
            center + new Vector3(+extents.x, +extents.y, +extents.z),
            center + new Vector3(+extents.x, +extents.y, -extents.z),
            center + new Vector3(+extents.x, -extents.y, +extents.z),
            center + new Vector3(+extents.x, -extents.y, -extents.z),
            center + new Vector3(-extents.x, +extents.y, +extents.z),
            center + new Vector3(-extents.x, +extents.y, -extents.z),
            center + new Vector3(-extents.x, -extents.y, +extents.z),
            center + new Vector3(-extents.x, -extents.y, -extents.z)
        };

    }
}
