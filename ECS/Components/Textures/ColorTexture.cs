using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS.Components.Textures;

public class ColorTexture : BaseTexture
{
    public override Texture2D Texture2D { get; }
    
    public ColorTexture(Color color)
    {
        var texture = new Texture2D(InternalGame.Instance.GraphicsDevice, 1, 1);
        texture.SetData([color]);
        Texture2D = texture;
    }

}