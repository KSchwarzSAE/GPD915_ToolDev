using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;
using System.Text.RegularExpressions;

public enum SearchType
{
	REGEX,
	COMPONENT
}

public class SearchWindow : EditorWindow
{

	// erster menüeintrag
	// % => strg/ctrl/cmd
	// # => shift
	// & => alt
	[MenuItem("Utils/Search %#s")]
	public static void ShowSearchWindow()
	{
		// fenster holen, oder ggf. erstellen
		SearchWindow window = GetWindow<SearchWindow>();

		// titel des fensters ändern
		window.titleContent = new GUIContent("Search");
	}

	private bool showAll;

	private string searchText = "";

	private SearchType searchType;

	private Vector2 allObjsScrollPos;

	private Vector2 searchResultScrollPos;

	private void OnGUI()
	{
		// alle gameobjects holen
		GameObject[] allObjs = 
			GameObject.FindObjectsOfType<GameObject> ();

		// foldout anzeigen
		showAll = 
			EditorGUILayout.Foldout (showAll, "All GameObjects");

		// prüfen ob das foldout offen ist
		if (showAll)
		{
			// etwas einrücken
			EditorGUI.indentLevel++;

			// Scroll View anzeigen
			allObjsScrollPos = 
				EditorGUILayout.BeginScrollView (allObjsScrollPos);
			
			// alle gameobjects anzeigen
			foreach (var obj in allObjs)
			{
				ShowGameObject (obj);
			}

			EditorGUILayout.EndScrollView ();

			// wieder zurück einrücken
			EditorGUI.indentLevel--;
		} // Ende Foldout

		// platz anzeigen
		EditorGUILayout.Space ();

		// der nutzer kann den suchtyp auswählen
		searchType = (SearchType) EditorGUILayout.EnumPopup(
				new GUIContent("Search Type"),
				searchType
			);

		// den suchstring abfragen
		searchText = 
			EditorGUILayout.TextField ("Search For:", searchText);

		// anhand des suchtyps die richtige methode aufrufen
		switch (searchType)
		{
			// die namen der gameobjects sollen per
			// regex durchsucht werden
			case SearchType.REGEX:
				SearchRegex (allObjs);
				break;

			// die gameobjects sollen nach bestimmten
			// components durchsucht werden
			case SearchType.COMPONENT:
				SearchComponent (allObjs);
				break;
		}
	}

	private void ShowGameObject(GameObject obj)
	{
		EditorGUILayout.BeginHorizontal ();
		{
			// name des gameobjects anzeigen
			EditorGUILayout.LabelField (obj.name);

			bool selected = Selection.activeGameObject == obj;

			// button anzeigen welcher bei click
			if (GUILayout.Button (
				selected ? "Deselect" : "Select",
				GUILayout.Width (125.0f)
			))
			{
				// das object auswählt
				Selection.activeObject = selected ? null : obj;
			}

		}
		EditorGUILayout.EndHorizontal ();
	}

	private void SearchRegex(GameObject[] allObjs)
	{
		// ist der suchstring gültig?
		try
		{
			Regex.Match ("", searchText);
		}
		catch (System.Exception ex)
		{
			// wird eine exception geworfen
			// dann ist der suchstring ungültig
			EditorGUILayout.HelpBox
			("Suchstring ist kein gültiger Regex",
				MessageType.Warning);
			return;
		}
			
		// etwas einrücken
		EditorGUI.indentLevel++;

		searchResultScrollPos =
			EditorGUILayout.BeginScrollView (searchResultScrollPos);

		// über alle gameobjects iterieren
		foreach (var obj in allObjs)
		{
			// und prüfen ob sie dem suchstring entsprechen
			if (Regex.IsMatch (obj.name, searchText))
			{
				// das object anzeigen
				ShowGameObject (obj);
			}
		}

		EditorGUILayout.EndScrollView ();

		// wieder zurück einrücken
		EditorGUI.indentLevel--;
	}

	private void SearchComponent(GameObject[] allObjs)
	{
		// etwas einrücken
		EditorGUI.indentLevel++;

		searchResultScrollPos =
			EditorGUILayout.BeginScrollView (searchResultScrollPos);

		// über alle gameobjects iterieren
		foreach (var obj in allObjs)
		{
			// prüfen ob das gameobject
			// die genannte component besitzt
			if (obj.GetComponent (searchText) != null)
			{
				// das object anzeigen
				ShowGameObject (obj);
			}
		}

		EditorGUILayout.EndScrollView ();

		// wieder zurück einrücken
		EditorGUI.indentLevel--;
	}

}