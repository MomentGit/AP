using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Threading;




namespace AudioPlayerV2
{
    public class AudioPlayer : INotifyPropertyChanged
    {
        private DispatcherTimer timer = new DispatcherTimer();

        private MediaPlayer player = new MediaPlayer();      

        public AudioPlayer()
        {
            timer.Interval = new TimeSpan(0,0,0,0,200);
            timer.Tick += TimerOnTick;
            timer.Start();
            player.MediaEnded += PlayerOnMediaEnded;           
        }

        //What do we do when media ends.
        private void PlayerOnMediaEnded(object sender, EventArgs eventArgs)
        {
            if (QueuedPlayback)
            {
                RandomEnabled = false;
                IsLooped = false;
                Next();
                return;
            }
            if (IsLooped)
            {
                RandomEnabled = false;
                QueuedPlayback = false;
                player.Position = new TimeSpan(0, 0, 0, 0);
                return;
            }
            if (RandomEnabled)
            {
                IsLooped = false;
                QueuedPlayback = false;
                LoadRandom();
            }
            else
            {
                IsPlaying = false;
                player.Stop();
            }
        }

        //On Tick 300ms interval, due to stutter while trying to display results in window.
        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            SongTime = player.Position;
            if (player.NaturalDuration.HasTimeSpan) //Track player position, timespan etc.
            {
                PosTotal = player.NaturalDuration.TimeSpan;
                Max = PosTotal.TotalSeconds;
                Value = player.Position.TotalSeconds;
            }
        }

        public bool QueuedPlayback { get; set; } = true;

        public bool IsLooped { get; set; }

        public bool RandomEnabled { get; set; }

        private bool _isPlaying;

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { _isPlaying = value; NotifyPropertyChanged(); }
        }


        private int _index;

        public int Index
        {
            get { return _index; }
            set { _index = value; NotifyPropertyChanged(); }
        }

        private string _nowPlayingName;

        public string NowPlayingName
        {
            get { return _nowPlayingName; }
            set { _nowPlayingName = value; NotifyPropertyChanged(); }
        }

        //Current timespan in seconds for seekbar.
        private double _value;

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged();
                player.Position = TimeSpan.FromSeconds(value);
            }
        }

        //Song duration for seekbar in seconds. Initialized at 100.
        private double _max = 100;

        public double Max
        {
            get { return _max; }
            set { _max = value; NotifyPropertyChanged(); }
        }


        //Song time span.
        private TimeSpan _posTotal;

        public TimeSpan PosTotal
        {
            get { return _posTotal; }
            set { _posTotal = value; NotifyPropertyChanged(); }
        }

        private TimeSpan _songTime;

        public TimeSpan SongTime
        {
            get { return _songTime; }
            set { _songTime = value; NotifyPropertyChanged(); }
        }

        ObservableCollection<AudioFile> _playQueue = new ObservableCollection<AudioFile>();       

        public ObservableCollection<AudioFile> PlayQueue
        {
            get { return _playQueue; }
            set { _playQueue = value; NotifyPropertyChanged();}
        }

        public double Volume
        {
            get { return player.Volume; }
            set { player.Volume = value; NotifyPropertyChanged(); }
        }

        public void AddToQueue()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Audio Files (.mp3)|*.mp3;*.wav",
                Multiselect = true
            };

            bool? result = dlg.ShowDialog();
            if (result != true) return;
            var files = dlg.FileNames;
            foreach (var file in files)
            {
                PlayQueue.Add(new AudioFile(Path.GetFileName(file), file));
            }
        }

        public void LoadRandom()
        {
            Random rand = new Random();
            int random = rand.Next(0, PlayQueue.Count + 1);
            if (random == Index && Index < PlayQueue.Count)
            {
                random++;
                Index = random;
            }
            else if (random == Index && Index >= PlayQueue.Count)
            {
                random--;
                Index = random;
            }
            Index = random;
        }


        public void Next()
        {
            if (PlayQueue.Count > Index)
                Index++;
            else
            {
                player.Stop();
                IsPlaying = false;
            }
        }

        public void Previous()
        {
            if (0 < Index)
                Index--;
            else
            {
                player.Stop();
                IsPlaying = false;
            }

        }

        public void PlayPause()
        {
            if(IsPlaying != false)
                player.Play();
            else
                player.Pause();
        }

        public void LoadToPlayer()
        {
            player.Open(new Uri(PlayQueue[Index].FilePath));
            NowPlayingName = PlayQueue[Index].Name;

            if (!IsPlaying)
                return;
            else
                player.Play();
        }

        #region NotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
