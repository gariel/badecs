using ECS.Components;
using ECS.Components.Textures;
using ECS.Drawing;
using ECS.Resources;
using Microsoft.Xna.Framework;

namespace ECS.Systems;

internal class RenderSystem : ISystem
{
    public void Run(ISystemEnv env)
    {
        var middleX = InternalGame.Instance.Graphics.PreferredBackBufferWidth / 2;
        var middleY = InternalGame.Instance.Graphics.PreferredBackBufferHeight / 2;
        var variation = new Point(middleX, middleY);
        
        var render = env.Resource<RenderResource>();
        
        var window = env.Resource<WindowResource>();
        render.Add(new DrawingBackgroundColor(window.BackgroundColor));

        var items = env.Query<BaseTexture, Transform>()
            .Components();
        // TODO sort? Heap?
        
        foreach (var (texture, transform) in items)
        {
            var original = transform.Rect;
            
            // TODO: Camera
            
            var moved = new Rectangle(original.Location + variation, original.Size);
            render.Add(new TextureDrawing(texture, moved));
        }
    }
}