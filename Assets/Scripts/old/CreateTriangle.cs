using UnityEngine;

public class CreateTriangle : MonoBehaviour
{
    void Start()
    {
        // Ein neues Mesh erstellen
        Mesh mesh = new Mesh();

        // Die 3 Vertices für das Dreieck definieren
        Vector3[] vertices = new Vector3[100];
        vertices[0] = new Vector3(0, 0, 0);  // Vertex 1
        vertices[1] = new Vector3(1, 0, 0);  // Vertex 2
        vertices[2] = new Vector3(0, 1, 0);  // Vertex 3
        // Optional: Ein viertes Vertex hinzufügen, um ein Quadrat zu erstellen (falls benötigt)
        vertices[3] = new Vector3(1, 1, 0);  // Vertex 4 (für ein Quadrat)
        vertices[4] = new Vector3(0, 1, 0);  // Vertex 3 (für ein Quadrat)
        vertices[5] = new Vector3(1, 0, 0);  // Vertex 2 (für ein Quadrat)

        // Die Indizes für das Dreieck definieren
        // Ein Dreieck besteht aus 3 Vertices, daher benötigen wir 1 Dreieck
        int[] triangles = new int[] { 0, 1, 2, 2, 1, 3 }; // Beispiel für ein Quadrat


        // Optional: Ein zweites Dreieck hinzufügen, um ein Quadrat zu erstellen (falls benötigt)
        //int[] triangles = new int[6] { 0, 1, 2, 2, 1, 3 }; // Beispiel für ein Quadrat

        // Die Vertices und die Dreiecke dem Mesh zuweisen
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // Optional: Normals berechnen (wichtig für die Beleuchtung und das Rendering)
        mesh.RecalculateNormals();

        // Ein MeshRenderer und MeshFilter hinzufügen, um das Mesh anzuzeigen
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));  // Ein Standardmaterial verwenden
    }
}
