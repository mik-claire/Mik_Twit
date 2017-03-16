using System.ComponentModel;
using System.Net;
using System.Windows;

namespace Mik_Twit.Model
{
    public class ReplyToItem : INotifyPropertyChanged
    {
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

        public ReplyToItem(string screenName = null, string text = null)
        {
            if (screenName == null ||
                text == null)
            {
                Clear();
            }
            else
            {
                Set(screenName, text);
            }
        }

        public void Set(string screenName, string text)
        {
            this.ScreenName = screenName;
            this.Text = WebUtility.HtmlDecode(text);
            this.ExistStatus = true;
        }

        public void Clear()
        {
            this.ScreenName = string.Empty;
            this.Text = string.Empty;
            this.ExistStatus = false;
        }

        public string ScreenName { get; set; }
        public string Text { get; set; }

        private bool existStatus = false;
        public bool ExistStatus
        {
            get { return this.existStatus; }
            set
            {
                this.existStatus = value;
                OnPropertyChanged("ReplyToVisibility");
                OnPropertyChanged("ReplyToRowDefinition");
                OnPropertyChanged("ScreenName");
                OnPropertyChanged("Text");
            }
        }

        public Visibility ReplyToVisibility
        {
            get
            {
                if (this.existStatus)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
            set
            {

            }
        }

        public int ReplyToRowDefinition
        {
            get
            {
                if (this.existStatus)
                {
                    return 40;
                }

                return 0;
            }
            set
            {

            }
        }
    }
}
