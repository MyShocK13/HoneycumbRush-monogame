using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace HoneycombRush
{
    public class InputState
    {
        public enum MouseButton
        {
            Left,
            Middle,
            Right
        }

        public const int MaxInputs = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly MouseState[] CurrentMouseStates;
        public readonly GamePadState[] CurrentGamePadStates;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly MouseState[] LastMouseStates;
        public readonly GamePadState[] LastGamePadStates;

        public readonly bool[] GamePadWasConnected;

        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentMouseStates = new MouseState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];

            LastKeyboardStates = new KeyboardState[MaxInputs];
            LastMouseStates = new MouseState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];

            GamePadWasConnected = new bool[MaxInputs];
        }

        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastMouseStates[i] = CurrentMouseStates[i];
                LastGamePadStates[i] = CurrentGamePadStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                CurrentMouseStates[i] = Mouse.GetState();
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                // Keep track of whether a gamepad has ever been
                // connected, so we can detect if it is unplugged.
                if (CurrentGamePadStates[i].IsConnected)
                {
                    GamePadWasConnected[i] = true;
                }
            }
        }

        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key) && LastKeyboardStates[i].IsKeyUp(key));
            }

            // Accept input from any player.
            return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                    IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                    IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                    IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
        }

        public bool IsKeyDown(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key));
            }

            // Accept input from any player.
            return (IsKeyDown(key, PlayerIndex.One, out playerIndex) ||
                    IsKeyDown(key, PlayerIndex.Two, out playerIndex) ||
                    IsKeyDown(key, PlayerIndex.Three, out playerIndex) ||
                    IsKeyDown(key, PlayerIndex.Four, out playerIndex));
        }

        public bool IsNewMouseClick(MouseButton mouseButton, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;
                switch (mouseButton)
                {
                    case MouseButton.Left:
                        return (CurrentMouseStates[i].LeftButton == ButtonState.Pressed &&
                                LastMouseStates[i].LeftButton == ButtonState.Released);
                    case MouseButton.Right:
                        return (CurrentMouseStates[i].RightButton == ButtonState.Pressed &&
                                LastMouseStates[i].RightButton == ButtonState.Released);
                    case MouseButton.Middle:
                        return (CurrentMouseStates[i].MiddleButton == ButtonState.Pressed &&
                                LastMouseStates[i].MiddleButton == ButtonState.Released);
                    default:
                        return IsNewMouseClick(MouseButton.Left, controllingPlayer, out playerIndex) ||
                               IsNewMouseClick(MouseButton.Middle, controllingPlayer, out playerIndex) ||
                               IsNewMouseClick(MouseButton.Right, controllingPlayer, out playerIndex);
                }
            }

            // Accept input from any player.
            return (IsNewMouseClick(mouseButton, PlayerIndex.One, out playerIndex) ||
                    IsNewMouseClick(mouseButton, PlayerIndex.Two, out playerIndex) ||
                    IsNewMouseClick(mouseButton, PlayerIndex.Three, out playerIndex) ||
                    IsNewMouseClick(mouseButton, PlayerIndex.Four, out playerIndex));
        }

        public bool IsMouseDown(MouseButton mouseButton, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;
                switch (mouseButton)
                {
                    case MouseButton.Left:
                        return CurrentMouseStates[i].LeftButton == ButtonState.Pressed;
                    case MouseButton.Right:
                        return CurrentMouseStates[i].RightButton == ButtonState.Pressed;
                    case MouseButton.Middle:
                        return CurrentMouseStates[i].MiddleButton == ButtonState.Pressed;
                    default:
                        return IsMouseDown(MouseButton.Left, controllingPlayer, out playerIndex) ||
                               IsMouseDown(MouseButton.Middle, controllingPlayer, out playerIndex) ||
                               IsMouseDown(MouseButton.Right, controllingPlayer, out playerIndex);
                }

            }

            // Accept input from any player.
            return (IsMouseDown(mouseButton, PlayerIndex.One, out playerIndex) ||
                    IsMouseDown(mouseButton, PlayerIndex.Two, out playerIndex) ||
                    IsMouseDown(mouseButton, PlayerIndex.Three, out playerIndex) ||
                    IsMouseDown(mouseButton, PlayerIndex.Four, out playerIndex));
        }

        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonDown(button) &&
                        LastGamePadStates[i].IsButtonUp(button));
            }

            // Accept input from any player.
            return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                    IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                    IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                    IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
        }

        public bool IsMenuSelect(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   IsNewMouseClick(MouseButton.Left, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }

        public bool IsMenuCancel(PlayerIndex? controllingPlayer, out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
        }

        public bool IsMenuUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.Left, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadLeft, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickLeft, controllingPlayer, out playerIndex);
        }

        public bool IsMenuDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.Right, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadRight, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickRight, controllingPlayer, out playerIndex);
        }

        public bool IsPauseGame(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }
    }
}
