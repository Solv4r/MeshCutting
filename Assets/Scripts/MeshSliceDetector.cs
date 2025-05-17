using UnityEngine;
using System.Collections.Generic;

public class MeshSliceDetector : MonoBehaviour
{
    public SlicerPlane slicerPlane;
    bool stopIteration = false;
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void Update()
    {
        if (slicerPlane == null) return;

        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        float angleMoved = Quaternion.Angle(transform.rotation, lastRotation);
        Debug.Log($"Distance moved: {distanceMoved}");
        if (angleMoved < 5f && Vector3.Distance(transform.position, lastPosition) < 0.1f)
        {
            //return;
        }
        lastPosition = transform.position;
        lastRotation = transform.rotation;

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        Plane plane = slicerPlane.GetPlane();

        bool isIntersecting = false;

        if (stopIteration) return; // Stoppe die Iteration, wenn bereits ein Schnitt gefunden wurde

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);

            float d0 = plane.GetDistanceToPoint(v0);
            float d1 = plane.GetDistanceToPoint(v1);
            float d2 = plane.GetDistanceToPoint(v2);

            // Wenn mindestens ein Punkt auf anderer Seite als ein anderer liegt → Schnitt
            bool pos = d0 > 0 || d1 > 0 || d2 > 0;
            bool neg = d0 < 0 || d1 < 0 || d2 < 0;

            if (pos && neg)
            {

                isIntersecting = true;
                Debug.Log("Schnitt gefunden zwischen Dreieck: " + i / 3 + " und der Ebene.");
                Debug.Log("Dreieckspunkte: " + v0 + ", " + v1 + ", " + v2);
                // Erstelle Gameobjekte an den Schnittpunkten
                CreateDebugPoints(v0, v1, v2);
                Debug.Log("Distanzen: " + d0 + ", " + d1 + ", " + d2);


                if ((d0 > 0 && d1 < 0) || (d0 < 0 && d1 > 0))
                {
                    // Schnittpunkt auf Kante v0-v1 berechnen
                    Vector3 intersectionPoint = Vector3.Lerp(v0, v1, Mathf.Abs(d0) / (Mathf.Abs(d0) + Mathf.Abs(d1)));
                    CreatePointMarker(intersectionPoint, Color.yellow, "Intersection_V0_V1");
                }

                if ((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0))
                {
                    // Schnittpunkt auf Kante v1-v2 berechnen
                    Vector3 intersectionPoint = Vector3.Lerp(v1, v2, Mathf.Abs(d1) / (Mathf.Abs(d1) + Mathf.Abs(d2)));
                    CreatePointMarker(intersectionPoint, Color.yellow, "Intersection_V1_V2");
                }

                if ((d2 > 0 && d0 < 0) || (d2 < 0 && d0 > 0))
                {
                    // Schnittpunkt auf Kante v2-v0 berechnen
                    Vector3 intersectionPoint = Vector3.Lerp(v2, v0, Mathf.Abs(d2) / (Mathf.Abs(d2) + Mathf.Abs(d0)));
                    CreatePointMarker(intersectionPoint, Color.yellow, "Intersection_V2_V0");
                }
            }
        }

        if (isIntersecting)
        {
            Debug.Log("Objekt wird von Plane durchschnitten!");
            //stopIteration = true;
        }
    }
    void CreateDebugPoints(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        CreatePointMarker(v0, Color.red, "V0");
        CreatePointMarker(v1, Color.green, "V1");
        CreatePointMarker(v2, Color.blue, "V2");
    }

    void CreatePointMarker(Vector3 position, Color color, string label)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * 0.05f; // klein machen
        sphere.name = "DebugPoint_" + label;

        // Optional: Farbe setzen
        Renderer renderer = sphere.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = color;
            renderer.material = mat;
        }

        // Optional: Gizmo deaktivieren
        Destroy(sphere.GetComponent<Collider>());
        Destroy(sphere, 3f);
    }
}
