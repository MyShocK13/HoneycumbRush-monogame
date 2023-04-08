﻿using HoneycumbRush;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace HoneycombRush;

public class GameplayScreen : GameScreen
{
    private const string SmokeText = "Smoke";

    private Dictionary<string, ScaledAnimation> _animations;

    private SpriteFont _font16px;
    private SpriteFont _font36px;
    private Texture2D _arrowTexture;
    private Texture2D _background;
    private Texture2D _controlstickBoundary;
    private Texture2D _controlstick;
    private Texture2D _beehiveTexture;
    private Texture2D _smokeButton;
    private Vector2 _smokeButtonPosition;
    private Vector2 _smokeTextLocation;

    private DifficultyMode _gameDifficultyLevel;

    private bool _isAtStartupCountDown;
    private bool _levelEnded;
    private TimeSpan _startScreenTime;


    //        ScoreBar smokeButtonScorebar;

    //        Vector2 controlstickStartupPosition;
    //        Vector2 controlstickBoundaryPosition;
    //        Vector2 lastTouchPosition;
    //        /// <summary>
    //        /// A vector describing the movement direction according to the current user input.
    //        /// </summary>
    //        Vector2 movementVector;
    //        Vector2 vatArrowPosition;

    //        bool isSmokebuttonClicked;
    //        bool drawArrow;
    //        bool drawArrowInInterval;
    //        bool isInMotion;
    //        bool isLevelEnd;
    //        bool isUserWon;
    //        bool userInputToExit;


    //        int amountOfSoldierBee;
    //        int amountOfWorkerBee;
    //        int arrowCounter;

    //        List<Beehive> beehives = new List<Beehive>();
    //        List<Bee> bees = new List<Bee>();


    //        TimeSpan gameElapsed;

    //        BeeKeeper beeKeeper;
    //        HoneyJar jar;
    //        Vat vat;


    public bool IsStarted
    {
        get
        {
            return !_isAtStartupCountDown && !_levelEnded;
        }
    }

    //        private bool IsInMotion
    //        {
    //            get
    //            {
    //                return isInMotion;
    //            }
    //            set
    //            {
    //                isInMotion = value;
    //                if (beeKeeper != null)
    //                {
    //                    beeKeeper.IsInMotion = isInMotion;
    //                }
    //            }
    //        }

    //        DebugSystem debugSystem;
    //        bool showDebugInfo = false;
    //        Rectangle deviceUpperRightCorner = new Rectangle(750, 0, 50, 50);

    //        public static int FinalScore;


    //        #endregion

    //        #region Initializations

    public GameplayScreen(DifficultyMode gameDifficultyMode)
    {
        TransitionOnTime = TimeSpan.FromSeconds(0.0);
        TransitionOffTime = TimeSpan.FromSeconds(0.0);
        _startScreenTime = TimeSpan.FromSeconds(3);

        //            //Loads configuration

        //            //var config =   Path.Combine(NSBundle.MainBundle.ResourcePath,"Content/Configuration/Configuration.xml");
        //            ConfigurationManager.LoadConfiguration(XDocument.Load("Content/Configuration/Configuration.xml"));
        //            //ConfigurationManager.LoadConfiguration(XDocument.Load(config));

        //            ConfigurationManager.DifficultyMode = gameDifficultyMode;

        _gameDifficultyLevel = gameDifficultyMode;
        //            gameElapsed = ConfigurationManager.ModesConfiguration[gameDifficultyLevel].GameElapsed;

        //            amountOfSoldierBee = 4;
        //            amountOfWorkerBee = 16;

        //            controlstickBoundaryPosition = new Vector2(34, 347);
        _smokeButtonPosition = new Vector2(664, 346);
        //            controlstickStartupPosition = new Vector2(55, 369);

        //            IsInMotion = false;
        _isAtStartupCountDown = true;
        //            isLevelEnd = false;

        //            EnabledGestures = GestureType.Tap;
    }


    //        #endregion

    //        #region Loading and Unloading

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public void LoadAssets()
    {
        // Loads the animation dictionary from an xml file
        _animations = new Dictionary<string, ScaledAnimation>();
        LoadAnimationFromXML();

        // Loads all textures that are required
        LoadTextures();

        // Create all game components
        CreateGameComponents();
    }

    //        /// <summary>
    //        /// Unloads game components which are no longer needed once the game ends.
    //        /// </summary>
    //        public override void UnloadContent()
    //        {
    //            var componentList = ScreenManager.Game.Components;

    //            for (int index = 0; index < componentList.Count; index++)
    //            {
    //                if (componentList[index] is TexturedDrawableGameComponent || componentList[index] is ScoreBar)
    //                {
    //                    componentList.RemoveAt(index);
    //                    index--;
    //                }
    //            }

    //            base.UnloadContent();
    //        }


    //        #endregion

    //        #region Update


    //        /// <summary>
    //        /// Handle the player's input.
    //        /// </summary>
    //        /// <param name="input"></param>
    //        public override void HandleInput(GameTime gameTime, InputState input)
    //        {
    //            if (IsActive)
    //            {
    //                if (input == null)
    //                {
    //                    throw new ArgumentNullException("input");
    //                }

    //                if (input.IsPauseGame(null))
    //                {
    //                    PauseCurrentGame();
    //                }
    //            }

    //            if (input.TouchState.Count > 0)
    //            {
    //                foreach (TouchLocation touch in input.TouchState)
    //                {
    //                    lastTouchPosition = touch.Position;
    //                }
    //            }

