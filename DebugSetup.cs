using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monolith;
using Monolith.Debugger;
using Monolith.Nodes;

public class DebugSetup
{
    public class NodePanel : IDebugPanel
    {
        public IEnumerable<(string text, Color color)> GetLines()
        {
            int FPS = (int)Math.Round(Engine.FPS);

            Color fpsColor = FPS >= 60 ? Color.LimeGreen :
                             FPS >= 30 ? Color.Yellow :
                                         Color.Red;

            yield return ($"Current FPS: {FPS}", fpsColor);
            yield return ($"Total Nodes: {Node.AllInstances.Count}", Color.Green);
            yield return ($"Node Types: {Node.AllInstancesDetailed.Count}", Color.Aqua);
            yield return ($"Current Scene: {Engine.SceneManager.GetCurrentScene()}", Color.LightBlue);
            yield return ("", Color.White);
            yield return ($"Player Position: {Engine.MainCharacter?.Location}", Color.LimeGreen);

            yield return ("U: Toggle Debug", Color.Yellow);
            yield return ("R: Reload Scene", Color.Yellow);
            yield return ("T: Toggle Regions", Color.Yellow);
        }
    }

    public class RegionPanel : IDebugPanel
    {
        public static bool Enabled { get; set; }

        public IEnumerable<(string text, Color color)> GetLines()
        {
            if (!Enabled) yield break;

            foreach (var n in Node.AllInstances)
                yield return ("Region", Color.Red);
        }
    }
}
