namespace ECS;

public interface ICommander
{
    void AddEntity(Entity e);
    void AddEntity(EntityBundle eb);
    void RemoveEntity(Entity e);
    
    void AddResource(IResource r);
    void RemoveResource(IResource r);
}

public class Commander : ICommander
{
    private readonly List<(Type, IResource)> _resourcesToAdd = new();
    private readonly List<Type> _resourcesToRemove = new();

    private readonly List<Entity> _entitiesToAdd = new();
    private readonly List<Entity> _entitiesToRemove = new();
    
    public void AddEntity(Entity e) => _entitiesToAdd.Add(e);
    public void AddEntity(EntityBundle eb) => _entitiesToAdd.Add(eb.ToEntity());
    public void RemoveEntity(Entity e) => _entitiesToRemove.Add(e);

    public void AddResource(IResource r) => _resourcesToAdd.Add((r.GetType(), r));
    public void RemoveResource(IResource r) => _resourcesToRemove.Add(r.GetType());

    internal void UpdateResources(Dictionary<Type, IResource> resources)
    {
        foreach (var (type, resource) in _resourcesToAdd)
            resources[type] = resource;

        foreach (var type in _resourcesToRemove)
            resources.Remove(type);
    }
    
    internal void UpdateEntities(List<Entity> entities)
    {
        entities.AddRange(_entitiesToAdd);
        foreach (var entity in _entitiesToRemove)
            entities.Remove(entity);
    }
}