    //            isSmokebuttonClicked = false;

    //            PlayerIndex player;

    //            VirtualThumbsticks.Update(input);

    //            if (input.Gestures.Count > 0)
    //            {
    //                GestureSample topGesture = input.Gestures[0];

    //                if (topGesture.GestureType == GestureType.Tap &&
    //                    deviceUpperRightCorner.Contains(new Point((int)topGesture.Position.X, (int)topGesture.Position.Y)))
    //                {
    //                    showDebugInfo = !showDebugInfo;
    //                }
    //            }

    //            if (isLevelEnd)
    //            {
    //                if (input.Gestures.Count > 0)
    //                {
    //                    if (input.Gestures[0].GestureType == GestureType.Tap)
    //                    {
    //                        userInputToExit = true;
    //                    }
    //                }

    //                if (input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out player) ||
    //                    input.IsNewKeyPress(Keys.Space, ControllingPlayer, out player))
    //                {
    //                    userInputToExit = true;
    //                }
    //            }

    //            if (!IsStarted)
    //            {
    //                return;
    //            }

    //            // If there was any touch
    //            if (VirtualThumbsticks.RightThumbstickCenter.HasValue)
    //            {
    //                // Button Bounds
    //                Rectangle buttonRectangle = new Rectangle((int)smokeButtonPosition.X, (int)smokeButtonPosition.Y,
    //                                                            smokeButton.Width / 2, smokeButton.Height);

    //                // Touch Bounds
    //                Rectangle touchRectangle = new Rectangle((int)VirtualThumbsticks.RightThumbstickCenter.Value.X,
    //                                                        (int)VirtualThumbsticks.RightThumbstickCenter.Value.Y,
    //                                                        1, 1);
    //                // If the touch is in the button
    //                if (buttonRectangle.Contains(touchRectangle) && !beeKeeper.IsCollectingHoney && !beeKeeper.IsStung)
    //                {
    //                    isSmokebuttonClicked = true;
    //                }
    //            }

    //            // Handle keyboard
    //            if (input.IsNewKeyPress(Keys.Y, ControllingPlayer, out player))
    //            {
    //                showDebugInfo = !showDebugInfo;
    //            }
    //            if ((input.IsKeyDown(Keys.Space, ControllingPlayer, out player) ||
    //                  input.IsNewMouseClick(InputState.MouseButton.Right, ControllingPlayer, out player))
    //                && !beeKeeper.IsCollectingHoney
    //                && !beeKeeper.IsStung)
    //            {
    //                isSmokebuttonClicked = true;
    //            }

    //            movementVector = SetMotion(input);
    //            beeKeeper.SetDirection(movementVector);
    //        }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        // When the game starts the first thing the user sees is the count down before the game actually begins
        if (_isAtStartupCountDown)
        {
            _startScreenTime -= gameTime.ElapsedGameTime;
        }

        //            // Check for and handle a game over
        //            if (CheckIfCurrentGameFinished())
        //            {
        //                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        //                return;
        //            }

        //            if (!(IsActive && IsStarted))
        //            {
        //                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        //                return;
        //            }

        //            // Show all diagnostic counters
        //            debugSystem.FpsCounter.Visible = showDebugInfo;
        //            debugSystem.TimeRuler.Visible = showDebugInfo;
        //            debugSystem.TimeRuler.ShowLog = showDebugInfo;

        //            gameElapsed -= gameTime.ElapsedGameTime;

        //#if WINDOWS_PHONE
        //            HandleThumbStick();
        //#endif

        //            HandleSmoke();

        //            HandleCollision(gameTime);

        //            HandleVatHoneyArrow();

        //            beeKeeper.DrawOrder = 1;
        //            int beeKeeperY = (int)(beeKeeper.Position.Y + beeKeeper.Bounds.Height - 2);

        //            // We want to determine the draw order of the beekeeper,
        //            // if the beekeeper is under half the height of the beehive 
        //            // it should be drawn over the beehive.
        //            foreach (Beehive beehive in beehives)
        //            {
        //                if (beeKeeperY > beehive.Bounds.Y)
        //                {
        //                    if (beehive.Bounds.Y + beehive.Bounds.Height / 2 < beeKeeperY)
        //                    {
        //                        beeKeeper.DrawOrder = Math.Max(beeKeeper.DrawOrder, beehive.Bounds.Y + 1);
        //                    }
        //                }
        //            }

        //            if (gameElapsed.Minutes == 0 && gameElapsed.Seconds == 10)
        //            {
        //                AudioManager.PlaySound("10SecondCountDown");
        //            }
        //            if (gameElapsed.Minutes == 0 && gameElapsed.Seconds == 30)
        //            {
        //                AudioManager.PlaySound("30SecondWarning");
        //            }

        //            // Update the time remaining displayed on the vat
        //            vat.DrawTimeLeft(gameElapsed);

        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    public override void Draw(GameTime gameTime)
    {
        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

        spriteBatch.Begin();

        Rectangle bounds = ScreenManager.GraphicsDevice.Viewport.Bounds;
        spriteBatch.Draw(_background, bounds, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);


        // Draw count down screen
        if (_isAtStartupCountDown)
        {
            DrawStartupString();
        }

        if (IsActive && IsStarted)
        {
            DrawSmokeButton();

            spriteBatch.DrawString(_font16px, SmokeText, _smokeTextLocation, Color.White);

            //                DrawVatHoneyArrow();
        }

        //            DrawLevelEndIfNecessary();

        spriteBatch.End();

        base.Draw(gameTime);
    }


