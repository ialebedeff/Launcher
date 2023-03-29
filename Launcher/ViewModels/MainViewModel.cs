using Launcher.Consts;
using Launcher.Tools;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Windows;


namespace Launcher.ViewModels
{
    public class MainViewModel : ReactiveObject, IActivatableViewModel
    {
        public MainViewModel() 
        {
            GetApplicationsPathCommand = ReactiveCommand.Create(GetApplicationsPath);
            GetApplicationsCommand = ReactiveCommand.Create<string, IEnumerable<ApplicationViewModel>>(GetApplications);

            this.WhenActivated(disposables =>
            {
                GetApplicationsPathCommand.ThrownExceptions
                    .Subscribe(e => MessageBox.Show(e.Message))
                    .DisposeWith(disposables);

                GetApplicationsCommand.ThrownExceptions
                    .Subscribe(e => MessageBox.Show(e.Message))
                    .DisposeWith(disposables);

                GetApplicationsPathCommand
                    .Execute()
                    .Subscribe(result => {
                        Applications = GetApplications(result);
                    }).DisposeWith(disposables);
            });
        }
        /// <summary>
        /// Команда для получения пути до приложений
        /// </summary>
        public ReactiveCommand<Unit, string?> GetApplicationsPathCommand { get; set; }
        /// <summary>
        /// Команда для получения приложений
        /// </summary>
        public ReactiveCommand<string, IEnumerable<ApplicationViewModel>> GetApplicationsCommand { get; set; }
        /// <summary>
        /// Приложения
        /// </summary>
        private IEnumerable<ApplicationViewModel> _applications 
            = Enumerable.Empty<ApplicationViewModel>();
        public IEnumerable<ApplicationViewModel> Applications
        {
            get { return _applications; }
            set { this.RaiseAndSetIfChanged(ref _applications, value); }
        }
        /// <summary>
        /// Активатор вью модели
        /// </summary>
        public ViewModelActivator Activator { get; set; } = new();
        /// <summary>
        /// Получить путь до приложений
        /// </summary>
        /// <returns></returns>
        private string? GetApplicationsPath() => PathManagement.GetSmartMixPath();
        /// <summary>
        /// Получить Приложения
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        private IEnumerable<ApplicationViewModel> GetApplications(string? path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new Exception("На вашем компьютере не найдено приложение SmartMix");
            }

            return new List<ApplicationViewModel>
            {
                ApplicationFactory.Create(
                      ApplicationNames.Dispatcher, ApplicationNames.DispatcherExe
                    , PathManagement.CombinePath(path, ApplicationNames.DispatcherExe)),

                ApplicationFactory.Create(
                     ApplicationNames.Lab, ApplicationNames.LabExe
                   , PathManagement.CombinePath(path, ApplicationNames.LabExe)),

                ApplicationFactory.Create(
                     ApplicationNames.SmartMix, ApplicationNames.SmartMixExe
                   , PathManagement.CombinePath(path, ApplicationNames.SmartMixExe)),

                ApplicationFactory.Create(
                     ApplicationNames.Backuper, ApplicationNames.BackUperExe
                   , PathManagement.CombinePath(path, ApplicationNames.BackUperExe)),

                ApplicationFactory.Create(
                     ApplicationNames.Reporter, ApplicationNames.ReporterExe
                   , PathManagement.CombinePath(path, ApplicationNames.ReporterExe)),

                ApplicationFactory.Create(
                     ApplicationNames.PanelConfigurator, ApplicationNames.PanelConfiguratorExe
                   , PathManagement.CombinePath(path, ApplicationNames.PanelConfiguratorExe))
            };
        }
    }
}
