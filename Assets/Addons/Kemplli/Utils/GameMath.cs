using UnityEngine;
using System.Collections.Generic;

public static class GameMath
{
    private static Camera cam;

    public static Vector2 MousePos2D()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        var mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        return mousePos;
    }

    public static Vector2 DirectionToMouse2D(Vector2 origin)
    {
        return (MousePos2D() - origin).normalized;
    }
    public static Vector2 DirectionTo(Vector2 from,Vector2 to)
    {
        return (to - from).normalized;
    }

    public static string ToColor(this string str,Color c)
    {
        string coloredString = "<color=#" + ColorUtility.ToHtmlStringRGBA(c) + ">";
        coloredString += str;
        coloredString += "</color>";

        return coloredString;
    }
    public static Color GetColorWithAlpha(Color c, float alpha)
    {
        return new Color(c.r, c.g, c.b, alpha);
    }

    public static float GetMainVelocityDir(this Rigidbody2D rb)
    {
        return Mathf.Max(Mathf.Abs(rb.linearVelocity.x), Mathf.Abs(rb.linearVelocity.y));
    }

    public static float GetMainVelocityDir(Vector2 velocity)
    {
        return Mathf.Max(Mathf.Abs(velocity.x), Mathf.Abs(velocity.y));
    }

    public static Vector2 GetCenterOfScreen()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        Vector3 centerScreen = new Vector3(screenWidth / 2f, screenHeight / 2f, 0f);
        var centerScreenWorld = cam.ScreenToWorldPoint(centerScreen);
        centerScreenWorld = new Vector3(centerScreenWorld.x, centerScreenWorld.y, 0);

        return centerScreenWorld;
    }

    public static void FullListWithOther<T>(List<T> from,ref List<T> to)
    {
        to.Clear();
        foreach (T item in from)
        {
            to.Add(item);
        }
    }
}