    //        #endregion

    //        #region Private Methods


    //        /// <summary>
    //        /// If the level is over, draws text describing the level's outcome.
    //        /// </summary>
    //        private void DrawLevelEndIfNecessary()
    //        {
    //            if (isLevelEnd)
    //            {
    //                string stringToDisplay = string.Empty;

    //                if (isUserWon)
    //                {
    //                    if (FinalScore != 0 && HighScoreScreen.IsInHighscores(FinalScore))
    //                    {
    //                        stringToDisplay = "It's a new\nHigh-Score!";
    //                    }
    //                    else
    //                    {
    //                        stringToDisplay = "You Win!";
    //                    }
    //                }
    //                else
    //                {
    //                    stringToDisplay = "Time Is Up!";
    //                }

    //                Vector2 stringVector = font36px.MeasureString(stringToDisplay) * ScreenManager.SpriteBatch.ScaleVector;

    //                ScreenManager.SpriteBatch.DrawString(font36px, stringToDisplay,
    //                                new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - stringVector.X / 2,
    //                                            ScreenManager.GraphicsDevice.Viewport.Height / 2 - stringVector.Y / 2),
    //                                Color.White);
    //            }
    //        }

    //        /// <summary>
    //        /// Advances to the next screen based on the current difficulty and whether or not the user has won.
    //        /// </summary>
    //        /// <param name="isWon">Whether or not the user has won the current level.</param>
    //        private void MoveToNextScreen(bool isWon)
    //        {
    //            ScreenManager.AddScreen(new BackgroundScreen("pauseBackground"), null);

    //            if (isWon)
    //            {
    //                switch (gameDifficultyLevel)
    //                {
    //                    case DifficultyMode.Easy:
    //                    case DifficultyMode.Medium:
    //                        ScreenManager.AddScreen(
    //                                new LevelOverScreen("You Finished Level: " + gameDifficultyLevel.ToString(),
    //                                    ++gameDifficultyLevel), null);
    //                        break;
    //                    case DifficultyMode.Hard:
    //                        ScreenManager.AddScreen(new LevelOverScreen("You Win", null), null);
    //                        break;
    //                }
    //            }
    //            else
    //            {
    //                ScreenManager.AddScreen(new LevelOverScreen("You Lose", null), null);
    //            }

    //            AudioManager.StopMusic();
    //            AudioManager.StopSound("BeeBuzzing_Loop");
    //        }

    //        /// <summary>
    //        /// Pause the game.
    //        /// </summary>
    //        private void PauseCurrentGame()
    //        {
    //            debugSystem.FpsCounter.Visible = false;
    //            debugSystem.TimeRuler.Visible = false;
    //            debugSystem.TimeRuler.ShowLog = false;

    //            // Pause sounds
    //            AudioManager.PauseResumeSounds(false);

    //            // Set pause screen
    //            ScreenManager.AddScreen(new BackgroundScreen("pauseBackground"), null);
    //            ScreenManager.AddScreen(new PauseScreen(), null);
    //        }

    /// <summary>
    /// Loads animation settings from an xml file.
    /// </summary>
    private void LoadAnimationFromXML()
    {
        XDocument doc = XDocument.Load("Content/Textures/AnimationsDefinition.xml");
        XName name = XName.Get("Definition");
        var definitions = doc.Document.Descendants(name);

        // Loop over all definitions in the XML
        foreach (XElement animationDefinition in definitions)
        {
            // Get the name of the animation
            string animationAlias = animationDefinition.Attribute("Alias").Value;
            Texture2D texture = ScreenManager.Game.Content.Load<Texture2D>(animationDefinition.Attribute("SheetName").Value);

            // Get the frame size (width & height)
            Point frameSize = new Point();
            frameSize.X = int.Parse(animationDefinition.Attribute("FrameWidth").Value);
            frameSize.Y = int.Parse(animationDefinition.Attribute("FrameHeight").Value);

            // Get the frames sheet dimensions
            Point sheetSize = new Point();
            sheetSize.X = int.Parse(animationDefinition.Attribute("SheetColumns").Value);
            sheetSize.Y = int.Parse(animationDefinition.Attribute("SheetRows").Value);

            ScaledAnimation animation = new ScaledAnimation(texture, frameSize, sheetSize);

            // Checks for sub-animation definition
            if (animationDefinition.Element("SubDefinition") != null)
            {
                int startFrame = int.Parse(animationDefinition.Element("SubDefinition").Attribute("StartFrame").Value);

                int endFrame = int.Parse(animationDefinition.Element("SubDefinition").Attribute("EndFrame").Value);

                animation.SetSubAnimation(startFrame, endFrame);
            }

            if (animationDefinition.Attribute("Speed") != null)
            {
                animation.SetFrameInterval(TimeSpan.FromMilliseconds(
                    double.Parse(animationDefinition.Attribute("Speed").Value)));
            }

            // If the definition has an offset defined - it should be  
            // rendered relative to some element/animation

            if (null != animationDefinition.Attribute("OffsetX") &&
                null != animationDefinition.Attribute("OffsetY"))
            {
                animation.Offset = new Vector2(int.Parse(animationDefinition.Attribute("OffsetX").Value),
                    int.Parse(animationDefinition.Attribute("OffsetY").Value));
            }

            _animations.Add(animationAlias, animation);
        }
    }

