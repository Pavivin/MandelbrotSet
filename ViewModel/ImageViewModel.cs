using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using MаndelBrotSet.Model;

namespace MаndelBrotSet.ViewModel
{
    // Модель отображения связана с canvas, где будет отрисована
    public class ImageViewModel : ViewModelBase
    {

        public static Rect StartPos(double xmin, double ymin, double xmax, double ymax)
        {
            return new Rect(new Point(xmin, ymin), new Point(xmax, ymax));
        }

        public static Rect DefaultArea;

        private bool _drawing, _free;
        private ConfigurationViewModel _config;
        private MandelbrotImageGenerator _ImageGenerator;

        #region Properties

        public WriteableBitmap ImageSource
        {
            get { return Image.BitMap; }
        }

        public FractalImage Image
        {
            get; private set;
        }

        public ObservableCollection<FractalImage> ZoomList
        {
            get; private set;
        } 
            = new ObservableCollection<FractalImage>();

        public bool CanCancel
        {
            get { return !Free; }
        }

        public bool Free
        {
            get { return _free; }
            set
            {
                _free = value;
                UpdateProperty("Free");
                UpdateProperty("Status");
                UpdateProperty("CanCancel");
            }
        }

        public string Status
        {
            get
            {
                if (!Free)
                    return "Идёт обработка изображения";
                else
                    return "Готово";
            }
        }

        #endregion

        //  Конструктор для отображения
        /// <param name="config"> Configurations </param>
        public ImageViewModel(ConfigurationViewModel config)
        {
            Free = true;
            _ImageGenerator = new MandelbrotImageGenerator();
            _config = config;
        }

        #region Operations
        
        // Создание множества Мандельброта в различных частях изображения

        /// <param name="area"> Area that Mandelbrot is bounded to. </param>                 
        public void GenerateImage(Rect area)
        {
            if (!Free)
                return;

            // Create Image.
            Image = _ImageGenerator.CreateImage(_config.Height, _config.Width);
            Image.ViewPoint = area;
            _drawing = true;

            Thread drawThread = null;

            // Настройка потока рисования
            (drawThread = new Thread(() => { DrawImage(); })).Start();
            
            // Создание пикселей в отдельном потоке
            // Пока изображение отрисовывается, с ним ничего нельзя делать
            Free = false;
            (new Thread(() =>
            {
                _ImageGenerator.GeneratePixels(Image, 
                                               _config.MaxIterations,
                                               _config.SegmentIndex,
                                               _config.SelectedColor);

                // Ожидание выхода из потока
                _drawing = false;
                drawThread.Join();

                // Изображение отрисовано
                Free = true;
            })).Start();
        }

        // Отрисовка изображения
        public void DrawImage()
        {
            while (_drawing)
            {
                int delay = _config.DrawDelayMS;
                if (delay <= 0) delay = 10;
                Thread.Sleep(delay);
                ImageSource.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Image.WritePixelsToImage();
                    UpdateProperty("ImageSource");
                }));
            }
        }


        // Увеличение изображения
        public void ZoomIn()
        {
            if (Free)
            {
                var im = new FractalImage(Image);
                ZoomList.Add(im);
                UpdateProperty("ZoomList");
            }
        }

        public void ClearZoomList()
        {
            if (ZoomList.Count > 0)
            {
                ZoomList.Clear();
                UpdateProperty("ZoomList");
            }
        }

        #endregion
    }
}
