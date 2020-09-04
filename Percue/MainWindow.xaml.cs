using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Percue.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public Setlist CurrentShow { get => currentShow; set => currentShow = value; }

        
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

        
        public async void AddChannelExecute()
        {
            var ch = new Channel { Name = "Channel " + CurrentShow.Count };
            
            string filePath;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                openFileDialog.Filter = "All Media Files|*.wav;*.aac;*.wma;*.wmv;*.avi;*.mpg;*.mpeg;*.m1v;*.mp2;*.mp3;*.mpa;*.mpe;*.m3u;*.mp4;*.mov;*.3g2;*.3gp2;*.3gp;*.3gpp;*.m4a;*.cda;*.aif;*.aifc;*.aiff;*.mid;*.midi;*.rmi;*.mkv;*.WAV;*.AAC;*.WMA;*.WMV;*.AVI;*.MPG;*.MPEG;*.M1V;*.MP2;*.MP3;*.MPA;*.MPE;*.M3U;*.MP4;*.MOV;*.3G2;*.3GP2;*.3GP;*.3GPP;*.M4A;*.CDA;*.AIF;*.AIFC;*.AIFF;*.MID;*.MIDI;*.RMI;*.MKV";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    ch.LoadAudioFromFile(filePath);
                    var hotKey = await this.ShowInputAsync("HotKey", "Specify Hot Key");
                    ch.SetHotkey(hotKey);
                    
                    CurrentShow.Add(ch);
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
        public void SaveShowExecute() {
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
        public void OpenShowExecute()
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
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
