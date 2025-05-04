using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreateCube : MonoBehaviour
{
    public GameObject cubePrefab; // Könnte man später auslagern

    private Vector3[] vertices ={ // Die 8 Vertices für den Würfel definieren
            new Vector3(-0.5f,0.5f, -0.5f), // 0 Oben
            new Vector3(0.5f, 0.5f, -0.5f), // 1
            new Vector3(0.5f, 0.5f, 0.5f), // 2
            new Vector3(-0.5f,  0.5f, 0.5f), // 3

            new Vector3(-0.5f, -0.5f,  -0.5f), // 4 Unten
            new Vector3(0.5f, -0.5f,  -0.5f), // 5
            new Vector3(0.5f, -0.5f,  0.5f), // 6
            new Vector3(-0.5f,  -0.5f,  0.5f)  // 7
    };
    private int[] triangles = { // Die Indizes für die 12 Dreiecke definieren
        0, 2, 1, 0, 3, 2, // Oben
        4, 5, 6, 4, 6, 7, // Unten
        0, 1, 5, 0, 5, 4, // Vorne
        1, 2, 6, 1, 6, 5, // Rechts
        2, 3, 7, 2, 7, 6, // Hinten
        3, 0, 4, 3 ,4, 7  // Links
    };
    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private Shader shader;
    private Material material;
    private MeshCollider meshCollider;
    void Start()
    {
        mesh = new Mesh();         // Das Mesh des Würfels
        mesh.name = "CreateCube";       // Mesh-Name
        mesh.vertices = vertices;       // Die Eckpunkte des Meshes
        mesh.triangles = triangles;     // Die Indizes der Dreiecke
        mesh.RecalculateNormals();      // Die Normalen für die Beleuchtung, mesh.normals wird dadurch automatisch berechnet
                                        // mesh.uv = uv;                // UV-Koordinaten für Texturen, aktuell braucht man nicht unbedingt für unsere Zwecke
                                        // mesh.colors = colors;        // Farben für die Vertices, aktuell nicht unbedingt relevant für unsere Zwecke

        CalculateMesh(gameObject, mesh, Color.blue);

        //CutHorizontalInY(mesh); // Aufruf der Methode, um den Würfel zu schneiden
        //StartCoroutine(CutHorizontalInYCoroutine(mesh));

        //CutVerticalInX(mesh); // Aufruf der Methode, um den Würfel zu schneiden
        //StartCoroutine(CutVerticalInXCoroutine(mesh));

        //Cut3Vertices(mesh); // Aufruf der Methode, um den Würfel zu schneiden
        //StartCoroutine(Cut3VerticesCoroutine(mesh));

        //Cut6Vertices(mesh); // Aufruf der Methode, um den Würfel zu schneiden
        StartCoroutine(Cut6VerticesCoroutine(mesh));
    }



    void Update()
    {

    }

    private void CutHorizontalInY(Mesh mesh)
    {
        //Schnittpunkte erstellen
        Vector3[] verticesToCut = {
            new Vector3(-0.5f, 0, -0.5f),   // 0
            new Vector3(0.5f, 0, -0.5f),    // 1
            new Vector3(0.5f, 0, 0.5f),     // 2
            new Vector3(-0.5f, 0, 0.5f)     // 3
        };

        Mesh topMesh = new Mesh();
        Mesh bottomMesh = new Mesh();

        // Neues Mesh erstellen
        topMesh.vertices = new Vector3[] {
            mesh.vertices[0],
            mesh.vertices[1],
            mesh.vertices[2],
            mesh.vertices[3],
            verticesToCut[0],
            verticesToCut[1],
            verticesToCut[2],
            verticesToCut[3]
        };
        bottomMesh.vertices = new Vector3[] {
            verticesToCut[0],
            verticesToCut[1],
            verticesToCut[2],
            verticesToCut[3],
            mesh.vertices[4],
            mesh.vertices[5],
            mesh.vertices[6],
            mesh.vertices[7]
        };
        mesh.Clear(); // Vorheriges Mesh löschen, ka ob nötig

        // Neue Indizes für die Dreiecke definieren
        int[] topTriangles = new int[] {
        0, 2, 1, 0, 3, 2, // Oben
        4, 5, 6, 4, 6, 7, // Unten
        0, 1, 5, 0, 5, 4, // Vorne
        1, 2, 6, 1, 6, 5, // Rechts
        2, 3, 7, 2, 7, 6, // Hinten
        3, 0, 4, 3 ,4, 7  // Links
        };
        // Neue Indizes für die Dreiecke definieren
        int[] bottomTriangles = new int[] {
        0, 2, 1, 0, 3, 2, // Oben
        4, 5, 6, 4, 6, 7, // Unten
        0, 1, 5, 0, 5, 4, // Vorne
        1, 2, 6, 1, 6, 5, // Rechts
        2, 3, 7, 2, 7, 6, // Hinten
        3, 0, 4, 3 ,4, 7  // Links
        };

        // Das obere Mesh zusammenstellen
        mesh.vertices = topMesh.vertices; // Setze die Vertices des Meshes auf die obere Hälfte
        mesh.triangles = topTriangles; // Setze die Indizes für die obere Hälfte
        mesh.RecalculateNormals(); // Normalen für die obere Hälfte berechnen
        CalculateMesh(gameObject, mesh, Color.red); // Berechne das Mesh mit der Farbe Rot


        GameObject lowerHalf = new GameObject("BottomHalf");
        lowerHalf.transform.position = Vector3.zero;
        bottomMesh.name = "BottomMesh";
        bottomMesh.vertices = bottomMesh.vertices; // Setze die Vertices des Meshes auf die untere Hälfte
        bottomMesh.triangles = bottomTriangles; // Setze die Indizes für die untere Hälfte
        bottomMesh.RecalculateNormals(); // Normalen für die untere Hälfte berechnen
        CalculateMesh(lowerHalf, bottomMesh, Color.green); // Berechne das Mesh mit der Farbe Grün

    }

    private void CutVerticalInX(Mesh mesh)
    {
        //Schnittpunkte erstellen
        Vector3[] verticesToCut = {
            new Vector3(0, -0.5f, -0.5f),   // 0
            new Vector3(0, 0.5f, -0.5f),    // 1
            new Vector3(0, 0.5f, 0.5f),     // 2
            new Vector3(0, -0.5f, 0.5f)     // 3
        };

        Mesh leftMesh = new Mesh();
        Mesh rightMesh = new Mesh();

        // Neues Mesh erstellen
        leftMesh.vertices = new Vector3[] {
            mesh.vertices[0],
            mesh.vertices[3],
            mesh.vertices[7],
            mesh.vertices[4],
            verticesToCut[0],
            verticesToCut[1],
            verticesToCut[2],
            verticesToCut[3]
        };
        rightMesh.vertices = new Vector3[] {
            verticesToCut[0], // 0
            verticesToCut[1], // 1
            verticesToCut[2], // 2
            verticesToCut[3], // 3
            mesh.vertices[1], // 4
            mesh.vertices[2], // 5
            mesh.vertices[6], // 6
            mesh.vertices[5]  // 7
        };
        mesh.Clear(); // Vorheriges Mesh löschen, ka ob nötig

        int[] leftTriangles = new int[] {
        0, 2, 1, 0, 3, 2, // Oben
        4, 5, 6, 4, 6, 7, // Unten
        0, 1, 5, 0, 5, 4, // Vorne
        1, 2, 6, 1, 6, 5, // Rechts
        2, 3, 7, 2, 7, 6, // Hinten
        3, 0, 4, 3 ,4, 7  // Links
        };
        int[] rightTriangles = new int[] { // Scheinbar immer im Uhrzeigesinn, sonst ausgeblendet wegen Backface Culling
        1, 2, 5, 1, 5, 4, // Oben
        0, 7, 6, 0, 6, 3, // Unten
        1, 4, 7, 1, 7, 0, // Vorne
        4, 5, 6, 4, 6, 7, // Rechts
        2, 3, 6, 2, 6, 5, // Hinten
        1, 0, 3, 1 ,3, 2  // Links
        };

        // Das linke Mesh zusammenstellen
        mesh.vertices = leftMesh.vertices; // Setze die Vertices des Meshes auf die obere Hälfte
        mesh.triangles = leftTriangles; // Setze die Indizes für die obere Hälfte
        mesh.RecalculateNormals(); // Normalen für die obere Hälfte berechnen
        CalculateMesh(gameObject, mesh, Color.red); // Berechne das Mesh mit der Farbe Rot


        GameObject rightHalf = new GameObject("RightHalf");
        rightHalf.transform.position = Vector3.zero;
        rightMesh.name = "RightMesh";
        rightMesh.vertices = rightMesh.vertices; // Setze die Vertices des Meshes auf die untere Hälfte
        rightMesh.triangles = rightTriangles; // Setze die Indizes für die untere Hälfte
        rightMesh.RecalculateNormals(); // Normalen für die untere Hälfte berechnen
        CalculateMesh(rightHalf, rightMesh, Color.green); // Berechne das Mesh mit der Farbe Grün
    }
    private void Cut3Vertices(Mesh mesh)
    {

        //Schnittpunkte erstellen
        Vector3[] verticesToCut = {
            new Vector3(0, 0.5f, -0.5f),   // 0
            new Vector3(0.5f, 0.5f, 0),    // 1
            new Vector3(0.5f, 0, -0.5f)    // 2
        };

        Mesh topMesh = new Mesh();
        Mesh bottomMesh = new Mesh();
        // Neues Mesh erstellen 
        topMesh.vertices = new Vector3[] {
            mesh.vertices[1],
            verticesToCut[0],
            verticesToCut[1],
            verticesToCut[2]
        };
        bottomMesh.vertices = new Vector3[] {
            mesh.vertices[0],
            mesh.vertices[2],
            mesh.vertices[3],
            mesh.vertices[4],
            mesh.vertices[5],
            mesh.vertices[6],
            mesh.vertices[7],
            verticesToCut[0],
            verticesToCut[1],
            verticesToCut[2]
        };
        mesh.Clear();

        int[] topTriangles = new int[] {
        1, 2, 0, // Oben
        1, 3, 2, // Schnittfläche
        1, 0, 3, // Vorne
        0, 2, 3, // Rechts
    };
        int[] bottomTriangles = new int[] {
        0, 2, 1, 0, 1, 8, 0, 8, 7,  // Oben
        0, 7, 9, 0, 9, 4, 0, 4, 3,  // Vorne
        1, 5, 4, 1, 4, 9, 1, 9, 8,  // Rechts
        1, 2, 6, 1, 6, 5,           // Hinten	
        0, 3, 6, 0, 6, 2,           // Links
        3, 4, 5, 3, 5, 6,           // Unten
        7, 8, 9                     // Schnittfläche
    };

        // Das obere Mesh zusammenstellen
        mesh.vertices = topMesh.vertices; // Setze die Vertices des Meshes auf die obere Hälfte
        mesh.triangles = topTriangles; // Setze die Indizes für die obere Hälfte
        mesh.RecalculateNormals(); // Normalen für die obere Hälfte berechnen
        CalculateMesh(gameObject, mesh, Color.red); // Berechne das Mesh mit der Farbe Rot


        GameObject lowerHalf = new GameObject("BottomHalf");
        lowerHalf.transform.position = Vector3.zero;
        bottomMesh.name = "BottomMesh";
        bottomMesh.vertices = bottomMesh.vertices; // Setze die Vertices des Meshes auf die untere Hälfte
        bottomMesh.triangles = bottomTriangles; // Setze die Indizes für die untere Hälfte
        bottomMesh.RecalculateNormals(); // Normalen für die untere Hälfte berechnen
        CalculateMesh(lowerHalf, bottomMesh, Color.green); // Berechne das Mesh mit der Farbe Grün



    }
    private void Cut6Vertices(Mesh mesh)
    {

        //Schnittpunkte erstellen
        Vector3[] verticesToCut = {
            new Vector3(0.25f, 0.5f, 0.5f),     // 0
            new Vector3(0.5f, 0.5f, 0.25f),     // 1
            new Vector3(0.5f, 0, -0.5f),        // 2
            new Vector3(-0.25f, -0.5f, -0.5f),    // 3
            new Vector3(-0.5f, -0.5f, -0.25f),    // 4
            new Vector3(-0.5f, 0, 0.5f)           // 5
        };

        Mesh topMesh = new Mesh();
        Mesh bottomMesh = new Mesh();

        // Neues Mesh erstellen
        topMesh.vertices = new Vector3[] {
            mesh.vertices[0],   // 0 Punkt 0 
            mesh.vertices[1],   // 1 Punkt 1
            mesh.vertices[3],   // 2 Punkt 3
            mesh.vertices[4],   // 3 Punkt 4
            verticesToCut[0],   // 4 Schnitt 0
            verticesToCut[1],   // 5 Schnitt 1
            verticesToCut[2],   // 6 Schnitt 2
            verticesToCut[3],   // 7 Schnitt 3
            verticesToCut[4],   // 8 Schnitt 4
            verticesToCut[5]    // 9 Schnitt 5
        };
        foreach (Vector3 v in topMesh.vertices)
        {
            Debug.Log(v);
        }

        bottomMesh.vertices = new Vector3[] {
            mesh.vertices[2], // 0 Punkt 2
            mesh.vertices[5], // 1 Punkt 5
            mesh.vertices[6], // 2 Punkt 6
            mesh.vertices[7], // 3 Punkt 7
            verticesToCut[0], // 4 Schnitt 0
            verticesToCut[1], // 5 Schnitt 1
            verticesToCut[2], // 6 Schnitt 2
            verticesToCut[3], // 7 Schnitt 3
            verticesToCut[4], // 8 Schnitt 4
            verticesToCut[5], // 9 Schnitt 5
        };
        mesh.Clear(); // Vorheriges Mesh löschen, ka ob nötig

        // Neue Indizes für die Dreiecke definieren
        int[] topTriangles = new int[] {
        0, 2, 4, 0, 4, 5, 0, 5, 1, // Oben
         3, 7, 8, // Unten
         0, 1, 6, 0, 6, 7, 0, 7, 3, // Vorne
         1, 5, 6, // Rechts
         2, 0, 3, 2 ,3, 8, 2, 8, 9,  // Links
         4, 2, 9, // Hinten
         4, 9, 8, 4, 8, 7, 4, 7, 6, 4, 6, 5  // Schnittfläche
        };


        int[] bottomTriangles = new int[] {
        4, 0, 5, // Oben
        7, 1, 2, 7, 2, 3, 7, 3, 8, // Unten
        7, 6, 1, // Vorne
        5, 0, 2, 5, 2, 1, 5, 1, 6, // Rechts
        9, 8, 3,  // Links
        4, 9, 3, 4, 3, 2, 4, 2, 0, // Hinten
        4, 5, 6, 4, 6, 7, 4, 7, 8, 4, 8, 9  // Schnittfläche
    };


        // Das obere Mesh zusammenstellen
        mesh.vertices = topMesh.vertices; // Setze die Vertices des Meshes auf die obere Hälfte
        mesh.triangles = topTriangles; // Setze die Indizes für die obere Hälfte
        mesh.RecalculateNormals(); // Normalen für die obere Hälfte berechnen
        CalculateMesh(gameObject, mesh, Color.red); // Berechne das Mesh mit der Farbe Rot


        GameObject lowerHalf = new GameObject("BottomHalf");
        lowerHalf.transform.position = Vector3.zero;
        bottomMesh.name = "BottomMesh";
        bottomMesh.vertices = bottomMesh.vertices; // Setze die Vertices des Meshes auf die untere Hälfte
        bottomMesh.triangles = bottomTriangles; // Setze die Indizes für die untere Hälfte
        bottomMesh.RecalculateNormals(); // Normalen für die untere Hälfte berechnen
        CalculateMesh(lowerHalf, bottomMesh, Color.green); // Berechne das Mesh mit der Farbe Grün
    }


    IEnumerator CutHorizontalInYCoroutine(Mesh mesh)
    {
        yield return new WaitForSeconds(2f);
        CutHorizontalInY(mesh);

    }
    IEnumerator CutVerticalInXCoroutine(Mesh mesh)
    {
        yield return new WaitForSeconds(2f);
        CutVerticalInX(mesh);

    }

    IEnumerator Cut3VerticesCoroutine(Mesh mesh)
    {
        yield return new WaitForSeconds(2f);
        Cut3Vertices(mesh);

    }
    IEnumerator Cut6VerticesCoroutine(Mesh mesh)
    {
        yield return new WaitForSeconds(2f);
        Cut6Vertices(mesh);

    }

    private void CalculateMesh(GameObject gameObject, Mesh mesh, Color color)
    {
        // Meshfilter enthält das Mesh, das für das GameObject angezeigt werden soll.
        // Geometrische Form eines 3D-Objekts, das aus Eckpunkten, Kanten und Flächen besteht.
        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        else
        {
            meshFilter = gameObject.GetComponent<MeshFilter>();
        }
        meshFilter.mesh = mesh;

        // Meshrenderer sorgt dafür, dass das Mesh im Spiel korrekt gerendert wird.
        if (gameObject.GetComponent<MeshRenderer>() == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        else
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }
        // Shader fügt die Render Pipeline hinzu und setzt die Farbe des Materials.
        shader = Shader.Find("Universal Render Pipeline/Lit");
        material = new Material(shader);
        material.color = color; // Farbe des Materials
        meshRenderer.material = material;

        // MeshCollider, um Kollisionen mit anderen Objekten zu erkennen.
        if (gameObject.GetComponent<MeshCollider>() == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        else
        {
            meshCollider = gameObject.GetComponent<MeshCollider>();
        }
        meshCollider.sharedMesh = mesh;
    }
}




