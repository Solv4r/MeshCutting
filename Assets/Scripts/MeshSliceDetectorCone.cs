using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MeshSliceDetectorCone : MonoBehaviour
{

    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private SlicerObject slicerObject;

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

    private SlicerObject cone;
    Mesh coneMesh;
    Vector3[] coneVertices;
    int[] coneTriangles;
    private Vector3 conePosition;
    private Quaternion coneRotation;




    // Approximate comparer for Vector3 to handle floating point precision issues
    class ApproxVector3Comparer : IEqualityComparer<Vector3>
    {
        private readonly float tolerance;

        public ApproxVector3Comparer(float tolerance)
        {
            this.tolerance = tolerance;
        }

        public bool Equals(Vector3 a, Vector3 b)
        {
            return Vector3.SqrMagnitude(a - b) < tolerance * tolerance;
        }

        public int GetHashCode(Vector3 obj)
        {
            int x = Mathf.RoundToInt(obj.x / tolerance);
            int y = Mathf.RoundToInt(obj.y / tolerance);
            int z = Mathf.RoundToInt(obj.z / tolerance);
            return x * 73856093 ^ y * 19349663 ^ z * 83492791;
        }
    }
    private List<Vector3> newVertices = new();
    private List<int> newTriangles = new();
    private List<Vector3> intersectionPoints = new();
    private Dictionary<Vector3, int> vertexToIndex = new Dictionary<Vector3, int>(new ApproxVector3Comparer(0.01f));

    void Start()
    {
        // Speichere das Original-Mesh
        originalMeshFilter = GetComponent<MeshFilter>();
        originalMesh = originalMeshFilter.sharedMesh;
        // Speichere die Startposition und -rotation
        lastPosition = transform.position;
        lastRotation = transform.rotation;

        slicerObject.Init();

        conePosition = slicerObject.transform.position;
        coneRotation = slicerObject.transform.rotation;

        cone = slicerObject;
        coneMesh = cone.GetComponent<MeshFilter>().sharedMesh;
        coneTriangles = coneMesh.triangles;
        coneVertices = coneMesh.vertices;
    }
    void Update()
    {
        // Wenn die Plane oder das Objekt sich nicht bewegt hat, nichts tun
        if (!FilterMovement())
        {
            return;
        }

        cutMeshFilter = gameObject.GetComponent<MeshFilter>();
        cutMesh = Instantiate(originalMesh);

        Vector3[] vertices = cutMesh.vertices;
        int[] triangles = cutMesh.triangles;
        newVertices.Clear();
        newTriangles.Clear();
        vertexToIndex.Clear();
        //intersectionPoints.Clear();

        for (int i = 0; i < coneTriangles.Length / 3; i++)
        {
            Vector3 p0 = cone.transform.TransformPoint(coneVertices[coneTriangles[i * 3]]);
            Vector3 p1 = cone.transform.TransformPoint(coneVertices[coneTriangles[i * 3 + 1]]);
            Vector3 p2 = cone.transform.TransformPoint(coneVertices[coneTriangles[i * 3 + 2]]);
            Plane plane = new(p0, p1, p2);
            Vector3 segmentMidpoint = (p0 + p1 + p2) / 3f;

            for (int j = 0; j < triangles.Length; j += 3)
            {
                Vector3 v0 = transform.TransformPoint(vertices[triangles[j]]);
                Vector3 v1 = transform.TransformPoint(vertices[triangles[j + 1]]);
                Vector3 v2 = transform.TransformPoint(vertices[triangles[j + 2]]);

                Vector3 meshMidpoint = (v0 + v1 + v2) / 3f;

                float distance = Vector3.Distance(segmentMidpoint, meshMidpoint);

                if (distance > maxDistance)
                {
                    //AddTriangle(newVertices, newTriangles, v0, v1, v2);
                    newTriangles.Add(GetOrAddVertex(v0, vertexToIndex, newVertices));
                    newTriangles.Add(GetOrAddVertex(v1, vertexToIndex, newVertices));
                    newTriangles.Add(GetOrAddVertex(v2, vertexToIndex, newVertices));
                    continue;
                }

                float d0 = plane.GetDistanceToPoint(v0);
                float d1 = plane.GetDistanceToPoint(v1);
                float d2 = plane.GetDistanceToPoint(v2);

                // Wenn mindestens ein Punkt auf anderer Seite als ein anderer liegt → Schnitt
                bool pos = d0 > 0 || d1 > 0 || d2 > 0;
                bool neg = d0 < 0 || d1 < 0 || d2 < 0;

                if (pos && neg)
                {

                    List<Vector3> tempPoints = new();
                    if (d0 > 0) tempPoints.Add(v0);
                    if (d1 > 0) tempPoints.Add(v1);
                    if (d2 > 0) tempPoints.Add(v2);
                    if ((d0 > 0 && d1 < 0) || (d0 < 0 && d1 > 0))
                    {
                        // Schnittpunkt auf Kante v0-v1 berechnen
                        Vector3 intersectionPoint = Vector3.Lerp(v0, v1, Mathf.Abs(d0) / (Mathf.Abs(d0) + Mathf.Abs(d1)));
                        tempPoints.Add(intersectionPoint);
                        intersectionPoints.Add(intersectionPoint);
                    }
                    if ((d1 > 0 && d2 < 0) || (d1 < 0 && d2 > 0))
                    {
                        // Schnittpunkt auf Kante v1-v2 berechnen
                        Vector3 intersectionPoint = Vector3.Lerp(v1, v2, Mathf.Abs(d1) / (Mathf.Abs(d1) + Mathf.Abs(d2)));
                        tempPoints.Add(intersectionPoint);
                        intersectionPoints.Add(intersectionPoint);
                    }
                    if ((d2 > 0 && d0 < 0) || (d2 < 0 && d0 > 0))
                    {
                        // Schnittpunkt auf Kante v2-v0 berechnen
                        Vector3 intersectionPoint = Vector3.Lerp(v2, v0, Mathf.Abs(d2) / (Mathf.Abs(d2) + Mathf.Abs(d0)));
                        tempPoints.Add(intersectionPoint);
                        intersectionPoints.Add(intersectionPoint);
                    }
                    if (tempPoints.Count >= 3)
                    {
                        for (int k = 1; k < tempPoints.Count - 1; k++)
                        {

                            //AddTriangle(newVertices, newTriangles, tempPoints[0], tempPoints[k], tempPoints[k + 1]);
                            newTriangles.Add(GetOrAddVertex(tempPoints[0], vertexToIndex, newVertices));
                            newTriangles.Add(GetOrAddVertex(tempPoints[k], vertexToIndex, newVertices));
                            newTriangles.Add(GetOrAddVertex(tempPoints[k + 1], vertexToIndex, newVertices));
                        }
                        tempPoints.Clear();
                    }
                }
                if (pos && !neg)
                {
                    //AddTriangle(newVertices, newTriangles, v0, v1, v2);
                    newTriangles.Add(GetOrAddVertex(v0, vertexToIndex, newVertices));
                    newTriangles.Add(GetOrAddVertex(v1, vertexToIndex, newVertices));
                    newTriangles.Add(GetOrAddVertex(v2, vertexToIndex, newVertices));
                }
            } 
        }
        cutMesh.Clear();


        for (int k = 0; k < newVertices.Count; k++)
        {
            newVertices[k] = transform.InverseTransformPoint(newVertices[k]);
        }

        cutMesh.vertices = newVertices.ToArray();
        cutMesh.triangles = newTriangles.ToArray();
        cutMesh.RecalculateNormals();
        cutMesh.RecalculateBounds();

        // Assign mehsh
        cutMeshFilter.sharedMesh = cutMesh;
        cutMeshFilter.mesh = cutMesh;

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = cutMesh;
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
        Destroy(sphere.GetComponent<Collider>());
        Destroy(sphere, 3f);
    }

    private bool FilterMovement()
    {
        bool coneMoved = Vector3.Distance(cone.transform.position, conePosition) >= 0.01f ||
                           Quaternion.Angle(cone.transform.rotation, coneRotation) >= 1f;

        bool objectMoved = Vector3.Distance(transform.position, lastPosition) >= 0.01f ||
                           Quaternion.Angle(transform.rotation, lastRotation) >= 1f;

        if (coneMoved || objectMoved)
        {
            // Update the last known positions and rotations
            conePosition = cone.transform.position;
            coneRotation = cone.transform.rotation;

            lastPosition = transform.position;
            lastRotation = transform.rotation;
            return true;
        }
        return false;
    }
    private void AddTriangle(List<Vector3> vertices, List<int> triangles, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        int startIndex = vertices.Count;
        vertices.Add(v0);
        vertices.Add(v1);
        vertices.Add(v2);
        triangles.Add(startIndex);
        triangles.Add(startIndex + 1);
        triangles.Add(startIndex + 2);
    }

    private int GetOrAddVertex(Vector3 point, Dictionary<Vector3, int> vertexToIndex, List<Vector3> newVertices)
    {
        if (vertexToIndex.TryGetValue(point, out int index))
        {
            return index;
        }
        else
        {
            index = newVertices.Count;
            newVertices.Add(point);
            vertexToIndex[point] = index;
            return index;
        }
    }
}
