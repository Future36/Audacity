using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace AudioPlayer
{
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            var folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select folder with audio files";
            folderBrowserDialog.ShowNewFolderButton = false;

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var directoryPath = folderBrowserDialog.SelectedPath;
                var audioExtensions = new[] { ".mp3", ".wav" };
                var audioFiles = Directory.GetFiles(directoryPath)
                    .Where(file => audioExtensions.Contains(Path.GetExtension(file)))
                    .ToList();

                lstPlaylist.ItemsSource = audioFiles;
            }
        }


        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select folder with audio files";
            folderBrowserDialog.ShowNewFolderButton = false;

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var directoryPath = folderBrowserDialog.SelectedPath;
                var audioExtensions = new[] { ".mp3", ".wav" };
                var audioFiles = Directory.GetFiles(directoryPath)
                    .Where(file => audioExtensions.Contains(Path.GetExtension(file)))
                    .ToList();

                lstPlaylist.ItemsSource = audioFiles;
            }
        }

        private void lstPlaylist_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string selectedFile = lstPlaylist.SelectedItem as string;
            if (selectedFile != null)
            {
                lblSongInfo.Content = Path.GetFileName(selectedFile);
                mediaPlayer.Open(new Uri(selectedFile, UriKind.RelativeOrAbsolute));
                mediaPlayer.Play();
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            timer.Start();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            timer.Stop();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            timer.Stop();
            sldPosition.Value = 1;
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            int index = lstPlaylist.SelectedIndex;
            if (index > 0)
            {
                lstPlaylist.SelectedIndex = index - 1;
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            int index = lstPlaylist.SelectedIndex;
            if (index < lstPlaylist.Items.Count - 1)
            {
                lstPlaylist.SelectedIndex = index + 1;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!sldPosition.IsMouseCaptureWithin)
            {
                sldPosition.Value = mediaPlayer.Position.TotalSeconds;
            }
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            if (mediaPlayer.NaturalDuration != TimeSpan.Zero)
            {
                sldPosition.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            timer.Stop();
            sldPosition.Value = 0;
            btnNext_Click(null, null);
        }

        private void OnVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = e.NewValue;
        }
    }
}



