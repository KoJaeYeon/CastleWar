using ViewModel;

public class PopulationPanelViewModel : ViewModelBase
{
    private int _mana;
    private int _population;
    private int _maxPopulation;
    public int Mana
    {
        get { return _mana; }
        set
        {
            if (_mana == value)
                return;

            _mana = value;
            OnPropertyChanged(nameof(Mana));
        }
    }

    public int Population
    {
        get { return _population; }
        set
        {
            if (_population == value)
                return;

            _population = value;
            OnPropertyChanged(nameof(Population));
        }
    }

    public int MaxPopulation
    {
        get { return _maxPopulation; }
        set
        {
            if (_maxPopulation == value)
                return;

            _maxPopulation = value;
            OnPropertyChanged(nameof(MaxPopulation));
        }
    }

}
