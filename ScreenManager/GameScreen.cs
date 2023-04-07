using Microsoft.Xna.Framework;

namespace HoneycumbRush;

public abstract class GameScreen
{
    private float _transitionPosition = 1;
    public float TransitionPosition
    {
        get { return _transitionPosition; }
        protected set { _transitionPosition = value; }
    }

    public float TransitionAlpha
    {
        get { return 1f - TransitionPosition; }
    }

    private ScreenState _screenState = ScreenState.TransitionOn;
    public ScreenState ScreenState
    {
        get { return _screenState; }
        protected set { _screenState = value; }
    }

    private bool _otherScreenHasFocus;
    public bool IsActive
    {
        get
        {
            return !_otherScreenHasFocus && (_screenState == ScreenState.Active);
        }
    }

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
