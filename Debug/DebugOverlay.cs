using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class DebugOverlay
{
    public static bool IsEnabled = true;
    private static SpriteFont font;

    private static Dictionary<string, Func<string>> debugInfos = new Dictionary<string, Func<string>>();

    public static void Initialize(SpriteFont debugFont)
    {
        font = debugFont;
    }

    public static void AddInfo(string key, Func<string> infoFunc)
    {
        debugInfos[key] = infoFunc;
    }

    public static void RemoveInfo(string key)
    {
        debugInfos.Remove(key);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        if (!IsEnabled || font == null)
            return;

        int y = 10;

        foreach (var info in debugInfos.Values)
        {
            string text = info.Invoke();
            spriteBatch.DrawString(font, text, new Vector2(10, y), Color.White);
            y += 20;
        }
    }
}
