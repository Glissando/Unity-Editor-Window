#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class QuestEditor : EditorWindow {
	//Directory to store quest data
	private static string directory = "/QuestSystem/Xml/";
	public static readonly string filePath = Application.dataPath + directory;
	public static readonly string fileName = "Quest.xml";

	bool groupEnabled;

	private static List<bool> foldouts = new List<bool>();
	private static List<Quest> quests = new List<Quest>();

	Quest quest = new Quest();

	Vector2[] descriptionScrolls = new Vector2[quests.Count];
	Vector2 scrollPosition;

	public TextAsset QuestXml;

	[MenuItem("Window/QuestEditor")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(QuestEditor));
		LoadXml();
	}

	private static void LoadXml() {
		quests = new List<Quest>();
		foldouts = new List<bool>();

		if(File.Exists(filePath + fileName)) {
			XmlSerializer serializer = new XmlSerializer(typeof(List<Quest>));

			using(FileStream file = new FileStream(directory + fileName, FileMode.Open)) {
				try {
					quests = serializer.Deserialize(file) as List<Quest>;
					foldouts = new List<bool>(quests.Count);
				}
				catch(System.Exception e) {
					Debug.LogError(e.Message);
				}
			}
		}
	}

	private void SaveXML() {
		if(!Directory.Exists(filePath)) {
			Directory.CreateDirectory(filePath);
		}

		using(FileStream file = new FileStream(filePath + fileName, FileMode.Create)) {
			XmlSerializer serializer = new XmlSerializer(typeof(List<Quest>));
			serializer.Serialize(file, quests);
			//Refresh the asset window to make sure the file is displayed
			AssetDatabase.Refresh();
		}
	}

	void OnGUI() {
		EditorGUILayout.BeginVertical();
		GUILayout.Label("Quest Data", EditorStyles.boldLabel);
		directory = EditorGUILayout.TextField ("Directory", directory);

		groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
		quest.name = EditorGUILayout.TextField("Name",quest.name,GUILayout.Width(300));
		EditorGUI.indentLevel += 1;
		quest.description = EditorGUILayout.TextField("Description",quest.description,GUILayout.Height(100));
		EditorGUILayout.BeginHorizontal();

		//Set values
		quest.count = EditorGUILayout.IntField("Amount",Mathf.Clamp(quest.count,0,int.MaxValue),GUILayout.Width(200));
		quest.money = EditorGUILayout.IntField("Money",Mathf.Clamp(quest.money,0,int.MaxValue),GUILayout.Width(200));
		quest.exp = EditorGUILayout.IntField("Expierence",Mathf.Clamp(quest.exp,0,int.MaxValue),GUILayout.Width(200));
		quest.returnToQuestGiver = EditorGUILayout.Toggle(quest.returnToQuestGiver);

		EditorGUILayout.EndHorizontal();
		EditorGUI.indentLevel -= 1;
		EditorGUILayout.EndVertical();
		EditorGUILayout.BeginHorizontal();

		//Check for input
		if(GUILayout.Button("Add")) {
			quests.Add(new Quest(quest));
			foldouts.Add(false);
			Debug.Log (quest.name + " Has been saved");
		}

		if(GUILayout.Button	("Clear List")) {
			string[] options = new string[2];
			options[0] = "Yes";
			options[1] = "No";

			if(EditorGUILayout.Popup("Clear List?",1,options)==0){
				quests.Clear();
				foldouts.Clear();
			}
		}

		if(GUILayout.Button ("Save")) {
			Debug.Log ("Saved in " + filePath);
			SaveXML();
		}

		EditorGUILayout.EndHorizontal();
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		bool[] foldoutArray = foldouts.ToArray();

		for(int i = 0; i < quests.Count; ++i) {
			EditorGUILayout.Foldout(foldoutArray[i],new GUIContent("Info: " + quests[i].name));

			if(GUILayout.Button(new GUIContent("Toggle"), GUILayout.Width(position.width * 0.2f))){
				if(!EditorPrefs.GetBool("QuestFoldout" + i))
					EditorPrefs.SetBool("QuestFoldout" + i,true);
				else
					EditorPrefs.SetBool("QuestFoldout" + i,false);
			}
			foldoutArray[i] = EditorPrefs.GetBool("QuestFoldout"+i);
			//Draw foldout?
			if(foldoutArray[i] == true){
				EditorGUI.indentLevel += 1;
				quests[i].name = EditorGUILayout.TextField(new GUIContent("Name"), quests[i].name);
				EditorGUI.indentLevel += 1;
				//quests[i].description = EditorGUILayout.TextArea(quests[i].description,GUILayout.Height(position.height - 30));

				quests[i].money = EditorGUILayout.IntField(new GUIContent("Money"),quests[i].money);
				quests[i].exp = EditorGUILayout.IntField(new GUIContent("Experience"),quests[i].exp);
				quests[i].count = EditorGUILayout.IntField(new GUIContent("Amount"),quests[i].count);

				EditorGUI.indentLevel -= 2;
				EditorGUILayout.BeginHorizontal();

				if(GUILayout.Button(new GUIContent("Remove Item"), GUILayout.Width(position.width * 0.5f))) {
					quests.RemoveAt(i);
					foldouts.RemoveAt(i);
					i--;
				}
				EditorGUILayout.EndHorizontal();
			}

		}
		EditorGUILayout.EndScrollView();
		EditorGUILayout.EndToggleGroup ();
	}
}
#endif