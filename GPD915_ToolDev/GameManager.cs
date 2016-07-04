using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace GPD915_ToolDev
{

    // Verwaltet die Einstellungen der Spiele
    public class GameManager
    {
        
        // eine liste aller spiele
        public List<Game> games = new List<Game>();
        
        // Speichert alle Game einstellungen in ein
        // XML-Dokument
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
                // spiel in das Element speichern
                game.Save(gameElement);
            }

            // das document in die _saveLoc datei speichern
            doc.Save(_saveLoc);
        }

        // Lädt eine Einstellungs-Datei
        public void Load(string _saveLoc)
        {
            try
            {
                // reset
                games.Clear();

                // unser xml document laden
                XDocument doc = XDocument.Load(_saveLoc);
                XElement gameManagerElement = doc.Element("GameManager");

                // über alle child-elemente des GameManager iterieren
                foreach (XElement gameElement in
                    gameManagerElement.Elements("Game"))
                {
                    // das element als spiel einstellung laden
                    games.Add(new Game(gameElement));
                }
            }
            catch (Exception e)
            {
                // fehlermeldung zeigen
                MessageBox.Show(e.ToString(), "Fehler beim Laden.");

                // resetten
                games.Clear();
            }
        }

    }

    // Enthält die Einstellungen eines Spieles
    public class Game
    {
        // der Name des Spieles
        public string Name { get; set; }

        // der speicherort des spieles (null = nicht installiert)
        public string GameDir { get; set; }
        
        public Game()
        { }

        public Game(XElement _element)
        {
            // das attribut für die GameDir suchen
            XAttribute gameDirAtt = _element.Attribute("GameDir");
            // die GameDir auslesen
            GameDir = (string)gameDirAtt;
            
            // ist die länge der GameDir 0
            if(GameDir.Length == 0)
            {
                // GameDir auf null (= nicht installiert) setzten
                GameDir = null;
            }

            // das attribut für den namen suchen
            XAttribute nameAtt = _element.Attribute("Name");
            // und den namen auslesen
            Name = (string)nameAtt;
        }

        // Speichert das Game Objekt in dem XElement
        public void Save(XElement _element)
        {
            // Ein Attribut mit dem Namen "GameDir",
            // welches die GameDir beinhaltet
            XAttribute gameDirAtt =
                new XAttribute("GameDir", GameDir != null ? GameDir : "");
            _element.Add(gameDirAtt);

            // Ein Attribut mit dem Namen "Name",
            // welches den Namen beinhaltet
            XAttribute nameAtt = new XAttribute("Name", Name);
            _element.Add(nameAtt);
        }

    }

}