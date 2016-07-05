using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Unser GameManager
        private GameManager gameManager;

        // der pfad zu unserer Einstellungsdatei
        private string configFilePath = "./Config.xml";

        public MainWindow()
        {
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
                item.Content = new GameTab(game);
                // das tabitem dem tabcontrol hinzufügen
                GameTabControl.Items.Add(item);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // alle etwaigen Änderungen speichern
            gameManager.Save(configFilePath);
        }        

    }
}
