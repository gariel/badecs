using Microsoft.Xna.Framework;

namespace ECS.Components;

public class Transform(Vector3 translation, Vector2 scale) : IComponent
{
    public Vector3 Translation { get; set; } = translation;
    public Vector2 Scale { get; set; } = scale;

    public Rectangle Rect => new(
        (int)(Translation.X - Scale.X / 2), (int)(Translation.Y - Scale.Y / 2),
        (int)Scale.X, (int)Scale.Y);
}
