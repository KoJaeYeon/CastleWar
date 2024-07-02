
namespace ViewModel.Extensions
{
    public static class UtilityPanelViewModelExtension
    {
        public static void RefreshViewModel(this UtilityPanelViewModel vm)
        {
            GameManager.Instance.RefreshManaInfo(vm.OnRefreshViewModel);
        }

        public static void OnRefreshViewModel(this UtilityPanelViewModel vm, int mana)
        {
            vm.Mana = mana;
        }

        public static void RegisterEventsOnEnable(this UtilityPanelViewModel vm)
        {
            GameManager.Instance.RegisterManaChangeCallback(vm.OnResponseManaChange);
        }

        public static void UnRegisterOnDisable(this UtilityPanelViewModel vm)
        {
            GameManager.Instance.UnRegisterManaChangeCallback(vm.OnResponseManaChange);
        }

        public static void OnResponseManaChange(this UtilityPanelViewModel vm, int mana)
        {
            vm.Mana = mana;
        }

    }

}

