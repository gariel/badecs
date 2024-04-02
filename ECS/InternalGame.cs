using ECS.Drawing;
using ECS.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ECS;

internal sealed class InternalGame : Game
{
    public static InternalGame Instance { get; private set; }
    public readonly GraphicsDeviceManager Graphics;
    
    private readonly IScene _initial;
    private SpriteBatch? _spriteBatch;

    private Manager? _manager;
    private List<IDrawing>? _drawings;

    // private Icons _icons;

    public InternalGame(IScene initial)
    {
        Instance = this;
        _initial = initial;
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        //IsFixedTimeStep = false; // TODO?
    }

    protected override void Initialize()
    {
        Graphics.IsFullScreen = false;
        Graphics.PreferredBackBufferWidth = 1024;
        Graphics.PreferredBackBufferHeight = 768;
        Graphics.ApplyChanges();
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // _icons = new Icons(Content.Load<Texture2D>("icons"));
    }

    protected override void Update(GameTime gameTime)
    {
        //Console.WriteLine(gameTime.ElapsedGameTime.TotalMilliseconds);
        _drawings = _manager?
            .Update(gameTime.ElapsedGameTime)
            .Drawings;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        if (_spriteBatch is not null && _drawings is not null)
        {
            _spriteBatch.Begin();
            foreach (var drawing in _drawings)
                drawing.Draw(_spriteBatch);
            _spriteBatch.End();
        }

        base.Draw(gameTime);
    }

    public void Start()
    {
        _manager = new Manager();
        
        var config = new SceneConfig(_initial);
        _manager.ChangeScene(config);
        Run();
    }
}