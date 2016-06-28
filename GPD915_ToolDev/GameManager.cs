using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GPD915_ToolDev
{

    // Verwaltet die Einstellungen der Spiele
    public class GameManager
    {
        
        // eine liste aller spiele
        public List<Game> games;

        public void Save(string _saveLoc)
        {
            // xml document erstellen
            XDocument doc = new XDocument();

            // ein Element <GameManager> erstellen,
            XElement gameManagerElement = new XElement("GameManager");
            // und dem document hinzufuegen
            doc.Add(gameManagerElement);

            // ueber alle spiele iterieren
            foreach (var game in games)
            {
                // ein Element <Game> erstellen,
                XElement gameElement = new XElement("Game");
                // und dem Element <GameManager> unterordnen
                gameManagerElement.Add(gameElement);


            }

            // das document in die _saveLoc datei speichern
            doc.Save(_saveLoc);
        }

    }

    // Enthält die Einstellungen eines Spieles
    public class Game
    {

        // der speicherort des spieles (null = nicht installiert)
        public string GameDir { get; set; }
        


    }

}