using System.Windows;
using System;
using MаndelBrotSet.Model;

namespace MаndelBrotSet.ViewModel
{
    //  Начальная модель отображения
    public class MainViewModel : ViewModelBase
    {
        // Модель для контроля отображения
        public ImageViewModel ImageVM { get; set; }

        // Модель для конфигурации
        public ConfigurationViewModel ConfigsVM { get; set; }

        /// <param name="width"> Начальная ширина изображения </param>
        public MainViewModel(int width, int height)
        {
            ConfigsVM = new ConfigurationViewModel
            {
                MaxIterations = 1000,
                DrawDelayMS = 10,
                SegmentIndex = Environment.ProcessorCount
            };

            ImageVM = new ImageViewModel(ConfigsVM);
            Resize(width, height);
        }

        // Возвращение к начальной точке
        public void StartPoint()
        {
            ImageVM.ClearZoomList();
            UpdateImage(ImageViewModel.DefaultArea);
        }

        //  Begins the zoom in process.
        public void AskZoom(double RectWidth, double RectHeight, double TopLocation, double LeftLocation)
        {
            double xscale = MandelbrotComputation.CalculateXScalar(ImageVM.Image);
            double yscale = MandelbrotComputation.CalculateYScalar(ImageVM.Image);

            Point TopLeft = new Point(ImageVM.Image.ViewPoint.Left + LeftLocation * xscale, 
                ImageVM.Image.ViewPoint.Top - TopLocation * yscale);

            Point BottomRight = TopLeft + new Vector(RectWidth * xscale, -RectHeight * yscale);

            ImageVM.ZoomIn();
            UpdateImage(new Rect(TopLeft, BottomRight));
        }

        /// <param name="width"> ширина изображения </param>
        public void Resize(int width, int height)
        {
            ConfigsVM.Width = width; 
            ConfigsVM.Height = height;
            UpdateProperty("Configs");
            UpdateImage(ImageViewModel.DefaultArea);
        }

        //  Отрисовка изображения с выбранной зоной
        public void UpdateImage(Rect Area)
        {
            ImageVM.GenerateImage(Area);
            UpdateProperty("ImageSource");
        }
    }
}
