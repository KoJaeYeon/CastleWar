using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Extensions;

public class UtilityPanelView : MonoBehaviour
{
    [SerializeField] Image Img_Button;
    [SerializeField] Image Img_ManaMask;
    [SerializeField] TextMeshProUGUI Text_TierUPNeedMana;
    [SerializeField] GameObject TierUpMask;

    private UtilityPanelViewModel _vm;
    private void OnEnable()
    {
        if (_vm == null)
        {
            _vm = new UtilityPanelViewModel();
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
        if(TierUpMask.activeSelf) { return; }
        int needMana = GameManager.Instance.getTierUpNeedMana();
        Text_TierUPNeedMana.text = needMana.ToString();
        if (_vm.Mana >= needMana)
        {
            Img_Button.color = Color.white;
            Text_TierUPNeedMana.color = Color.white;
            Img_ManaMask.gameObject.SetActive(false);
        }
        else
        {
            Img_Button.color = Color.gray;
            Img_ManaMask.fillAmount = (needMana - _vm.Mana) /(float)needMana;
            Text_TierUPNeedMana.color = Color.red;
            Img_ManaMask.gameObject.SetActive(true);
        }
    }
}
