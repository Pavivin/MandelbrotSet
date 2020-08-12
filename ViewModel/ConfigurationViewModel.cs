namespace MаndelBrotSet.ViewModel
{
    public class ConfigurationViewModel : ViewModelBase
    {
        //Максимальное значение c
        public long MaxIterations { get; set; }

        // Высота сгенерированного изображения
        public int Height { get; set; }

        // Ширина изображения
        public int Width { get; set; }

        // Выбранная цветовая схема
        public string SelectedColor { get; set; }

        // Количество использованных потоков
        public int SegmentIndex { get; set; }

        // Задержка отображения
        public int DrawDelayMS { get; set; }
    }
}
