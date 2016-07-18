using Microsoft.Win32;
using System;
using System.IO;
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

// path soll immer als System.IO.Path interpretiert werden
using Path = System.IO.Path;
using System.Threading;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections;
using System.Collections.ObjectModel;

namespace GPD915_ToolDev
{

    public enum GameTabState
    {
        NOT_INSTALLED,
        UPDATE_REQUIRED,
        UPDATE_PENDING,
        UPDATING,
        PLAYABLE,
        PLAYING
    }

    // Stellt ein Erfolg dar
    public class Achievement
    {
        // Der Name des Erfolges
        public string Name { get; set; }

        // Wurde der Erfolg erreicht
        public bool Achieved { get; set; }

        public Achievement(string _name, bool _achieved = false)
        {
            Name = _name;
            Achieved = _achieved;
        }

    }

    /// <summary>
    /// Interaktionslogik für GameTab.xaml
    /// </summary>
    public partial class GameTab : UserControl
    {

        // die einstellungen des spiels
        public Game GameSettings { get; set; }

        // der aktuelle zustand des games
        public GameTabState State { get; set; }

        private DispatcherTimer updateTimer;

        private MainWindow window;

        private ObservableCollection<Achievement> Avmnts;

        public GameTab(Game _game, MainWindow _window)
        {
            this.DataContext = this;
            Avmnts = new ObservableCollection<Achievement>();
            Avmnts.Add(new Achievement("Started the Game"));
            Avmnts.Add(new Achievement("Ate an Icecream"));

            // window setzen
            window = _window;

            // einstellungen setzen
            GameSettings = _game;

            // das tab initialisieren
            InitializeComponent();

            // Label mit dem Namen des Spieles setzten
            // gameNameLabel.Content = game.Name;

            avmntList.ItemsSource = Avmnts;

            // wenn das spiel installiert ist
            if(GameSettings.IsInstalled())
            {
                // min. einmal die spiel dateien überprüfen
                State = GameTabState.UPDATE_REQUIRED;

                // prüfen wir auf updates
                CheckForUpdates();
            }
            else
            {
                State = GameTabState.NOT_INSTALLED;
            }

            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromSeconds(5);
            updateTimer.Tick += updateTimer_Tick;

            RefreshUI();
        }

        void updateTimer_Tick(object sender, EventArgs e)
        {
            // Wenn das spiel nicht spielbar ist => nicht auf updates prüfen
            if (State != GameTabState.PLAYABLE) { return; }

            // Update check starten
            CheckForUpdates();
        }

        private void CheckForUpdates()
        {
            // wenn bereits ein update läuft => nicht nochmal updaten
            if (State == GameTabState.UPDATING
                || State == GameTabState.UPDATE_PENDING) { return; }

            State = GameTabState.UPDATE_PENDING;

            window.QueueUpdate(this);

            // ui aktualisieren
            RefreshUI();
        }

        public void OnUpdateCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // wurde das update abgebrochen (durch den nutzer oder einen fehler)?
            if(e.Cancelled || e.Error != null)
            {
                // update nicht fertig
                State = GameTabState.UPDATE_REQUIRED;
            }
            else
            {
                // spiel ist spielbar
                State = GameTabState.PLAYABLE;
            }

            // ui aktualisieren
            RefreshUI();
        }

        public void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // den wert der progressbar auf den fortschritt setzen
            updateProgressBar.Value = e.ProgressPercentage;

