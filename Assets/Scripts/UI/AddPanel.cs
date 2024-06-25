using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AddPanel : MonoBehaviour
{
    PlayerPanel _playerPanel;
    Animator _animator;
    [SerializeField] GameObject ChoicePanel;
    int _index = 0;

    UnitSelectSlot _slot;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerPanel = transform.parent.GetComponent<PlayerPanel>();
    }

    private void OnEnable()
    {
        ChoicePanel.SetActive(false);
    }

    public void OnClick_ActivePanel()
    {
        gameObject.SetActive(true);
        _animator.SetBool("isActive", true);
    }

    public void OnClick_DeactivePanel()
    {
        _animator.SetBool("isActive",false);
    }

    public void m_OnCalled_SetActiveFalse() //애니메이션에서 호출하는 함수
    {
        gameObject.SetActive(false);
    }

    //유닛 초상화를 클릭 했을 때
    public void OnCalled_UnitSelctSlot(UnitSelectSlot unitSelectSlot)
    {
        _slot = unitSelectSlot;
        ChoicePanel.SetActive(true);
        Renew_ChoicePanel();
    }

    public void OnClick_AddButton()
    {
        AddSlot(_index++);
    }

    private void AddSlot(int index)
    {
        if (_slot != null)
        {
            SpawnManager.Instance.OnAdd_ObjectPoolingSlot(index, _slot.id);
            ChoicePanel.SetActive(false);
            _slot.OnCalled_Add();
            _playerPanel.OnCalled_Added(index,_slot.transform.position, _slot.id);
        }
    }

    #region ChoicePanel
    [SerializeField] TextMeshProUGUI Text_Population;
    [SerializeField] TextMeshProUGUI Text_SelectUnitName;
    [SerializeField] TextMeshProUGUI Text_SpawnTimer;
    void Renew_ChoicePanel()
    {
        UnitData unitData = DatabaseManager.Instance.OnGetUnitData(_slot.id);
        Text_SelectUnitName.text = unitData.name;
        Text_Population.text = unitData.Population.ToString();
        Text_SpawnTimer.text = "10";
    }
    #endregion
}
