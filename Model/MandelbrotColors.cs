using System;
using System.Windows.Media;

namespace MаndelBrotSet.Model
{
    // Класс с цветовыми схемами
    static public class MandelbrotColors
    {
        public static Color SchemeBlack()
        {
            Color pixelcolor = new Color
            {
                R = 255,
                G = 255,
                B = 255,
                A = 255
            };

            return pixelcolor;
        }

        public static Color SchemeContinious(double iterations, double max_iter)
        {
            // Вычисление интервала
            double x = iterations / max_iter;
            // вспомогательные переменные для быстрого вычисления

            double revX = (1 - x);
            double revXX = revX * revX;

            double byteX = x * 255;
            double byteXX = byteX * x;

            // 9 * (1 - x) * x^3 * 255
            // 14 * (1 - x) ^ 2 * x * 255
            // 8 * (1 - x) ^ 3 * x * 255

            int r = (int) Math.Floor(9 * revX * x * byteXX);
            int g = (int) Math.Floor(14 * revXX * byteXX);
            int b = (int) Math.Floor(8 * revXX * revX * byteX);

            Color pixelcolor = new Color
            {
                A = 255,
                R = (byte)(r * 1.1),
                G = (byte)(g * 1.1),
                B = (byte)(b * 1.1)
            };

            return pixelcolor;
        }
    }
}
