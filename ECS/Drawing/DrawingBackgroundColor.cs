using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS.Drawing;

internal record DrawingBackgroundColor(Color BgColor) : IDrawing
{
    public void Draw(SpriteBatch sb)
    {
        sb.GraphicsDevice.Clear(BgColor);
    }
}