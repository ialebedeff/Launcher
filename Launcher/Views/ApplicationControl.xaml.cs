﻿using Launcher.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Launcher.Views
{
    /// <summary>
    /// Логика взаимодействия для ApplicationControl.xaml
    /// </summary>
    public partial class ApplicationControl : ReactiveUserControl<ApplicationViewModel>
    {
        public ApplicationControl()
        {
            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                this.Bind(this.ViewModel, x => x.Name, x => x.ApplicationName.Text)
                    .DisposeWith(disposable);

                this.Bind(this.ViewModel, x => x.Status, x => x.ApplicationStatus.Text)
                    .DisposeWith(disposable);

                this.OneWayBind(this.ViewModel, x => x.StartApplicationCommand, x => x.StartApplicationButton.Command)
                    .DisposeWith(disposable);
            });
        }
    }
}
