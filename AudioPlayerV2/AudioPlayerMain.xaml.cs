using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;



namespace AudioPlayerV2
{
    /// <summary>
    /// Interaction logic for AudioPlayerMain.xaml
    /// </summary>
    public partial class AudioPlayerMain : Window
    {
        AudioPlayer player = new AudioPlayer();

        public AudioPlayerMain()
        {
            InitializeComponent();
            DataContext = player;
        }

        private void Add_OnClick(object sender, RoutedEventArgs e)
        {
           player.AddToQueue();
        }

        private void Play_OnClick(object sender, RoutedEventArgs e)
        {
            player.PlayPause();
        }

        private void TrackList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            player.LoadToPlayer();
        }

        private void Rewind_OnClick(object sender, RoutedEventArgs e)
        {
            player.Previous();
        }

        private void Forward_OnClick(object sender, RoutedEventArgs e)
        {
            player.Next();
        }

        private void Info_OnClick(object sender, RoutedEventArgs e)
        {
            var infoDialog = new InfoDialog();
            infoDialog.ShowDialog();
        }
    }
}
