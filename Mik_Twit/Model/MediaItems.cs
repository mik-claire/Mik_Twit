using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Mik_Twit.Model
{
    public class MediaItems : INotifyPropertyChanged
    {
        private List<string> files = new List<string>();
        public List<string> Files
        {
            get { return this.files; }
            set { this.files = value; }
        }

        public int Count
        {
            get
            {
                return this.files.Count;
            }
            set
            {

            }
        }

        public void AddMedia(string filePath)
        {
            if (4 < this.files.Count)
            {
                return;
            }

            if (!File.Exists(filePath))
            {
                return;
            }

            this.files.Add(filePath);
            OnPropertyChanged("Count");
        }

        public void Clear()
        {
            this.files.Clear();
            OnPropertyChanged("Count");
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
