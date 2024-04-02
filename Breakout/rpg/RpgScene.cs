using ECS;
using ECS.Components;
using ECS.Resources;
using ECS.Scene;
using Microsoft.Xna.Framework;

namespace Breakout.rpg;

class RpgScene : Main, IScene
{
    public override IScene StartScene => this;

    public void Configure(SceneConfig sc)
    {
        sc.AddStartupSystem<Setup>();
    }

    class Setup : ISystem
    {
        public void Run(ISystemEnv env)
        {
            var window = env.Resource<WindowResource>();
            window.BackgroundColor = Color.DarkViolet;
            
            var assets = env.Resource<AssetResource>();
            var iconTileSheet = assets.LoadTileSheet<IconTileSheet>();
            
            env.Commander.AddEntity(new SpriteBundle
            {
                Texture = iconTileSheet.Pawn(Color.Aqua),
                Transform = new Transform(new Vector3(0, 0, 1), new Vector2(50, 50)),
            });
        }
    }
}