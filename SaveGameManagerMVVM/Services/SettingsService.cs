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
        private bool _replaceEnabled = true;
        private bool _loadEnabled = true;
        private bool _deleteEnabled = true;

        public SettingsService() { }
        public bool ReplaceEnabled 
        {
            get => _replaceEnabled;
            set
            {
                if (_replaceEnabled == value)
                    return;

                _replaceEnabled = value;
                OnPropertyChanged(nameof(ReplaceEnabled));
            }
        }
        public bool LoadEnabled
        {
            get => _loadEnabled;
            set
            {
                if (_loadEnabled == value)
                    return;

                _loadEnabled = value;
                OnPropertyChanged(nameof(LoadEnabled));
            }
        }
        public bool DeleteEnabled
        {
            get => _deleteEnabled;
            set
            {
                if (_deleteEnabled == value)
                    return;

                _deleteEnabled = value;
                OnPropertyChanged(nameof(DeleteEnabled));
            }
        }

    }
}
