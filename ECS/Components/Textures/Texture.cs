using Microsoft.Xna.Framework.Graphics;

namespace ECS.Components.Textures;

public class Texture(Texture2D texture) : BaseTexture
{
    public override Texture2D Texture2D => texture;
}