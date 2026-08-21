using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

public class MoveableBrowser : EditorWindow
{
    private static List<MoveableBase> moveableBases = new List<MoveableBase>();
    private static MonoBehaviour target;

    public static bool Closed = true;

    public static void ShowWindow(MonoBehaviour obj)
    {
        Closed = false;

        moveableBases.Clear();
        GetWindow<MoveableBrowser>("Choose Moveable");
        var list = ReflectionUtil.FindAllSubclaces<MoveableBase>().ToList();

        foreach (var type in list)
        {
            moveableBases.Add(Activator.CreateInstance(type) as MoveableBase);
        }

        target = obj;
    }

    private Vector2 scrollPosition;

    private void OnGUI()
    {

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

        foreach (var moveable in moveableBases)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(moveable.GetType().FullName))
            {
                Component newComponent = target.gameObject.AddComponent(moveable.GetType());
                this.Close();
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    private void OnDisable()
    {
        Closed = true;
    }
}