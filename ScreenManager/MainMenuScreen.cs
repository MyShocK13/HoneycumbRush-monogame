using HoneycombRush;
using Microsoft.Xna.Framework;
using System;

namespace HoneycumbRush;

class MainMenuScreen : MenuScreen
{
    private bool _isExiting = false;

    public MainMenuScreen()
            : base("")
    {
    }

    public override void LoadContent()
    {
        // Create our menu entries.
        MenuEntry startGameMenuEntry = new MenuEntry("Start");
        MenuEntry exitMenuEntry = new MenuEntry("Exit");

        // Calculate menu positions - we do this here since we want the screen
        // manager to be available
        int quarterViewportWidth = ScreenManager.GraphicsDevice.Viewport.Width / 4;
        int menuEntryHeight = SafeArea.Bottom - ScreenManager.ButtonBackground.Height * 2;

        startGameMenuEntry.Position = new Vector2(
            quarterViewportWidth - ScreenManager.ButtonBackground.Width / 2,
            menuEntryHeight);
        exitMenuEntry.Position = new Vector2(
            3 * quarterViewportWidth - ScreenManager.ButtonBackground.Width / 2,
            menuEntryHeight);

        // Hook up menu event handlers.
        startGameMenuEntry.Selected += StartGameMenuEntrySelected;
        exitMenuEntry.Selected += OnCancel;

        // Add entries to the menu.
        MenuEntries.Add(startGameMenuEntry);
        MenuEntries.Add(exitMenuEntry);

        base.LoadContent();
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        if (_isExiting)
        {
            _isExiting = false;
            ScreenManager.Game.Exit();
        }

        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    void StartGameMenuEntrySelected(object sender, EventArgs e)
    {
        foreach (GameScreen screen in ScreenManager.GetScreens())
        {
            screen.ExitScreen();
        }

        ScreenManager.AddScreen(new BackgroundScreen("InstructionsPC"), null);
        ScreenManager.AddScreen(new LoadingAndInstructionScreen(), null);
    }

    protected override void OnCancel(PlayerIndex playerIndex)
    {
        _isExiting = true;
    }
}
