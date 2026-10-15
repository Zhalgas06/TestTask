using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshController : MonoBehaviour
{
    [Header("Brush Controlls")]
    [SerializeField]
    [Range(0f, 100f)]
    private float brushSize = 10;
    [SerializeField]
    [Range(-50f, 50f)]
    private int brushStrength = 10;

    [Header("Mesh Details")]
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
        if (Keyboard.current == null || Mouse.current == null)
            return;

        if (Keyboard.current.spaceKey.isPressed)
        {
            AddMaterial(50, 50);
        }

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0f)
        {
            brushSize += Mathf.Sign(scroll) * 1f;
            brushSize = Mathf.Clamp(brushSize, 0.1f, 100f);
        }
    }


    private void AddMaterial(int brushCenterX, int brushCenterZ)
    {
        for (int i = 0; i < vertexCount; i++)
        {
            for (int j = 0; j < vertexCount; j++)
            {
                float xDifference = vertices[i * vertexCount + j].x - vertices[i * vertexCount + brushCenterX].x;
                float zDifference = vertices[i * vertexCount + j].z - vertices[brushCenterZ * vertexCount + j].z;

                if (xDifference * xDifference + zDifference * zDifference <= brushSize * brushSize)
                {
                    heightMap[i, j] = heightMap[i, j] + brushStrength * Time.deltaTime;
                }
            }
        }

        UpdateHeightMap();
    }
}
