using ECS.Components;
using ECS.Components.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS.Drawing;

internal class TextureDrawing(BaseTexture texture, Rectangle rectangle) : IDrawing
{
    public void Draw(SpriteBatch sb)
    {
        sb.Draw(texture.Texture2D, rectangle, texture.Source, texture.Modulate);
    }
}