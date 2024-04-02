namespace ECS;

public abstract class BaseQuery<TQ> where TQ : BaseQuery<TQ>
{
    protected readonly IEnumerable<Entity> FilterEntities;

    protected BaseQuery(IEnumerable<Entity> filterEntities, params Type[] firstFilters)
    {
        FilterEntities = filterEntities
            .Where(e => firstFilters.All(e.HasComponent));
    }

    public TQ With<TWith>() where TWith : IComponent
        => Sub(FilterEntities.Where(e => e.HasComponent<TWith>()));

    public TQ Without<TWith>() where TWith : IComponent
        => Sub(FilterEntities.Where(e => !e.HasComponent<TWith>()));

    public Entity Entity() => FilterEntities.First();
    public Entity[] Entities() => FilterEntities.ToArray();

    private TQ Sub(IEnumerable<Entity> entities)
        => (TQ)Activator.CreateInstance(typeof(TQ), entities)!;
}

public class Query<T> : BaseQuery<Query<T>> where T : IComponent
{
    public Query(IEnumerable<Entity> filterEntities) : base(filterEntities, typeof(T)) {}

    public T Component() => LinqComponent().First();
    public T[] Components() => LinqComponent().ToArray();
    
    private IEnumerable<T> LinqComponent()
        => FilterEntities.Select(e => e.Component<T>());
}

public class Query<T1, T2> : BaseQuery<Query<T1, T2>> where T1 : IComponent where T2 : IComponent
{
    public Query(IEnumerable<Entity> filterEntities) : base(filterEntities, typeof(T1), typeof(T2)) {}

    public (T1, T2) Component() => LinqComponent().First(); 
    public (T1, T2)[] Components() => LinqComponent().ToArray();

    private IEnumerable<(T1, T2)> LinqComponent()
        => FilterEntities.Select(e => (e.Component<T1>(), e.Component<T2>()));
}