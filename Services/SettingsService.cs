using SaveGameManager.Core;
using SaveGameManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveGameManager.Services
{
    public class SettingsService : OberservableObject, ISettingsService
    {
        private bool _mainUiEnabled = true;
        private bool _profileUiEnabled = true;

        public SettingsService() { }
        public bool MainUiEnabled
        {
            get => _mainUiEnabled;
            set
            {
                if (_mainUiEnabled == value)
                    return;

                _mainUiEnabled = value;
                OnPropertyChanged(nameof(MainUiEnabled));
            }
        }

        public bool ProfileUiEnabled
        {
            get => _profileUiEnabled;
            set
            {
                if (_profileUiEnabled == value)
                    return;

                _profileUiEnabled = value;
                OnPropertyChanged(nameof(ProfileUiEnabled));
            }
        }
    }
}
