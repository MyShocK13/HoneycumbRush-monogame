using Microsoft.Xna.Framework;

namespace HoneycumbRush;

public abstract class GameScreen
{
    public ScreenManager ScreenManager
    {
        get { return _screenManager; }
        internal set { _screenManager = value; }
    }

    private ScreenManager _screenManager;

    public virtual void LoadContent() { }

    public virtual void UnloadContent() { }

    public virtual void Update(GameTime gameTime) 
    { 
    }

    public virtual void Draw(GameTime gameTime) { }

    //public T Load<T>(string assetName)
    //{
    //    return ScreenManager.Game.Content.Load<T>(assetName);
    //}
}
