using UnityEngine;
using System.Collections.Generic;

public class CircleUnion3D : MonoBehaviour
{
    public Transform area1;
    public Transform area2;

    private Mesh unionMesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private Mesh area1Mesh;
    private Mesh area2Mesh;
    private MeshRenderer area1Renderer;
    private MeshRenderer area2Renderer;
    private MeshFilter area1Filter;
    private MeshFilter area2Filter;

    GameObject unionObject;

    void Start()
    {
        // 합집합 메쉬 초기화
        unionMesh = new Mesh();
        unionObject = new GameObject("UnionObject", typeof(MeshRenderer), typeof(MeshFilter));
        unionObject.transform.position = Vector3.zero;  // 원점으로 초기화
        unionObject.transform.Rotate(180, 0, 0);
        meshRenderer = unionObject.GetComponent<MeshRenderer>();
        meshFilter = unionObject.GetComponent<MeshFilter>();

        meshFilter.mesh = unionMesh;
        meshRenderer.material = new Material(Shader.Find("Standard"));

        // 개별 원 메쉬 초기화
        area1Mesh = new Mesh();
        area1Filter = area1.gameObject.AddComponent<MeshFilter>();
        area1Renderer = area1.gameObject.AddComponent<MeshRenderer>();
        area1Filter.mesh = area1Mesh;
        area1Renderer.material = new Material(Shader.Find("Standard"));

        area2Mesh = new Mesh();
        area2Filter = area2.gameObject.AddComponent<MeshFilter>();
        area2Renderer = area2.gameObject.AddComponent<MeshRenderer>();
        area2Filter.mesh = area2Mesh;
        area2Renderer.material = new Material(Shader.Find("Standard"));
    }

    void Update()
    {
        Vector2 center1 = new Vector2(area1.position.x, area1.position.z);
        float radius1 = area1.localScale.x / 2;

        Vector2 center2 = new Vector2(area2.position.x, area2.position.z);
        float radius2 = area2.localScale.x / 2;

        Vector2 intersection1, intersection2;
        if (CircleIntersection.FindCircleIntersections(center1, radius1, center2, radius2, out intersection1, out intersection2))
        {
            // 영역이 겹칠 때 합집합 메쉬를 업데이트합니다.
            UpdateUnionMesh(center1, radius1, center2, radius2, intersection1, intersection2);
            SetCircleMeshesActive(false);
        }
        else
        {
            // 영역이 겹치지 않을 때 개별 원 메쉬를 업데이트합니다.
            UpdateCircleMesh(area1Mesh, center1, radius1);
            UpdateCircleMesh(area2Mesh, center2, radius2);
            SetCircleMeshesActive(true);
        }
    }

    void UpdateUnionMesh(Vector2 center1, float radius1, Vector2 center2, float radius2, Vector2 intersection1, Vector2 intersection2)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        vertices.Add(new Vector3(center1.x, 0, center1.y));
        vertices.Add(new Vector3(intersection1.x, 0, intersection1.y));
        vertices.Add(new Vector3(center2.x, 0, center2.y));
        vertices.Add(new Vector3(intersection2.x, 0, intersection2.y));

        // 원1의 점들을 추가
        float angle1 = Mathf.Atan2(intersection1.y - center1.y, intersection1.x - center1.x);
        float angle2 = Mathf.Atan2(intersection2.y - center1.y, intersection2.x - center1.x);

        if (angle2 < angle1) angle2 += 2 * Mathf.PI;

        for (float angle = angle1; angle < angle2; angle += Mathf.PI / 16)
        {
            Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius1;
            vertices.Add(new Vector3(center1.x + point.x, 0, center1.y + point.y));
        }

        // 원2의 점들을 추가
        angle1 = Mathf.Atan2(intersection2.y - center2.y, intersection2.x - center2.x);
        angle2 = Mathf.Atan2(intersection1.y - center2.y, intersection1.x - center2.x);

        if (angle2 < angle1) angle2 += 2 * Mathf.PI;

        for (float angle = angle1; angle < angle2; angle += Mathf.PI / 16)
        {
            Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius2;
            vertices.Add(new Vector3(center2.x + point.x, 0, center2.y + point.y));
        }

        // 삼각형을 생성
        for (int i = 1; i < vertices.Count - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        unionMesh.Clear();
        unionMesh.vertices = vertices.ToArray();
        unionMesh.triangles = triangles.ToArray();
        unionMesh.RecalculateNormals();
        unionObject.transform.position = Vector3.zero;  // 원점으로 초기화
    }

    void UpdateCircleMesh(Mesh mesh, Vector2 center, float radius)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        vertices.Add(new Vector3(0, 0, 0)); // 중심점 추가
        int segments = 32;
        float angleIncrement = 2 * Mathf.PI / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleIncrement;
            Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            vertices.Add(new Vector3(point.x, 0, point.y)); // 점을 중심점에 더하지 않습니다.
            if (i > 0)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }
        }

        // 마지막 삼각형의 마지막 점을 첫 번째로 연결합니다.
        triangles.Add(0);
        triangles.Add(segments);
        triangles.Add(1);

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        // Mesh를 중앙에 배치
        var meshObject = meshFilter.gameObject;
        meshObject.transform.position = new Vector3(center.x, 0, center.y);
    }

    void SetCircleMeshesActive(bool isActive)
    {
        area1Renderer.enabled = isActive;
        area2Renderer.enabled = isActive;
        meshRenderer.enabled = !isActive;
    }
}