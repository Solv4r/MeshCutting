using UnityEngine;
using System.Collections.Generic;

public class CubeSplitter : MonoBehaviour
{
    // Plane Koordinaten
    public Vector3 planeNormal = Vector3.up;  // Normalvektor der Ebene (z.B. Y-Achse)
    public float planeDistance = 0f;  // Abstand von der Ebene zum Ursprung

    // Aufgerufen, wenn das Skript gestartet wird
    void Start()
    {
        // Hole das Mesh des Würfels
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        // Hole die lokalen Vertices des Würfels
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Liste für neue Vertices und Indices
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles1 = new List<int>();  // Mesh 1 (erste Hälfte)
        List<int> newTriangles2 = new List<int>();  // Mesh 2 (zweite Hälfte)

        // Gehe jede Kante des Würfels durch
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Hole die Indices der 3 Vertices eines Dreiecks
            int idx1 = triangles[i];
            int idx2 = triangles[i + 1];
            int idx3 = triangles[i + 2];

            Vector3 v1 = vertices[idx1];
            Vector3 v2 = vertices[idx2];
            Vector3 v3 = vertices[idx3];

            // Überprüfe die Kanten des Dreiecks auf Schnittpunkte mit der Plane
            List<Vector3> edgeVertices = new List<Vector3>();
            List<int> newIdx = new List<int>();

            Vector3[] edgePoints = new Vector3[3];
            edgePoints[0] = v1;
            edgePoints[1] = v2;
            edgePoints[2] = v3;

            // Kanten-Überprüfung (drei Kanten: v1->v2, v2->v3, v3->v1)
            for (int j = 0; j < 3; j++)
            {
                int next = (j + 1) % 3;
                Vector3 start = edgePoints[j];
                Vector3 end = edgePoints[next];

                float t = IntersectionTest(start, end);

                if (t >= 0f && t <= 1f)  // Wenn der Schnittpunkt auf der Kante liegt
                {
                    // Berechne den Schnittpunkt
                    Vector3 intersectionPoint = Vector3.Lerp(start, end, t);
                    edgeVertices.Add(intersectionPoint);
                    newIdx.Add(newVertices.Count);
                    newVertices.Add(intersectionPoint);
                }
                else
                {
                    edgeVertices.Add(start);
                    newIdx.Add(newVertices.Count);
                    newVertices.Add(start);
                }
            }

            // Jetzt haben wir die geschnittenen Kanten, wir können das Dreieck teilen
            if (edgeVertices.Count == 4)  // Normalerweise nur ein Schnittpunkt zwischen den Kanten
            {
                // Teile das Dreieck
                newTriangles1.Add(newIdx[0]);
                newTriangles1.Add(newIdx[1]);
                newTriangles1.Add(newIdx[2]);

                newTriangles2.Add(newIdx[0]);
                newTriangles2.Add(newIdx[2]);
                newTriangles2.Add(newIdx[3]);
            }
        }

        // Erstelle zwei neue Meshes und weise sie den jeweiligen GameObjects zu
        Mesh mesh1 = new Mesh();
        mesh1.vertices = newVertices.ToArray();
        mesh1.triangles = newTriangles1.ToArray();

        Mesh mesh2 = new Mesh();
        mesh2.vertices = newVertices.ToArray();
        mesh2.triangles = newTriangles2.ToArray();

        // Zuweisen des Meshes zum jeweiligen GameObject (z.B. für 2 verschiedene Würfelteile)
        GameObject part1 = new GameObject("CubePart1");
        part1.AddComponent<MeshFilter>().mesh = mesh1;
        part1.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;

        GameObject part2 = new GameObject("CubePart2");
        part2.AddComponent<MeshFilter>().mesh = mesh2;
        part2.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
    }

    // Funktion zur Berechnung des Schnittpunkts
    float IntersectionTest(Vector3 start, Vector3 end)
    {
        // Plane-Gleichung: Ax + By + Cz + D = 0
        float startDot = Vector3.Dot(planeNormal, start) + planeDistance;
        float endDot = Vector3.Dot(planeNormal, end) + planeDistance;

        // Wenn beide Punkte auf derselben Seite der Ebene sind, gibt es keinen Schnitt
        if (startDot * endDot > 0)
        {
            return -1f;
        }

        // Berechne den Schnittpunkt t entlang der Kante
        float t = -startDot / (endDot - startDot);
        return t;
    }
}
