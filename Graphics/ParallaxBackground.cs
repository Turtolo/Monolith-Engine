using ConstructEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstructEngine.Graphics
{
    public class ParallaxBackground
    {
        public Texture2D Texture { get; }
        public float ParallaxFactor { get; }
        public Vector2 Position;
        public SamplerState SamplerState { get; }
        public Camera Camera { get; }

        public ParallaxBackground(
            Texture2D texture, 
            float parallaxFactor, 
            SamplerState samplerState, 
            Camera camera = null,
            Vector2? position = null
        ) {
            Texture = texture;
            ParallaxFactor = parallaxFactor;
            SamplerState = samplerState;
            Camera = camera;
            Position = position ?? Vector2.Zero;
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            Vector2 cameraOffset = Camera != null ? Camera.cameraPosition : Vector2.Zero;

            Rectangle viewport = graphics.Viewport.Bounds;

            float offsetX = ((Position.X + cameraOffset.X) * ParallaxFactor) % Texture.Width;
            float offsetY = ((Position.Y + cameraOffset.Y) * ParallaxFactor) % Texture.Height;

            if (offsetX < 0) offsetX += Texture.Width;
            if (offsetY < 0) offsetY += Texture.Height;

            bool repeatX = SamplerState.AddressU == TextureAddressMode.Wrap;
            bool repeatY = SamplerState.AddressV == TextureAddressMode.Wrap;

            spriteBatch.Begin(samplerState: SamplerState);

            if (!repeatX && !repeatY)
            {
                // Draw once (no repeat in either direction)
                spriteBatch.Draw(Texture, Position - cameraOffset, Color.White);
            }
            else
            {
                // Compute tiling bounds
                float startX = repeatX ? -offsetX : 0;
                float startY = repeatY ? -offsetY : 0;

                float endX = repeatX ? viewport.Width : Texture.Width;
                float endY = repeatY ? viewport.Height : Texture.Height;

                for (float y = startY; y < endY; y += Texture.Height)
                {
                    for (float x = startX; x < endX; x += Texture.Width)
                    {
                        spriteBatch.Draw(
                            Texture,
                            new Vector2(x, y),
                            Color.White
                        );
                    }
                }
            }

            spriteBatch.End();
        }




    }

    public static class ParallaxSamplers
    {
        public static readonly SamplerState RepeatX = new SamplerState
        {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Clamp,
            Filter = TextureFilter.Point
            
        };

        public static readonly SamplerState RepeatY = new SamplerState
        {
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Wrap,
            Filter = TextureFilter.Point
        };

        public static readonly SamplerState RepeatBoth = new SamplerState
        {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            Filter = TextureFilter.Point
        };
    }
}
