using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPanel : MonoBehaviour
{
    Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void OnClick_ActivePanel()
    {
        gameObject.SetActive(true);
        _animator.SetBool("isActive", true);
    }

    public void OnClick_DeactivePanel()
    {
        _animator.SetBool("isActive", false);
    }

    public void m_OnCalled_SetActiveFalse() //애니메이션에서 호출하는 함수
    {
        gameObject.SetActive(false);
    }

    public void OnClick_Surrender()
    {
        TcpSender.Instance.RequestCommand(4);
    }
}
