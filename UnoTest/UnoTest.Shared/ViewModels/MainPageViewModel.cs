using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace UnoTest.Shared.ViewModels
{

#if __WASM__
    [Bindable(true)]
#endif
    public class MainPageViewModel : ViewModelBase
    {
        private int numberOfClicks = 0;
        private ObservableCollection<int> numbers = new ObservableCollection<int>();

        public MainPageViewModel()
        {
            ClickMeButtonCommand = new RelayCommand(OnClickMeButton);          
        }

        public ICommand ClickMeButtonCommand { get; } 

        public int NumberOfClicks
        {
            get => numberOfClicks;
            set
            {
                numberOfClicks = value;
                RaisePropertyChanged(nameof(NumberOfClicks));
            }
        }

        public ObservableCollection<int> Numbers => numbers;
        
        private void OnClickMeButton()
        {
            NumberOfClicks += 1;
            Numbers.Add(NumberOfClicks);
        }
    }
}