    /// <summary>
    /// Create all the game components.
    /// </summary>
    private void CreateGameComponents()
    {
        Debug.WriteLine("Create Game Components");

        Rectangle safeArea = SafeArea;

        //Texture2D jarTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/honeyJar");
        //Vector2 honeyJarLocation = safeArea.GetVector() + new Vector2(UIConstants.HoneyJarLeftMargin, UIConstants.HoneyJarTopMargin);

        //            Vector2 jarBarLocation = honeyJarLocation + new Vector2(0, jarTexture.Height * scaleVector.Y + 7);

        //            ScoreBar scoreBar = new ScoreBar(ScreenManager.Game, 0, 100, jarBarLocation,
        //                (int)(jarTexture.Height / 6 * scaleVector.Y), (int)(jarTexture.Width * scaleVector.X), Color.Blue,
        //                ScoreBar.ScoreBarOrientation.Horizontal, 0, this, true);
        //            ScreenManager.Game.Components.Add(scoreBar);

        //            // Create the honey jar
        //            jar = new HoneyJar(ScreenManager.Game, this, honeyJarLocation, scoreBar);
        //            ScreenManager.Game.Components.Add(jar);

        //            // Create all the beehives and the bees
        //            CreateBeehives(safeArea, jar);

        // We only initialize the smoke button position here since we need access
        // to the screen manager in order to do so (and it is null in the 
        // constructor)
        _smokeButtonPosition = new Vector2(
            safeArea.Right - UIConstants.SmokeButtonRightAbsoluteMargin,
            safeArea.Bottom - UIConstants.SmokeButtonBottomAbsoluteMargin);

        //            // Create the smoke gun's score bar
        //            int totalSmokeAmount = ConfigurationManager.ModesConfiguration[gameDifficultyLevel].TotalSmokeAmount;
        //            Vector2 smokeBarLocation = smokeButtonPosition +
        //                new Vector2(UIConstants.SmokeButtonSize * scaleVector.X / 8,
        //                UIConstants.SmokeButtonSize * scaleVector.Y);

        //            smokeButtonScorebar = new ScoreBar(ScreenManager.Game, 0, totalSmokeAmount,
        //                smokeBarLocation, (int)(UIConstants.SmokeButtonSize * scaleVector.X / 10),
        //                (int)(UIConstants.SmokeButtonSize * scaleVector.Y * 3 / 4), Color.White,
        //                ScoreBar.ScoreBarOrientation.Horizontal, totalSmokeAmount, this, false);

        _smokeTextLocation = _smokeButtonPosition + new Vector2(
            UIConstants.SmokeButtonSize / 2 - _font16px.MeasureString(SmokeText).X / 2,
            UIConstants.SmokeButtonSize * 11 / 10);

        //            ScreenManager.Game.Components.Add(smokeButtonScorebar);

        //            // Creates the BeeKeeper
        //            beeKeeper = new BeeKeeper(ScreenManager.Game, this);
        //            beeKeeper.AnimationDefinitions = animations;
        //            beeKeeper.ThumbStickArea = new Rectangle((int)controlstickBoundaryPosition.X,
        //                        (int)controlstickBoundaryPosition.Y, controlstickBoundary.Width, controlstickBoundary.Height);
        //            ScreenManager.Game.Components.Add(beeKeeper);

        //            // Creates the vat
        //            Texture2D vatTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/vat");

        //            Vector2 vatLocation = new Vector2(safeArea.Center.X - vatTexture.Width * scaleVector.X / 2,
        //                safeArea.Bottom - vatTexture.Height * scaleVector.Y - UIConstants.VatBottomMargin);
        //            Vector2 vatScorebarLocation = vatLocation +
        //                new Vector2((vatTexture.Width * scaleVector.X - UIConstants.VatScorebarWidth) / 2,
        //            vatTexture.Height * scaleVector.Y * 7 / 10);

        //            scoreBar = new ScoreBar(ScreenManager.Game, 0, 300, vatScorebarLocation, UIConstants.VatScorebarHeight,
        //                UIConstants.VatScorebarWidth, Color.White, ScoreBar.ScoreBarOrientation.Horizontal, 0, this, true);

        //            vat = new Vat(ScreenManager.Game, this, vatTexture, vatLocation, scoreBar);
        //            ScreenManager.Game.Components.Add(vat);

        //            vatArrowPosition =
        //                vatLocation + new Vector2(vatTexture.Width * scaleVector.X / 2 -
        //                    arrowTexture.Width * scaleVector.X / 2, UIConstants.VatArrowOffset);

        //            ScreenManager.Game.Components.Add(scoreBar);

        //            scoreBar.DrawOrder = vat.DrawOrder + 1;
    }

    //        /// <summary>
    //        /// Creates all the beehives and bees.
    //        /// </summary>
    //        private void CreateBeehives(Rectangle safeArea, HoneyJar jar)
    //        {
    //            Vector2 scaleVector = ScreenManager.SpriteBatch.ScaleVector;

    //            // Init position parameters
    //            Vector2 scorebarPosition =
    //                new Vector2(beehiveTexture.Width * scaleVector.X / 4, beehiveTexture.Height * scaleVector.Y * 9 / 10);

