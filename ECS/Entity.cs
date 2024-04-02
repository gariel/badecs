using System.Collections;

namespace ECS;

public class Entity : IEnumerable
{
    private List<IComponent> _components = new();
    
    public IEnumerator GetEnumerator() => _components.GetEnumerator();
    public void Add(IComponent component) => _components.Add(component);
    
    public T Component<T>() where T : IComponent => (T) GetComponent(typeof(T))!;

    public bool HasComponent<T>() => HasComponent(typeof(T));
    public bool HasComponent(Type t) => GetComponent(t) is not null;
    
    private IComponent? GetComponent(Type type)
        => _components.FirstOrDefault(c => c.GetType().IsAssignableTo(type));

    public override string ToString()
    {
        var c = GetComponent(typeof(IIdentifier));
        if (c is not null)
            return c.GetType().Name;
        
        return base.ToString() ?? "Entity";
    }
}
