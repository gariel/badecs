using ECS.Components;
using ECS.Components.Textures;

namespace ECS;

public class SpriteBundle : EntityBundle
{
    public Transform Transform { get; set; }
    public BaseTexture Texture { get; set; }
}