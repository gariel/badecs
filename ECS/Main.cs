using ECS.Scene;

namespace ECS;

public abstract class Main : IDisposable
{
    private InternalGame? _game;
    
    public void Run()
    {
        _game = new InternalGame(StartScene);
        _game.Start();
    }

    public void Dispose() => _game?.Dispose();
    
    public abstract IScene StartScene { get; }
}