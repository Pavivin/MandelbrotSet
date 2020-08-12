using System;
using System.Threading;

namespace MаndelBrotSet.Model
{
    // Абстрактный класс для генерации фрактала
    public abstract class FractalGenerator
    {
        protected long _maxIterations;        // Текущее значение c
        protected string _colorIndex;         // Текущая цветовая схема
        protected FractalImage _currentImage;   // Обработка изображения


        // Создает изображение с определенными размерами
        public FractalImage CreateImage(int height, int width)
        {
            return new FractalImage(width, height);
        }

        // Изображение будет разделено на количество сегментов
        // зависящих от количества потоков

        public void GeneratePixels(FractalImage image, long iterations, int segments, string colorIndex)
        {
            // Этот объект заблокирован до тех пор
            // пока не будет досчитан
            lock (this)
            {
                // Внешние состояния
                _colorIndex = colorIndex;
                _maxIterations = iterations;
                _currentImage = image;

                // Стандартное количество сегментов зависящее от количества ядер
                if (segments == 0)
                    segments = Environment.ProcessorCount - 1;

                Thread[] _generationThreads = new Thread[segments];

                // Коэффициенты масштаба
                double xfactor = MandelbrotComputation.CalculateXScalar(image);
                double yfactor = MandelbrotComputation.CalculateYScalar(image);

                // Вычисление количества сегментов и пикселей в изображении
                int pixels = image.RawPixels.Length / image.BytesPixel;
                int pixelsPerthread = pixels / segments;

                // Начало вычисления пикселей в различных потоках
                for (int i = 0; i < segments; i++)
                {
                    int segmentStart = i * pixelsPerthread;
                    int segmentEnd = segmentStart + pixelsPerthread;
                    _generationThreads[i] = new Thread(() => ProcessSegment(segmentStart, segmentEnd, xfactor, yfactor));
                    _generationThreads[i].Start();
                }

                // Ожидание конца работы потоков
                foreach (var thread in _generationThreads)
                    thread.Join();
            }
        }

    
        /// <param name="xfactor"> Скалярный коэффициент сегмента </param>
        protected abstract void ProcessSegment(int segmentStart, int segmentEnd, double xfactor, double yfactor);
    }
}