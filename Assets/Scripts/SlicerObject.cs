using Unity.Burst.Intrinsics;
using UnityEngine;

public class SlicerObject : MonoBehaviour
{

    [SerializeField] private float coneLength;
    [SerializeField] private int segments;
    [SerializeField] private float radius;

    private Mesh mesh;
    private MeshFilter meshFilter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the mesh generation with the specified parameters
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init()
    {
        GenerateMesh(coneLength, segments, radius);
    }

    private void GenerateMesh(float coneLength, int segments, float radius)
    {
        // Mesh generation logic goes here
        // This function will create a mesh based on the provided parameters
        // For example, you can use a Mesh class to define vertices, triangles, and normals
        // and then assign it to a MeshFilter component.

        mesh = new Mesh();
        Vector3[] vertices = new Vector3[segments + 2]; // +2 for the base center and apex vertex
        int[] triangles = new int[segments * 3 * 2]; // 2 triangles per segment (one for the base and one for the sides)
        Vector3[] normals = new Vector3[vertices.Length]; // Normals for lighting calculations
        float angleStep = 360f / segments; // Angle step for each segment

        // Create the base vertex at the bottom
        vertices[0] = Vector3.zero; // apex vertex 
        normals[0] = Vector3.up; // Normal for the apex vertex

        // Create the base vertices in a circle
        // The base vertices will be positioned in a circle around the origin
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * i * angleStep; // Convert angle to radians
            float x = radius * Mathf.Cos(angle); // Calculate x position based on radius and angle
            float z = radius * Mathf.Sin(angle); // Calculate z position based on radius and angle
            vertices[i + 1] = new Vector3(x, coneLength, z); // Base vertices
            normals[i + 1] = new Vector3(x, 0, z).normalized; // Normal for the base vertices
        }
        vertices[segments + 1] = new Vector3(0, coneLength, 0); // Base vertex
        normals[segments + 1] = Vector3.up; // Normal for the base vertex

        // Create triangles for the base
        for (int i = 0; i < segments; i++)
        {
            int nextIndex = (i + 1) % segments;
            triangles[i * 3] = segments + 1; // Base vertex
            triangles[i * 3 + 1] = nextIndex + 1; // Next base vertex
            triangles[i * 3 + 2] = i + 1; // Current base vertex
        }

        // Create triangles for the sides
        for (int i = 0; i < segments; i++)
        {
            int nextIndex = (i + 1) % segments;
            triangles[segments * 3 + i * 3] = 0; // Current base vertex
            triangles[segments * 3 + i * 3 + 2] = nextIndex + 1; // Apex vertex
            triangles[segments * 3 + i * 3 + 1] = i + 1; // Next base vertex
        }


        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.RecalculateBounds();
        mesh.Optimize(); // Optimize the mesh for better performance
        mesh.name = "GeneratedConeMesh"; // Set a name for the mesh


        // Assign the generated mesh to a MeshFilter component
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = mesh;
        // Optionally, you can also add a MeshRenderer component to visualize the mesh
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        // Set a default material for the MeshRenderer
        meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        // You can also set the material properties here if needed
        meshRenderer.material.color = Color.white; // Set a default color for the material

        // if (meshCollider == null)
        // {
        //     meshCollider = gameObject.AddComponent<MeshCollider>();
        // }
        // meshCollider.sharedMesh = mesh; // Assign the generated mesh to the MeshCollider
        // meshCollider.convex = true; // Set the collider to be convex if needed
        // meshCollider.isTrigger = true; // Set the collider to be a trigger if needed
        // Optionally, you can add a Rigidbody if you want physics interactions

    }

    public int GetSegments()
    {
        return segments;
    }

    public Plane GetPlaneOfCone(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        // return new Plane(v0, v1, v2);
        Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
        float distance = -Vector3.Dot(normal, v0);
        return new Plane(normal, distance);
    }

}
