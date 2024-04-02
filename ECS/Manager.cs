using ECS.Resources;
using ECS.Scene;
using ECS.Systems;

namespace ECS;

class Manager
{
    private SceneConfig? _config;
    private readonly TimeDelta _timeDelta = new();
    private readonly WindowResource _window = new();
    private readonly RenderResource _render = new();
    private readonly KeyboardResource _keyboard = new();
    private readonly AssetResource _assets = new();
    private SystemEnv? _env;
    private bool _startupDone;

    public void ChangeScene(SceneConfig config)
    {
        _config = config;
        _startupDone = false;
        _env = new SystemEnv(_config);
        _env.Commander.AddResource(_timeDelta);
        _env.Commander.AddResource(_window);
        _env.Commander.AddResource(_render);
        _env.Commander.AddResource(_keyboard);
        _env.Commander.AddResource(_assets);
        _config.AddAfterUpdateSystem<RenderSystem>();
        _env.LastStep();
    }

    public RenderResource Update(TimeSpan elapsed)
    {
        _render.Reset();
        _timeDelta.UpdateTimeElapsed(elapsed);
        
        if (_config is null || _env is null)
            return _render;
        
        if (!_startupDone)
        {
            _config.Systems[SceneStage.StartUp].ForEach(s => s.Run(_env));
            _startupDone = true;
        }
        else
        {
            _config.Systems[SceneStage.BeforeUpdate].ForEach(s => s.Run(_env));
            _config.Systems[SceneStage.Update].ForEach(s => s.Run(_env));
            _config.Systems[SceneStage.AfterUpdate].ForEach(s => s.Run(_env));
        }

        _env.LastStep();
        return _render;
    }
}