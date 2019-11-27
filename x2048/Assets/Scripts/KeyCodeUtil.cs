using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class KeyCodeUtil
    {
        private static Dictionary<KeyCode, Vector2> KeyMapping = new Dictionary<KeyCode, Vector2>
        {
            {KeyCode.LeftArrow, Vector2.left},
            {KeyCode.RightArrow, Vector2.right},
            {KeyCode.UpArrow, Vector2.up},
            {KeyCode.DownArrow, Vector2.down},
        };

        public static Vector2 GetDirection(this KeyCode key)
        {
            return KeyMapping[key];
        }
    }
}
