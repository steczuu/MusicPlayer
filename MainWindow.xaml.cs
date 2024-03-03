using Microsoft.Win32;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.IO;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isDragged = false;
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private bool WasClicked = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3";
            if (openFileDialog.ShowDialog() == true)
            {
                mediaPlayer.Open(new Uri(openFileDialog.FileName));
                ControlTemplate controlTemplate = PlayBtn.Template;
                Image image = (Image)controlTemplate.FindName("PlayImg", PlayBtn);
                image.Source = new BitmapImage(new Uri("/Images/pause.png", UriKind.RelativeOrAbsolute));
                mediaPlayer.Play();
                SongTitle.Text = Path.GetFileName(openFileDialog.FileName);
            }

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += _timer;
            timer.Start();
        }

        private void _timer(object? sender, EventArgs e)
        {
            if (mediaPlayer.Source != null && WasClicked)
            {
                Timer.Content = String.Format("{0} / {1}", mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                Timeline.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                Timeline.Value++;
            }
            if (WasClicked)
            {
                Timer.Content = mediaPlayer.Position.ToString(@"mm\:ss");
            }
            if(mediaPlayer.Source == null) 
            {
                Timer.Content = "Choose file";
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (!WasClicked)
            {
                ControlTemplate controlTemplate = PlayBtn.Template;
                Image image = (Image)controlTemplate.FindName("PlayImg", PlayBtn);
                image.Source = new BitmapImage(new Uri("/Images/pause.png", UriKind.RelativeOrAbsolute));
                mediaPlayer.Play();
                WasClicked = true;
            }
            else
            {
                ControlTemplate controlTemplate = PlayBtn.Template;
                Image image = (Image)controlTemplate.FindName("PlayImg", PlayBtn);
                image.Source = new BitmapImage(new Uri("/Images/play.png", UriKind.RelativeOrAbsolute));
                mediaPlayer.Pause();
                WasClicked = false;
            }
        }

        private void Timeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int SliderValue = (int)Timeline.Value;
            TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, SliderValue);
            Timer.Content = TimeSpan.FromSeconds(SliderValue).ToString(@"mm\:ss");
        }

        private void Timeline_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            isDragged = true;
        }

        private void Timeline_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            isDragged = false;
            mediaPlayer.Position = TimeSpan.FromSeconds(Timeline.Value);
        }
    }
}
   