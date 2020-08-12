using System.ComponentModel;

namespace MаndelBrotSet.ViewModel
{
    // Абстрактный родительский класс, используемый для обновления интерфейса
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void UpdateProperty(string str)
        {
            PropertyChangedEventHandler even = PropertyChanged;
            if (even == null)
                return;
            even(this, new PropertyChangedEventArgs(str));
        }
    }
}

