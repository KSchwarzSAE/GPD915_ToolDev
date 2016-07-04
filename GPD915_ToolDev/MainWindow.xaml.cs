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

namespace GPD915_ToolDev
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private GameManager gameManager;

        public MainWindow()
        {
            InitializeComponent();

            // einstellungen laden
            gameManager = new GameManager();
            gameManager.Load("E://Test.xml");

            foreach (Game game in gameManager.games)
            {
                // tabitem erstellen
                TabItem item = new TabItem();
                // dem tabitem den namen des spieles als namen geben
                // item.Name = game.Name;
                // den anzeigenamen des tabs auf den spielenamen setzten
                item.Header = game.Name;
                // ein neues gametab erstellen und dem tabitem hinzufügen
                item.Content = new GameTab();
                // das tabitem dem tabcontrol hinzufügen
                GameTabControl.Items.Add(item);
            }
        }        

    }
}
