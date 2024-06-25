using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler,IPointerDownHandler,IPointerUpHandler
{
    private Camera mainCamera; // ���� ī�޶�
    Vector3 mapPoint;

    bool isDown = false;
    void Start()
    {
        // ī�޶� ������Ʈ ĳ��
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
        Debug.Log("Pointer entered at screen position: " + eventData.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer exited from screen position: " + eventData.position);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        //Debug.Log("Pointer moving at screen position: " + eventData.position);
        RaycastToWorld(eventData.position); // ���� ��ǥ ��ȯ �� ����ĳ��Ʈ ȣ��
    }

    void RaycastToWorld(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Map");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // ���̰� ������Ʈ�� �浹���� ��
            //Debug.Log("Ray hit " + hit.collider.gameObject.name + " at position " + hit.point);
            mapPoint = hit.point;
        }
        else
        {
            // ���̰� �ƹ��͵� �浹���� �ʾ��� ��
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
