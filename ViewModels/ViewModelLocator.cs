
namespace MT5SignalReceiver.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            _Main = new MainViewModel();
        }

        private static MainViewModel _Main;
        public MainViewModel Main
        {
            get
            {
                return _Main;
            }
        }
    }
}