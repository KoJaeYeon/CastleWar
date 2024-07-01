using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Extensions;

public class PopulationPanelView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_Population;
    [SerializeField] Image Img_ManaMask;
    [SerializeField] Image Img_Button;
    [SerializeField] TextMeshProUGUI Text_CampNeedMana;

    private PopulationPanelViewModel _vm;
    private void OnEnable()
    {
        if (_vm == null)
        {
            _vm = new PopulationPanelViewModel();
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

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_vm.Mana):
                if (_vm.Mana >= 50)
                {
                    Img_Button.color = Color.white;
                    Text_CampNeedMana.color = Color.white;
                    Img_ManaMask.gameObject.SetActive(false);
                }
                else
                {
                    Img_Button.color = Color.gray;
                    Img_ManaMask.fillAmount = (50 - _vm.Mana) / 50f;
                    Text_CampNeedMana.color = Color.red;
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
