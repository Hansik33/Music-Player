using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.Win32;
using System.Windows.Threading;

namespace Music_Player
{
    public partial class MainWindow : Window
    {
        MediaPlayer mediaPlayer = new MediaPlayer();
        OpenFileDialog openFileDialog = new OpenFileDialog();
        DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Send);
        BrushConverter brushConverter = new BrushConverter();

        bool IsMuted = false;
        bool IsRepeatEnabled = false;

        public enum Status
        {
            ON, OFF
        }

        public MainWindow()
        {
            InitializeComponent();
            Customize();
            this.PreviewKeyDown += new KeyEventHandler(ApplicationClose);
        }

        public void FastForward()
        {
            if (mediaPlayer.Source != null)
            {
                mediaPlayer.Position += TimeSpan.FromSeconds(10);
                if (mediaPlayer.Position > mediaPlayer.NaturalDuration.TimeSpan)
                    mediaPlayer.Position = mediaPlayer.NaturalDuration.TimeSpan;
            }
        }

        public void Rewind()
        {
            if (mediaPlayer.Source != null)
            {
                mediaPlayer.Position -= TimeSpan.FromSeconds(10);
            }
        }

        public void MusicOver()
        {
            if (IsRepeatEnabled)
            {
                mediaPlayer.Position = TimeSpan.Zero;
                mediaPlayer.Play();
            }
        }

        public void Repeat(Status status)
        {
            if (mediaPlayer.Source != null)
            {
                if (status == Status.OFF)
                {
                    IsRepeatEnabled = true;
                    ButtonRepeat.Background = (Brush)brushConverter.ConvertFrom("#cf4848");
                }

                if (status == Status.ON)
                {
                    IsRepeatEnabled = false;
                    ButtonRepeat.Background = (Brush)brushConverter.ConvertFrom("#FF2E2E2E");
                }
            }
        }

        public void Mute(Status status)
        {
            if (mediaPlayer.Source != null)
            {
                if (status == Status.OFF)
                {
                    IsMuted = true;
                    ButtonMute.Background = (Brush)brushConverter.ConvertFrom("#cf4848");
                    mediaPlayer.Volume = 0;
                }

                if (status == Status.ON) ChangeVolume();
            }
        }

        public void ChangeVolume()
        {
            IsMuted = false;
            ButtonMute.Background = (Brush)brushConverter.ConvertFrom("#FF2E2E2E");
            var value = (SliderVolume.Value * 0.01);
            mediaPlayer.Volume = value;
        }

        public void Customize()
        {
            openFileDialog.Filter = "Pliki MP3 (*.mp3)|*.mp3";
            mediaPlayer.Volume = 1;
        }

        public void OpenFile()
        {
            if (openFileDialog.ShowDialog() == true)
                mediaPlayer.Open(new Uri(openFileDialog.FileName));
            var FileName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
            LabelNameFile.Content = FileName;
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += TimerTick;
            mediaPlayer.Play();
            timer.Start();
        }

        public void TimerTick(object sender, EventArgs e)
        {
            if (mediaPlayer.Source != null)
            {
                LabelStatus.Content = String.Format("{0} / {1}", mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                if (mediaPlayer.Position == mediaPlayer.NaturalDuration.TimeSpan) MusicOver();
            }
            else LabelStatus.Content = "00:00 / 00:00";
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangeVolume();
        }

        private void ApplicationClose(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }

        private void ButtonRepeat_Click(object sender, RoutedEventArgs e)
        {
            if (!IsRepeatEnabled) Repeat(Status.OFF);
            else Repeat(Status.ON);
        }

        private void ButtonMute_Click(object sender, RoutedEventArgs e)
        {
            if (!IsMuted) Mute(Status.OFF);
            else Mute(Status.ON); 
        }

        private void ButtonFastForward_Click(object sender, RoutedEventArgs e)
        {
            FastForward();
        }

        private void ButtonRewind_Click(object sender, RoutedEventArgs e)
        {
            Rewind();
        }
    }
}
