using UnityEngine;

namespace Com.SHUPDP.JUST
{
    public class JUSTConstantsAndDefinitions
    {

        public const string gameVersion = "1";
        public const byte maxPlayersPerRoom = 4;
        public const string playerNamePrefKey = "PlayerName"; // The lookup id for playerprefs name storage;
        public const string readyStatus = "isPlayerReady"; // Used as lookup key in custom networked player properties
        public const string customColour = "customColour";
        public const string scores = "scores";

        // Borrowed this currently from the demo asteroids project to check how it works in the lobby system
        // Imagine we could maybe use this for more fancy icons, etc in our version.. linking to image sources or suchlike
        public static Color GetColor(int colorChoice)
        {
            Color playerColor;
            switch (colorChoice)
            {
                case 0:
                    playerColor = Color.red;
                    break;
                case 1:
                    playerColor = Color.green;
                    break;
                case 2:
                    playerColor = Color.blue;
                    break;
                case 3:
                    playerColor = Color.yellow;
                    break;
                case 4:
                    playerColor = Color.cyan;
                    break;
                case 5:
                    playerColor = Color.grey;
                    break;
                case 6:
                    playerColor = Color.magenta;
                    break;
                case 7:
                    playerColor = Color.white;
                    break;
                default:
                    playerColor = Color.black;
                    break;
            }

            return playerColor;
        }
    }
}
