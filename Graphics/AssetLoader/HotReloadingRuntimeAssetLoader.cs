using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Monolith.Graphics
{
    public class HotReloadingRuntimeAssetLoader : RuntimeAssetLoader
    {
        private FileSystemWatcher _watcher;

        public HotReloadingRuntimeAssetLoader(GraphicsDevice graphicsDevice, string assetsDirectory) 
                : base(graphicsDevice)
        {
            _watcher = new FileSystemWatcher(Engine.Instance.Config.AssetsFolder)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite
            };

            _watcher.Changed += OnFileChanged;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Unload(e.FullPath);
            Console.WriteLine($"Hot-reloaded: {e.FullPath}");
        }

        public new Texture2D LoadTexture(string path)
        {
            return base.LoadTexture(path);
        }

        public new SoundEffect LoadSound(string path)
        {
            return base.LoadSound(path);
        }

        public new Song LoadMusic(string path)
        {
            return base.LoadMusic(path);
        }

        public new string LoadText(string path)
        {
            return base.LoadText(path);
        }

        public new T LoadJson<T>(string path)
        {
            return base.LoadJson<T>(path);
        }

        public new SpriteFont LoadFont(string path)
        {
            return base.LoadFont(path);
        }

        public new byte[] LoadRaw(string path)
        {
            return base.LoadRaw(path);
        }
    }
}