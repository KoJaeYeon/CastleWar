using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Extensions;

public class Btn_AddView : MonoBehaviour
{
    [SerializeField] GameObject view;
    [SerializeField] TextMeshProUGUI Text_Population;
    [SerializeField] Image Img_ManaMask;
    [SerializeField] Image Img_Button;
    [SerializeField] TextMeshProUGUI Text_UnitNeedMana;

    private Btn_AddViewModel _vm;
    int _cost;
    int _needPopulation;
    private void OnEnable()
    {
        InitEnable();
    }
    private void OnDisable()
    {
        if (_vm != null)
        {
            _vm.UnRegisterOnDisable();
            _vm.PropertyChanged -= OnPropertyChanged;
            _vm = null;
        }
    }

    public void InitEnable()
    {
        if (_vm == null)
        {
            _vm = new Btn_AddViewModel();
            _vm.PropertyChanged += OnPropertyChanged;
            _vm.RegisterEventsOnEnable();
            _vm.RefreshViewModel();
        }
    }

    public void InitData(int mana, int population)
    {
        _cost = mana;
        _needPopulation = population;
        view.SetActive(true);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        return;
        switch (e.PropertyName)
        {
            case nameof(_vm.Mana):
                if (_vm.Mana >= _cost)
                {
                    Img_Button.color = Color.white;
                    Text_UnitNeedMana.color = Color.white;
                    Img_ManaMask.gameObject.SetActive(false);
                }
                else
                {
                    Img_Button.color = Color.gray;
                    Img_ManaMask.fillAmount = (_cost - _vm.Mana) / (float)_cost;
                    Text_UnitNeedMana.color = Color.red;
                    Img_ManaMask.gameObject.SetActive(true);
                }
                break;
            case nameof(_vm.Population):
            case nameof(_vm.MaxPopulation):
                Text_Population.text = $"{_vm.Population}/{_vm.MaxPopulation}";
                break;
        }
    }
}
