using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPanel : MonoBehaviour
{
    Animator _animator;
    int _index = -1;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OnClick_ActivePanel(int index)
    {
        gameObject.SetActive(true);
        _animator.SetBool("isActive", true);
        _index = index;
    }

    public void OnClick_DeactivePanel()
    {
        _animator.SetBool("isActive",false);
    }

    public void OnCalled_SetActiveFalse()
    {
        gameObject.SetActive(false);
    }

    public void OnClick_AddButton()
    {
        AddSlot(0);
    }

    private void AddSlot(int index)
    {
        SpawnManager.Instance.OnAdd_ObjectPoolingSlot(index, 0);
    }
}
