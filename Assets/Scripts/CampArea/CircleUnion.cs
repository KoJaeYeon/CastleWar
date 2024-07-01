using UnityEngine;
using System.Collections.Generic;

public class CircleUnionManager : MonoBehaviour
{
    public List<Transform> circles = new List<Transform>(); // 원 리스트

    private Mesh unionMesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private List<Mesh> circleMeshes = new List<Mesh>();
    private List<MeshRenderer> circleRenderers = new List<MeshRenderer>();
    private List<MeshFilter> circleFilters = new List<MeshFilter>();
    private List<CircleData> circleDataList = new List<CircleData>();

    GameObject unionObject;
    public GameObject circlePrefab;

    private float minX = -28f;
    private float maxX = 28f;
    private float minZ = -57f;
    private float maxZ = 57f;
    [SerializeField] bool isTagAlly;

    void Start()
    {
        // 합집합 메쉬 초기화
        unionMesh = new Mesh();
        unionObject = new GameObject("UnionObject", typeof(MeshRenderer), typeof(MeshFilter));
        Debug.Log(name);
        unionObject.transform.position = Vector3.zero;  // 원점으로 초기화
        meshRenderer = unionObject.GetComponent<MeshRenderer>();
        meshFilter = unionObject.GetComponent<MeshFilter>();

        meshFilter.mesh = unionMesh;
        meshRenderer.material = isTagAlly ? CastleManager.Instance.AllyMaterial : CastleManager.Instance.EnemyMaterial;
    }

    void CheckMeshOnValueChanged()
    {
        if (circles.Count < 2)
        {
            return; // 원이 두 개 이상일 때만 합집합 계산
        }

        List<Vector3> unionVertices = new List<Vector3>();
        List<int> unionTriangles = new List<int>();
        HashSet<int> overlappingCircles = new HashSet<int>();

        for (int i = 0; i < circles.Count; i++)
        {
            for (int j = i + 1; j < circles.Count; j++)
            {
                Vector2 center1 = new Vector2(circles[i].position.x, circles[i].position.z);
                float radius1 = circleDataList[i].radius;

                Vector2 center2 = new Vector2(circles[j].position.x, circles[j].position.z);
                float radius2 = circleDataList[j].radius;

                Vector2 intersection1, intersection2;
                if (CircleIntersection.FindCircleIntersections(center1, radius1, center2, radius2, out intersection1, out intersection2))
                {
                    // 영역이 겹칠 때 합집합 메쉬를 업데이트합니다.
                    UpdateUnionMesh(center1, radius1, center2, radius2, intersection1, intersection2, unionVertices, unionTriangles);
                    overlappingCircles.Add(i);
                    overlappingCircles.Add(j);
                }
            }
        }

        if (unionVertices.Count > 0)
        {
            unionMesh.Clear();
            unionMesh.vertices = unionVertices.ToArray();
            unionMesh.triangles = unionTriangles.ToArray();
            unionMesh.RecalculateNormals();
            unionObject.SetActive(true);
        }
        else
        {
            unionObject.SetActive(false);
        }

        UpdateCircleMeshes();
        SetCircleMeshesActive(true, overlappingCircles);
        unionObject.transform.position = Vector3.zero;
    }

    public void AddCircle(Transform transform, float radius = 13f)
    {
        //GameObject newCircle = new GameObject("Circle");
        GameObject newCircle = Instantiate(circlePrefab);
        newCircle.transform.SetParent(transform);
        newCircle.transform.position = transform.position;
        circles.Add(newCircle.transform);

        newCircle.layer = isTagAlly? LayerMask.NameToLayer("Map") : LayerMask.NameToLayer("Default");


        MeshFilter meshFilter = newCircle.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = newCircle.GetComponent<MeshRenderer>();
        CircleData circleData = newCircle.GetComponent<CircleData>();

        circleData.radius = radius; // 반지름 설정

        Mesh circleMesh = new Mesh();
        meshFilter.mesh = circleMesh;
        meshRenderer.material = isTagAlly ? CastleManager.Instance.AllyMaterial : CastleManager.Instance.EnemyMaterial;

        circleMeshes.Add(circleMesh);
        circleFilters.Add(meshFilter);
        circleRenderers.Add(meshRenderer);
        circleDataList.Add(circleData);

        UpdateCircleMesh(circleMesh, new Vector2(transform.position.x,transform. position.z), circleData.radius);

        CheckMeshOnValueChanged();

        MeshCollider meshCollider = newCircle.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
    }

