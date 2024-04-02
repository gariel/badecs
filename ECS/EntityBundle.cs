namespace ECS;

public abstract class EntityBundle
{
    public IList<IComponent> Extras { get; set; } = new List<IComponent>(0);

    private IEnumerable<IComponent> GetComponents()
    {
        var props = GetType()
            .GetProperties();
        
        var propComponents = props
            .Where(p => p.PropertyType.IsAssignableTo(typeof(IComponent)))
            .Select(p => p.GetValue(this))
            .Cast<IComponent>();
        
        var bundleComponents = props
            .Where(p => p.PropertyType.IsAssignableTo(typeof(EntityBundle)))
            .Select(p => p.GetValue(this))
            .Cast<EntityBundle>()
            .SelectMany(eb => eb.GetComponents());

        return propComponents
            .Union(bundleComponents)
            .Union(Extras);
    }

    public Entity ToEntity()
    {
        var e = new Entity();
        GetComponents()
            .ToList()
            .ForEach(e.Add);
        return e;
    }
}