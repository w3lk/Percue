
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Percue.Model;
using Percue.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Percue
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged
    {

        private Setlist currentShow = new Setlist();
        public Setlist CurrentShow
        {
            get => currentShow; set
            {
                currentShow = value;
                OnPropertyChanged(nameof(CurrentShow));
            }
        }


        public MainWindow()
        {
            InitializeComponent();


        }


        private RelayCommand addChannel;
        public ICommand AddChannel
        {
            get
            {
                if (addChannel == null)
                {
                    addChannel = new RelayCommand(AddChannelExecute);
                }
                return addChannel;
            }
        }

        

        public async void AddChannelExecute(object obj)
        {
            
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                openFileDialog.Filter = "All Media Files|*.wav;*.aac;*.wma;*.wmv;*.avi;*.mpg;*.mpeg;*.m1v;*.mp2;*.mp3;*.mpa;*.mpe;*.m3u;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.cda;*.aif;*.aifc;*.aiff;*.mid;*.midi;*.rmi;*.mkv;*.WAV;*.AAC;*.WMA;*.WMV;*.AVI;*.MPG;*.MPEG;*.M1V;*.MP2;*.MP3;*.MPA;*.MPE;*.M3U;*.MP4;*.MOV;*.3G2;*.3GP2;*.3GP;*.3GPP;*.M4A;*.CDA;*.AIF;*.AIFC;*.AIFF;*.MID;*.MIDI;*.RMI;*.MKV";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Get the path of specified file
                    foreach (var filePath in openFileDialog.FileNames)
                    {
                        var fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                        var ch = new Channel { Name = fileName };
                        ch.LoadAudioFromFile(filePath);
                        var hotKey = await this.ShowInputAsync("HotKey", "Specify Hot Key for "+ fileName);
                        ch.ChannelHotKey = new Hotkey(Key.A,ModifierKeys.None);

                        CurrentShow.Add(ch);
                    }
                }
            }

        }

        private RelayCommand saveShow;
        public ICommand SaveShow
        {
            get
            {
                if (saveShow == null)
                {
                    saveShow = new RelayCommand(SaveShowExecute);
                }
                return saveShow;
            }
        }
        public void SaveShowExecute(object obj) {
            // Displays a SaveFileDialog so the user can save the Show
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Percue|*.pc";
            saveFileDialog1.Title = "Save Percue Show File";
            saveFileDialog1.ShowDialog();
            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                System.IO.FileStream fs =
                    (System.IO.FileStream)saveFileDialog1.OpenFile();

                var ser = new XmlSerializer(typeof(Setlist));
                ser.Serialize(fs, CurrentShow);
                fs.Close();
            }
        }

        private RelayCommand openShow;

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand OpenShow
        {
            get
            {
                if (openShow == null)
                {
                    openShow = new RelayCommand(OpenShowExecute);
                }
                return openShow;
            }
        }
        public void OpenShowExecute(object obj)
        {
            // Displays a OpenFileDialog so the user can open the Show
            
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Percue|*.pc";
            openFileDialog1.Title = "Save Percue Show File";
            openFileDialog1.ShowDialog();
            // If the file name is not an empty string open it for saving.
            if (openFileDialog1.FileName != "")
            {
                System.IO.FileStream fs =
                    (System.IO.FileStream)openFileDialog1.OpenFile();

                var ser = new XmlSerializer(typeof(Setlist));
                CurrentShow= (Setlist)ser.Deserialize(fs);
                OnPropertyChanged(nameof(CurrentShow));
                fs.Close();
                
            }
        }

        private RelayCommand newShow;
        public ICommand NewShow
        {
            get
            {
                if(newShow == null)
                {
                    newShow = new RelayCommand(NewShowExecute);
                }
                return newShow;
            }
        }

        public async void NewShowExecute(object obj)
        {
            if (CurrentShow.Count == 0) return;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ok",
                NegativeButtonText = "Cancel",
                
            };

            MessageDialogResult result = await this.ShowMessageAsync("New Show", "All Unsaved Changes will be lost",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
            {

                foreach(var channel in CurrentShow)
                {
                    channel.UnsetHotkey();
                }
                CurrentShow = new Setlist();
                

            }
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string LastShowPath = AppDomain.CurrentDomain.BaseDirectory + "//LastShow.pc";
        private bool closeMe;
        private async void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            if (e.Cancel) return;

            e.Cancel = !this.closeMe;

            if (this.closeMe) return;

            var dialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await this.ShowMessageAsync(
                "Quit application?",
                "Sure you want to quit application?",
                MessageDialogStyle.AffirmativeAndNegative, dialogSettings);

            if (result == MessageDialogResult.Affirmative)
            {
                if (CurrentShow.Count > 0)
                {
                    var fs = new StreamWriter(LastShowPath);
                    var ser = new XmlSerializer(typeof(Setlist));
                    ser.Serialize(fs, CurrentShow);
                    fs.Close();
                }
            }

            this.closeMe = result == MessageDialogResult.Affirmative;

            if (this.closeMe) this.Close();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(LastShowPath)) return;
            var fs = File.Open(LastShowPath,FileMode.Open);
            var ser = new XmlSerializer(typeof(Setlist));
            CurrentShow = (Setlist)ser.Deserialize(fs);
            OnPropertyChanged(nameof(CurrentShow));          
            fs.Close();
            
        }
    }

    
}