using System.Reflection;
using ECS.Components;
using ECS.Components.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Texture = ECS.Components.Textures.Texture;

namespace ECS.Resources;

public class AssetResource : IResource
{
    public T LoadTileSheet<T>() where T : TextureTileSheet, new()
    {
        var options = typeof(T).GetCustomAttribute<TextureTileSheetOptionsAttribute>();
        if (options is null)
            throw new TypeLoadException($"Texture TileSheet needs have {nameof(TextureTileSheetOptionsAttribute)}");
        
        var texture2D = InternalGame.Instance.Content.Load<Texture2D>(options.Name);
        var tilesheet = new T();
        tilesheet.InjectOptions(texture2D, options.Size);
        return tilesheet;
    }

    public Texture LoadTexture(string name)
    {
        var texture2D = InternalGame.Instance.Content.Load<Texture2D>(name);
        return new Texture(texture2D);
    }
}

public abstract class TextureTileSheet
{
    protected Texture2D Texture2D { get; private set; }
    protected int Size { get; private set; }
    private Point _sizePoint;

    internal void InjectOptions(Texture2D texture2D, int size)
    {
        Texture2D = texture2D;
        Size = size;
        _sizePoint = new Point(size, size);
    }
    
    protected TexturePart Get(int x, int y)
    {
        var location = new Point(x * Size, y * Size);
        return new TexturePart(Texture2D, new Rectangle(location, _sizePoint));
    }
}

public class TextureTileSheetOptionsAttribute(string name, int size=32) : Attribute
{
    public string Name => name;
    public int Size => size;
}