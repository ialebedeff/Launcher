using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DynamicData.Binding;
using ReactiveUI;

namespace Launcher.ViewModels
{
    public class ProcessStateListener : ReactiveObject
    { 
        public ProcessStateListener(string execution) 
        {
            Execution = execution;
        }
        /// <summary>
        /// 
        /// </summary>
        private string _execution;
        /// <summary>
        /// 
        /// </summary>
        public string Execution
        {
            get { return this._execution; }
            set { this.RaiseAndSetIfChanged(ref _execution, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public Process? Process { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        private bool _isRunning;
        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning
        {
            get { return this._isRunning; }
            set { this.RaiseAndSetIfChanged(ref _isRunning, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ObserveProcessStateAsync()
        {
            if (Process is not null)
            {
                await Process.WaitForExitAsync();
            }
            IsRunning = false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void SetProcessState()
        { 
            if (Process is not null)
                IsRunning = true;
        }
        /// <summary>
        /// 
        /// </summary>
        private void SetProcess()
            => Process = Process
                .GetProcessesByName(Execution.Replace(".exe", ""))
                .FirstOrDefault();

        public void CloseProcess()
            => Process?.Kill();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task? ReloadListenerAsync()
        {
            SetProcess();
            SetProcessState();
            return ObserveProcessStateAsync();
        }
    }
    public class ApplicationViewModel : ReactiveObject, IActivatableViewModel
    {
        public ApplicationViewModel(string name, string execution, string path)
        {
            Name = name;
            Path = path;
            Execution= execution;
            StartApplicationCommand = ReactiveCommand.Create(StartApplication);
            ProcessStateListener = new ProcessStateListener(execution);

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.ProcessStateListener.IsRunning)
                    .Subscribe(x =>
                    {
                        if (x) Status = "Запущено";
                        else Status = "Не запущено";
                    })
                    .DisposeWith(disposables);

                StartApplicationCommand
                    .Subscribe(_ =>
                    {
                        ProcessStateListener.ReloadListenerAsync();
                    })
                    .DisposeWith(disposables);

                StartApplicationCommand.ThrownExceptions
                    .Subscribe(exception => MessageBox.Show(exception.Message))
                    .DisposeWith(disposables);
            });

            _ = ProcessStateListener.ReloadListenerAsync();
        }
        public ProcessStateListener ProcessStateListener { get; set; }
        /// <summary>
        /// Активатор вью модели
        /// </summary>
        public ViewModelActivator Activator { get; set; } = new();
        /// <summary>
        /// Команда для старта приложения
        /// </summary>
        public ReactiveCommand<Unit, Unit> StartApplicationCommand { get; set; }
        /// <summary>
        /// Название приложения
        /// </summary>
        private string _execution = string.Empty;
        /// <summary>
        /// Название приложения
        /// </summary>
        public string Execution
        {
            get { return this._execution; }
            set { this.RaiseAndSetIfChanged(ref _execution, value); }
        }
        /// <summary>
        /// Название приложения
        /// </summary>
        private string _name = string.Empty;
        /// <summary>
        /// Название приложения
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }
        private string _status = string.Empty;
        /// <summary>
        /// Название приложения
        /// </summary>
        public string Status
        {
            get { return this._status; }
            set { this.RaiseAndSetIfChanged(ref _status, value); }
        }
        /// <summary>
        /// Название приложения
        /// </summary>
        private string _path = string.Empty;
        /// <summary>
        /// Название приложения
        /// </summary>
        public string Path
        {
            get { return this._path; }
            set { this.RaiseAndSetIfChanged(ref _path, value); }
        }
        /// <summary>
        /// Запустить приложение
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private void StartApplication()
        {
            if (!File.Exists(Path))
            {
                throw new Exception($"Приложение {Name} не найдено на этом компьютере");
            }

            if (ProcessStateListener.IsRunning)
            {
                ProcessStateListener.CloseProcess();
            }
            else
            {
                Process.Start(Path);
            }
        }
    }
}
