using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MаndelBrotSet.Model
{
    // Сгенерированное множество Мандельброта
    public class FractalImage
    {
        // Дополнительные байтовые отступы для хранения изображения
        private readonly int _stride;

        public byte[] RawPixels { get; }

        // Размеры изображения
        public int Height { get; }
        public int Width { get; }

        public int BytesPixel { get; }

        public WriteableBitmap BitMap { get; }

        public Rect ViewPoint { get; set; }

        // Стандартный конструктор

        public FractalImage(int imageWidth, int imageHeight)
        {
            Height = imageHeight;
            Width = imageWidth;

            BitMap = new WriteableBitmap(imageWidth, imageHeight, 96, 96, PixelFormats.Bgra32, null);

            // Calculate Bytes per pixel  and raw pixel array.
            BytesPixel = BitMap.Format.BitsPerPixel / 8;
            RawPixels = new byte[Height * Width * BytesPixel];
            _stride = Width * BytesPixel;
        }

        // Конструктор копирования

        /// <param name="instance"></param>
        public FractalImage(FractalImage instance)
        {
            Height = instance.Height;
            Width = instance.Width;
            _stride = instance._stride;
            BitMap = instance.BitMap.Clone();
            RawPixels = (byte[])instance.RawPixels.Clone();
            ViewPoint = instance.ViewPoint;
        }


        //  Вычисление одного пикселя

        /// <param name="index"> индекс пикселя в массиве </param>
        /// <param name="colorInformation"></param>
        public void ColorPixel(long index, Color colorInformation)
        {
            if (index + 4 > RawPixels.Length || index < 0)
                new Exception("Error: Accessing Invalid location in array");

            RawPixels[index] = colorInformation.B;
            RawPixels[index + 1] = colorInformation.G;
            RawPixels[index + 2] = colorInformation.R;
            RawPixels[index + 3] = colorInformation.A;
        }

        // Запись массива байтов в изображение
        public void WritePixelsToImage()
        {
            Int32Rect rect = new Int32Rect(0, 0, Width, Height);
            BitMap.WritePixels(rect, RawPixels, _stride, 0);
        }

        // Вычисление местоположения пикселя на экране
        // учитывая буфер шага
        public void CalculatePixelCoordinates(int pixelIndex, out int ycord, out int xcord)
        {
            ycord = pixelIndex / _stride;
            xcord = pixelIndex % _stride / BytesPixel;
        }
    }
}
