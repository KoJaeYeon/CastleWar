using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AddPanel : MonoBehaviour
{
    Animator _animator;
    [SerializeField] GameObject ChoicePanel;
    int _index = 0;

    UnitSelectSlot _slot;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        ChoicePanel.SetActive(false);
    }

    public void OnClick_ActivePanel(int index, Transform brnTrans)
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
        }
    }

    #region ChoicePanel
    [SerializeField] TextMeshProUGUI Text_Population;
    [SerializeField] TextMeshProUGUI Text_SelectUnitName;
    [SerializeField] TextMeshProUGUI Text_SpawnTimer;
    void Renew_ChoicePanel()
    {
        UnitData unitData = DatabaseManager.Instance.GetUnitData(_slot.id);
        Text_SelectUnitName.text = unitData.name;
        Text_Population.text = unitData.Population.ToString();
        Text_SpawnTimer.text = "10";
    }
    #endregion
}
