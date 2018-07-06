using System;
using System.Collections.ObjectModel;
using Caliburn.Micro;

namespace WatchingArchiver.ViewModels
{
    internal class MainWindowViewModel : Screen
    {
        #region Properties

        public ObservableCollection<string> Archived { get; set; }

        public bool Archiving { get; set; }

        public string ActionButtonText => Archiving ? "Stop archiving" : "Start archiving";

        #endregion

        #region Methods

        public void ToggleAction()
        {
            Archiving = !Archiving;
        }

        #endregion
    }
}