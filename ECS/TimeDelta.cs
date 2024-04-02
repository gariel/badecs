namespace ECS;

public class TimeDelta : IResource
{
    public TimeSpan Elapsed { get; private set; }

    internal void UpdateTimeElapsed(TimeSpan elapsed)
        => Elapsed = elapsed;
}