using ECS.Scene;

namespace ECS;

public interface ISystem
{
    void Run(ISystemEnv env);
}

public interface ISystemEnv
{
    ICommander Commander { get; }
    T Resource<T>() where T : IResource;

    Query<T> Query<T>() where T : IComponent;
    Query<T1, T2> Query<T1, T2>() where T1 : IComponent where T2 : IComponent;
}

public class SystemEnv : ISystemEnv
{
    private readonly Dictionary<Type, IResource> _resources = new();
    private readonly List<Entity> _entities = new();
    
    public SystemEnv(SceneConfig config)
    {
        foreach (var resource in config.Resources)
        {
            var type = resource.GetType();
            _resources[type] = resource;
        }
        _commander = new Commander();
    }

    private Commander _commander;
    public ICommander Commander => _commander;
    
    public T Resource<T>() where T : IResource
    {
        var type = typeof(T);
        if (_resources.TryGetValue(type, out var resource))
            return (T) resource;
        
        // TODO
        throw new KeyNotFoundException();
    }

    public Query<T> Query<T>() where T : IComponent
    {
        return new Query<T>(_entities);
    }

    public Query<T1, T2> Query<T1, T2>() where T1 : IComponent where T2 : IComponent
    {
        return new Query<T1, T2>(_entities);
    }

    internal void LastStep()
    {
        _commander.UpdateResources(_resources);
        _commander.UpdateEntities(_entities);
        _commander = new Commander();
    }
}

