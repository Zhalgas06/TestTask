using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshController : MonoBehaviour
{
    [SerializeField]
    private int vertexCount = 100;

    [SerializeField]
    private int distanceBetweenVertexes = 5;

    private Mesh mesh;
    private MeshFilter meshFilter;

    private Vector3[] vertices;
    private int[] triangles;
    private float[,] heightMap;


    private void Start()
    {
        vertexCount++;
        meshFilter = GetComponent<MeshFilter>();

        GenerateMesh();
    }

    private void GenerateMesh()
    {
        mesh = new Mesh();
        mesh.name = "Generated Mesh";

        vertices = new Vector3[vertexCount * vertexCount];
        heightMap = new float[vertexCount, vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            for (int j = 0; j < vertexCount; j++)
            {
                vertices[i * vertexCount + j] = new Vector3(j * distanceBetweenVertexes, heightMap[i, j], i * distanceBetweenVertexes);
            }
        }

        triangles = new int[(vertexCount - 1) * (vertexCount - 1) * 6];
        int index = 0;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            for (int j = 0; j < vertexCount - 1; j++)
            {
                int x = i * vertexCount + j;

                triangles[index++] = x;
                triangles[index++] = x + vertexCount;
                triangles[index++] = x + 1;

                triangles[index++] = x + 1;
                triangles[index++] = x + vertexCount;
                triangles[index++] = x + vertexCount + 1;
            }
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;
    }

    private void UpdateHeightMap()
    {
        for (int i = 0; i < vertexCount; i++)
        {
            for (int j = 0; j < vertexCount; j++)
            {
                vertices[i * vertexCount + j].y = heightMap[i, j];
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }


    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            heightMap[50, 50]++;
            UpdateHeightMap();
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            heightMap[50, 50]--;
            UpdateHeightMap();
        }
    }
}
