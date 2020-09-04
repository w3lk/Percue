using GalaSoft.MvvmLight.Command;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Xml.Serialization;

namespace Percue.Model
{
    public class Channel : WaveOut, INotifyPropertyChanged
    {
       

        public Channel()
        {
            PlaybackStopped += Channel_PlaybackStopped;
            
            HOTKEY_ID = 9000 + ChannelId;
            ChannelId += 1;
            
        }

        private static int ChannelId = 0;

        private void Channel_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            IsPlaying = false;
        }

        private string name = "New Channel";
        public string Name
        {
            get => name; set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private bool isPlaying;
        [XmlIgnoreAttribute]
        public bool IsPlaying
        {
            get { return isPlaying; }
            set { isPlaying = value;
                if (isPlaying)
                {
                    
                    Play();
                }
                else
                {
                    Stop();
                }
                OnPropertyChanged(nameof(IsPlaying));
            }
        }

        

        private VolumeSampleProvider volumeSampleProvider;
        private float channelVolume = 1.0f;
        public float ChannelVolume{
            get{ return channelVolume; }

            set {
                if(volumeSampleProvider != null)
                {
                    volumeSampleProvider.Volume = value;
                }

                channelVolume = value;
                OnPropertyChanged(nameof(ChannelVolume));
            }
        }

        private byte[] audio;
        public byte[] Audio 
        { 
            get { return audio; }
            set { audio = value;                  
                  this. OnPropertyChanged(nameof(Audio)); }
        }

        public new void Play()
        {
            if (Audio == null) return;
            if (Audio.Length <= 0) return;
            IWaveProvider provider = new RawSourceWaveStream(
                         new MemoryStream(Audio), new WaveFormat());
            volumeSampleProvider = new VolumeSampleProvider(provider.ToSampleProvider());
            volumeSampleProvider.Volume = ChannelVolume;
            base.Init(volumeSampleProvider.ToWaveProvider());            
            base.Play();
        }
        
         
        public void LoadAudioFromFile(string path)
        {
            var outfile = @"C:\Temp\converted.wav";
            using (var reader = new MediaFoundationReader(path))
            {
                WaveFileWriter.CreateWaveFile(outfile, reader);               
            }
                        
            Audio = File.ReadAllBytes(outfile);
            
        }


        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private Keys channelHotKey = Keys.None;
        public Keys ChannelHotKey
        {
            get { return channelHotKey; }
            set { channelHotKey = value;
                if (channelHotKey == Keys.None)
                    UnsetHotkey();
                else
                    RegisterHotKey(channelHotKey);
                OnPropertyChanged(nameof(ChannelHotKey));
                }

        }

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
           [In] IntPtr hWnd,
           [In] int id,
           [In] uint fsModifiers,
           [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private HwndSource _source;
        public int HOTKEY_ID { get; set; } = 9000;
        

        public void SetHotkey(string keyString)
        {
            
            
            Keys k;
            Enum.TryParse<Keys>(keyString, out k);
            ChannelHotKey = k;
        }

        public void UnsetHotkey()
        {
            if(_source != null)
                _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            
        }

        public void RegisterHotKey(Keys key)
        {
            var helper = new WindowInteropHelper(System.Windows.Application.Current.MainWindow);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);

            const uint MOD_CTRL = 0x0000;
            
            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL, (uint)key))
            {
                // handle error
            }
           
        }

        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(System.Windows.Application.Current.MainWindow);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    if(wParam.ToInt32() == HOTKEY_ID)
                    { 
                            OnHotKeyPressed();
                            handled = true;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void OnHotKeyPressed()
        {
            IsPlaying = !IsPlaying;
        }
    }
}
