namespace ECS.Scene;

public class SceneConfig
{
    public readonly List<IResource> Resources = new();
    public readonly IDictionary<SceneStage, List<ISystem>> Systems;

    public SceneConfig(IScene scene)
    {
        Systems = Enum.GetValues(typeof(SceneStage))
            .Cast<SceneStage>()
            .ToDictionary(e => e, _ => new List<ISystem>());

        scene.Configure(this);
    }
    
    public SceneConfig AddSystem<T>(SceneStage stage, T system) where T : ISystem, new()
    {
        Systems[stage].Add(system);
        return this;
    }
    
    public SceneConfig AddSystem<T>(SceneStage stage) where T : ISystem, new()
        => AddSystem(stage, new T());
    
    public SceneConfig AddStartupSystem<T>() where T : ISystem, new()
        => AddSystem(SceneStage.StartUp, new T());
    
    public SceneConfig AddStartupSystem<T>(T system) where T : ISystem, new()
        => AddSystem(SceneStage.StartUp, system);

    public SceneConfig AddBeforeUpdateSystem<T>() where T : ISystem, new()
        => AddSystem(SceneStage.BeforeUpdate, new T());
    
    public SceneConfig AddBeforeUpdateSystem<T>(T system) where T : ISystem, new()
        => AddSystem(SceneStage.BeforeUpdate, system);
    
    public SceneConfig AddUpdateSystem<T>() where T : ISystem, new()
        => AddSystem(SceneStage.Update, new T());
    
    public SceneConfig AddUpdateSystem<T>(T system) where T : ISystem, new()
        => AddSystem(SceneStage.Update, system);
    
    public SceneConfig AddAfterUpdateSystem<T>() where T : ISystem, new()
        => AddSystem(SceneStage.AfterUpdate, new T());
    
    public SceneConfig AddAfterUpdateSystem<T>(T system) where T : ISystem, new()
        => AddSystem(SceneStage.AfterUpdate, system);

    public SceneConfig AddResource(IResource resource)
    {
        Resources.Add(resource);
        return this;
    }
}