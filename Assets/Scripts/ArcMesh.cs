using UnityEngine;

[ExecuteInEditMode]
public class ArcMesh : MonoBehaviour
{
    public string MeshName = "ArcMesh";
    public float AngleStart = 0.0f;
    public float AngleEnd = 0.0f;
    public float UStart = 0.0f;
    public float UEnd = 0.0f;
    public float AngleOfSubdivision = 10.0f;
    public Vector2[] ExtrudeProfileVertexPositions = null;
    public Vector2[] ExtrudeProfileVertexNormals = null;
    public float[] ExtrudeProfileVertexVs = null;
    public bool CloseLoop = false;

    private void Start()
    {
        RebuildMesh();
    }

    private void OnValidate()
    {
        //Beware - I get Unity crashes when building asset bundle with this line here!
        //RebuildMesh();
    }

    public void RebuildMesh()
    {
        if (ExtrudeProfileVertexPositions == null || ExtrudeProfileVertexPositions.Length == 0 ||
            ExtrudeProfileVertexNormals == null || ExtrudeProfileVertexNormals.Length == 0 ||
            ExtrudeProfileVertexVs == null || ExtrudeProfileVertexVs.Length == 0 ||
            ExtrudeProfileVertexPositions.Length != ExtrudeProfileVertexNormals.Length ||
            ExtrudeProfileVertexPositions.Length != ExtrudeProfileVertexVs.Length)
        {
            return;
        }

        if (AngleOfSubdivision <= 0.0f)
        {
            return;
        }

        float angleRange = AngleEnd - AngleStart;
        if (angleRange <= 0.0f || angleRange > 360.0f)
        {
            return;
        }

        int subdivisionCount = Mathf.CeilToInt(angleRange / AngleOfSubdivision) + 1;

        int vertexStride = ExtrudeProfileVertexPositions.Length;
        int quadStride = vertexStride - 1;
        int triangleIndexStride = quadStride * 6;

        int vertexCount = vertexStride * subdivisionCount;
        if (CloseLoop)
        {
            vertexCount += vertexStride;
        }

        int triangleIndexCount = triangleIndexStride * (subdivisionCount - 1);
        if (CloseLoop)
        {
            triangleIndexCount += triangleIndexStride;
        }

        Vector3[] vertPositions = new Vector3[vertexCount];
        Vector3[] vertNormals = new Vector3[vertexCount];
        Vector2[] vertUVs = new Vector2[vertexCount];
        int[] triangleIndices = new int[triangleIndexCount];

        for (int subdivisionIndex = 0; subdivisionIndex < subdivisionCount; ++subdivisionIndex)
        {
            int vertexIndexStart = subdivisionIndex * vertexStride;

            float angle = AngleStart + AngleOfSubdivision * subdivisionIndex;
            if (subdivisionIndex == subdivisionCount - 1)
            {
                angle = AngleEnd;
            }
            else
            {
                int triangleIndexStart = subdivisionIndex * triangleIndexStride;
                for (int vertexIndexOffset = 0; vertexIndexOffset < quadStride; ++vertexIndexOffset)
                {
                    int firstVertexIndex = vertexIndexStart + vertexIndexOffset;
                    triangleIndices[triangleIndexStart + (vertexIndexOffset * 6) + 0] = firstVertexIndex + 0;
                    triangleIndices[triangleIndexStart + (vertexIndexOffset * 6) + 1] = firstVertexIndex + 1;
                    triangleIndices[triangleIndexStart + (vertexIndexOffset * 6) + 2] = firstVertexIndex + vertexStride + 0;
                    triangleIndices[triangleIndexStart + (vertexIndexOffset * 6) + 3] = firstVertexIndex + vertexStride + 0;
                    triangleIndices[triangleIndexStart + (vertexIndexOffset * 6) + 4] = firstVertexIndex + 1;
                    triangleIndices[triangleIndexStart + (vertexIndexOffset * 6) + 5] = firstVertexIndex + vertexStride + 1;
                }
            }

            float angleNormalised = (angle - AngleStart) / (AngleEnd - AngleStart);
            float finalU = Mathf.Lerp(UStart, UEnd, angleNormalised);

            float angleRad = angle * Mathf.Deg2Rad;

            for (int vertexIndexOffset = 0; vertexIndexOffset < vertexStride; ++vertexIndexOffset)
            {
                float sinAngle = Mathf.Sin(angleRad);
                float cosAngle = Mathf.Cos(angleRad);

                Vector2 extrudePosition = ExtrudeProfileVertexPositions[vertexIndexOffset];
                Vector2 extrudeNormal = ExtrudeProfileVertexNormals[vertexIndexOffset];
                float extrudeV = ExtrudeProfileVertexVs[vertexIndexOffset];

                vertPositions[vertexIndexStart + vertexIndexOffset] = new Vector3(sinAngle * extrudePosition.x, extrudePosition.y, cosAngle * extrudePosition.x);
                vertNormals[vertexIndexStart + vertexIndexOffset] = new Vector3(sinAngle * extrudeNormal.x, extrudeNormal.y, cosAngle * extrudeNormal.x).normalized;
                vertUVs[vertexIndexStart + vertexIndexOffset] = new Vector2(finalU, extrudeV);
            }
        }

        if (CloseLoop)
        {
            float angleRad = AngleStart * Mathf.Deg2Rad;

            int vertexIndexStart = subdivisionCount * vertexStride;
            int triangleIndexStart = (subdivisionCount - 1) * triangleIndexStride;
            for (int vertexIndexOffset = 0; vertexIndexOffset < vertexStride; ++vertexIndexOffset)
            {
                if (vertexIndexOffset < quadStride)
                {
                    int firstVertexIndex = vertexIndexStart + vertexIndexOffset;
                    triangleIndices[triangleIndexStart + (vertexIndexOffset * 6) + 0] = (firstVertexIndex - vertexStride) + 0;
                    triangleIndices[triangleIndexStart + (vertexIndexOffset * 6) + 1] = (firstVertexIndex - vertexStride) + 1;
                    triangleIndices[triangleIndexStart + (vertexIndexOffset * 6) + 2] = firstVertexIndex + 0;
                    triangleIndices[triangleIndexStart + (vertexIndexOffset * 6) + 3] = firstVertexIndex + 0;
                    triangleIndices[triangleIndexStart + (vertexIndexOffset * 6) + 4] = (firstVertexIndex - vertexStride) + 1;
                    triangleIndices[triangleIndexStart + (vertexIndexOffset * 6) + 5] = firstVertexIndex + 1;
                }

                float sinAngle = Mathf.Sin(angleRad);
                float cosAngle = Mathf.Cos(angleRad);

                Vector2 extrudePosition = ExtrudeProfileVertexPositions[vertexIndexOffset];
                Vector2 extrudeNormal = ExtrudeProfileVertexNormals[vertexIndexOffset];
                float extrudeV = ExtrudeProfileVertexVs[vertexIndexOffset];

                vertPositions[vertexIndexStart + vertexIndexOffset] = new Vector3(sinAngle * extrudePosition.x, extrudePosition.y, cosAngle * extrudePosition.x);
                vertNormals[vertexIndexStart + vertexIndexOffset] = new Vector3(sinAngle * extrudeNormal.x, extrudeNormal.y, cosAngle * extrudeNormal.x).normalized;
                vertUVs[vertexIndexStart + vertexIndexOffset] = new Vector2(UEnd, extrudeV);
            }
        }

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            mesh = new Mesh();
            meshFilter.sharedMesh = mesh;
        }

        mesh.name = MeshName;
        mesh.Clear();
        mesh.vertices = vertPositions;
        mesh.normals = vertNormals;
        mesh.uv = vertUVs;
        mesh.triangles = triangleIndices;
    }
}
