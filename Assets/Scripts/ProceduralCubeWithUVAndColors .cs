using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralCubeWithUVAndColors : MonoBehaviour
{

    [SerializeField] Vector3 Schnittpunkt1;
    [SerializeField] Vector3 Schnittpunkt2;
    [SerializeField] Vector3 Schnittpunkt3;
    [SerializeField] Vector3 Schnittpunkt4;

    void Start()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Cube with UV and Colors";

        // 8 unique vertices (Würfel von -0.5 bis 0.5)
        Vector3[] vertices = {
            new Vector3(-0.5f, -0.5f, -0.5f), // 0
            new Vector3( 0.5f, -0.5f, -0.5f), // 1
            new Vector3( 0.5f,  0.5f, -0.5f), // 2
            new Vector3(-0.5f,  0.5f, -0.5f), // 3
            new Vector3(-0.5f, -0.5f,  0.5f), // 4
            new Vector3( 0.5f, -0.5f,  0.5f), // 5
            new Vector3( 0.5f,  0.5f,  0.5f), // 6
            new Vector3(-0.5f,  0.5f,  0.5f)  // 7
        };
        mesh.vertices = vertices;

        // Triangle indices
        int[] triangles = {
            // Back
            0, 2, 1, 0, 3, 2,
            // Front
            4, 5, 6, 4, 6, 7,
            // Left
            0, 7, 3, 0, 4, 7,
            // Right
            1, 2, 6, 1, 6, 5,
            // Top
            3, 7, 6, 3, 6, 2,
            // Bottom
            0, 1, 5, 0, 5, 4
        };
        mesh.triangles = triangles;

        // UVs (eine einfache Projektion)
        Vector2[] uvs = {
            new Vector2(0, 0), // 0
            new Vector2(1, 0), // 1
            new Vector2(1, 1), // 2
            new Vector2(0, 1), // 3
            new Vector2(0, 0), // 4
            new Vector2(1, 0), // 5
            new Vector2(1, 1), // 6
            new Vector2(0, 1)  // 7
        };
        mesh.uv = uvs;

        // Farben pro Vertex
        Color[] colors = {
            Color.red,     // 0
            Color.green,   // 1
            Color.blue,    // 2
            Color.yellow,  // 3
            Color.cyan,    // 4
            Color.magenta, // 5
            Color.white,   // 6
            Color.black    // 7
        };
        mesh.colors = colors;

        mesh.RecalculateNormals();

        // Set mesh
        GetComponent<MeshFilter>().mesh = mesh;

        // Set Rigidbody
        //Rigidbody rb = gameObject.AddComponent<Rigidbody>();

        // Set Collider
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(1, 1, 1); // Setze die Größe des BoxColliders auf 1x1x1

        // Shader laden und Material erstellen
        Shader shader = Shader.Find("Custom/UnlitVertexColor");
        Material mat = new Material(shader);

        // Material zuweisen
        GetComponent<MeshRenderer>().material = mat;

        // Coroutine
        StartCoroutine(StartUpperCube());
    }

    IEnumerator StartUpperCube()
    {
        yield return new WaitForSeconds(3f); // Warte 1 Sekunde
        UpperCube();
        LowerCube();
    }

    public void UpperCube()
    {
        {
            Vector3[] vertices = new Vector3[]
            {
            new Vector3(-0.5f, 0.5f, -0.5f), // 0 oben vorne links
            new Vector3( 0.5f, 0.5f, -0.5f), // 1 oben vorne rechts
            new Vector3(-0.5f, 0.5f,  0.5f), // 2 oben hinten links
            new Vector3( 0.5f, 0.5f,  0.5f), // 3 oben hinten rechts

            new Vector3(-0.5f, 0.0f, -0.5f), // 4 schnitt vorne links
            new Vector3( 0.5f, 0.0f, -0.5f), // 5 schnitt vorne rechts
            new Vector3(-0.5f, 0.0f,  0.5f), // 6 schnitt hinten links
            new Vector3( 0.5f, 0.0f,  0.5f), // 7 schnitt hinten rechts
            };

            int[] triangles = {
            0, 1, 3, 0, 3, 2,       // Top
            0, 4, 5, 0, 5, 1,       // Front
            1, 5, 7, 1, 7, 3,       // Right
            3, 7, 6, 3, 6, 2,       // Back
            2, 6, 4, 2, 4, 0,       // Left
            4, 5, 7, 4, 7, 6        // Bottom (Schnittfläche)
        };

            // Optional: einfache UVs und Farben
            Vector2[] uvs = new Vector2[8];
            for (int i = 0; i < uvs.Length; i++)
                uvs[i] = new Vector2(vertices[i].x + 0.5f, vertices[i].z + 0.5f);

            Color[] colors = new Color[8];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = (vertices[i].y > 0.1f) ? Color.red : Color.blue;

            Mesh mesh = new Mesh();
            mesh.name = "UpperHalfCube";
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.colors = colors;
            mesh.RecalculateNormals();

            GetComponent<MeshFilter>().mesh = mesh;

            // Shader zuweisen
            Shader shader = Shader.Find("Custom/UnlitVertexColor");
            Material mat = new Material(shader);
            GetComponent<MeshRenderer>().material = mat;

            // BoxCollider aktualisieren
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            boxCollider.center = new Vector3(0, 0.25f, 0); // Setze den Mittelpunkt des BoxColliders auf die Mitte des Würfels
            boxCollider.size = new Vector3(1, 0.5f, 1); // Setze die Größe des BoxColliders auf 1x0.5x1



        }
    }

    void LowerCube()
    {
        // Neues GameObject erzeugen
        GameObject lowerHalf = new GameObject("LowerHalfCube");
        lowerHalf.transform.position = Vector3.zero;

        MeshFilter mf = lowerHalf.AddComponent<MeshFilter>();
        MeshRenderer mr = lowerHalf.AddComponent<MeshRenderer>();

        // Vertices wie vorher (unterer Bereich + Schnittfläche)
        Vector3[] vertices = new Vector3[]
        {
        new Vector3(-0.5f, -0.5f, -0.5f), // 0 unten
        new Vector3( 0.5f, -0.5f, -0.5f), // 1
        new Vector3(-0.5f, -0.5f,  0.5f), // 2
        new Vector3( 0.5f, -0.5f,  0.5f), // 3

        new Vector3(-0.5f,  0.0f, -0.5f), // 4 schnitt
        new Vector3( 0.5f,  0.0f, -0.5f), // 5
        new Vector3(-0.5f,  0.0f,  0.5f), // 6
        new Vector3( 0.5f,  0.0f,  0.5f), // 7
        };

        int[] triangles = {
        0, 3, 1,
        0, 2, 3,
        0, 1, 5,
        0, 5, 4,
        1, 3, 7,
        1, 7, 5,
        3, 2, 6,
        3, 6, 7,
        2, 0, 4,
        2, 4, 6,
        4, 5, 7,
        4, 7, 6
    };

        Vector2[] uvs = new Vector2[8];
        for (int i = 0; i < uvs.Length; i++)
            uvs[i] = new Vector2(vertices[i].x + 0.5f, vertices[i].z + 0.5f);

        Color[] colors = new Color[8];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = (vertices[i].y < -0.1f) ? Color.green : Color.cyan;

        Mesh mesh = new Mesh();
        mesh.name = "LowerHalfMesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.colors = colors;
        mesh.RecalculateNormals();

        mf.mesh = mesh;

        // Shader/Material zuweisen
        Shader shader = Shader.Find("Custom/UnlitVertexColor");
        Material mat = new Material(shader);
        mr.material = mat;

        // BoxCollider hinzufügen und anpassen
        BoxCollider boxCollider = lowerHalf.AddComponent<BoxCollider>();
        boxCollider.center = new Vector3(0, -0.25f, 0); // Setze den Mittelpunkt des BoxColliders auf die Mitte des Würfels
        boxCollider.size = new Vector3(1, 0.5f, 1); // Setze die Größe des BoxColliders auf 1x0.5x1

        // Rigidbody hinzufügen
        Rigidbody rb = lowerHalf.AddComponent<Rigidbody>();

    }

    // Methode zum Erstellen der Vertices auf der gleichen Ebene
    private Dictionary<float, List<Vector2>> GetVerticesOnSamePlane(Vector3[] vertices)
    {
        // Dictionary für die Speicherung der Vertices auf der gleichen Ebene
        Dictionary<float, List<Vector2>> planes = new Dictionary<float, List<Vector2>>();
        // Iteriere über alle Vertices und füge sie dem Dictionary hinzu
        for (int i = 0; i < vertices.Length; i++)
        {
            // For x
            float x = vertices[i].x;
            if (!planes.ContainsKey(x))
            {
                planes.Add(x, new List<Vector2>());
                planes[x].Add(new Vector2(vertices[i].y, vertices[i].z));
            }
            else if (planes.ContainsKey(x))
            {
                planes[x].Add(new Vector2(vertices[i].y, vertices[i].z));
            }
            // For y
            float y = vertices[i].y;
            if (!planes.ContainsKey(y))
            {
                planes.Add(y, new List<Vector2>());
                planes[y].Add(new Vector2(vertices[i].x, vertices[i].z));
            }
            else if (planes.ContainsKey(y))
            {
                planes[y].Add(new Vector2(vertices[i].x, vertices[i].z));
            }
            // For z
            float z = vertices[i].z;
            if (!planes.ContainsKey(z))
            {
                planes.Add(z, new List<Vector2>());
                planes[z].Add(new Vector2(vertices[i].x, vertices[i].y));
            }
            else if (planes.ContainsKey(z))
            {
                planes[z].Add(new Vector2(vertices[i].x, vertices[i].y));
            }
        }
        return planes;
    }

    private void TriangulateVertices(Dictionary<float, List<Vector2>> vertices)
    {
        // Iteriere über alle Vertices und erstelle Dreiecke
        foreach (KeyValuePair<float, List<Vector2>> kvp in vertices)
        {
            List<Vector2> vertexList = kvp.Value;
            for (int i = 0; i < vertexList.Count - 1; i++)
            {
                // Erstelle Dreiecke aus den Vertices
                int[] triangles = new int[3];
                triangles[0] = i;
                triangles[1] = i + 1;
                triangles[2] = i + 2;
            }
        }

    }

}

