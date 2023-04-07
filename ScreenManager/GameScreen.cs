using HoneycombRush;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static System.TimeZoneInfo;

namespace HoneycumbRush;

public abstract class GameScreen
{
    private bool _isPopup = false;
    public bool IsPopup
    {
        get { return _isPopup; }
        protected set { _isPopup = value; }
    }

    public Rectangle SafeArea
    {
        get
        {
            Viewport viewport = ScreenManager.Game.GraphicsDevice.Viewport;

            return viewport.TitleSafeArea;
        }
    }

    private TimeSpan _transitionOnTime = TimeSpan.Zero;
    public TimeSpan TransitionOnTime
    {
        get { return _transitionOnTime; }
        protected set { _transitionOnTime = value; }
    }

    private TimeSpan _transitionOffTime = TimeSpan.Zero;
    public TimeSpan TransitionOffTime
    {
        get { return _transitionOffTime; }
        protected set { _transitionOffTime = value; }
    }

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

    private bool _isExiting = false;
    public bool IsExiting
    {
        get { return _isExiting; }
        protected internal set { _isExiting = value; }
    }


    private bool _otherScreenHasFocus;
    public bool IsActive
    {
        get
        {
            return !_otherScreenHasFocus && (_screenState == ScreenState.Active);
        }
    }

    private ScreenManager _screenManager;
    public ScreenManager ScreenManager
    {
        get { return _screenManager; }
        internal set { _screenManager = value; }
    }

    private PlayerIndex? _controllingPlayer;
    public PlayerIndex? ControllingPlayer
    {
        get { return _controllingPlayer; }
        internal set { _controllingPlayer = value; }
    }

    public virtual void LoadContent() { }

    public virtual void UnloadContent() { }

    public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) 
    {
        _otherScreenHasFocus = otherScreenHasFocus;

        if (_isExiting)
        {
            // If the screen is going away to die, it should transition off.
            _screenState = ScreenState.TransitionOff;

            if (!UpdateTransition(gameTime, _transitionOffTime, 1))
            {
                // When the transition finishes, remove the screen.
                ScreenManager.RemoveScreen(this);
            }
        }
        else if (coveredByOtherScreen)
        {
            // If the screen is covered by another, it should transition off.
            if (UpdateTransition(gameTime, _transitionOffTime, 1))
            {
                // Still busy transitioning.
                _screenState = ScreenState.TransitionOff;
            }
            else
            {
                // Transition finished!
                _screenState = ScreenState.Hidden;
            }
        }
        else
        {
            // Otherwise the screen should transition on and become active.
            if (UpdateTransition(gameTime, _transitionOnTime, -1))
            {
                // Still busy transitioning.
                _screenState = ScreenState.TransitionOn;
            }
            else
            {
                // Transition finished!
                _screenState = ScreenState.Active;
            }
        }
    }

    private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
    {
        // How much should we move by?
        float transitionDelta;

        if (time == TimeSpan.Zero)
        {
            transitionDelta = 1;
        }
        else
        {
            transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);
        }

        // Update the transition position.
        _transitionPosition += transitionDelta * direction;

        // Did we reach the end of the transition?
        if (((direction < 0) && (_transitionPosition <= 0)) ||
            ((direction > 0) && (_transitionPosition >= 1)))
        {
            _transitionPosition = MathHelper.Clamp(_transitionPosition, 0, 1);
            return false;
        }

        // Otherwise we are still busy transitioning.
        return true;
    }

    public virtual void HandleInput(GameTime gameTime, InputState input) { }

    public virtual void Draw(GameTime gameTime) { }

    public void ExitScreen()
    {
        if (TransitionOffTime == TimeSpan.Zero)
        {
            // If the screen has a zero transition time, remove it immediately.
            ScreenManager.RemoveScreen(this);
        }
        else
        {
            // Otherwise flag that it should transition off and then exit.
            _isExiting = true;
        }
    }

    //public T Load<T>(string assetName)
    //{
    //    return ScreenManager.Game.Content.Load<T>(assetName);
    //}
}
