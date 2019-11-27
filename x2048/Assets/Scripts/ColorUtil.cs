using UnityEngine;

namespace Assets.Scripts
{
    public static class ColorUtil
    {
        private static readonly int DIVIDE_COEF = 256;
        private static readonly int BIT_SHIFT_COEF = 8;
        private static readonly float COLOR_FORMAT_DIVIDE_COEF = 255f;

        public static Color ToColor(this int color)
        {
            var b = color % DIVIDE_COEF / COLOR_FORMAT_DIVIDE_COEF;
            color = color >> BIT_SHIFT_COEF;
            var g = color % DIVIDE_COEF / COLOR_FORMAT_DIVIDE_COEF;
            color = color >> BIT_SHIFT_COEF;
            var r = color % DIVIDE_COEF / COLOR_FORMAT_DIVIDE_COEF;

            return new Color(r, g, b, 1);
        }
    }
}
