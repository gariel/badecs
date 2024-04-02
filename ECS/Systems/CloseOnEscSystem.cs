using ECS.Resources;
using Microsoft.Xna.Framework.Input;

namespace ECS.Systems;

public class CloseOnEscSystem : ISystem
{
    public void Run(ISystemEnv env)
    {
        var keyboard = env.Resource<KeyboardResource>();
        if (keyboard.IsKeyPressed(Keys.Escape)) 
            InternalGame.Instance.Exit();
    }
}