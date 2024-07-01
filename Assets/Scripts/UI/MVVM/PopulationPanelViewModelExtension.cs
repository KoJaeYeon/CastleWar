
namespace ViewModel.Extensions
{
    public static class PupulationPanelViewModelExtension
    {
        public static void RefreshViewModel(this PopulationPanelViewModel vm)
        {
            GameManager.Instance.RefreshManaInfo(vm.OnRefreshViewModel);
            GameManager.Instance.RefreshPopulationInfo(vm.OnRefreshPopulationViewModel);
        }

        public static void OnRefreshViewModel(this PopulationPanelViewModel vm, int mana)
        {
            vm.Mana = mana;
        }

        public static void OnRefreshPopulationViewModel(this PopulationPanelViewModel vm, int population, int maxPopulation)
        {
            vm.Population = population;
            vm.MaxPopulation = maxPopulation;
        }

        public static void RegisterEventsOnEnable(this PopulationPanelViewModel vm)
        {
            GameManager.Instance.RegisterManaChangeCallback(vm.OnResponseManaChange);
            GameManager.Instance.RegisterPopulationChangeCallback(vm.OnResponsePopulationChange);
        }

        public static void UnRegisterOnDisable(this PopulationPanelViewModel vm)
        {
            GameManager.Instance.UnRegisterManaChangeCallback(vm.OnResponseManaChange);
            GameManager.Instance.UnRegisterPopulationChangeCallback(vm.OnResponsePopulationChange);
        }

        public static void OnResponseManaChange(this PopulationPanelViewModel vm, int mana)
        {
            vm.Mana = mana;
        }
        public static void OnResponsePopulationChange(this PopulationPanelViewModel vm, int population, int maxPopulation)
        {
            vm.Population = population;
            vm.MaxPopulation = maxPopulation;
        }

    }
}
