
namespace ViewModel.Extensions
{
    public static class Btn_AddViewModelExtension
    {
        public static void RefreshViewModel(this Btn_AddViewModel vm)
        {
            GameManager.Instance.RefreshManaInfo(vm.OnRefreshViewModel);
            GameManager.Instance.RefreshPopulationInfo(vm.OnRefreshPopulationViewModel);
        }

        public static void OnRefreshViewModel(this Btn_AddViewModel vm, int mana)
        {
            vm.Mana = mana;
        }

        public static void OnRefreshPopulationViewModel(this Btn_AddViewModel vm, int population, int maxPopulation)
        {
            vm.Population = population;
            vm.MaxPopulation = maxPopulation;
        }

        public static void RegisterEventsOnEnable(this Btn_AddViewModel vm)
        {
            GameManager.Instance.RegisterManaChangeCallback(vm.OnResponseManaChange);
            GameManager.Instance.RegisterPopulationChangeCallback(vm.OnResponsePopulationChange);
        }

        public static void UnRegisterOnDisable(this Btn_AddViewModel vm)
        {
            GameManager.Instance.UnRegisterManaChangeCallback(vm.OnResponseManaChange);
            GameManager.Instance.UnRegisterPopulationChangeCallback(vm.OnResponsePopulationChange);
        }

        public static void OnResponseManaChange(this Btn_AddViewModel vm, int mana)
        {
            vm.Mana = mana;
        }
        public static void OnResponsePopulationChange(this Btn_AddViewModel vm, int population, int maxPopulation)
        {
            vm.Population = population;
            vm.MaxPopulation = maxPopulation;
        }

    }
}
