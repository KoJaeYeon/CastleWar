using ViewModel;

public class ManaPanelViewModel : ViewModelBase
{
    private int _mana;
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
}
