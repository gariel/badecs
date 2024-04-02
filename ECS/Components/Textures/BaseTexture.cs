using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS.Components.Textures;

public abstract class BaseTexture : IComponent
{
    public abstract Texture2D Texture2D { get; }
    public virtual Rectangle Source => Texture2D.Bounds;
    public Color Modulate { get; set; } = Color.White;
}