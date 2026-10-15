using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshController : MonoBehaviour
{
    [Header("Brush Controlls")]
    [SerializeField]
    private int brushStrength = 10;

    [SerializeField]
    private float brushMoveSpeed = 10f;

    [SerializeField] private float baseRadius = 1f;
    [SerializeField] private float radiusAmplitude = 0.3f;
    [SerializeField] private float radiusFrequency = 1f;

    [SerializeField]
    private AnimationCurve radiusCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    private float brushSize = 10;

    private float radiusTimer;

    private float brushX;
    private float brushZ;

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
        brushX = vertexCount / 2;
        brushZ = vertexCount / 2;

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

        MoveBrush();
        UpdateBrushRadius();

        if (Keyboard.current.spaceKey.isPressed)
            AddContinuousMaterial();

        if (Keyboard.current.fKey.isPressed)
            ResetHeightMap();
    }


    private void AddMaterial(int brushCenterX, int brushCenterZ, int steps)
    {
        for (int i = 0; i < vertexCount; i++)
        {
            for (int j = 0; j < vertexCount; j++)
            {
                float xDifference = vertices[i * vertexCount + j].x - vertices[i * vertexCount + brushCenterX].x;
                float zDifference = vertices[i * vertexCount + j].z - vertices[brushCenterZ * vertexCount + j].z;

                if (xDifference * xDifference + zDifference * zDifference <= brushSize * brushSize)
                {
                    heightMap[i, j] = heightMap[i, j] + brushStrength * Time.deltaTime / steps;
                }
            }
        }

        UpdateHeightMap();
    }


    private void MoveBrush()
    {
        float x = 0f;
        float z = 0f;

        if (Keyboard.current.wKey.isPressed) z += 1f;
        if (Keyboard.current.sKey.isPressed) z -= 1f;
        if (Keyboard.current.dKey.isPressed) x += 1f;
        if (Keyboard.current.aKey.isPressed) x -= 1f;

        Vector2 direction = new Vector2(x, z).normalized;

        brushX = Mathf.Clamp(brushX + direction.x * brushMoveSpeed * Time.deltaTime, 0f, vertexCount - 1);

        brushZ = Mathf.Clamp(brushZ + direction.y * brushMoveSpeed * Time.deltaTime, 0f, vertexCount - 1);
    }

    private void ResetHeightMap()
    {
        System.Array.Clear(heightMap, 0, heightMap.Length);
        UpdateHeightMap();
    }

    private Vector2 previousBrushPosition;
    private bool hasPreviousBrushPosition;
    private void AddContinuousMaterial()
    {
        Vector2 currentPosition = new Vector2(brushX, brushZ);

        if (!hasPreviousBrushPosition)
        {
            previousBrushPosition = currentPosition;
            hasPreviousBrushPosition = true;
        }

        float distance = Vector2.Distance(previousBrushPosition, currentPosition);

        float step = Mathf.Max(0.5f, brushSize * 0.5f);

        int steps = Mathf.Max(1, Mathf.CeilToInt(distance / step));

        for (int i = 1; i <= steps; i++)
        {
            Vector2 position = Vector2.Lerp(previousBrushPosition, currentPosition, (float)i / steps);

            AddMaterial(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), steps);
        }

        previousBrushPosition = currentPosition;
    }

    private void UpdateBrushRadius()
    {
        radiusTimer += Time.deltaTime * radiusFrequency;

        float t = Mathf.Repeat(radiusTimer, 1f);
        float curveValue = radiusCurve.Evaluate(t);

        brushSize = baseRadius + (curveValue * 2f - 1f) * radiusAmplitude;

        brushSize = Mathf.Max(0.1f, brushSize);
    }
}
