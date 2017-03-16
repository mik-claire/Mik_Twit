using CoreTweet;
using MetroRadiance.Controls;
using Mik_Twit.Model;
using Mik_Twit.Util;
using System.Windows;
using System.Windows.Input;

namespace Mik_Twit.View
{
    /// <summary>
    /// UserInfoWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class UserInfoWindow : MetroWindow
    {
        private TitleTextItem title = null;
        private UserInfoItem userInfo = null;
        private Account account = null;
        private long targetId = 0;
        private string targetScreenName = string.Empty;

        public UserInfoWindow(Account ac, long id)
        {
            InitializeComponent();

            this.account = ac;
            this.targetId = id;

            this.Activate();
        }

        public UserInfoWindow(Account ac, string screenName)
        {
            InitializeComponent();

            this.account = ac;
            this.targetScreenName = screenName;

            this.Activate();
        }

        #region Event Handler

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            User user = null;
            try
            {
                if (string.IsNullOrEmpty(this.targetScreenName))
                {
                    user = await TwitterUtil.GetUserAsync(this.account.TokensData, this.targetId);
                }
                else
                {
                    user = await TwitterUtil.GetUserAsync(this.account.TokensData, this.targetScreenName);
                }
            }
            catch (TwitterException tex)
            {
                MessageBox.Show(tex.Message,
                    "Twitter Exception.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                this.Close();
                return;
            }

            if (user == null)
            {
                this.Close();
                return;
            }

            UserInfoItem item = new UserInfoItem(this.account.TokensData, user);
            this.userInfo = item;
            this.DataContext = this.userInfo;

            try
            {
                this.timeLineViewer_Tweet.Initialize(this, this.account, TimeLineMode.UserTweet);
                await this.timeLineViewer_Tweet.SetUserStatusesAsync(this.userInfo.Id);

                this.userListViewer_Follow.Initialize(this, this.account, UserListMode.Follow);
                await this.userListViewer_Follow.SetFollowsAsync(this.userInfo.Id);

                this.userListViewer_Follower.Initialize(this, this.account, UserListMode.Follower);
                await this.userListViewer_Follower.SetFollowersAsync(this.userInfo.Id);
            }
            catch (TwitterException tex)
            {
                MessageBox.Show(tex.Message,
                    "Twitter Exception.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            this.title = new TitleTextItem(string.Format("User Information - @{0}", user.ScreenName));
            this.textBlock_Title.DataContext = this.title;

            this.Activate();
        }

        private void textBlock_Location_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void textBlock_URL_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        #endregion

        private async void textBlock_FollowRemove_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.userInfo.IsFollowing)
            {
                // Remove
                await this.userInfo.RemoveAsync();
            }
            else
            {
                // Follow
                await this.userInfo.FollowAsync();
            }
        }

        private async void textBlock_Block_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            await this.userInfo.BlockAsync();
        }
    }
}
