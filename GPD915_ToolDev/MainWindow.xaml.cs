using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace GPD915_ToolDev
{

    public enum LauncherState
    {
        IDLE,
        GAME_UPDATING,
        GAME_PLAYING
    }

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Der aktuelle Zustand des Launchers
        public LauncherState State;

        // Unser GameManager
        private GameManager gameManager;

        // der pfad zu unserer Einstellungsdatei
        private string configFilePath = "./Config.xml";

        private BackgroundWorker updateWorker;

        private Queue<GameTab> pendingUpdates;

        // das spiel das gerade geupdatet wird
        private GameTab currentUpdate;

        public MainWindow()
        {
            // udpate worker initalisieren
            updateWorker = new BackgroundWorker();
            updateWorker.WorkerReportsProgress = true;
            updateWorker.WorkerSupportsCancellation = true;

            // nichts passiert gerade
            State = LauncherState.IDLE;

            pendingUpdates = new Queue<GameTab>();

            InitializeComponent();

            // einstellungen laden
            gameManager = new GameManager();

            // prüfen ob die datei existiert
            if(File.Exists(configFilePath))
            {
                // wenn ja => dann laden wir sie
                gameManager.Load(configFilePath);
            }
            else
            {
                // wenn nicht => erstellen wir standard games
                Game game1 = new Game();
                game1.Name = "Erstes Spiel";
                gameManager.games.Add(game1);

                // config speichern
                gameManager.Save(configFilePath);
            }            

            // alle spiele durchgehen
            foreach (Game game in gameManager.games)
            {
                // tabitem erstellen
                TabItem item = new TabItem();
                // dem tabitem den namen des spieles als namen geben
                // item.Name = game.Name;
                // den anzeigenamen des tabs auf den spielenamen setzten
                item.Header = game.Name;
                // ein neues gametab erstellen und dem tabitem hinzufügen
                item.Content = new GameTab(game, this);
                // das tabitem dem tabcontrol hinzufügen
                GameTabControl.Items.Add(item);
            }
        }

        public void QueueUpdate(GameTab _tab)
        {
            // das update der queue hinzufügen
            pendingUpdates.Enqueue(_tab);

            // prüfen ob ein neues update gestarten werden soll
            CheckStartUpdate();
        }

        private void CheckStartUpdate()
        {
            // falls ein spiel gespielt wird, oder ein update läuft,
            // wird kein neues update gestartet
            if (State != LauncherState.IDLE) { return; }

            // sind keine updates eingereiht => nichts updaten
            if (pendingUpdates.Count == 0) { return; }

            currentUpdate = pendingUpdates.Dequeue();

            // events setzen
            updateWorker.DoWork += currentUpdate.OnCheckForUpdates;
            updateWorker.ProgressChanged += currentUpdate.OnProgressChanged;
            updateWorker.RunWorkerCompleted += currentUpdate.OnUpdateCompleted;
            updateWorker.RunWorkerCompleted += updateWorker_RunWorkerCompleted;

            // update starten
            updateWorker.RunWorkerAsync(updateWorker);
            State = LauncherState.GAME_UPDATING;
        }

        void updateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // worker resetten
            updateWorker.DoWork -= currentUpdate.OnCheckForUpdates;
            updateWorker.RunWorkerCompleted -= currentUpdate.OnUpdateCompleted;
            updateWorker.RunWorkerCompleted -= updateWorker_RunWorkerCompleted;
            updateWorker.ProgressChanged -= currentUpdate.OnProgressChanged;

            // launcher state resetten
            currentUpdate = null;
            State = LauncherState.IDLE;

            // prüfen ob das nächste Update starten soll
            CheckStartUpdate();
        }

        public void OnGameStarted()
        {
            State = LauncherState.GAME_PLAYING;
        }

        public void OnGameStopped()
        {
            // war ein update am laufen? weitermachen ansonsten idle
            State = currentUpdate != null ? LauncherState.GAME_UPDATING : LauncherState.IDLE;
        }

        public void CancelUpdate()
        {
            updateWorker.CancelAsync();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // alle etwaigen Änderungen speichern
            gameManager.Save(configFilePath);
        }        

    }
}
