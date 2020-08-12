using System.Numerics;
using System.Windows.Media;

namespace MаndelBrotSet.Model
{
    /// Генерирование изображения
    public class MandelbrotImageGenerator : FractalGenerator
    {
        public static double R;

        // Вычисление части изображения

        /// <param name="xfactor"> Коэффициент масштабирования </param>
        protected override void ProcessSegment(int segmentStart, int segmentEnd, double xfactor, double yfactor)
        {
            segmentStart *= _currentImage.BytesPixel;
            segmentEnd *= _currentImage.BytesPixel;

            for (int pixelIndex = segmentStart; pixelIndex < segmentEnd; pixelIndex += _currentImage.BytesPixel)
            {

                _currentImage.CalculatePixelCoordinates(pixelIndex, out int ycord, out int xcord);

                // Масштабированиие точек в соответствии с позициями на изображении
                double yscaled = _currentImage.ViewPoint.Top - ycord * yfactor;
                double xscaled = _currentImage.ViewPoint.Left + xcord * xfactor;

                Complex c = new Complex(xscaled, yscaled);
                long count = MandelbrotComputation.Compute(c, _maxIterations, R);

                // использование итераций и вычисление значения Z для выбора цвета
                _currentImage.ColorPixel(pixelIndex, SelectColor(count));
            }
        }

        // Выбор цвета в зависимости от значений параметров
        private Color SelectColor(long iterations)
        {
            Color pixelcolor;
            if (iterations == _maxIterations)
            {
                // окрашивание в чёрный цвет при достижении 
                // максимального количества итераций
                pixelcolor = new Color
                {
                    A = 255
                };
                pixelcolor.B = pixelcolor.G = pixelcolor.R = 0;
            }
            else
            {
                switch (_colorIndex)
                {
                    case "Black":
                        pixelcolor = MandelbrotColors.SchemeBlack();
                        break;                   
                    default:
                    case "Continious":
                        pixelcolor = MandelbrotColors.SchemeContinious(iterations, _maxIterations);
                        break;
                }
            }
            return pixelcolor;
        }
    }
}
