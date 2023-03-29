using Launcher.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Launcher.Views
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : ReactivePage<MainViewModel>
    {
        public MainPage()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();
            ViewModel.Activator.Activate();

            this.WhenActivated(disposable =>
            {
                this.OneWayBind(this.ViewModel, x => x.Applications, x => x.Applications.ItemsSource)
                    .DisposeWith(disposable);
            });
        }
    }
}
