using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using MаndelBrotSet.ViewModel;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.IO;
using MаndelBrotSet.Model;

namespace MаndelBrotSet
{
    public partial class MainWindow : Window
    {
        private IList<string> _Colors = new List<string>();
        private Point _Position;
        private MainViewModel _ViewModel = null;
        private bool _LeftClickHold = false;

        double Xmin = -2;
        double Xmax = 0.9;
        double Ymin = -1.4;
        double Ymax = 1.4;

        internal static double R 
        {
            get; set;
        }

        public MainWindow()
        {
            _Colors.Add("Continious");
            _Colors.Add("Black");

            InitializeComponent();

            Loaded += delegate
            {
                int height = (int)Application.Current.MainWindow.ActualHeight;
                CanvasBoard.Width = height;

                _ViewModel = new MainViewModel(height, height);

                DataContext = _ViewModel;
                StatusValue.DataContext = _ViewModel.ImageVM;
                ImageBoard.DataContext = _ViewModel.ImageVM;
                ZoomListBox.DataContext = _ViewModel.ImageVM;
                IterationTextBox.Text = _ViewModel.ConfigsVM.MaxIterations.ToString();

                MandelbrotImageGenerator.R = Convert.ToDouble(RSet.Text);

                // Отображения цветовых схем в программе
                ColorScheme.ItemsSource = _Colors;
                ColorScheme.SelectedIndex = 0;
               
            };
            ImageViewModel.DefaultArea = ImageViewModel.StartPos(Xmin, Ymin, Xmax, Ymax);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            MandelbrotImageGenerator.R = Convert.ToDouble(RSet.Text);
            ImageViewModel.DefaultArea = UpdateCoordinats();
            _ViewModel.StartPoint();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _ViewModel.UpdateImage(_ViewModel.ImageVM.Image.ViewPoint);
        }

        private Rect UpdateCoordinats()
        {
            Xmin = double.Parse(XMin.Text);       
            Xmax = double.Parse(XMax.Text);
            Ymin = double.Parse(YMin.Text);
            Ymax = double.Parse(YMax.Text);

            return ImageViewModel.StartPos(Xmin, Ymin, Xmax, Ymax);
        }

        private void ColorScheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _ViewModel.ConfigsVM.SelectedColor = ColorScheme.SelectedItem as string;

            _ViewModel.UpdateImage(_ViewModel.ImageVM.Image.ViewPoint);
        }

        // Количество итераций
        private void IterationTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            long number;
            try
            {
                number = long.Parse(IterationTextBox.Text);
            }
            catch (Exception)
            {
                number = 0;
            }
            
            if (number > 0 && number != _ViewModel.ConfigsVM.MaxIterations)
            {
                _ViewModel.ConfigsVM.MaxIterations = number;

                _ViewModel.UpdateImage(_ViewModel.ImageVM.Image.ViewPoint);
            }
        }

        private void Window_SizeChanged(object sender, RoutedEventArgs e)
        {
            if (_ViewModel == null)
                return;

            int height = (int)Application.Current.MainWindow.ActualHeight;
            CanvasBoard.Width = height;

            // Изображение должно изменяться пропорционально
            _ViewModel.Resize(height, height);
        }

        private void Canvas_Clicked(object sender, MouseButtonEventArgs e)
        {
            _LeftClickHold = true;
            _Position = e.GetPosition(CanvasBoard);

            // Координаты нажатия кнопка 
            Canvas.SetLeft(SelectionSquare, _Position.X);
            Canvas.SetTop(SelectionSquare, _Position.Y);
        }

        // При удерживании ЛКМ - изображение увеличивается
        private void Canvas_MouseReleased(object sender, MouseButtonEventArgs e)
        {
            _LeftClickHold = false;

            // Проверка на то, что квадрат не слишком маленький
            if (SelectionSquare.Width > 2 && SelectionSquare.Height > 2)
            {

                _ViewModel.AskZoom(SelectionSquare.Width, SelectionSquare.Height,
                                   Canvas.GetTop(SelectionSquare), Canvas.GetLeft(SelectionSquare));
            }

            SelectionSquare.Width = SelectionSquare.Height = 0;
        }

        private void CanvasBoard_MouseMove(object sender, MouseEventArgs e)
        {
            if (_LeftClickHold)
            {
                CalculateDimensions(e.GetPosition(CanvasBoard));
            }
        }

        // Вычисление позиции квадрата

        /// <param name="currentMP"> Mouse position </param>
        private void CalculateDimensions(Point currentMP)
        {
            var changexy = currentMP - _Position;

            // Квадрат не в IV четверти
            if (changexy.Y <= 0)
                return;

            SelectionSquare.Width = SelectionSquare.Height = changexy.Y;
            Canvas.SetLeft(SelectionSquare, _Position.X);
            Canvas.SetTop(SelectionSquare, _Position.Y);
        }

        private void ButtonSaveBild_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog
            {
                Filter = "JPG Files (*.jpg)|*.jpg"
            };
            if (save.ShowDialog() == true)
            {
                JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
                jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(ImageBoard.Source as BitmapSource));
                using (FileStream fileStream = new FileStream(save.FileName, FileMode.Create))
                    jpegBitmapEncoder.Save(fileStream);
            }
        }
    }
}