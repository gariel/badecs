using ECS.Drawing;

namespace ECS.Resources;

public class RenderResource : IResource
{
    public List<IDrawing>? Drawings { get; private set; } = new();
    public void Add(IDrawing drawing) => Drawings?.Add(drawing);
    public void Reset() => Drawings = new();
}