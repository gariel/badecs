using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS.Components.Textures;

public class TexturePart(Texture2D texture, Rectangle part) : BaseTexture
{
    public override Texture2D Texture2D => texture;
    public override Rectangle Source => part;
}