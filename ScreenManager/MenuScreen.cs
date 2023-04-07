using HoneycombRush;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace HoneycumbRush;

abstract class MenuScreen : GameScreen
{
    private List<MenuEntry> _menuEntries = new List<MenuEntry>();

    public int _selectedEntry = 0;
    private string _menuTitle;

    protected IList<MenuEntry> MenuEntries
    {
        get { return _menuEntries; }
    }

    public MenuScreen(string menuTitle)
    {
        _menuTitle = menuTitle;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Update each nested MenuEntry object.
        for (int i = 0; i < _menuEntries.Count; i++)
        {
            bool isSelected = IsActive && (i == _selectedEntry);

            _menuEntries[i].Update(this, isSelected, gameTime);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice graphics = ScreenManager.GraphicsDevice;
        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
        SpriteFont font = ScreenManager.Font;

        spriteBatch.Begin();

        // Draw each menu entry in turn.
        for (int i = 0; i < _menuEntries.Count; i++)
        {
            MenuEntry menuEntry = _menuEntries[i];

            bool isSelected = IsActive && (i == _selectedEntry);

            menuEntry.Draw(this, isSelected, gameTime);
        }

        // Make the menu slide into place during transitions, using a
        // power curve to make things look more interesting (this makes
        // the movement slow down as it nears the end).
        float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

        // Draw the menu title centered on the screen
        Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 375);
        Vector2 titleOrigin = font.MeasureString(_menuTitle) / 2;
        Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
        float titleScale = 1.25f;

        titlePosition.Y -= transitionOffset * 100;

        spriteBatch.DrawString(font, _menuTitle, titlePosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);

        spriteBatch.End();
    }
}
