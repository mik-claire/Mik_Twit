using CoreTweet;
using HtmlAgilityPack;
using Mik_Twit.Common;
using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace Mik_Twit.Model
{
    public class TimeLineItem : INotifyPropertyChanged
    {
        public Status Tweet { get; set; }
        private string myScreenName = string.Empty;

        public string ScreenName { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public DateTime Time { get; set; }
        public string Via { get; set; }
        public string ViaUrl { get; set; }
        public string Text { get; set; }

        private bool isFav = false;
        private string fav = string.Empty;
        public bool IsRetweeted { get; set; }
        public bool IsLockedUser { get; set; }
        public string RtScreenName { get; set; }
        public string RtName { get; set; }
        public string RtIconUrl { get; set; }

        private DispatchObservableCollection<ContentsItem> contentsItemList = new DispatchObservableCollection<ContentsItem>();
        public DispatchObservableCollection<ContentsItem> ContentsItemList
        {
            get { return this.contentsItemList; }
            set { this.contentsItemList = value; }
        }

        public string ScreenName2
        {
            get
            {
                return "@" + this.ScreenName;
            }
            set { }
        }

        public string RtScreenName2
        {
            get
            {
                return "@" + this.RtScreenName;
            }
            set { }
        }

        public Visibility LockIconVisibility
        {
            get
            {
                if (this.IsLockedUser)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
            set { }
        }

        public string LockedImage
        {
            get
            {
                return "pack://application:,,,/Resources/Image/Lock_1.png";
            }
            set { }
        }

        public string TimeText
        {
            get
            {
                string formatted = this.Time.ToString("yyyy/MM/dd HH:mm:ss");
                return formatted;
            }
            set { }
        }

        public bool IsFav
        {
            get { return this.isFav; }
            set
            {
                this.isFav = value;
                OnPropertyChanged("FavImage");
                OnPropertyChanged("FavImage2");
            }
        }

        public string Fav
        {
            get
            {
                if (this.IsFav)
                {
                    return "★";
                }

                return "☆";
            }
            private set
            {
                this.fav = value;
                OnPropertyChanged("FavImage");
                OnPropertyChanged("FavImage2");
            }
        }

        public string ReplyImage
        {
            get
            {
                return "pack://application:,,,/Resources/Image/Reply_1.png";
            }
            set { }
        }

        public string ReplyImage2
        {
            get
            {
                return "pack://application:,,,/Resources/Image/Reply_2.png";
            }
            set { }
        }

        public string TalkImage
        {
            get
            {
                return "pack://application:,,,/Resources/Image/Talk_1.png";
            }
            set { }
        }

        public string TalkImage2
        {
            get
            {
                return "pack://application:,,,/Resources/Image/Talk_2.png";
            }
            set { }
        }

        public Visibility IsEnabledRtQt
        {
            get
            {
                if (this.IsLockedUser)
                {
                    return Visibility.Collapsed;
                }

                return Visibility.Visible;
            }
            set { }
        }

        public string RtImage
        {
            get
            {
                return "pack://application:,,,/Resources/Image/RT_1.png";
            }
            set { }
        }

        public string RtImage2
        {
            get
            {
                return "pack://application:,,,/Resources/Image/RT_2.png";
            }
            set { }
        }

        public string QtImage
        {
            get
            {
                return "pack://application:,,,/Resources/Image/QT_1.png";
            }
            set { }
        }

        public string QtImage2
        {
            get
            {
                return "pack://application:,,,/Resources/Image/QT_2.png";
            }
            set { }
        }

        public string FavImage
        {
            get
            {
                if (this.IsFav)
                {
                    return "pack://application:,,,/Resources/Image/Fav2_1.png";
                }

                return "pack://application:,,,/Resources/Image/Fav1_1.png";
            }
            private set
            {
                // this.fav = value;
                OnPropertyChanged("FavImage");
            }
        }

        public string FavImage2
        {
            get
            {
                if (this.IsFav)
                {
                    return "pack://application:,,,/Resources/Image/Fav2_2.png";
                }

                return "pack://application:,,,/Resources/Image/Fav1_2.png";
            }
            private set
            {
                // this.fav = value;
                OnPropertyChanged("FavImage2");
            }
        }

        public bool HasTalk { get; set; }
        
        public Visibility TalkVisibility
        {
            get
            {
                if (this.HasTalk)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public Visibility RetweeterVisibility
        {
            get
            {
                if (this.IsRetweeted)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public int RetweeterRowDefinition
        {
            get
            {
                if (this.IsRetweeted)
                {
                    return 20;
                }

                return 0;
            }
        }

        public Visibility ContentsVisibility
        {
            get
            {
                if (this.ContentsItemList != null &&
                    0 < this.ContentsItemList.Count)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        public int ContentsRowDefinition
        {
            get
            {
                if (2 < this.contentsItemList.Count)
                {
                    return 300;
                }
                else if (0 < this.contentsItemList.Count)
                {
                    return 150;
                }

                return 0;
            }
        }

        public Brush BackColor
        {
            get
            {
                if (this.IsRetweeted)
                {
                    // 千歳緑
                    // return new SolidColorBrush(Color.FromRgb(60, 103, 84));

                    // #1C401C
                    return new SolidColorBrush(Color.FromRgb(28, 64, 28));
                }

                if (this.Tweet.User.ScreenName == this.myScreenName)
                {
                    // 深緋
                    // return new SolidColorBrush(Color.FromRgb(100, 25, 35));

                    // #401C1C
                    return new SolidColorBrush(Color.FromRgb(64, 28, 28));
                }

                if (this.Text.Contains("@" + this.myScreenName))
                {
                    // 紺色
                    // return new SolidColorBrush(Color.FromRgb(29, 49, 86));

                    // #1C1C40
                    return new SolidColorBrush(Color.FromRgb(28, 28, 64));
                }

                // #696969 105, 105, 105
                return new SolidColorBrush(Colors.DimGray);
            }
        }

        public TimeLineItem(Status status, string myScreenName)
        {
            this.myScreenName = myScreenName;
            this.IsRetweeted = status.RetweetedStatus != null;
            if (this.IsRetweeted)
            {
                this.RtScreenName = status.User.ScreenName;
                this.RtName = status.User.Name;
                this.RtIconUrl = status.User.ProfileImageUrl;
                status = status.RetweetedStatus;
            }
            User user = status.User;
            this.ScreenName = user.ScreenName;
            this.Name = user.Name;
            this.IconUrl = user.ProfileImageUrl;
            this.IsLockedUser = user.IsProtected;
            this.Time = status.CreatedAt.LocalDateTime;
            setVia(status.Source);
            this.Text = WebUtility.HtmlDecode(status.Text);
            this.IsFav = status.IsFavorited == true;

            if (status.ExtendedEntities != null)
            {
                foreach (MediaEntity me in status.ExtendedEntities.Media)
                {
                    ContentsItem ci = new ContentsItem(me.MediaUrl);
                    this.contentsItemList.Add(ci);
                }
            }

            if (status.InReplyToStatusId != null)
            {
                this.HasTalk = true;
            }

            this.Tweet = status;
        }

        private void setVia(string viaHtml)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(viaHtml);
            var node = doc.DocumentNode.SelectSingleNode("//a/@href");
            this.Via = node.InnerText;
            this.ViaUrl = node.GetAttributeValue("href", string.Empty);
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
