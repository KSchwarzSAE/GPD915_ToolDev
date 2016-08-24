using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Monster))]
public class MonsterEditor : Editor
{

	public override void OnInspectorGUI ()
	{
		// sicherstellen das alle variablen aktuell sind
		serializedObject.Update ();

		// die variable maxHealth suchen
		SerializedProperty maxHealth =
			serializedObject.FindProperty ("maxHealth");

		// die variable zum verändern anzeigen
		EditorGUILayout.IntSlider (maxHealth, 0, 100);

		// die variable health suchen
		SerializedProperty health = 
			serializedObject.FindProperty ("health");

		// eine progress bar als lebensleiste anzeigen
		ProgressBar (1.0f * health.intValue / maxHealth.intValue, "Health");

		if (GUILayout.Button ("Full Health"))
		{
			health.intValue = maxHealth.intValue;
		}

		// anzeigen das variablen ggf. verändert wurden
		serializedObject.ApplyModifiedProperties ();
	}

	// Custom GUILayout progress bar.
	void ProgressBar (float value, string label) {
		// Get a rect for the progress bar using
		// the same margins as a textfield:
		Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");
		EditorGUI.ProgressBar (rect, value, label);
		EditorGUILayout.Space ();
	}

}