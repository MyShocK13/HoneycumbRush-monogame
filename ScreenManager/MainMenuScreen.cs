using HoneycombRush;
using Microsoft.Xna.Framework;

namespace HoneycumbRush;

class MainMenuScreen : MenuScreen
{
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
        //int menuEntryHeight = SafeArea.Bottom - ScreenManager.ButtonBackground.Height * 2;
        int menuEntryHeight = ScreenManager.ButtonBackground.Height * 2;

        startGameMenuEntry.Position = new Vector2(
            quarterViewportWidth - ScreenManager.ButtonBackground.Width / 2,
            menuEntryHeight);
        exitMenuEntry.Position = new Vector2(
            3 * quarterViewportWidth - ScreenManager.ButtonBackground.Width / 2,
            menuEntryHeight);

        // Add entries to the menu.
        MenuEntries.Add(startGameMenuEntry);
        MenuEntries.Add(exitMenuEntry);

        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }


}