    //            Vector2[] beehivePositions = new Vector2[5]
    //            {
    //                // top left
    //                new Vector2(safeArea.Left + UIConstants.BeehiveLeftMargin,
    //                    safeArea.Top + UIConstants.BeehiveTopMargin),
    //                // top middle
    //                new Vector2(safeArea.Center.X - beehiveTexture.Width * scaleVector.X / 2,
    //                    safeArea.Top + UIConstants.BeehiveTopMargin),
    //                // top right
    //                new Vector2(safeArea.Right - beehiveTexture.Width * scaleVector.X - UIConstants.BeehiveRightMargin,
    //                    safeArea.Top + UIConstants.BeehiveTopMargin),
    //                // left
    //                new Vector2(safeArea.Left + UIConstants.BeehiveLeftMargin,
    //                    safeArea.Center.Y - beehiveTexture.Height * scaleVector.Y / 2 + UIConstants.BeehiveMiddleOffset),
    //                // right
    //                new Vector2(safeArea.Right - beehiveTexture.Width * scaleVector.X - UIConstants.BeehiveRightMargin,
    //                    safeArea.Center.Y - beehiveTexture.Height * scaleVector.Y / 2  + UIConstants.BeehiveMiddleOffset)
    //                };

    //            // Create the beehives
    //            for (int beehiveCounter = 0; beehiveCounter < beehivePositions.Length; beehiveCounter++)
    //            {
    //                ScoreBar scoreBar = new ScoreBar(ScreenManager.Game, 0, 100, beehivePositions[beehiveCounter] +
    //                                        scorebarPosition, (int)(beehiveTexture.Height * scaleVector.Y / 10),
    //                                        (int)(beehiveTexture.Width * scaleVector.X / 2), Color.Green,
    //                                        ScoreBar.ScoreBarOrientation.Horizontal, 100, this, false);
    //                ScreenManager.Game.Components.Add(scoreBar);

    //                Beehive beehive =
    //                    new Beehive(ScreenManager.Game, this, beehiveTexture, scoreBar, beehivePositions[beehiveCounter]);

    //                beehive.AnimationDefinitions = animations;

    //                ScreenManager.Game.Components.Add(beehive);
    //                beehives.Add(beehive);
    //                scoreBar.DrawOrder = beehive.DrawOrder;
    //            }

    //            for (int beehiveIndex = 0; beehiveIndex < beehivePositions.Length; beehiveIndex++)
    //            {
    //                // Create the Soldier bees
    //                for (int SoldierBeeCounter = 0; SoldierBeeCounter < amountOfSoldierBee; SoldierBeeCounter++)
    //                {
    //                    SoldierBee bee = new SoldierBee(ScreenManager.Game, this, beehives[beehiveIndex]);
    //                    bee.AnimationDefinitions = animations;
    //                    ScreenManager.Game.Components.Add(bee);
    //                    bees.Add(bee);
    //                }

    //                // Creates the worker bees
    //                for (int workerBeeCounter = 0; workerBeeCounter < amountOfWorkerBee; workerBeeCounter++)
    //                {
    //                    WorkerBee bee = new WorkerBee(ScreenManager.Game, this, beehives[beehiveIndex]);
    //                    bee.AnimationDefinitions = animations;
    //                    ScreenManager.Game.Components.Add(bee);
    //                    bees.Add(bee);
    //                }
    //            }
    //        }

    /// <summary>
    /// Loads all the necessary textures.
    /// </summary>
    private void LoadTextures()
    {
        _font16px = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/GameScreenFont16px");
        _font36px = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/GameScreenFont36px");
        
        _beehiveTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/beehive");
        _smokeButton = ScreenManager.Game.Content.Load<Texture2D>("Textures/smokeBtn");
        _arrowTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/arrow");

        _background = ScreenManager.Game.Content.Load<Texture2D>("Textures/Backgrounds/GamePlayBackground");
        
        _controlstickBoundary = ScreenManager.Game.Content.Load<Texture2D>("Textures/controlstickBoundary");
        _controlstick = ScreenManager.Game.Content.Load<Texture2D>("Textures/controlstick");
    }

    //#if WINDOWS_PHONE
    //        /// <summary>
    //        /// Handle ThumbStick logic
    //        /// </summary>
    //        private void HandleThumbStick()
    //        {
    //            // Calculate the rectangle of the outer circle of the thumbstick
    //            Rectangle outerControlstick = new Rectangle(0, (int)controlstickBoundaryPosition.Y - 35,
    //                controlstickBoundary.Width + 60, controlstickBoundary.Height + 60);

    //            // Reset the thumbstick position when it is idle
    //            if (VirtualThumbsticks.LeftThumbstick == Vector2.Zero)
    //            {
    //                IsInMotion = false;
    //                controlstickStartupPosition = new Vector2(55, 369);
    //            }
    //            else
    //            {
    //                // If not in motion and the touch point is not in the control bounds - there is no movement
    //                Rectangle touchRectangle = new Rectangle((int)lastTouchPosition.X, (int)lastTouchPosition.Y, 1, 1);

    //                if (!outerControlstick.Contains(touchRectangle))
    //                {
    //                    controlstickStartupPosition = new Vector2(55, 369);
    //                    IsInMotion = false;
    //                    return;
    //                }

    //                // Moves the thumbstick's inner circle
    //                float radius = controlstick.Width / 2 + 35;
    //                controlstickStartupPosition = new Vector2(55, 369) + (VirtualThumbsticks.LeftThumbstick * radius);
    //            }
    //        }
    //#endif

