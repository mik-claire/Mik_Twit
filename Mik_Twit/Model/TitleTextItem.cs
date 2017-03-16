using System.ComponentModel;

namespace Mik_Twit.Model
{
    public class TitleTextItem : INotifyPropertyChanged
    {
        private string title = string.Empty;
        public string Title
        {
            get { return this.title; }
            set
            {
                this.title = value;
                OnPropertyChanged("Title");
            }
        }

        public TitleTextItem(string title)
        {
            this.title = title;
            OnPropertyChanged("Title");
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
    }
}
