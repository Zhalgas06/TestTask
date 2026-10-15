using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshController : MonoBehaviour
{
    [SerializeField]
    private int vertexCount = 100;

    [SerializeField]
    private int distanceBetweenVertexes = 5;

    private Mesh mesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Vector3[] vertices;
    private int[] triangles;


    private void Start()
    {
        vertexCount++;
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        mesh = new Mesh();
        mesh.name = "New Mesh";

        vertices = new Vector3[vertexCount * vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            for (int j = 0; j < vertexCount; j++)
            {
                vertices[i * vertexCount + j] = new Vector3(j * distanceBetweenVertexes, 0, i * distanceBetweenVertexes);
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
}