    //        /// <summary>
    //        /// Moves the beekeeper.
    //        /// </summary>
    //        /// <returns>Returns a vector indicating the beekeeper's movement direction.
    //        /// </returns>
    //        private Vector2 SetMotion(InputState input)
    //        {
    //            // Calculate the beekeeper location, if allow moving
    //            Rectangle safeArea = SafeArea;

    //            PlayerIndex playerIndex;

    //            Vector2 leftThumbstick = VirtualThumbsticks.LeftThumbstick;

    //            // Move on to keyboard input if we still have nothing
    //            if (leftThumbstick == Vector2.Zero)
    //            {
    //                float vecX = 0;
    //                float vecY = 0;

    //                if (input.IsKeyDown(Keys.Left, ControllingPlayer, out playerIndex))
    //                {
    //                    vecX--;
    //                }
    //                if (input.IsKeyDown(Keys.Right, ControllingPlayer, out playerIndex))
    //                {
    //                    vecX++;
    //                }
    //                if (input.IsKeyDown(Keys.Up, ControllingPlayer, out playerIndex))
    //                {
    //                    vecY--;
    //                }
    //                if (input.IsKeyDown(Keys.Down, ControllingPlayer, out playerIndex))
    //                {
    //                    vecY++;
    //                }
    //                if (input.IsMouseDown(InputState.MouseButton.Left, ControllingPlayer, out playerIndex))
    //                {
    //                    vecX = input.CurrentMouseStates[(int)playerIndex].X - beeKeeper.Bounds.X;
    //                    vecY = input.CurrentMouseStates[(int)playerIndex].Y - beeKeeper.Bounds.Y;
    //                }
    //                leftThumbstick = new Vector2(vecX, vecY);
    //                leftThumbstick.Normalize();
    //            }

    //            Vector2 movementVector = leftThumbstick * 12f * ScreenManager.SpriteBatch.ScaleVector;

    //            Rectangle futureBounds = beeKeeper.Bounds;
    //            futureBounds.X += (int)movementVector.X;
    //            futureBounds.Y += (int)movementVector.Y;

    //            if (futureBounds.Left <= safeArea.Left || futureBounds.Right >= safeArea.Right)
    //            {
    //                movementVector.X = 0;
    //            }
    //            if (futureBounds.Top <= safeArea.Top || futureBounds.Bottom >= safeArea.Bottom)
    //            {
    //                movementVector.Y = 0;
    //            }

    //            if (movementVector == Vector2.Zero)
    //            {
    //                IsInMotion = false;
    //                beeKeeper.SetMovement(Vector2.Zero);
    //            }
    //            else
    //            {
    //                Vector2 beekeeperCalculatedPosition =
    //                    new Vector2(beeKeeper.CentralCollisionArea.X, beeKeeper.CentralCollisionArea.Y) + movementVector;

    //                if (!CheckBeehiveCollision(beekeeperCalculatedPosition))
    //                {
    //                    beeKeeper.SetMovement(movementVector);
    //                    IsInMotion = true;
    //                }
    //            }

    //            return movementVector;
    //        }

    //        /// <summary>
    //        /// Checks if the beekeeper collides with a beehive.
    //        /// </summary>
    //        /// <param name="beekeeperPosition">The beekeeper's position.</param>
    //        /// <returns>True if the beekeeper collides with a beehive and false otherwise.</returns>
    //        private bool CheckBeehiveCollision(Vector2 beekeeperPosition)
    //        {
    //            // We do not use the beekeeper's collision area property as he has not actually moved at this point and
    //            // is still in his previous position
    //            Rectangle beekeeperTempCollisionArea = new Rectangle((int)beekeeperPosition.X, (int)beekeeperPosition.Y,
    //                beeKeeper.CentralCollisionArea.Width, beeKeeper.CentralCollisionArea.Height);

    //            foreach (Beehive currentBeehive in beehives)
    //            {
    //                if (beekeeperTempCollisionArea.Intersects(currentBeehive.CentralCollisionArea))
    //                {
    //                    return true;
    //                }
    //            }

    //            return false;
    //        }

    //        /// <summary>
    //        /// Check for any of the possible collisions.
    //        /// </summary>
    //        /// <param name="gameTime">Game time information.</param>
    //        private void HandleCollision(GameTime gameTime)
    //        {
    //            bool isCollectingHoney = HandleBeeKeeperBeehiveCollision();

    //            HandleSmokeBeehiveCollision();

    //            bool hasCollisionWithVat = HandleVatCollision();

    //            HandleBeeInteractions(gameTime, hasCollisionWithVat, isCollectingHoney);
    //        }

    //        /// <summary>
    //        /// Handle the interaction of the bees with other game components.
    //        /// </summary>
    //        /// <param name="gameTime">Game time information.</param>
    //        /// <param name="isBeeKeeperCollideWithVat">Whether the beekeeper is currently touching the vat.</param>
    //        /// <param name="isBeeKeeperCollideWithBeehive">Whether the beekeeper is currently touching a beehive.</param>
    //        private void HandleBeeInteractions(GameTime gameTime, bool isBeeKeeperCollideWithVat,
    //            bool isBeeKeeperCollideWithBeehive)
    //        {
    //            // Goes over all the bees
    //            foreach (Bee bee in bees)
    //            {
    //                // Check for smoke collisions
    //                SmokePuff intersectingPuff = beeKeeper.CheckSmokeCollision(bee.Bounds);

