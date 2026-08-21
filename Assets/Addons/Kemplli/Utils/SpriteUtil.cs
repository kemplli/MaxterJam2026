using UnityEngine;

public static class SpriteUtil 
{
    public static Sprite LoadSprite(string name)
    {
        return Resources.Load<Sprite>("Art/" + name);
    }

    public static Sprite LoadSprite(string imageName, string spriteName)
    {
        Sprite[] all = Resources.LoadAll<Sprite>("Art/" + imageName);

        foreach (Sprite sprite in all)
        {
            if (sprite.name == spriteName)
            {
                return sprite;
            }
        }

        return null;
    }

    public static Sprite[] LoadSpriteSheet(string fileName)
    {
        return Resources.LoadAll<Sprite>("Art/" + fileName);
    }
}
