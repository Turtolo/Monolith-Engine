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
    public class ContentPipelineAssetLoader : IAssetLoader
    {
        private ContentManager ctm;

        public ContentPipelineAssetLoader(ContentManager ctm)
        {
            this.ctm = ctm;
        }

        public Texture2D LoadTexture(string path)
        {
            return ctm.Load<Texture2D>(path);
        }

        public SoundEffect LoadSound(string path)
        {
            return ctm.Load<SoundEffect>(path);
        }

        public Song LoadMusic(string path)
        {
            return ctm.Load<Song>(path);
        }

        public string LoadText(string path)
        {
            return ctm.Load<string>(path);
        }

        public T LoadJson<T>(string path)
        {
            string json = LoadText(path);
            return JsonSerializer.Deserialize<T>(json);
        }

        public SpriteFont LoadFont(string path)
        {
            return ctm.Load<SpriteFont>(path);
        }

        public Effect LoadEffect(string path)
        {
            return ctm.Load<Effect>(path);
        }

        public byte[] LoadRaw(string path)
        {
            return ctm.Load<byte[]>(path);
        }

        public void Unload(string path)
        {
            Console.WriteLine("You cannot unload individual items with the pipeline.");
        }

        public void ClearCache()
        {
            ctm.Unload();
        }
    }
}
