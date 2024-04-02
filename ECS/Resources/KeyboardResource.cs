using Microsoft.Xna.Framework.Input;

namespace ECS.Resources;

public class KeyboardResource : IResource
{
    public bool IsKeyPressed(Keys key)
        => Keyboard.GetState().IsKeyDown(key);
}