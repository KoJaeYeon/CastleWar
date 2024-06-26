using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler,IPointerDownHandler,IPointerUpHandler
{
    private Camera mainCamera; // 메인 카메라
    Vector3 mapPoint;

    bool isDown = false;
    void Start()
    {
        // 카메라 컴포넌트 캐싱
        mainCamera = Camera.main;
    }

    void Update()
    {
        if(isDown)
        {
        TouchManager.Instance.UpdateSelectable(mapPoint);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Pointer entered at screen position: " + eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Pointer exited from screen position: " + eventData.position);
        TouchManager.Instance.OnPointerExitSelcetable();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        //Debug.Log("Pointer moving at screen position: " + eventData.position);
        RaycastToWorld(eventData.position); // 월드 좌표 변환 및 레이캐스트 호출
    }

    void RaycastToWorld(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Map");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // 레이가 오브젝트와 충돌했을 때
            //Debug.Log("Ray hit " + hit.collider.gameObject.name + " at position " + hit.point);
            mapPoint = hit.point;
        }
        else
        {
            // 레이가 아무것도 충돌하지 않았을 때
            //Debug.Log("Ray did not hit any object on the Map layer.");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TouchManager.Instance.OnPointerDownSelectable();
        isDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        TouchManager.Instance.OnPointerUpSelectable();
        isDown = false;
    }
}
