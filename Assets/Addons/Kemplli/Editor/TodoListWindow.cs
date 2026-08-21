using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TodoListWindow : EditorWindow
{
    private List<string> todoList = new List<string>();
    private Vector2 scrollPos;
    private string newTask = "";

    private const string EditorPrefsKey = "CustomTodoList";

    [MenuItem("Window/Custom TODO List")]
    public static void ShowWindow()
    {
        GetWindow<TodoListWindow>("TODO List");
    }

    private void OnEnable()
    {
        LoadTodoList();
    }

    private void OnGUI()
    {
        GUILayout.Label("TODO List", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        newTask = EditorGUILayout.TextField(newTask);
        if (GUILayout.Button("Add", GUILayout.Width(70)))
        {
            if (!string.IsNullOrWhiteSpace(newTask))
            {
                todoList.Add(newTask.Trim());
                newTask = "";
                SaveTodoList();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(200));
        for (int i = 0; i < todoList.Count; i++)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label($"- {todoList[i]}");

            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                todoList.RemoveAt(i);
                SaveTodoList();
                break;
            }

            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button("Destroy all"))
        {
            if (EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to clear the entire list?", "Yes", "Cancel"))
            {
                todoList.Clear();
                SaveTodoList();
            }
        }
    }

    private void SaveTodoList()
    {
        string serialized = string.Join("\n", todoList.ToArray());
        EditorPrefs.SetString(EditorPrefsKey, serialized);
    }

    private void LoadTodoList()
    {
        todoList.Clear();
        string serialized = EditorPrefs.GetString(EditorPrefsKey, "");
        if (!string.IsNullOrEmpty(serialized))
        {
            string[] items = serialized.Split('\n');
            todoList.AddRange(items);
        }
    }
}
