using CoreTweet;
using HtmlAgilityPack;
using Mik_Twit.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mik_Twit.Model
{
    public class DirectMessageItem : INotifyPropertyChanged
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

        public DirectMessage Dm { get; set; }
        public User UserData { get; set; }

        public string ScreenName { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public bool IsLockedUser { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }

        private DispatchObservableCollection<ContentsItem> contentsItemList = new DispatchObservableCollection<ContentsItem>();
        public DispatchObservableCollection<ContentsItem> ContentsItemList
        {
            get { return this.contentsItemList; }
            set { this.contentsItemList = value; }
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

        public string ScreenName2
        {
            get
            {
                return "@" + this.ScreenName;
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

        public string TimeText
        {
            get
            {
                string formatted = this.Time.ToString("yyyy/MM/dd HH:mm:ss");
                return formatted;
            }
            set { }
        }

        public DirectMessageItem(DirectMessage target, bool sentByMe)
        {
            this.Dm = target;
            if (sentByMe)
            {
                this.UserData = target.Recipient;
            }
            else
            {
                this.UserData = target.Sender;
            }
            this.ScreenName = this.UserData.ScreenName;
            this.Name = this.UserData.Name;
            this.IconUrl = this.UserData.ProfileImageUrl;
            this.IsLockedUser = this.UserData.IsProtected;
            this.Time = target.CreatedAt.LocalDateTime;
            this.Text = WebUtility.HtmlDecode(target.Text);

            if (target.Entities.Media != null)
            {
                foreach (MediaEntity me in target.Entities.Media)
                {
                    ContentsItem ci = new ContentsItem(me.MediaUrl);
                    this.contentsItemList.Add(ci);
                }
            }
        }

        public DirectMessageItem(DirectMessage target)
        {
            this.Dm = target;
            this.UserData = target.Sender;
            this.ScreenName = this.UserData.ScreenName;
            this.Name = this.UserData.Name;
            this.IconUrl = this.UserData.ProfileImageUrl;
            this.IsLockedUser = this.UserData.IsProtected;
            this.Time = target.CreatedAt.LocalDateTime;
            this.Text = WebUtility.HtmlDecode(target.Text);

            if (target.Entities.Media != null)
            {
                foreach (MediaEntity me in target.Entities.Media)
                {
                    ContentsItem ci = new ContentsItem(me.MediaUrl);
                    this.contentsItemList.Add(ci);
                }
            }
        }
    }
}
