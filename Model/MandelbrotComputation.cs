using System.Numerics;

namespace MаndelBrotSet.Model
{
    public static class MandelbrotComputation
    {
        //  Вычисляются скаляры X и Y для масштабирования изображения

        /// <param name="image"> Изображение, для которого будет вычислен скаляр </param>
        public static double CalculateXScalar(FractalImage image)
        {
            return (image.ViewPoint.Right - image.ViewPoint.Left) / image.Width;
        }

        public static double CalculateYScalar(FractalImage image)
        {
            return  (image.ViewPoint.Top - image.ViewPoint.Bottom) / image.Height;
        }

        /// <param name="lastZ"> Параметр для цветовой схемы </param>
        public static long Compute(Complex c, long MaxIterations, double R = 2)
        {
            Complex z = Complex.Zero;
            long iteration;

            // z.Magnitude - модуль числа
            for (iteration = 0; iteration < MaxIterations && z.Magnitude < R; iteration++)
            {
                z = z * z + c;
                iteration++;
            }

            return iteration;
        }
    }
}