    public void RemoveCircle(Transform circleParent)
    {
        Transform circle = circleParent.GetComponentInChildren<CircleData>().transform;
        
        int index = circles.IndexOf(circle);
        if (index != -1)
        {
            // 리스트에서 제거
            circles.RemoveAt(index);
            Destroy(circle.gameObject);

            circleMeshes.RemoveAt(index);
            circleRenderers.RemoveAt(index);
            circleFilters.RemoveAt(index);
            circleDataList.RemoveAt(index);
        }

        CheckMeshOnValueChanged();
    }

    void UpdateUnionMesh(Vector2 center1, float radius1, Vector2 center2, float radius2, Vector2 intersection1, Vector2 intersection2, List<Vector3> vertices, List<int> triangles)
    {
        int initialVertexCount = vertices.Count;

        vertices.Add(ClampVertex(new Vector3(center1.x, 0, center1.y)));
        vertices.Add(ClampVertex(new Vector3(intersection1.x, 0, intersection1.y)));
        vertices.Add(ClampVertex(new Vector3(center2.x, 0, center2.y)));
        vertices.Add(ClampVertex(new Vector3(intersection2.x, 0, intersection2.y)));

        // 원1의 점들을 추가
        float angle1 = Mathf.Atan2(intersection1.y - center1.y, intersection1.x - center1.x);
        float angle2 = Mathf.Atan2(intersection2.y - center1.y, intersection2.x - center1.x);

        if (angle2 < angle1) angle2 += 2 * Mathf.PI;

        for (float angle = angle1; angle < angle2; angle += Mathf.PI / 16)
        {
            Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius1;
            vertices.Add(ClampVertex(new Vector3(center1.x + point.x, 0, center1.y + point.y)));
        }

        // 원2의 점들을 추가
        angle1 = Mathf.Atan2(intersection2.y - center2.y, intersection2.x - center2.x);
        angle2 = Mathf.Atan2(intersection1.y - center2.y, intersection1.x - center2.x);

        if (angle2 < angle1) angle2 += 2 * Mathf.PI;

        for (float angle = angle1; angle < angle2; angle += Mathf.PI / 16)
        {
            Vector2 point = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius2;
            vertices.Add(ClampVertex(new Vector3(center2.x + point.x, 0, center2.y + point.y)));
        }

        // 삼각형을 생성
        for (int i = initialVertexCount + 1; i < vertices.Count - 1; i++)
        {
            triangles.Add(initialVertexCount);
            triangles.Add(i + 1);
            triangles.Add(i);
        }
    }

    void UpdateCircleMeshes()
    {
        for (int i = 0; i < circles.Count; i++)
        {
            Vector2 center = new Vector2(circles[i].position.x, circles[i].position.z);
            float radius = circleDataList[i].radius;

            UpdateCircleMesh(circleMeshes[i], center, radius);
        }
    }

    void UpdateCircleMesh(Mesh mesh, Vector2 center, float radius)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        vertices.Add(Vector3.zero); // 중심점 추가
        int segments = 32;
        float angleIncrement = 2 * Mathf.PI / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleIncrement;
            Vector2 point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            vertices.Add(ClampVertex(new Vector3(point.x, 0, point.y)) - new Vector3(center.x, 0, center.y)); // 점을 중심점에 더하지 않습니다.
            if (i > 0)
            {
                triangles.Add(0);
                triangles.Add(i + 1);
                triangles.Add(i);
            }
        }

        // 마지막 삼각형의 마지막 점을 첫 번째로 연결합니다.
        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(segments);

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        // Mesh를 중앙에 배치
        var meshObject = meshFilter.gameObject;
        meshObject.transform.position = new Vector3(center.x, 0, center.y);
    }

    Vector3 ClampVertex(Vector3 vertex)
    {
        vertex.x = Mathf.Clamp(vertex.x, minX, maxX);
        vertex.z = Mathf.Clamp(vertex.z, minZ, maxZ);
        return vertex;
    }

    void SetCircleMeshesActive(bool isActive, HashSet<int> overlappingCircles)
    {
        for (int i = 0; i < circleRenderers.Count; i++)
        {
            circleRenderers[i].enabled = isActive && !overlappingCircles.Contains(i);
        }
        meshRenderer.enabled = isActive; // unionObject의 렌더러도 활성화/비활성화
    }
}