            RefreshUI();
        }

        public void OnCheckForUpdates(object sender, DoWorkEventArgs e)
        {
            // signalisieren das geupdated wird
            State = GameTabState.UPDATING;

            // den parameter holen und in einen BackgroundWorker casten
            BackgroundWorker updateWorker = e.Argument as BackgroundWorker;

            // den progress resetten
            updateWorker.ReportProgress(0);

            // installationspfad
            string installDirectory = GameSettings.GameDir;

            // liste aller dateien welche noch gedownloadet werden müssen
            List<string> toDownload = new List<string>();

            // welche dateien gibt es?
            foreach (string file in Directory.GetFiles(GameSettings.Origin))
            {
                // vorübergehend ordener ignorieren
                if(Directory.Exists(file)) { continue; }

                // der pfad der datei lokal
                string localFilePath = Path.Combine(installDirectory, Path.GetFileName(file));
                bool needsDownload = true;

                // gibt es die datei?
                if(File.Exists(localFilePath))
                {
                    // prüfen ob die datei aktuell ist
                    if(true) // TODO: Prüfen ob die Datei aktuell ist
                    {
                        // wenn ja => dann nicht laden
                        needsDownload = false;
                    }
                }
                
                // wenn die datei geladen werden muss
                if(needsDownload)
                {
                    // datei für den download vormerken
                    toDownload.Add(file);
                }
            }

            // zähler für gedownloadete files
            int numFile = 0;
            foreach (string fileToDownload in toDownload)
            {
                // soll abgebrochen werden?
                if(updateWorker.CancellationPending)
                {
                    // signalisieren das abgebrochen wurde
                    e.Cancel = true;
                    // beenden
                    return;
                }

                // läuft gerade ein spiel?
                while(window.State == LauncherState.GAME_PLAYING)
                {
                    // thread pausieren
                    Thread.Yield();
                }

                // der pfad der datei lokal
                string localFilePath = 
                    Path.Combine(installDirectory, Path.GetFileName(fileToDownload));

                // datei downloaden
                // Pseudo downloaded
                File.Copy(fileToDownload, localFilePath);

                // simulieren dowloadspeed => Niemals in Production Code nutzen!
                Thread.Sleep(2000);

                // file counter erhöhen
                numFile++;

                // Progress berechnen
                float progress = 100.0f * numFile / toDownload.Count;
                // und übergeben
                updateWorker.ReportProgress((int)progress);
            }
        }

        private void RefreshUI()
        {
            string launchButtonText = "";
            updateProgressBar.Visibility = System.Windows.Visibility.Hidden;

            switch (State)
            {
                case GameTabState.NOT_INSTALLED:
                    launchButtonText = "Install";
                    break;
                case GameTabState.UPDATE_REQUIRED:
                    launchButtonText = "Update";
                    break;
                case GameTabState.UPDATE_PENDING:
                    updateProgressBar.Visibility = System.Windows.Visibility.Visible;
                    updateProgressBar.IsIndeterminate = true;
                    launchButtonText = "Update pending";
                    break;
                case GameTabState.UPDATING:
                    updateProgressBar.Visibility = System.Windows.Visibility.Visible;
                    updateProgressBar.IsIndeterminate = false;
                    launchButtonText = "Cancel Update";
                    break;
                case GameTabState.PLAYABLE:
                    launchButtonText = "Play";
                    break;
                case GameTabState.PLAYING:
                    launchButtonText = "Running";
                    break;
            }

            // den text auf dem button aktualisieren
            LaunchOrInstallButton.Content = launchButtonText;
        }

        private void LaunchOrInstallButton_Click(object sender, RoutedEventArgs e)
        {
            switch (State)
            {
                case GameTabState.NOT_INSTALLED:
                    // installation starten
                    Install();
                    break;
                case GameTabState.UPDATE_REQUIRED:
                    // update starten
                    CheckForUpdates();
                    break;
                case GameTabState.UPDATING:
                    // update abbrechen
                    window.CancelUpdate();
                    break;
                case GameTabState.PLAYABLE:
                    // spiel starten
                    Launch();
                    break;
                case GameTabState.PLAYING:
                    // nichts machen
                    break;
            }
        }

        private void Launch()
        {
            if (window.State == LauncherState.GAME_PLAYING)
            {
                MessageBox.Show("Nur ein Spiel gleichzeitig!");
                return;
            }

            Avmnts.Add(new Achievement("Play Button Pressed", true));
        }

        private void Install()
        {
            // den benutzer fragen wo das spiel gespeichert werden soll
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Choose save location";
            saveFileDialog.InitialDirectory = Path.GetFullPath("./Games/" + GameSettings.Name + "/");
            saveFileDialog.Filter = "Executables (*.exe)|*.exe|All Files|*.*";

            // den dialog anzeigen
            bool? ret = saveFileDialog.ShowDialog();
            
            // wurde nichts ausgewählt?
            if(!ret.HasValue || ret.Value == false)
            {
                MessageBox.Show("Please choose an install location!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // installationspfad setzten0
            GameSettings.GameDir = Path.GetDirectoryName(saveFileDialog.FileName);

            MessageBox.Show("Du hast folgenden Pfad ausgewählt: " + GameSettings.GameDir);

            // das spiel muss geladen werden
            State = GameTabState.UPDATE_REQUIRED;

            // das update soll das spiel installieren
            CheckForUpdates();

            // anzeige aktualisieren
            RefreshUI();
        }

    }
}
