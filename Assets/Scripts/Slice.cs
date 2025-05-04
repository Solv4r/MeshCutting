using UnityEngine;
using System.Collections.Generic;

public class Slice : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        // Abrufen der lokalen Vertices des Meshes
        Vector3[] vertices = mesh.vertices;

        // Set für einzigartige Vertices
        HashSet<Vector3> uniqueVertices = new HashSet<Vector3>();

        // Umwandlung der lokalen Vertices in Welt-Raum und Filtern der doppelten Vertices
        foreach (Vector3 vertex in vertices)
        {
            // Transformiere die lokalen Vertices in Welt-Raum
            Vector3 worldVertex = transform.TransformPoint(vertex);

            // Füge nur einzigartige Vertices hinzu (keine Duplikate)
            uniqueVertices.Add(worldVertex);
        }

        // Ausgabe der einzigartigen Vertices
        foreach (Vector3 vertex in uniqueVertices)
        {
            Debug.Log("Unique Vertex in Welt-Raum: " + vertex);
        }

        // Die Anzahl der einzigartigen Vertices
        Debug.Log("Anzahl der einzigartigen Vertices: " + uniqueVertices.Count);
    }
}
