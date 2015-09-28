using UnityEngine;
using UnityEditor;
using System.IO;

public class DungeonLayoutWindow : EditorWindow
{
	private static int SIZE = 15;
	bool[] layout = new bool[SIZE * SIZE];
	public int index = 0;
	public int lastIndex = 0;
	public string selectedFileName;
	public string newFileName;
	public bool showFileNameError = false;

	[MenuItem("Window/DungeonLayoutWindow")]
	static void Init()
	{
		DungeonLayoutWindow window = (DungeonLayoutWindow)EditorWindow.GetWindow(typeof(DungeonLayoutWindow));
		window.Show();
	}

	void OnGUI()
	{
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Layout Editor");
		EditorGUILayout.Space();

		ShowFileNames();

		EditorGUILayout.Space();

		if(index != 0 && lastIndex != index)
		{
			LoadData();
		}

		if(index == 0)
		{
			EditorGUILayout.LabelField("Enter Layout File Name");
			newFileName = EditorGUILayout.TextField(newFileName);
		}

		for (int i = 0; i < SIZE; i++)
		{
			Rect r = EditorGUILayout.BeginHorizontal("Button");
			for (int j = 0; j < SIZE; j++)
			{
				layout[i * SIZE + j] = EditorGUILayout.Toggle(layout[i * SIZE + j]);
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Save"))
		{
			SaveLayout();	
		}
		if(GUILayout.Button("Clear"))
		{
			if(EditorUtility.DisplayDialog("Clear Layout", "Are you sure you want to clear the current layout? Remember: You will need to save to keep changes", "Yes", "No"))
			{
				for(int i = 0; i < layout.Length; i++)
				{
					layout[i] = false;
				}
			}
		}
		EditorGUILayout.EndHorizontal();

		if(showFileNameError)
		{
			EditorGUILayout.HelpBox("File name is invalid or already taken", MessageType.Error);
		}
	}

	void ShowFileNames()
	{
		FileInfo[] fileInfo = GetFileInfo();
		string[] fileNames = new string[fileInfo.Length + 1];
		fileNames[0] = "New";
		for(int i = 1; i < fileNames.Length; i++)
		{
			fileNames[i] = fileInfo[i - 1].Name;
		}
		lastIndex = index;
		index = EditorGUILayout.Popup(index, fileNames);
		selectedFileName = fileNames[index];
	}

	FileInfo[] GetFileInfo()
	{
		DirectoryInfo info = new DirectoryInfo(Application.dataPath + "/Layouts/");
		FileInfo[] fileInfo = info.GetFiles();
		return fileInfo;
	}

	void LoadData()
	{
		string filePath = Application.dataPath + "/Layouts/" + selectedFileName;

		string line;
		int lineNum = 0;
		StreamReader reader = new StreamReader(filePath);
		using(reader)
		{
			do
			{
				line = reader.ReadLine();

				if (line != null)
				{
					string[] entries = line.Split(',');
					if (entries.Length > 0)
					{
						for (int i = 0; i < entries.Length; i++)
						{
							if (entries[i] == "1" || entries[i] == " 1")
								layout[lineNum * SIZE + i] = true;
							else
								layout[lineNum * SIZE + i] = false;
						}
					}
				}

				lineNum++;
			}
			while (line != null);
			
			reader.Close();
			
		}
	}

	void SaveLayout()
	{
		string path = Application.dataPath + "/Layouts/";
		if(index == 0)
		{
			if(newFileName != "" && !File.Exists(path + "" + newFileName + ".csv"))
			{
				string output = CreateSaveString();
				File.WriteAllText(path + "" + newFileName + ".csv", output);
				showFileNameError = false;
			}
			else
			{
				showFileNameError = true;
			}
		}
		else
		{
			string output = CreateSaveString();
			File.WriteAllText(path + "" + selectedFileName, output);
		}
		Debug.Log(newFileName);
	}

	string CreateSaveString()
	{
		string retval = "";
		for(int i = 0; i < SIZE; i++)
		{
			for(int j = 0; j < SIZE; j++)
			{
				if (layout[i * SIZE + j])
					retval += 1;
				else
					retval += 0;

				if (i == SIZE - 1 && j == SIZE - 1)
				{ }
				else
					retval += ", ";
			}
			if (i != (SIZE - 1))
				retval += "\n";
		}

		return retval;
	}
}