using Microsoft.Xna.Framework;

namespace ECS;

public static class VectorExtensions
{
    public static Vector2 XY(this Vector3 self) => new(self.X, self.Y);
}