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
        if (_vm == null)
        {
            _vm = new Btn_AddViewModel();
            _vm.PropertyChanged += OnPropertyChanged;
            _vm.RegisterEventsOnEnable();
            _vm.RefreshViewModel();
        }
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

    public void InitData(int mana, int population)
    {
        _cost = mana;
        _needPopulation = population;
        view.SetActive(true);
        Text_UnitNeedMana.text = _cost.ToString();
        Text_Population.text = _needPopulation.ToString();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_vm.Mana):
                if (_vm.Mana >= _cost)
                {
                    Text_UnitNeedMana.color = Color.white;
                    Img_ManaMask.gameObject.SetActive(false);
                    if (_vm.Population + _needPopulation <= _vm.MaxPopulation) // 인구수가 될 때
                    {
                        Img_Button.color = Color.white;
                    }
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
                if (_vm.Population + _needPopulation > _vm.MaxPopulation) // 인구수가 모자를 때
                {
                    Text_Population.color = Color.red;
                    Img_Button.color = Color.gray;
                }
                else // 인구수 여유일 때
                {
                    Text_Population.color = Color.white;
                    if (_vm.Mana >= _cost)
                    {
                        Img_Button.color = Color.white;

                    }
                }
                break;
        }
    }
}
