using ECS.Components.Textures;
using ECS.Resources;
using Microsoft.Xna.Framework;

namespace Breakout.rpg;

[TextureTileSheetOptions("icons", 128)]
class IconTileSheet : TextureTileSheet
{
    private TexturePart GetInColor(Color color, int x, int y)
    {
        var tp = Get(x, y);
        tp.Modulate = color;
        return tp;
    }
        
    public TexturePart Pawn(Color color) => GetInColor(color, 11, 2);
}