using CoreTweet;
using Mik_Twit.Common;
using Mik_Twit.Model;
using Mik_Twit.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Mik_Twit.View.Control
{
    /// <summary>
    /// UserListViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class UserListViewer : UserControl
    {
        private MainWindow main = null;
        private Window parent = null;
        private Account account;
        private DispatchObservableCollection<UserListItem> userList = new DispatchObservableCollection<UserListItem>();
        private UserListMode mode;
        private bool isLoading = false;

        private long userId = 0;
        private long cursor = -1;

        public UserListViewer()
        {
            InitializeComponent();
        }

        public void Initialize(Window w, Account ac, UserListMode mode)
        {
            this.main = Application.Current.MainWindow as MainWindow;
            this.parent = w;
            this.account = ac;
            this.mode = mode;

            this.listBox_UserList.DataContext = this.userList;
        }

        public void SetUserList(List<User> users)
        {
            foreach (User user in users)
            {
                UserListItem item = new UserListItem(user);
                this.userList.Add(item);
            }
        }

        public async Task SetFollowsAsync(long userId)
        {
            var follow = await TwitterUtil.GetUserFollowsAsync(this.account.TokensData, userId, this.cursor);
            this.userId = userId;
            this.cursor = follow.NextCursor;

            SetUserList(follow.ToList());
            this.isLoading = false;
        }

        public async Task SetFollowersAsync(long userId)
        {
            var follower = await TwitterUtil.GetUserFollowersAsync(this.account.TokensData, userId, this.cursor);
            this.userId = userId;
            this.cursor = follower.NextCursor;

            SetUserList(follower.ToList());
            this.isLoading = false;
        }

        #region Event Handler

        private void listBox_UserList_Item_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem selectedItem = sender as ListBoxItem;
            if (selectedItem == null)
            {
                return;
            }

            UserListItem target = selectedItem.Content as UserListItem;
            if (target == null)
            {
                return;
            }

            if (this.mode == UserListMode.Account)
            {
                if (e.ClickCount != 2)
                {
                    return;
                }

                AccountManager am = AccountManager.GetInstance();
                am.SetCurrentIndexFromScreenName(target.ScreenName);

                this.main.ChangeAccount();
                this.parent.Close();
                e.Handled = true;
                return;
            }

            long id = (long)target.Id;
            UserInfoWindow w = new UserInfoWindow(this.account, id);
            w.Owner = this.main;
            w.Show();

            e.Handled = true;
        }

        private async void listBox_UserList_ScrollBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.mode != UserListMode.Follow &&
                this.mode != UserListMode.Follower)
            {
                return;
            }

            ListBox lb = sender as ListBox;
            Border b1 = VisualTreeHelper.GetChild(lb, 0) as Border;
            ScrollViewer sv = VisualTreeHelper.GetChild(b1, 0) as ScrollViewer;

            double max = sv.ScrollableHeight;
            double now = sv.VerticalOffset;
            int per = (int)(now / max * 100);

            if (per < 99)
            {
                return;
            }

            if (this.isLoading)
            {
                return;
            }

            this.isLoading = true;

            try
            {
                if (this.mode == UserListMode.Follow)
                {
                    await SetFollowsAsync(this.userId);
                }
                else if (this.mode == UserListMode.Follower)
                {
                    await SetFollowersAsync(this.userId);
                }
            }
            catch (TwitterException tex)
            {
                MessageBox.Show(tex.Message,
                    "Twitter Exception.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        #endregion
    }
}
