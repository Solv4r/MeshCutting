using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MeshSliceDetectorTest : MonoBehaviour
{
    [SerializeField] public SlicerPlane slicerPlaneLeft;
    [SerializeField] public SlicerPlane slicerPlaneRight;


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
    private Vector3 lastPlaneLeftPosition;
    private Quaternion lastPlaneLeftRotation;

    private Vector3 lastPlaneRightPosition;
    private Quaternion lastPlaneRightRotation;

    // Flag, ob das Mesh mit der Plane schneidet
    //private bool isIntersecting = false;

    void Start()
    {
        // Speichere das Original-Mesh
        originalMeshFilter = GetComponent<MeshFilter>();
        originalMesh = originalMeshFilter.sharedMesh;
        // Speichere die Startposition und -rotation
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        // Speichere die Startposition und -rotation der Slicer-Plane
        lastPlaneRightPosition = slicerPlaneLeft.transform.position;
        lastPlaneRightRotation = slicerPlaneLeft.transform.rotation;

        lastPlaneLeftPosition = slicerPlaneRight.transform.position;
        lastPlaneLeftRotation = slicerPlaneRight.transform.rotation;
    }
    void Update()
    {
        // Wenn die Plane oder das Objekt sich nicht bewegt hat, nichts tun
        if (!FilterMovement())
        {
            return;
        }

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

            float d0 = slicerPlaneLeft.GetPlane().GetDistanceToPoint(v0);
            float d1 = slicerPlaneLeft.GetPlane().GetDistanceToPoint(v1);
            float d2 = slicerPlaneLeft.GetPlane().GetDistanceToPoint(v2);

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

                if (tempPoints.Count == 3)
                {
                    // Orientierung der original Punkte
                    Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
                    Vector3 newNormal = Vector3.Cross(tempPoints[1] - tempPoints[0], tempPoints[2] - tempPoints[0]).normalized;

                    if (Vector3.Dot(normal, newNormal) < 0)
                    {
                        // Swap the first two points to maintain the correct winding order
                        Vector3 p1 = tempPoints[1];
                        Vector3 p2 = tempPoints[2];
                        tempPoints[1] = p2;
                        tempPoints[2] = p1;
                    }

                    // Wenn 3 Punkte vorhanden sind, füge das Dreieck hinzu
                    newVertices.Add(tempPoints[0]);
                    newVertices.Add(tempPoints[1]);
                    newVertices.Add(tempPoints[2]);
                    newTriangles.Add(newVertices.Count - 3);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 1);


                }
                else if (tempPoints.Count == 4)
                {

                    // Orientierung der original Punkte
                    Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

                    Vector3 newNormal = Vector3.Cross(tempPoints[1] - tempPoints[0], tempPoints[2] - tempPoints[0]).normalized;
                    Vector3 newNormal2 = Vector3.Cross(tempPoints[2] - tempPoints[0], tempPoints[3] - tempPoints[0]).normalized;
                    if (Vector3.Dot(normal, newNormal) < 0)
                    {
                        // Swap the first two points to maintain the correct winding order
                        Vector3 p1 = tempPoints[1];
                        Vector3 p2 = tempPoints[2];
                        tempPoints[1] = p2;
                        tempPoints[2] = p1;
                        Debug.Log("Swapped Points: " + tempPoints[0] + " " + tempPoints[1] + " " + tempPoints[2] + " " + tempPoints[3]);
                    }

                    // // Wenn 4 Punkte vorhanden sind, füge zwei Dreiecke hinzu
                    newVertices.Add(tempPoints[0]);
                    newVertices.Add(tempPoints[1]);
                    newVertices.Add(tempPoints[2]);
                    newTriangles.Add(newVertices.Count - 3);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 1);

                    if (Vector3.Dot(normal, newNormal2) < 0)
                    {
                        // Swap the first two points to maintain the correct winding order
                        Vector3 p2 = tempPoints[2];
                        Vector3 p3 = tempPoints[3];
                        tempPoints[2] = p3;
                        tempPoints[3] = p2;
                    }
                    newVertices.Add(tempPoints[0]);
                    newVertices.Add(tempPoints[2]);
                    newVertices.Add(tempPoints[3]);
                    newTriangles.Add(newVertices.Count - 3);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 1);


                }
                tempPoints.Clear();
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
            v0 = transform.TransformPoint(vertices[triangles[i]]);
            v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
            v2 = transform.TransformPoint(vertices[triangles[i + 2]]);

            d0 = slicerPlaneRight.GetPlane().GetDistanceToPoint(v0);
            d1 = slicerPlaneRight.GetPlane().GetDistanceToPoint(v1);
            d2 = slicerPlaneRight.GetPlane().GetDistanceToPoint(v2);

            // Wenn mindestens ein Punkt auf anderer Seite als ein anderer liegt → Schnitt
            pos = d0 > 0 || d1 > 0 || d2 > 0;
            neg = d0 < 0 || d1 < 0 || d2 < 0;

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

                if (tempPoints.Count == 3)
                {
                    // Orientierung der original Punkte
                    Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
                    Vector3 newNormal = Vector3.Cross(tempPoints[1] - tempPoints[0], tempPoints[2] - tempPoints[0]).normalized;

                    if (Vector3.Dot(normal, newNormal) < 0)
                    {
                        // Swap the first two points to maintain the correct winding order
                        Vector3 p1 = tempPoints[1];
                        Vector3 p2 = tempPoints[2];
                        tempPoints[1] = p2;
                        tempPoints[2] = p1;
                    }

                    // Wenn 3 Punkte vorhanden sind, füge das Dreieck hinzu
                    newVertices.Add(tempPoints[0]);
                    newVertices.Add(tempPoints[1]);
                    newVertices.Add(tempPoints[2]);
                    newTriangles.Add(newVertices.Count - 3);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 1);


                }
                else if (tempPoints.Count == 4)
                {

                    // Orientierung der original Punkte
                    Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

                    Vector3 newNormal = Vector3.Cross(tempPoints[1] - tempPoints[0], tempPoints[2] - tempPoints[0]).normalized;
                    Vector3 newNormal2 = Vector3.Cross(tempPoints[2] - tempPoints[0], tempPoints[3] - tempPoints[0]).normalized;



                    Debug.Log("Temp Point before sort: " + tempPoints[0] + " Temp Point 1: " + tempPoints[1] + " Temp Point 2: " + tempPoints[2] + " Temp Point 3: " + tempPoints[3]);


                    //Debug.Log("Temp Point after sort: " + tempPoints[0] + " Temp Point 1: " + tempPoints[1] + " Temp Point 2: " + tempPoints[2] + " Temp Point 3: " + tempPoints[3]);

                    if (Vector3.Dot(normal, newNormal) < 0)
                    {
                        // Swap the first two points to maintain the correct winding order
                        Vector3 p1 = tempPoints[1];
                        Vector3 p2 = tempPoints[2];
                        tempPoints[1] = p2;
                        tempPoints[2] = p1;
                    }

                    // // Wenn 4 Punkte vorhanden sind, füge zwei Dreiecke hinzu
                    newVertices.Add(tempPoints[0]);
                    newVertices.Add(tempPoints[1]);
                    newVertices.Add(tempPoints[2]);
                    newTriangles.Add(newVertices.Count - 3);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 1);

                    if (Vector3.Dot(normal, newNormal2) < 0)
                    {
                        // Swap the first two points to maintain the correct winding order
                        Vector3 p2 = tempPoints[2];
                        Vector3 p3 = tempPoints[3];
                        tempPoints[2] = p3;
                        tempPoints[3] = p2;
                    }
                    newVertices.Add(tempPoints[0]);
                    newVertices.Add(tempPoints[2]);
                    newVertices.Add(tempPoints[3]);
                    newTriangles.Add(newVertices.Count - 3);
                    newTriangles.Add(newVertices.Count - 2);
                    newTriangles.Add(newVertices.Count - 1);


                }
                tempPoints.Clear();
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



        cutMesh.Clear();

        for (int i = 0; i < newVertices.Count; i++)
        {
            newVertices[i] = transform.InverseTransformPoint(newVertices[i]);
        }

        cutMesh.vertices = newVertices.ToArray();
        cutMesh.triangles = newTriangles.ToArray();
        cutMesh.RecalculateNormals();
        cutMesh.RecalculateBounds();
        //cutMesh.RecalculateTangents();

        //cutMeshFilter.mesh = cutMesh;
        //cutMeshFilter.sharedMesh = cutMesh;

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
        bool planeMovedRight = Vector3.Distance(slicerPlaneRight.transform.position, lastPlaneRightPosition) >= 0.01f ||
                          Quaternion.Angle(slicerPlaneRight.transform.rotation, lastPlaneRightRotation) >= 1f;

        bool planeMovedLeft = Vector3.Distance(slicerPlaneLeft.transform.position, lastPlaneLeftPosition) >= 0.01f ||
                          Quaternion.Angle(slicerPlaneLeft.transform.rotation, lastPlaneLeftRotation) >= 1f;

        bool objectMoved = Vector3.Distance(transform.position, lastPosition) >= 0.01f ||
                           Quaternion.Angle(transform.rotation, lastRotation) >= 1f;

        if (planeMovedRight || planeMovedLeft || objectMoved)
        {
            // Update the last known positions and rotations
            lastPlaneRightPosition = slicerPlaneRight.transform.position;
            lastPlaneRightRotation = slicerPlaneRight.transform.rotation;
            lastPlaneLeftPosition = slicerPlaneLeft.transform.position;
            lastPlaneLeftRotation = slicerPlaneLeft.transform.rotation;
            lastPosition = transform.position;
            lastRotation = transform.rotation;
            return true;
        }
        return false;
    }


}
