using System;
using Microsoft.Xna.Framework;

namespace HoneycombRush;

class PlayerIndexEventArgs : EventArgs
{
    private PlayerIndex _playerIndex;
    public PlayerIndex PlayerIndex
    {
        get { return _playerIndex; }
    }

    public PlayerIndexEventArgs(PlayerIndex playerIndex)
    {
        _playerIndex = playerIndex;
    }
}
