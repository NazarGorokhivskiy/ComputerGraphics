using UnityEngine;

namespace Assets.Scripts
{
    class CoordinatesUtil
    {
        private static readonly float SCALE_COEF = 1.7f;
        private static readonly float OFFSET_COEF = 0.95f;

        public static Vector2 FromGridToCoordinates(int x, int y)
        {
            return new Vector2(SCALE_COEF * x + OFFSET_COEF, Manager.gridY[y]);
        }

        public static Vector2 FromCoordinatesToGrid(float x, float y)
        {
            for (int i = 0; i <= 3; i++)
            {
                if (Manager.gridY[i] == y) y = i;
            }
            return new Vector2((x - OFFSET_COEF) / SCALE_COEF, y);
        }
    }
}
