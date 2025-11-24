using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monolith.Helpers;
using Monolith.Nodes;
using Monolith.Util;

namespace Monolith.Debugger
{
    public interface IDebugPanel
    {
        IEnumerable<(string text, Color color)> GetLines();
    }

    public class DebugCommand
    {
        private Keys key;
        private Action action;

        public DebugCommand(Keys key, Action action)
        {
            this.key = key;
            this.action = action;
        }

        public void Check()
        {
            if (Engine.Input.Keyboard.WasKeyJustPressed(key))
                action();
        }
    }

    public static class DebugManager
    {
        public enum AnchorPoint
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public static bool PanelsVisible = true;

        private static List<(IDebugPanel panel, AnchorPoint anchor)> allPanels = new();
        private static List<DebugCommand> allCommands = new();

        private static readonly Vector2 Padding = new(10, 10);
        private const float LineHeight = 18f;

        public static void Update()
        {
            foreach (var cmd in allCommands)
                cmd.Check();

            if (!PanelsVisible)
                return;

            foreach (var (panel, _) in allPanels) { }
        }

        public static void Draw()
        {
            if (!PanelsVisible || MCamera.CurrentCamera == null) return;

            var edges = MCamera.CurrentCamera.GetScreenEdges();

            foreach (var (panel, anchor) in allPanels)
            {
                Vector2 pos = anchor switch
                {
                    AnchorPoint.TopLeft => edges.TopLeft,
                    AnchorPoint.TopRight => edges.TopRight,
                    AnchorPoint.BottomLeft => edges.BottomLeft,
                    AnchorPoint.BottomRight => edges.BottomRight,
                    _ => edges.TopLeft
                };

                Vector2 offset = anchor switch
                {
                    AnchorPoint.TopLeft => new Vector2(10, 10),
                    AnchorPoint.TopRight => new Vector2(-10, 10),
                    AnchorPoint.BottomLeft => new Vector2(10, -10),
                    AnchorPoint.BottomRight => new Vector2(-10, -10),
                    _ => new Vector2(10, 10)
                };

                bool alignRight = anchor == AnchorPoint.TopRight || anchor == AnchorPoint.BottomRight;
                pos += offset;

                foreach (var (text, color) in panel.GetLines())
                {
                    Vector2 p = alignRight ? pos - new Vector2(Engine.Font.MeasureString(text).X, 0) : pos;
                    DrawHelper.DrawString(text, color, p);
                    pos.Y += LineHeight;
                }
            }
        }


        public static void AddPanel(IDebugPanel panel, AnchorPoint anchor)
        {
            allPanels.Add((panel, anchor));
        }

        public static void AddCommand(Keys key, Action action)
        {
            allCommands.Add(new DebugCommand(key, action));
        }

        public static IReadOnlyList<(IDebugPanel panel, AnchorPoint anchor)> AllPanels => allPanels.AsReadOnly();
        public static IReadOnlyList<DebugCommand> AllCommands => allCommands.AsReadOnly();
    }
}
