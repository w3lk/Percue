using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.WaveFormRenderer;
using Percue.Extensions;
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
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
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

            parentView = (MainView)System.Windows.Application.Current.MainWindow;

        }

        private static int ChannelId = 1;

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

        private MainView parentView;

        private bool isPlaying;
        [XmlIgnoreAttribute]
        public bool IsPlaying
        {
            get { return isPlaying; }
            set
            {
                isPlaying = value;
                if (isPlaying)
                {
                    if(DoStopOthers)
                    {
                        parentView.CurrentShow.StopAllButThis(this);
                    }
                    
                    if (PlaybackState == PlaybackState.Paused)
                        Play();
                    if (PlaybackState == PlaybackState.Stopped)
                    {
                        Play();
                    }

                }
                else
                {
                    if (DoPause)
                        Pause();
                    else
                        Stop();

                    // stop any running fade
                    CancelFade();

                }
                OnPropertyChanged(nameof(IsPlaying));
            }
        }

        private bool doPause;
        public bool DoPause
        {
            get { return doPause; }
            set { doPause = value; OnPropertyChanged(nameof(DoPause)); }
        }

        private bool doStopOthers;
        public bool DoStopOthers
        {
            get { return doStopOthers; }
            set { doStopOthers = value; OnPropertyChanged(nameof(DoStopOthers)); }
        }
        
        private bool waveImgLoaded;
        private BitmapImage waveImg;
        [XmlIgnore]
        public BitmapImage WaveImg
        {
            get
            {
                if (!waveImgLoaded)
                {
                    if (WaveImgData != null)
                    {
                        var waveImgSource = WaveImgData.Deserialize();
                        var encoder = new PngBitmapEncoder();
                        var memoryStream = new MemoryStream();
                        waveImg = new BitmapImage();

                        encoder.Frames.Add(BitmapFrame.Create(waveImgSource));
                        encoder.Save(memoryStream);

                        memoryStream.Position = 0;
                        waveImg.BeginInit();
                        waveImg.StreamSource = new MemoryStream(memoryStream.ToArray());
                        waveImg.EndInit();

                        memoryStream.Close();
                        waveImg.Freeze();
                        waveImgLoaded = true;
                    }
                }
                return waveImg;
            }
            set
            {
                waveImg = value;
                waveImgLoaded = true;
                WaveImgData = new SerializedBitmapImage();
                WaveImgData.Serialize(waveImg);
                OnPropertyChanged(nameof(WaveImg));
            }
        }


        private SerializedBitmapImage waveImgData;
        public SerializedBitmapImage WaveImgData
        {
            get { return waveImgData; }
            set { waveImgData = value;
                OnPropertyChanged(nameof(WaveImgData));
            }

        }


        private VolumeSampleProvider volumeSampleProvider;
        private float channelVolume = 1.0f;
        private bool isMuted = false;
        private bool isLooping = false;
        private float pan = 0.0f; // -1.0 (left) .. 1.0 (right)
        private double fadeInSeconds = 0.0;
        private double startOffsetSeconds = 0.0;
        private CancellationTokenSource fadeCancellationTokenSource = null;
        public float ChannelVolume
        {
            get { return channelVolume; }

            set
            {
                channelVolume = value;
                if (volumeSampleProvider != null)
                {
                    // respect mute state
                    // If a fade is running, let the fade task control the volume; otherwise set immediately
                    if (fadeCancellationTokenSource == null)
                    {
                        volumeSampleProvider.Volume = isMuted ? 0f : value;
                    }
                }

                OnPropertyChanged(nameof(ChannelVolume));
            }
        }

        public bool IsMuted
        {
            get => isMuted;
            set
            {
                isMuted = value;
                if (volumeSampleProvider != null)
                {
                    volumeSampleProvider.Volume = isMuted ? 0f : channelVolume;
                }
                OnPropertyChanged(nameof(IsMuted));
            }
        }

        public bool IsLooping
        {
            get => isLooping;
            set { isLooping = value; OnPropertyChanged(nameof(IsLooping)); }
        }

        public float Pan
        {
            get => pan;
            set { pan = value; OnPropertyChanged(nameof(Pan)); }
        }

        public double FadeInSeconds
        {
            get => fadeInSeconds;
            set { fadeInSeconds = value; OnPropertyChanged(nameof(FadeInSeconds)); }
        }

        [XmlElement]
        public double StartOffsetSeconds
        {
            get => startOffsetSeconds;
            set { startOffsetSeconds = value; OnPropertyChanged(nameof(StartOffsetSeconds)); }
        }

        private byte[] audio;
        public byte[] Audio
        {
            get { return audio; }
            set
            {
                audio = value;
                this.OnPropertyChanged(nameof(Audio));
            }
        }

        public new void Play()
        {
            if (Audio == null) return;
            if (Audio.Length <= 0) return;
            // Create a WaveStream from raw audio bytes
            var memStream = new MemoryStream(Audio);
            var rawStream = new RawSourceWaveStream(memStream, new WaveFormat());

            WaveStream playbackStream = rawStream;
            if (IsLooping)
            {
                playbackStream = new LoopStream(rawStream);
            }

            // If a start offset is configured, seek into the stream by the given number of seconds
            try
            {
                if (StartOffsetSeconds > 0 && playbackStream != null && playbackStream.Length > 0)
                {
                    var bytesPerSec = playbackStream.WaveFormat.AverageBytesPerSecond;
                    var offsetBytes = (long)(StartOffsetSeconds * bytesPerSec);
                    if (offsetBytes < playbackStream.Length)
                    {
                        playbackStream.Position = offsetBytes;
                    }
                    else
                    {
                        // If offset exceeds length, start at end (no audio) to avoid exception
                        playbackStream.Position = playbackStream.Length;
                    }
                }
            }
            catch { }

            // Build sample provider pipeline: to sample provider -> ensure stereo for panning -> apply panning -> apply volume
            ISampleProvider sample = playbackStream.ToSampleProvider();

            // If mono, convert to stereo so panning works
            if (sample.WaveFormat.Channels == 1)
            {
                sample = new MonoToStereoSampleProvider(sample);
            }

            if (Pan != 0f)
            {
                var monoSample = new StereoToMonoSampleProvider(sample);
                var panner = new PanningSampleProvider(monoSample);
                panner.Pan = Pan;
                sample = panner;
            }

            volumeSampleProvider = new VolumeSampleProvider(sample);

            var waveProvider = volumeSampleProvider.ToWaveProvider();
            base.Init(waveProvider);
            base.Play();

            // handle fade-in
            CancelFade();
            if (!IsMuted && FadeInSeconds > 0)
            {
                // start from 0 and ramp to ChannelVolume
                volumeSampleProvider.Volume = 0f;
                fadeCancellationTokenSource = new CancellationTokenSource();
                var token = fadeCancellationTokenSource.Token;
                var target = ChannelVolume;
                var duration = FadeInSeconds;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var sw = System.Diagnostics.Stopwatch.StartNew();
                        while (!token.IsCancellationRequested)
                        {
                            var elapsed = sw.Elapsed.TotalSeconds;
                            var t = Math.Min(1.0, elapsed / duration);
                            var vol = (float)(target * t);
                            volumeSampleProvider.Volume = vol;
                            if (t >= 1.0) break;
                            await Task.Delay(30, token).ConfigureAwait(false);
                        }
                        // ensure final volume
                        if (!token.IsCancellationRequested)
                        {
                            volumeSampleProvider.Volume = target;
                        }
                    }
                    catch (OperationCanceledException) { }
                }, token);
            }
            else
            {
                volumeSampleProvider.Volume = IsMuted ? 0f : ChannelVolume;
            }

        }


        public void LoadAudioFromFile(string path) 
        { 
            var tempFolder = @"C:\Temp";
            var outfile = Path.Combine(tempFolder, "converted.wav");

            // Check + create folder
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }


            using (var reader = new MediaFoundationReader(path))
            {
                WaveFileWriter.CreateWaveFile(outfile, reader);
            }

            Audio = File.ReadAllBytes(outfile);




            var renderer = new WaveFormRenderer();

            
            

            var settings = new StandardWaveFormRendererSettings();
            settings.Width = 640;
            settings.TopHeight = 32;
            settings.BottomHeight = 32;
            settings.BackgroundColor = System.Drawing.Color.Transparent;
            try
            {
                using (var waveStream = new WaveFileReader(outfile))
                {
                    Bitmap img = renderer.Render(waveStream, settings) as Bitmap;
                    if (img != null)
                    {
                        img.Save(@"C:\Temp\imgRenderer.bmp");
                        WaveImg = BitmapExtensions.ToBitmapImage(img);
                        img.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool isHotKeySet;
        public bool IsHotKeySet
        {
            get { return isHotKeySet; }
            set { isHotKeySet = value; OnPropertyChanged(nameof(IsHotKeySet)); }
        }

        private Hotkey channelHotKey;
        public Hotkey ChannelHotKey
        {
            get { return channelHotKey; }
            set
            {
                channelHotKey = value;
                if (IsHotKeySet)
                {
                    UnsetHotkey();
                    IsHotKeySet = false;
                }
                
                    RegisterHotKey(channelHotKey);
                    IsHotKeySet = true;
                
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

        [XmlIgnore]
        public int HOTKEY_ID { get; set; } = 9000;



        public void UnsetHotkey()
        {
            if (_source != null)
                _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();

        }

        private void CancelFade()
        {
            try
            {
                if (fadeCancellationTokenSource != null)
                {
                    fadeCancellationTokenSource.Cancel();
                    fadeCancellationTokenSource.Dispose();
                    fadeCancellationTokenSource = null;
                }
            }
            catch { }
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
        public void RegisterHotKey(Hotkey key)
        {
            var helper = new WindowInteropHelper(System.Windows.Application.Current.MainWindow);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);

            
            uint MOD_CTRL = (uint)key.Modifiers;
            uint KEY_ID = (uint)KeyInterop.VirtualKeyFromKey(key.Key);
            if (KEY_ID > 0)
            {
                if (!RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL, KEY_ID))
                {
                    // handle error
                }
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
                    if (wParam.ToInt32() == HOTKEY_ID)
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

    // Simple looping WaveStream wrapper
    internal class LoopStream : WaveStream
    {
        private readonly WaveStream sourceStream;

        public LoopStream(WaveStream source)
        {
            this.sourceStream = source;
        }

        public override WaveFormat WaveFormat => sourceStream.WaveFormat;

        public override long Length => sourceStream.Length;

        public override long Position
        {
            get => sourceStream.Position;
            set => sourceStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;
            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    // restart
                    sourceStream.Position = 0;
                    // if still no data, break to avoid infinite loop
                    bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                    if (bytesRead == 0)
                        break;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }
}