    //                if (intersectingPuff != null)
    //                {
    //                    bee.HitBySmoke(intersectingPuff);
    //                }

    //                // Check for vat collision
    //                if (vat.Bounds.HasCollision(bee.Bounds))
    //                {
    //                    bee.Collide(vat.Bounds);
    //                }
    //                // Check for beekeeper collision
    //                if (beeKeeper.Bounds.HasCollision(bee.Bounds))
    //                {
    //                    if (!bee.IsBeeHit && !isBeeKeeperCollideWithVat && !beeKeeper.IsStung && !beeKeeper.IsFlashing &&
    //                        !isBeeKeeperCollideWithBeehive)
    //                    {
    //                        jar.DecreaseHoneyByPercent(20);
    //                        beeKeeper.Stung(gameTime.TotalGameTime);
    //                        AudioManager.PlaySound("HoneyPotBreak");
    //                        AudioManager.PlaySound("Stung");
    //                    }

    //                    bee.Collide(beeKeeper.Bounds);
    //                }
    //                // Soldier bee chase logic
    //                if (bee is SoldierBee)
    //                {
    //                    SoldierBee SoldierBee = bee as SoldierBee;
    //                    SoldierBee.DistanceFromBeeKeeper =
    //                        (Vector2.Distance(beeKeeper.Bounds.GetVector(), SoldierBee.Bounds.GetVector()));

    //                    SoldierBee.BeeKeeperVector = beeKeeper.Bounds.GetVector() - SoldierBee.Bounds.GetVector();
    //                }
    //            }
    //        }

    //        /// <summary>
    //        /// Handle the beekeeper's collision with the vat component.
    //        /// </summary>
    //        /// <returns>True if the beekeeper collides with the vat and false otherwise.</returns>
    //        private bool HandleVatCollision()
    //        {
    //            if (beeKeeper.Bounds.HasCollision(vat.VatDepositArea))
    //            {
    //                if (jar.HasHoney && !beeKeeper.IsStung && !beeKeeper.IsDepositingHoney &&
    //                    movementVector == Vector2.Zero)
    //                {
    //                    beeKeeper.StartTransferHoney(4, EndHoneyDeposit);
    //                }

    //                return true;
    //            }

    //            beeKeeper.EndTransferHoney();
    //            return false;
    //        }

    //        /// <summary>
    //        /// Handler for finalizing the honey deposit to the vat.
    //        /// </summary>
    //        /// <param name="result"></param>
    //        public void EndHoneyDeposit(IAsyncResult result)
    //        {
    //            int HoneyAmount = jar.DecreaseHoneyByPercent(100);
    //            vat.IncreaseHoney(HoneyAmount);
    //            AudioManager.StopSound("DepositingIntoVat_Loop");
    //        }

    //        /// <summary>
    //        /// Handle the beekeeper's collision with beehive components.
    //        /// </summary>
    //        /// <returns>True if the beekeeper collides with a beehive and false otherwise.</returns>
    //        /// <remarks>This method is also responsible for allowing bees to regenerate when the beekeeper is not
    //        /// intersecting with a specific hive.</remarks>
    //        private bool HandleBeeKeeperBeehiveCollision()
    //        {
    //            bool isCollidingWithBeehive = false;

    //            Beehive collidedBeehive = null;

    //            // Goes over all the beehives
    //            foreach (Beehive beehive in beehives)
    //            {
    //                // If the beekeeper intersects with the beehive
    //                if (beeKeeper.Bounds.HasCollision(beehive.Bounds))
    //                {
    //                    if (movementVector == Vector2.Zero)
    //                    {
    //                        collidedBeehive = beehive;
    //                        isCollidingWithBeehive = true;
    //                    }
    //                }
    //                else
    //                {
    //                    beehive.AllowBeesToGenerate = true;
    //                }
    //            }

    //            if (collidedBeehive != null)
    //            {
    //                // The beehive has honey, the jar can carry more honey, and the beekeeper is not stung
    //                if (collidedBeehive.HasHoney && jar.CanCarryMore && !beeKeeper.IsStung)
    //                {
    //                    // Take honey from the beehive and put it in the jar
    //                    collidedBeehive.DecreaseHoney(1);
    //                    jar.IncreaseHoney(1);
    //                    beeKeeper.IsCollectingHoney = true;
    //                    AudioManager.PlaySound("FillingHoneyPot_Loop");
    //                }
    //                else
    //                {
    //                    beeKeeper.IsCollectingHoney = false;
    //                }

    //                // Bees are not allowed to regenerate while the beekeeper is colliding with their beehive
    //                isCollidingWithBeehive = true;
    //                collidedBeehive.AllowBeesToGenerate = false;
    //            }
    //            else
    //            {
    //                beeKeeper.IsCollectingHoney = false;
    //                AudioManager.StopSound("FillingHoneyPot_Loop");
    //            }

    //            return isCollidingWithBeehive;
    //        }

    //        /// <summary>
    //        /// Handle the smoke puff collision with beehive components.
    //        /// </summary>
    //        /// <remarks>Only disables bee regeneration, as it assumes that it will be enabled by 
    //        /// <see cref="HandleBeeKeeperBeehiveCollision"/></remarks>
    //        private void HandleSmokeBeehiveCollision()
    //        {
    //            foreach (Beehive beehive in beehives)
    //            {
    //                foreach (SmokePuff smokePuff in beeKeeper.FiredSmokePuffs)
    //                {
    //                    if (beehive.Bounds.HasCollision(smokePuff.CentralCollisionArea))
    //                    {
    //                        beehive.AllowBeesToGenerate = false;
    //                    }
    //                }
    //            }
    //        }

