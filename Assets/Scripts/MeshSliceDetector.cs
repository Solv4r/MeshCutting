using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MeshSliceDetector : MonoBehaviour
{
    [SerializeField] public SlicerPlane slicerPlane;

    // Original Mesh zwischenspeichern
    // Mit der Hoffnung, dass das Mesh benutzt werden kann, 
    // wenn die Plane sich zurückbewegt, um es "zurückzuschneiden"
    private Mesh originalMesh;
    private MeshFilter originalMeshFilter;


    private Mesh cutMesh;
    private MeshFilter cutMeshFilter;

    // Position und Rotation des Objekts, zum Filtern der Updates
    // (Verhindert unnötige Berechnungen, wenn sich nichts geändert hat)
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    // Position und Rotation der Slicer-Plane
    // (Verhindert unnötige Berechnungen, wenn sich nichts geändert hat)

    private Plane plane;
    private Vector3 lastPlanePosition;
    private Quaternion lastPlaneRotation;

    // Flag, ob das Mesh mit der Plane schneidet
    void Start()
    {
        // Speichere das Original-Mesh
        originalMeshFilter = GetComponent<MeshFilter>();
        originalMesh = originalMeshFilter.sharedMesh;
        // Speichere die Startposition und -rotation
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        // Speichere die Startposition und -rotation der Slicer-Plane
        lastPlanePosition = slicerPlane.transform.position;
        lastPlaneRotation = slicerPlane.transform.rotation;
    }

    void Update()
    {
        // Wenn die Plane oder das Objekt sich nicht bewegt hat, nichts tun
        if (!FilterMovement())
        {
            return;
        }
        cutMesh = Instantiate(originalMesh);
        cutMeshFilter = gameObject.GetComponent<MeshFilter>();
        Vector3[] vertices = cutMesh.vertices;
        int[] triangles = cutMesh.triangles;
        List<Vector3> newVertices = new();
        List<int> newTriangles = new();
        List<Vector3> tempIntersection = new();


        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);

            float d0 = slicerPlane.GetPlane().GetDistanceToPoint(v0);
            float d1 = slicerPlane.GetPlane().GetDistanceToPoint(v1);
            float d2 = slicerPlane.GetPlane().GetDistanceToPoint(v2);

            // Wenn mindestens ein Punkt auf anderer Seite als ein anderer liegt → Schnitt
            bool pos = d0 > 0 || d1 > 0 || d2 > 0;
            bool neg = d0 < 0 || d1 < 0 || d2 < 0;

            if (pos && neg)
            {

                List<Vector3> tempPoints = new();
                if (d0 > 0)
                {
                    tempPoints.Add(v0);
                }
                if (d1 > 0)
                {
                    tempPoints.Add(v1);
                }
                if (d2 > 0)
                {
                    tempPoints.Add(v2);
                }
                if ((d0 > 0 && d1 < 0) || (d0 < 0 && d1 > 0))
                {
                    // Schnittpunkt auf Kante v0-v1 berechnen
                    Vector3 intersectionPoint = Vector3.Lerp(v0, v1, Mathf.Abs(d0) / (Mathf.Abs(d0) + Mathf.Abs(d1)));
                    tempPoints.Add(intersectionPoint);
                    tempIntersection.Add(intersectionPoint);
                }
                if ((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0))
                {
                    // Schnittpunkt auf Kante v1-v2 berechnen
                    Vector3 intersectionPoint = Vector3.Lerp(v1, v2, Mathf.Abs(d1) / (Mathf.Abs(d1) + Mathf.Abs(d2)));
                    tempPoints.Add(intersectionPoint);
                    tempIntersection.Add(intersectionPoint);
                }
                if ((d2 > 0 && d0 < 0) || (d2 < 0 && d0 > 0))
                {
                    // Schnittpunkt auf Kante v2-v0 berechnen
                    Vector3 intersectionPoint = Vector3.Lerp(v2, v0, Mathf.Abs(d2) / (Mathf.Abs(d2) + Mathf.Abs(d0)));
                    tempPoints.Add(intersectionPoint);
                    tempIntersection.Add(intersectionPoint);
                }

                if (tempPoints.Count >= 3)
                {
                    for (int k = 1; k < tempPoints.Count - 1; k++)
                    {
                        newVertices.Add(tempPoints[0]);
                        newTriangles.Add(newVertices.Count - 1);
                        newVertices.Add(tempPoints[k]);
                        newTriangles.Add(newVertices.Count - 1);
                        newVertices.Add(tempPoints[k + 1]);
                        newTriangles.Add(newVertices.Count - 1);

                        CreateDebugPoints(tempPoints[0], tempPoints[k], tempPoints[k + 1]);
                    }
                    tempPoints.Clear();
                }
            }
            if (pos && !neg)
            {
                // Wenn nur positive Punkte vorhanden sind, füge das Dreieck unverändert hinzu
                newVertices.Add(v0);
                newVertices.Add(v1);
                newVertices.Add(v2);
                newTriangles.Add(newVertices.Count - 3);
                newTriangles.Add(newVertices.Count - 2);
                newTriangles.Add(newVertices.Count - 1);
            }
        }

        for (int i = 1; i < tempIntersection.Count - 1; i++)
        {
            newVertices.Add(tempIntersection[0]);
            newTriangles.Add(newVertices.Count - 1);
            newVertices.Add(tempIntersection[i]);
            newTriangles.Add(newVertices.Count - 1);
            newVertices.Add(tempIntersection[i + 1]);
            newTriangles.Add(newVertices.Count - 1);

            //CreateDebugPoints(tempIntersection[0], tempIntersection[i], tempIntersection[i + 1]);
        }
        tempIntersection.Clear();

        cutMesh.Clear();

        for (int i = 0; i < newVertices.Count; i++)
        {
            newVertices[i] = transform.InverseTransformPoint(newVertices[i]);
        }

        cutMesh.vertices = newVertices.ToArray();
        cutMesh.triangles = newTriangles.ToArray();
        cutMesh.RecalculateNormals();
        cutMesh.RecalculateBounds();
        cutMesh.RecalculateTangents();

        cutMeshFilter.mesh = cutMesh;
        cutMeshFilter.sharedMesh = cutMesh;

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = cutMesh;
        }
        newVertices.Clear();
        newTriangles.Clear();
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
        Destroy(sphere.GetComponent<Collider>());
        Destroy(sphere, 3f);
    }

    private bool FilterMovement()
    {
        bool planeMoved = Vector3.Distance(slicerPlane.transform.position, lastPlanePosition) >= 0.01f ||
                          Quaternion.Angle(slicerPlane.transform.rotation, lastPlaneRotation) >= 1f;

        bool objectMoved = Vector3.Distance(transform.position, lastPosition) >= 0.01f ||
                           Quaternion.Angle(transform.rotation, lastRotation) >= 1f;

        if (planeMoved || objectMoved)
        {
            lastPlanePosition = slicerPlane.transform.position;
            lastPlaneRotation = slicerPlane.transform.rotation;
            lastPosition = transform.position;
            lastRotation = transform.rotation;
            return true;
        }
        return false;
    }


}
