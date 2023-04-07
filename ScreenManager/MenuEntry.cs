using HoneycumbRush;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace HoneycombRush;

class MenuEntry
{
    private string _text;
    private float _selectionFade;
    private Vector2 _position;
    private Texture2D _buttonTexture;

    public string Text
    {
        get { return _text; }
        set { _text = value; }
    }

    public Vector2 Position
    {
        get { return _position; }
        set { _position = value; }
    }

    public Rectangle Bounds
    {
        get
        {
            return new Rectangle((int)_position.X, (int)_position.Y, _buttonTexture.Width, _buttonTexture.Height);
        }
    }

    public float Scale { get; set; }
    public float Rotation { get; set; }

    //public event EventHandler<PlayerIndexEventArgs> Selected;

    //protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
    //{
    //    if (Selected != null)
    //        Selected(this, new PlayerIndexEventArgs(playerIndex));
    //}

    public MenuEntry(string text)
    {
        _text = text;

        Scale = 1f;
        Rotation = 0f;
    }

    public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
    {
        // When the menu selection changes, entries gradually fade between
        // their selected and deselected appearance, rather than instantly
        // popping to the new state.
        float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

        if (isSelected)
        {
            _selectionFade = Math.Min(_selectionFade + fadeSpeed, 1);
        }
        else
        {
            _selectionFade = Math.Max(_selectionFade - fadeSpeed, 0);
        }
    }

    public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
    {

        Color textColor = isSelected ? Color.White : Color.Black;
        Color tintColor = isSelected ? Color.White : Color.Gray;

        // Draw text, centered on the middle of each line.
        ScreenManager screenManager = screen.ScreenManager;
        SpriteBatch spriteBatch = screenManager.SpriteBatch;
        SpriteFont font = screenManager.Font;
        _buttonTexture = screenManager.ButtonBackground;

        spriteBatch.Draw(_buttonTexture, new Vector2((int)_position.X, (int)_position.Y), tintColor);

        spriteBatch.DrawString(font, _text, getTextPosition(screen), textColor, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0);
    }

    public virtual int GetHeight(MenuScreen screen)
    {
        return (int)screen.ScreenManager.Font.MeasureString(Text).Y;
    }

    public virtual int GetWidth(MenuScreen screen)
    {
        return (int)screen.ScreenManager.Font.MeasureString(Text).X;
    }

    private Vector2 getTextPosition(MenuScreen screen)
    {
        Vector2 textPosition = Vector2.Zero;
        if (Scale == 1f)
        {
            textPosition = new Vector2(
                (int)_position.X + _buttonTexture.Width / 2 - GetWidth(screen) / 2,
                (int)_position.Y);
        }
        else
        {
            textPosition = new Vector2(
                (int)_position.X + (_buttonTexture.Width / 2 - ((GetWidth(screen) / 2) * Scale)),
                (int)_position.Y + (GetHeight(screen) - GetHeight(screen) * Scale) / 2);
        }

        return textPosition;
    }
}
