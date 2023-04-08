using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HoneycumbRush;

public class GameplayScreen : GameScreen
{
    private DifficultyMode _gameDifficultyLevel;

    private TimeSpan _startScreenTime;


    public GameplayScreen(DifficultyMode gameDifficultyMode)
    {
        TransitionOnTime = TimeSpan.FromSeconds(0.0);
        TransitionOffTime = TimeSpan.FromSeconds(0.0);
        _startScreenTime = TimeSpan.FromSeconds(3);

        _gameDifficultyLevel = gameDifficultyMode;

        Debug.WriteLine("Gameplay screen created");
    }

    public void LoadAssets()
    {
        Debug.WriteLine("LoadAssets");
    }
}