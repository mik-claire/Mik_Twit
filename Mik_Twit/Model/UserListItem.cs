using CoreTweet;
using System.ComponentModel;
using System.Windows;

namespace Mik_Twit.Model
{
    public class UserListItem : INotifyPropertyChanged
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

        public User UserData { get; set; }
        public long Id { get; set; }
        public string ScreenName { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public bool IsLockedUser { get; set; }

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

        public UserListItem(User user)
        {
            this.UserData = user;

            this.Id = (long)user.Id;
            this.ScreenName = user.ScreenName;
            this.Name = user.Name;
            this.IconUrl = user.ProfileImageUrl;
            this.IsLockedUser = user.IsProtected;
        }
    }
}
