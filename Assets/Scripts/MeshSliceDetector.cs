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
    private bool isIntersecting = false;

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

    void OnTriggerEnter(Collider other)
    {
        isIntersecting = true;
    }

    void Update()
    {
        // Wenn die Plane oder das Objekt sich nicht bewegt hat, nichts tun
        if (!FilterMovement())
        {
            return;
        }

        // // 1. Get the mesh bounds in world space
        // Bounds bounds = originalMeshFilter.mesh.bounds;

        // // 2. Get the 8 corners of the bounds
        // Vector3[] corners = new Vector3[8];
        // Vector3 center = bounds.center;
        // Vector3 ext = bounds.extents;

        // corners[0] = center + new Vector3(ext.x, ext.y, ext.z);
        // corners[1] = center + new Vector3(ext.x, ext.y, -ext.z);
        // corners[2] = center + new Vector3(ext.x, -ext.y, ext.z);
        // corners[3] = center + new Vector3(ext.x, -ext.y, -ext.z);
        // corners[4] = center + new Vector3(-ext.x, ext.y, ext.z);
        // corners[5] = center + new Vector3(-ext.x, ext.y, -ext.z);
        // corners[6] = center + new Vector3(-ext.x, -ext.y, ext.z);
        // corners[7] = center + new Vector3(-ext.x, -ext.y, -ext.z);

        // // 4. Count how many corners are on each side of the plane
        // int above = 0;
        // int below = 0;

        // foreach (var corner in corners)
        // {
        //     float distance = slicerPlane.GetPlane().GetDistanceToPoint(transform.TransformPoint(corner));
        //     if (distance > 0.001f) above++;
        //     else if (distance < -0.001f) below++;
        // }

        // // 5. Only update if some points are on each side
        // isIntersecting = (above > 0 && below > 0);

        // if (!isIntersecting)
        // {
        //     return;
        // }


        // Wenn das cutMesh nicht gesetzt ist, weise es das Original-Mesh zu
        if (cutMesh == null)
        {
        }
        cutMesh = Instantiate(originalMesh);
        cutMeshFilter = gameObject.GetComponent<MeshFilter>();



        Vector3[] vertices = cutMesh.vertices;
        int[] triangles = cutMesh.triangles;

        List<Vector3> newVertices = new();
        List<int> newTriangles = new();
        List<int> tempTriangle = new();
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
                    //CreatePointMarker(v0, Color.red, "V0");
                }
                if (d1 > 0)
                {
                    tempPoints.Add(v1);
                    //CreatePointMarker(v1, Color.green, "V1");
                }
                if (d2 > 0)
                {
                    tempPoints.Add(v2);
                    //CreatePointMarker(v2, Color.blue, "V2");
                }
                if ((d0 > 0 && d1 < 0) || (d0 < 0 && d1 > 0))
                {
                    // Schnittpunkt auf Kante v0-v1 berechnen
                    Vector3 intersectionPoint = Vector3.Lerp(v0, v1, Mathf.Abs(d0) / (Mathf.Abs(d0) + Mathf.Abs(d1)));
                    tempPoints.Add(intersectionPoint);
                    //CreatePointMarker(intersectionPoint, Color.white, "Intersection_V0_V1");
                    tempIntersection.Add(intersectionPoint);
                }
                if ((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0))
                {
                    // Schnittpunkt auf Kante v1-v2 berechnen
                    Vector3 intersectionPoint = Vector3.Lerp(v1, v2, Mathf.Abs(d1) / (Mathf.Abs(d1) + Mathf.Abs(d2)));
                    tempPoints.Add(intersectionPoint);
                    //CreatePointMarker(intersectionPoint, Color.black, "Intersection_V1_V2");
                    tempIntersection.Add(intersectionPoint);
                }
                if ((d2 > 0 && d0 < 0) || (d2 < 0 && d0 > 0))
                {
                    // Schnittpunkt auf Kante v2-v0 berechnen
                    Vector3 intersectionPoint = Vector3.Lerp(v2, v0, Mathf.Abs(d2) / (Mathf.Abs(d2) + Mathf.Abs(d0)));
                    tempPoints.Add(intersectionPoint);
                    //CreatePointMarker(intersectionPoint, Color.magenta, "Intersection_V2_V0");
                    tempIntersection.Add(intersectionPoint);
                }


                // Hier die tempPoints bearbeiten
                if (tempPoints.Count == 3)
                {
                    // Wenn 3 Punkte vorhanden sind, füge das Dreieck hinzu
                    newVertices.Add(tempPoints[0]);
                    newVertices.Add(tempPoints[1]);
                    newVertices.Add(tempPoints[2]);
                    newTriangles.Add(newVertices.Count - 3);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 1);

                    newVertices.Add(tempPoints[0]);
                    newVertices.Add(tempPoints[2]);
                    newVertices.Add(tempPoints[1]);
                    newTriangles.Add(newVertices.Count - 3);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 1);
                }
                else if (tempPoints.Count == 4)
                {
                    // // Wenn 4 Punkte vorhanden sind, füge zwei Dreiecke hinzu
                    newVertices.Add(tempPoints[0]);
                    newVertices.Add(tempPoints[1]);
                    newVertices.Add(tempPoints[2]);
                    newTriangles.Add(newVertices.Count - 3);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 1);

                    newVertices.Add(tempPoints[0]);
                    newVertices.Add(tempPoints[2]);
                    newVertices.Add(tempPoints[3]);
                    newTriangles.Add(newVertices.Count - 3);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 1);

                    newVertices.Add(tempPoints[0]);
                    newVertices.Add(tempPoints[2]);
                    newVertices.Add(tempPoints[1]);
                    newTriangles.Add(newVertices.Count - 3);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 1);

                    newVertices.Add(tempPoints[0]);
                    newVertices.Add(tempPoints[3]);
                    newVertices.Add(tempPoints[2]);
                    newTriangles.Add(newVertices.Count - 3);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 1);

                    newVertices.Add(tempPoints[1]);
                    newVertices.Add(tempPoints[2]);
                    newVertices.Add(tempPoints[3]);
                    newTriangles.Add(newVertices.Count - 3);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 1);

                    newVertices.Add(tempPoints[3]);
                    newVertices.Add(tempPoints[2]);
                    newVertices.Add(tempPoints[1]);
                    newTriangles.Add(newVertices.Count - 3);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 1);
                }
                // Intersection part

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

            for (int j = 0; j < tempTriangle.Count - 2; j++)
            {
                newTriangles[i + j] = tempTriangle[j];

            }
            tempTriangle.Clear();
        }

        Debug.Log("Temp Intersection Count: " + tempIntersection.Count);

        for (int i = 1; i < tempIntersection.Count - 2; i++)
        {
            newVertices.Add(tempIntersection[0]);
            newTriangles.Add(newVertices.Count - 1);
            newVertices.Add(tempIntersection[i]);
            newTriangles.Add(newVertices.Count - 1);
            newVertices.Add(tempIntersection[i + 1]);
            newTriangles.Add(newVertices.Count - 1);

            //CreateDebugPoints(tempIntersection[0], tempIntersection[i], tempIntersection[i + 1]);


            // newVertices.Add(tempIntersection[0]);
            // newTriangles.Add(newVertices.Count - 1);
            // newVertices.Add(tempIntersection[i + 1]);
            // newTriangles.Add(newVertices.Count - 1);
            // newVertices.Add(tempIntersection[i]);
            // newTriangles.Add(newVertices.Count - 1);

        }
        for (int i = tempIntersection.Count - 2; i > 0; i--)
        {
            newVertices.Add(tempIntersection[0]);
            newTriangles.Add(newVertices.Count - 1);

            newVertices.Add(tempIntersection[i + 1]);
            newTriangles.Add(newVertices.Count - 1);

            newVertices.Add(tempIntersection[i]);
            newTriangles.Add(newVertices.Count - 1);
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
