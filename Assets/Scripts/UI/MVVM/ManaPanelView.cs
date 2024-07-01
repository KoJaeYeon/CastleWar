using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Extensions;

public class ManaPanelView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_Mana;
    [SerializeField] Image Img_ManaMask;
    [SerializeField] TextMeshProUGUI Text_SancNeedMana;

    private ManaPanelViewModel _vm;

    private void OnEnable()
    {
        if (_vm == null)
        {
            _vm = new ManaPanelViewModel();
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
        Debug.Log("dfs");
        Text_Mana.text = _vm.Mana.ToString();
        if (_vm.Mana >= 75)
        {
            Text_SancNeedMana.color = Color.white;
        }
        else
        {
            Img_ManaMask.fillAmount = (75 - _vm.Mana) / 75;
            Text_SancNeedMana.color = Color.red;
        }
    }
}
