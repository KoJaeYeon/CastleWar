using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Extensions;

public class ManaPanelView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Text_Mana;
    [SerializeField] Image Img_Button;
    [SerializeField] Image Img_ManaMask;
    [SerializeField] TextMeshProUGUI Text_SancNeedMana;

    private ManaPanelViewModel _vm;
    private int targetMana;
    private int nowLookMana = 75;
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

    private void Update()
    {
        if(targetMana != nowLookMana)
        {
            nowLookMana += targetMana > nowLookMana ? 1 : -1;
            Text_Mana.text= nowLookMana.ToString();
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
        targetMana = _vm.Mana;
        if (_vm.Mana >= 75)
        {
            Img_Button.color = Color.white;
            Text_SancNeedMana.color = Color.white;
            Img_ManaMask.gameObject.SetActive(false);
        }
        else
        {
            Img_Button.color = Color.gray;
            Img_ManaMask.fillAmount = (75 - _vm.Mana) / 75f;
            Text_SancNeedMana.color = Color.red;
            Img_ManaMask.gameObject.SetActive(true);
        }
    }
}