    //        /// <summary>
    //        /// Sets an internal value which determines whether or not to display an arrow above the vat.
    //        /// </summary>
    //        private void HandleVatHoneyArrow()
    //        {
    //            if (jar.HasHoney)
    //            {
    //                drawArrow = true;
    //            }
    //            else
    //            {
    //                drawArrow = false;
    //            }
    //        }

    //        /// <summary>
    //        /// Handle smoke logic.
    //        /// </summary>
    //        private void HandleSmoke()
    //        {
    //            // If not currently shooting, refill the gun
    //            if (!isSmokebuttonClicked)
    //            {
    //                smokeButtonScorebar.IncreaseCurrentValue(
    //                    ConfigurationManager.ModesConfiguration[gameDifficultyLevel].IncreaseAmountSpeed);

    //                beeKeeper.IsShootingSmoke = false;
    //            }
    //            else
    //            {
    //                // Check that the gun is not empty
    //                if (smokeButtonScorebar.CurrentValue <= smokeButtonScorebar.MinValue)
    //                {
    //                    beeKeeper.IsShootingSmoke = false;
    //                }
    //                else
    //                {
    //                    beeKeeper.IsShootingSmoke = true;

    //                    smokeButtonScorebar.DecreaseCurrentValue(
    //                        ConfigurationManager.ModesConfiguration[gameDifficultyLevel].DecreaseAmountSpeed);
    //                }
    //            }
    //        }

    //        /// <summary>
    //        /// Checks whether the current game is over, and if so performs the necessary actions.
    //        /// </summary>
    //        /// <returns>True if the current game is over and false otherwise.</returns>
    //        private bool CheckIfCurrentGameFinished()
    //        {
    //            levelEnded = false;
    //            isUserWon = vat.CurrentVatCapacity >= vat.MaxVatCapacity;

    //            // If the vat is full, the player wins
    //            if (isUserWon || gameElapsed <= TimeSpan.Zero)
    //            {
    //                levelEnded = true;

    //                if (gameDifficultyLevel == DifficultyMode.Hard)
    //                {
    //                    FinalScore = ConfigurationManager.ModesConfiguration[gameDifficultyLevel].HighScoreFactor
    //                        * (int)gameElapsed.TotalMilliseconds;
    //                }
    //                else
    //                {
    //                    FinalScore = 0;
    //                }
    //            }

    //            // if true, game is over
    //            if (gameElapsed <= TimeSpan.Zero || levelEnded)
    //            {
    //                isLevelEnd = true;

    //                if (userInputToExit)
    //                {
    //                    ScreenManager.RemoveScreen(this);

    //                    if (isUserWon) // True - the user won
    //                    {
    //                        AudioManager.PlaySound("Victory");
    //                    }
    //                    else
    //                    {
    //                        AudioManager.PlaySound("Defeat");
    //                    }

    //                    MoveToNextScreen(isUserWon);
    //                }
    //            }

    //            return false;
    //        }

    //        /// <summary>
    //        /// Draws the arrow in intervals of 20 game update loops.        
    //        /// </summary>
    //        private void DrawVatHoneyArrow()
    //        {
    //            // If the arrow needs to be drawn, and it is not invisible during the current interval
    //            if (drawArrow && drawArrowInInterval)
    //            {
    //                ScreenManager.SpriteBatch.Draw(arrowTexture, vatArrowPosition, Color.White);

    //                if (arrowCounter == 20)
    //                {
    //                    drawArrowInInterval = false;
    //                    arrowCounter = 0;
    //                }
    //                arrowCounter++;
    //            }
    //            else
    //            {
    //                if (arrowCounter == 20)
    //                {
    //                    drawArrowInInterval = true;
    //                    arrowCounter = 0;
    //                }
    //                arrowCounter++;
    //            }
    //        }

    /// <summary>
    /// Draws the smoke button.
    /// </summary>
    private void DrawSmokeButton()
    {
        ScreenManager.SpriteBatch.Draw(
            _smokeButton,
            new Rectangle(
                (int)_smokeButtonPosition.X,
                (int)_smokeButtonPosition.Y,
                (int)(UIConstants.SmokeButtonSize),
                (int)(UIConstants.SmokeButtonSize)),
            Color.White);
    }

    /// <summary>
    /// Draws the count down string.
    /// </summary>
    private void DrawStartupString()
    {
        // If needed
        if (_isAtStartupCountDown)
        {
            string text = string.Empty;

            // If countdown is done
            if (_startScreenTime.Seconds == 0)
            {
                text = "Go!";
                _isAtStartupCountDown = false;
            }
            else
            {
                text = _startScreenTime.Seconds.ToString();
            }

            Vector2 viewportSize = new Vector2(
                ScreenManager.GraphicsDevice.Viewport.Width,
                ScreenManager.GraphicsDevice.Viewport.Height);
            Vector2 size = _font16px.MeasureString(text);

            Vector2 textPosition = (viewportSize - size) / 2f;

            ScreenManager.SpriteBatch.DrawString(_font36px, text, textPosition, Color.White);
        }
    }
}
