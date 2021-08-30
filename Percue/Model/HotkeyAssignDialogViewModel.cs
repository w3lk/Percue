using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Percue.Resources;

namespace Percue.Model
{
    public class HotkeyAssignDialogViewModel : INotifyPropertyChanged
    {
        private string _title;
        private string _message;
        private string _affirmativeButtonText = "Yes";
        private string _negativeButtonText = "No";
        private MessageDialogResult _result;
        private Hotkey _hotkey;
        private ICommand _dialogCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler CloseRequested;
        public HotkeyAssignDialogViewModel()
        {
            _dialogCommand = new RelayCommand(DialogCommandExecute);
        }

        private void DialogCommandExecute(object obj)
        {
            if (!(obj is MessageDialogResult msgResult)) return;
            Result = msgResult;
            if(Result == MessageDialogResult.Negative)
            {
                Hotkey = new Hotkey();
            }
            CloseRequested?.Invoke(this, new EventArgs());
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value) return;
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (_message == value) return;
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public string AffirmativeButtonText
        {
            get { return _affirmativeButtonText; }
            set
            {
                if (_affirmativeButtonText == value) return;
                _affirmativeButtonText = value;
                OnPropertyChanged(nameof(AffirmativeButtonText));
            }
        }

        public string NegativeButtonText
        {
            get { return _negativeButtonText; }
            set
            {
                if (_negativeButtonText == value) return;
                _negativeButtonText = value;
                OnPropertyChanged(nameof(NegativeButtonText));
            }
        }
        
        public Hotkey Hotkey
        {
            get { return _hotkey; }
            set
            {
                if (_hotkey == value) return;
                _hotkey = value;
                OnPropertyChanged(nameof(Hotkey));
            }
        }

        public MessageDialogResult Result
        {
            get { return _result; }
            set
            {
                if (_result == value) return;
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }
        public ICommand DialogCommand
        {
            get { return _dialogCommand; }
        }
    }
}
