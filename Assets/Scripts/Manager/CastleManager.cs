using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleManager : MonoBehaviour
{
    private static CastleManager _instance;

    public static CastleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("CastleManager").AddComponent<CastleManager>();
            }
            return _instance;
        }

    }

    public void Awake()
    {
        if (_instance == null || _instance == this)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
                
    }

    [SerializeField] GameObject AllyCastle;
    [SerializeField] GameObject EnemyCastle;
    [SerializeField] CircleUnionManager AllyUnion;
    [SerializeField] CircleUnionManager EnemyUnion;
    [SerializeField] Material _allyMaterial;
    [SerializeField] Material _enemyMaterial;    

    public Material AllyMaterial => _allyMaterial;
    public Material EnemyMaterial => _enemyMaterial;
    bool _destroyed = false;




    public GameObject GetCastleGameObj(bool isFriend)
    {
        if(isFriend)
        {
            return AllyCastle;
        }
        else
        {
            return EnemyCastle;
        }
    }

    private void Start()
    {
        float castleRadius = 26f;
        AllyUnion.AddCircle(AllyCastle.transform, castleRadius);
        EnemyUnion.AddCircle(EnemyCastle.transform,castleRadius);

        Castle allyCastle = AllyCastle.GetComponent<Castle>();
        Castle enemyCastle = EnemyCastle.GetComponent<Castle>();
        var castleData = DatabaseManager.Instance.OnGetUnitData(-3);
        allyCastle.InitData(castleData);
        enemyCastle.InitData(castleData);

        allyCastle.StartState();
        enemyCastle.StartState();

    }

    public void AddCampToUnion(Transform transform, bool isTagAlly)
    {
        if(isTagAlly)
        {
            AllyUnion.AddCircle(transform);
        }
        else
        {
            EnemyUnion.AddCircle(transform);
        }
    }

    public void AddSancToUnion(Transform transform, bool isTagAlly)
    {
        float sancRadius = 6f;
        if (isTagAlly)
        {
            AllyUnion.AddCircle(transform, sancRadius);
        }
        else
        {
            EnemyUnion.AddCircle(transform, sancRadius);
        }
    }

    public void RemoveUnion(Transform transform, bool isTagAlly)
    {
        if(isTagAlly)
        {
            AllyUnion.RemoveCircle(transform);
        }
        else
        {
            EnemyUnion.RemoveCircle(transform);
        }
    }

    public void Request_CastleTierUp(bool isTagAlly)
    {
        if(isTagAlly == true)
        {
            Castle allyCastle = AllyCastle.GetComponent<Castle>();
            allyCastle.RequestTierUp();
        }
        else
        {
            Castle enemyCastle = EnemyCastle.GetComponent<Castle>();
            enemyCastle.RequestTierUp();
        }

    }

    public void Request_CastleDestroy(bool isTagAlly)
    {
        if(_destroyed == false)
        {
            StartCoroutine(CameraMove(isTagAlly));
            _destroyed = true;
        }
        
    }

    IEnumerator CameraMove(bool isTagAlly)
    {
        Camera _camera = Camera.main;
        float duration = 2f;  // 총 걸리는 시간
        float elapsedTime = 0f;  // 경과 시간
        float startSize = _camera.orthographicSize;  // 시작 크기
        Vector3 startPos = _camera.transform.position;

        Vector3 targetPos = isTagAlly ? new Vector3(0, 45, -75) : new Vector3(0, 45, 25);

        while (elapsedTime < duration)
        {
            _camera.orthographicSize = Mathf.Lerp(startSize, 20, elapsedTime / duration);
            _camera.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;  // 경과 시간 업데이트
            yield return null;
        }

        // 최종 크기로 확실히 설정
        _camera.orthographicSize = 20;  
        _camera.transform.position = targetPos;

        yield return new WaitForSeconds(1f);

        AllyCastle.SetActive(false);
        EnemyCastle.SetActive(false);
        
    }
}
