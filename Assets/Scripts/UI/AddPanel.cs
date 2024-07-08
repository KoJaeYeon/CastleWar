using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddPanel : MonoBehaviour
{
    PlayerPanel _playerPanel;
    Animator _animator;
    [SerializeField] GameObject ChoicePanel;
    [SerializeField] GameObject[] Contents_TierUnit;
    [SerializeField] Button[] Button_Tier;
    
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
        GameManager.Instance.RefreshTierInfo(TierUpCheck);
        GameManager.Instance.RegisterTierChangeCallback(TierUpCheck);
    }

    public void TierUpCheck(int tier)
    {
        int index = 1;
        foreach (var btn in Button_Tier)
        {
            btn.interactable = (index <= tier);
            index++;
        }
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
        if (GameManager.Instance.RequestManaCheck(40))
        {
            GameManager.Instance.RequestManaUse(-40);
            AddSlot(_index++);
        }
    }

    public void OnClick_TierUnitSelect(int tier)
    {
        tier--;
        int idx = 0;
        foreach(var content in Contents_TierUnit)
        {
            if(idx == tier)
            {
                content.SetActive(true);
            }
            else
            {
                content.SetActive(false);
            }
            idx++;
        }

    }

    private void AddSlot(int slotIndex)
    {
        if (_slot != null)
        {
            TcpSender.Instance.RequestAddUnitSlot(slotIndex, _slot.Id);
            //SpawnManager.Instance.OnAdd_ObjectPoolingSlot(index, _slot.Id);
            ChoicePanel.SetActive(false);
            _slot.OnCalled_Add();
            _playerPanel.OnCalled_Added(slotIndex, _slot.transform.position, _slot.Id);
        }
    }

    #region ChoicePanel
    [SerializeField] Sprite[] Sprite_UnitType; // 0 Bulding, 1 Melee, 2 Range, 3 Mage
    [SerializeField] TextMeshProUGUI Text_Population;
    [SerializeField] TextMeshProUGUI Text_SelectUnitName;
    [SerializeField] TextMeshProUGUI Text_SpawnTimer;
    [SerializeField] Image Img_AttackType;
    void Renew_ChoicePanel()
    {
        UnitData unitData = DatabaseManager.Instance.OnGetUnitData(_slot.Id);
        Text_SelectUnitName.text = unitData.name;
        Text_Population.text = unitData.Population.ToString();
        Text_SpawnTimer.text = "10";
        List<int> ints = new List<int>() {6,12,15,19 };
        if(unitData.unitType == UnitType.Building)
        {
            Img_AttackType.sprite = Sprite_UnitType[0];
        }
        else if (unitData.AttackRange < 4)
        {
            Img_AttackType.sprite = Sprite_UnitType[1];
        }
        else if(ints.Contains(unitData.id))
        {
            Img_AttackType.sprite = Sprite_UnitType[3];
        }
        else
        {
            Img_AttackType.sprite= Sprite_UnitType[2];
        }
    }
    #endregion
}
