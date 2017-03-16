using System.ComponentModel;
using System.Windows.Media;

namespace Mik_Twit.Model
{
    public class InputTextItem : INotifyPropertyChanged
    {
        public int Remain
        {
            get
            {
                string doc = this.input.Replace("\r\n", "\n");
                return 140 - doc.Length;
            }
            set
            {

            }
        }

        private string input = string.Empty;
        public string Input
        {
            get { return this.input; }
            set
            {
                this.input = value;
                OnPropertyChanged("Remain");
                OnPropertyChanged("TextColor");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            handler(this, new PropertyChangedEventArgs(name));
        }

        public Brush TextColor
        {
            get
            {
                if (this.Remain < 1)
                {
                    return new SolidColorBrush(Colors.Crimson);
                }
                if (this.Remain < 30)
                {
                    return new SolidColorBrush(Colors.Salmon);
                }

                return new SolidColorBrush(Colors.AliceBlue);
            }
            set
            {

            }
        }
    }
}
