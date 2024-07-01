
namespace ViewModel.Extensions
{
    public static class ManaPanelViewModelExtension
    {
        public static void RefreshViewModel(this ManaPanelViewModel vm)
        {
            GameManager.Instance.RefreshManaInfo(vm.OnRefreshViewModel);
        }

        public static void OnRefreshViewModel(this ManaPanelViewModel vm, int mana)
        {
            vm.Mana = mana;
        }

        public static void RegisterEventsOnEnable(this ManaPanelViewModel vm)
        {
            GameManager.Instance.RegisterManaChangeCallback(vm.OnResponseManaChange);
        }

        public static void UnRegisterOnDisable(this ManaPanelViewModel vm)
        {
            GameManager.Instance.UnRegisterManaChangeCallback(vm.OnResponseManaChange);
        }

        public static void OnResponseManaChange(this ManaPanelViewModel vm, int mana)
        {
            vm.Mana = mana;
        }

    }

}

