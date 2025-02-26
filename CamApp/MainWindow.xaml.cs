using LibVLCSharp.Shared;
using System;
using System.Windows;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;


namespace KittyCam
{
    public partial class MainWindow : Window
    {
        private readonly LibVLC _libVLC;
        private string areiaGatos;
        private string quartoGatos;
        private string filename1;
        private string filename2;
        private bool isRecording = false;
        private string recordingPath = ConfigurationManager.AppSettings["RecordingsPath"];
        private MediaPlayer _mediaPlayer1;
        private MediaPlayer _mediaPlayer2;
        private MediaPlayer _recordingPlayer1;
        private MediaPlayer _recordingPlayer2;
        
        public MainWindow()
        {
            InitializeComponent();
            string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets");


            Core.Initialize(assetsPath); 
            _libVLC = new LibVLC();
            Loaded += async (s, e) => await StartCameras();
        }

        private async Task StartCameras()
        {
            bool stream1Online = false;
            bool stream2Online = false;
            string password = Environment.GetEnvironmentVariable("KITTY_CAM_PASS");
            int ip = 1;

            while (!stream1Online)
            {
                areiaGatos = String.Format("rtsp://areiaGatos:{0}@192.168.0.{1}/stream1", password, ip);
                _mediaPlayer1 = new MediaPlayer(_libVLC) { Media = new Media(_libVLC, new Uri(areiaGatos), ":no-audio")};
                
                await Dispatcher.InvokeAsync(() =>
                {
                    VideoView1.MediaPlayer = _mediaPlayer1;

                });

                _mediaPlayer1.Play();

                await Task.Delay(2000);

                if (_mediaPlayer1.State == VLCState.Playing)
                {
                    stream1Online = true;
                }
                else
                {
                    _mediaPlayer1.Stop();
                    ip++;
                }
            }

            ip = 1;
            while (!stream2Online)
            {
                quartoGatos = String.Format("rtsp://quartoGatos:{0}@192.168.0.{1}/stream1", password, ip);
                _mediaPlayer2 = new LibVLCSharp.Shared.MediaPlayer(_libVLC) { Media = new Media(_libVLC, new Uri(quartoGatos), ":no-audio") };

                await Dispatcher.InvokeAsync(() =>
                {
                    VideoView2.MediaPlayer = _mediaPlayer2;
                });

                _mediaPlayer2.Play();

                await Task.Delay(2000);

                if (_mediaPlayer2.State == VLCState.Playing)
                {
                    stream2Online = true;
                }
                else
                {
                    _mediaPlayer2.Stop();
                    ip++;
                }
            }
        }

        private void StartRecording()
        {
            if (_mediaPlayer1 == null || _mediaPlayer2 == null)
            {
                MessageBox.Show("Steram not ready yet.");
                return;
            }

            if (isRecording) return;

            _recordingPlayer1 = new MediaPlayer(_libVLC);
            _recordingPlayer2 = new MediaPlayer(_libVLC);
            var media1 = new Media(_libVLC, new Uri(areiaGatos));
            var media2 = new Media(_libVLC, new Uri(quartoGatos));
            filename1 = recordingPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "_areiaGatos.mp4";
            filename2 = recordingPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "_quartoGatos.mp4";
            media1.AddOption(":sout=#file{dst=" + filename1 +"}");
            media2.AddOption(":sout=#file{dst=" + filename2+ "}");
            
            _recordingPlayer1.Play(media1); 
            _recordingPlayer2.Play(media2);
            
            MessageBox.Show("Recording started!");

            isRecording = true;
            RecordButton.Content = "Stop Recording";            
        }

        private void StopRecording()
        {
            isRecording = false;
            _recordingPlayer1?.Stop();
            _recordingPlayer1?.Dispose();
            _recordingPlayer2?.Stop();
            _recordingPlayer2?.Dispose();
            RecordButton.Content = "Start Recording";

            if (File.Exists(filename1) || File.Exists(filename2))
            {
                MessageBox.Show($"Recordings saved");
            }
            else
            {
                MessageBox.Show("Recording failed. No file was created.");
            }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isRecording)
            {
                StartRecording();
            }
            else
            {
                StopRecording();
            }
        }

      
        protected override void OnClosed(EventArgs e)
        {
            _mediaPlayer1.Dispose();
            _mediaPlayer1.Dispose();
            _libVLC.Dispose();
            base.OnClosed(e);
        }
    }
}
