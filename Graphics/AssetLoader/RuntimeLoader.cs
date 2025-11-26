using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System.Text.Json;


namespace Monolith.Graphics
{
    public class RuntimeAssetLoader : IAssetLoader
    {
        private GraphicsDevice gfx;
        
        private Dictionary<string, Texture2D> _textureCache = new();
        private Dictionary<string, SoundEffect> _soundCache = new();
        private Dictionary<string, Song> _musicCache = new();
        private Dictionary<string, string> _textCache = new();

        public RuntimeAssetLoader(GraphicsDevice gfx)
        {
            this.gfx = gfx;
        }

        public Texture2D LoadTexture(string path)
        {
            if (_textureCache.TryGetValue(path, out var cached))
                return cached;

            using var stream = File.OpenRead(path);
            var texture = Texture2D.FromStream(gfx, stream);
            _textureCache[path] = texture;
            return texture;
        }

        public SoundEffect LoadSound(string path)
        {
            if (_soundCache.TryGetValue(path, out var cached))
                return cached;

            using var stream = File.OpenRead(path);
            var sound = SoundEffect.FromStream(stream);
            _soundCache[path] = sound;
            return sound;
        }

        public Song LoadMusic(string path)
        {
            if (_musicCache.TryGetValue(path, out var cached))
                return cached;

            var song = Song.FromUri(Path.GetFileName(path), new Uri(Path.GetFullPath(path)));
            _musicCache[path] = song;
            return song;
        }

        public string LoadText(string path)
        {
            if (_textCache.TryGetValue(path, out var cached))
                return cached;

            var text = File.ReadAllText(path);
            _textCache[path] = text;
            return text;
        }

        public T LoadJson<T>(string path)
        {
            string json = LoadText(path);
            return JsonSerializer.Deserialize<T>(json);
        }

        public SpriteFont LoadFont(string path)
        {
            throw new NotImplementedException("Runtime SpriteFont loader not implemented.");
        }

        public Effect LoadEffect(string path)
        {
            throw new NotImplementedException("Runtime Effect loader not implemented. Use Content Pipeline instead");
        }

        public byte[] LoadRaw(string path)
        {
            return File.ReadAllBytes(path);
        }

        public void Unload(string path)
        {
            _textureCache.Remove(path);
            _soundCache.Remove(path);
            _musicCache.Remove(path);
            _textCache.Remove(path);
        }

        public void ClearCache()
        {
            _textureCache.Clear();
            _soundCache.Clear();
            _musicCache.Clear();
            _textCache.Clear();
        }
    }
}
