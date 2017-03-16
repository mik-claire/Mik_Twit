using CoreTweet;
using CoreTweet.Core;
using CoreTweet.Streaming;
using Mik_Twit.Common;
using Mik_Twit.Model;
using Mik_Twit.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Mik_Twit.View.Control
{
    /// <summary>
    /// TimeLineViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class TimeLineViewer : UserControl
    {
        private MainWindow main = null;
        private Window parent = null;
        private Account account;
        private DispatchObservableCollection<TimeLineItem> timeLine = new DispatchObservableCollection<TimeLineItem>();
        private IDisposable stream = null;
        private TimeLineMode mode = TimeLineMode.Unknown;

        private bool isLoading = false;

        private long userId = 0;
        private long maxId = 0;

        public TimeLineViewer()
        {
            InitializeComponent();
        }

        public void Initialize(Window w, Account ac)
        {
            this.main = Application.Current.MainWindow as MainWindow;
            this.parent = w;
            this.account = ac;

            this.listBox_TimeLine.DataContext = this.timeLine;
        }

        public void Initialize(Window w, Account ac, TimeLineMode mode)
        {
            this.main = Application.Current.MainWindow as MainWindow;
            this.parent = w;
            this.account = ac;
            this.mode = mode;

            this.listBox_TimeLine.DataContext = this.timeLine;
        }

        public void Initialize(MainWindow parent, Account account, TimeLineMode mode)
        {
            this.main = parent;
            this.parent = parent;
            this.account = account;
            this.mode = mode;

            this.listBox_TimeLine.DataContext = this.timeLine;
        }

        public void Initialize(Account ac, TimeLineMode mode)
        {
            this.main = Application.Current.MainWindow as MainWindow;
            this.parent = Application.Current.MainWindow;
            this.account = ac;
            this.mode = mode;

            this.listBox_TimeLine.DataContext = this.timeLine;
        }

        public void SetTimeLine()
        {
            try
            {
                this.timeLine.Clear();
                ListedResponse<Status> list = null;

                switch (this.mode)
                {
                    case TimeLineMode.Home:
                        list = this.account.TokensData.Statuses.HomeTimeline(count => 100);
                        break;
                    case TimeLineMode.Mention:
                        list = this.account.TokensData.Statuses.MentionsTimeline(count => 100);
                        break;
                    case TimeLineMode.Fav:
                        list = this.account.TokensData.Favorites.List(count => 100);
                        break;
                    default:
                        break;
                }

                if (list == null)
                {
                    return;
                }

                foreach (Status status in list)
                {
                    TimeLineItem item = new TimeLineItem(status, this.account.ScreenName);
                    this.timeLine.Add(item);
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

        public void InsertStatus(Status status)
        {
            TimeLineItem item = new TimeLineItem(status, this.account.ScreenName);
            this.timeLine.Insert(0, item);

            if (this.mode == TimeLineMode.Mention)
            {
                // 通知
                string userName = string.Format("{0} / {1}", item.ScreenName2, item.Name);
                string text = item.Text;
                this.main.TaskIcon.ShowBaloon(text, userName, NotifyMode.Reply);
            }
        }

        public void StartStreaming()
        {
            StopStreaming();

            if (this.mode == TimeLineMode.Home)
            {
                var observable = this.account.TokensData.Streaming.UserAsObservable();
                this.stream = observable.Catch(observable.DelaySubscription(TimeSpan.FromSeconds(10)).Retry())
                    .Repeat()
                    .Where((StreamingMessage msg) => msg.Type == MessageType.Create)
                    .Cast<StatusMessage>()
                    .Select((StatusMessage msg) => msg.Status)
                    .Subscribe((Status st) => InsertStatus(st));
                /*    
                this.stream = this.account.TokensData.Streaming.UserAsObservable()
                    .Where((StreamingMessage msg) => msg.Type == MessageType.Create)
                    .Cast<StatusMessage>()
                    .Select((StatusMessage msg) => msg.Status)
                    .Subscribe((Status st) => InsertStatus(st));
                */
            }
            else if (this.mode == TimeLineMode.Mention)
            {
                var observable = this.account.TokensData.Streaming.UserAsObservable();
                this.stream = observable.Catch(observable.DelaySubscription(TimeSpan.FromSeconds(10)).Retry())
                    .Repeat()
                    .Where((StreamingMessage msg) => msg.Type == MessageType.Create)
                    .Cast<StatusMessage>()
                    .Select((StatusMessage msg) => msg.Status)
                    .Where((Status st) => st.Text.Contains("@" + this.account.ScreenName))
                    .Subscribe((Status st) => InsertStatus(st));
                /*
                this.stream = this.account.TokensData.Streaming.UserAsObservable()
                    .Where((StreamingMessage msg) => msg.Type == MessageType.Create)
                    .Cast<StatusMessage>()
                    .Select((StatusMessage msg) => msg.Status)
                    .Where((Status st) => st.Text.Contains("@" + this.account.ScreenName))
                    .Subscribe((Status st) => InsertStatus(st));
                */
            }
            else if (this.mode == TimeLineMode.Fav)
            {
                var observable = this.account.TokensData.Streaming.UserAsObservable();
                this.stream = observable.Catch(observable.DelaySubscription(TimeSpan.FromSeconds(10)).Retry())
                    .Repeat()
                    .Where((StreamingMessage msg) => msg.Type == MessageType.Create)
                    .Cast<StatusMessage>()
                    .Select((StatusMessage msg) => msg.Status)
                    .Where((Status st) => st.IsFavorited == true)
                    .Subscribe((Status st) => InsertStatus(st));
                /*
                this.stream = this.account.TokensData.Streaming.UserAsObservable()
                    .Where((StreamingMessage msg) => msg.Type == MessageType.Create)
                    .Cast<StatusMessage>()
                    .Select((StatusMessage msg) => msg.Status)
                    .Where((Status st) => st.IsFavorited == true)
                    .Subscribe((Status st) => InsertStatus(st));
                */
            }
        }

        public void StopStreaming()
        {
            if (this.stream != null)
            {
                this.stream.Dispose();
            }
        }

        public void SetStatuses(List<Status> statusList)
        {
            foreach (Status status in statusList)
            {
                TimeLineItem item = new TimeLineItem(status, this.account.ScreenName);
                this.timeLine.Add(item);
            }
        }

        public async Task SetUserStatusesAsync(long userId)
        {
            var timeLine = await TwitterUtil.GetUserTweetAsync(this.account.TokensData, userId, this.maxId);
            if (timeLine.Count < 1)
            {
                return;
            }
            this.maxId = timeLine[timeLine.Count - 1].Id - 1;
            this.userId = userId;

            SetStatuses(timeLine);
            this.isLoading = false;
        }

        #region Event Handler

        private void image_Contents_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContentsItem target = (sender as Image).DataContext as ContentsItem;
            ImagePreviewWindow w = new ImagePreviewWindow(target);
            w.Owner = this.main;
            w.Show();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void textBlock_Reply_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TimeLineItem target = ((sender as Image).TemplatedParent as ContentPresenter).Content as TimeLineItem;
            if (target == null)
            {
                return;
            }
            
            this.main.ReplyTo = target.Tweet;
            this.main.ReplyToItem.Set(target.ScreenName, target.Tweet.Text);
            this.main.textBox_Tweet.Text += "@" + target.ScreenName + " ";
            this.main.textBox_Tweet.Focus();
        }

        private async void textBlock_RT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TimeLineItem target = ((sender as Image).TemplatedParent as ContentPresenter).Content as TimeLineItem;
                if (target == null)
                {
                    return;
                }

                if (target.Tweet.User.IsProtected)
                {
                    MessageBox.Show("This user is protected.",
                        "Info.",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                long id = target.Tweet.Id;
                await this.account.TokensData.Statuses.RetweetAsync(id);

                target.IsRetweeted = true;
            }
            catch (TwitterException tex)
            {
                MessageBox.Show(tex.Message,
                    "Twitter Exception.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void textBlock_QT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TimeLineItem target = ((sender as Image).TemplatedParent as ContentPresenter).Content as TimeLineItem;
            if (target == null)
            {
                return;
            }

            if (target.Tweet.User.IsProtected)
            {
                MessageBox.Show("This user is protected.",
                    "Info.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            string screenName = target.Tweet.User.ScreenName;
            string id = target.Tweet.ToString();
            this.main.textBox_Tweet.Text += string.Format(@"https://twitter.com/{0}/status/{1}",
                screenName,
                id);
            this.main.textBox_Tweet.Focus();
        }

        private async void textBlock_Fav_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TimeLineItem target = ((sender as Image).TemplatedParent as ContentPresenter).Content as TimeLineItem;
                if (target == null)
                {
                    return;
                }

                long id = target.Tweet.Id;
                if (target.IsFav)
                {
                    await this.account.TokensData.Favorites.DestroyAsync(id);
                    target.IsFav = false;
                }
                else
                {
                    await this.account.TokensData.Favorites.CreateAsync(id);
                    target.IsFav = true;
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

        private void textBlock_Talk_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TimeLineItem target = ((sender as Image).TemplatedParent as ContentPresenter).Content as TimeLineItem;
                if (target == null)
                {
                    return;
                }

                TalkWindow w = new TalkWindow(this.account, target.Tweet);
                w.Owner = this.main;
                w.Show();
            }
            catch (TwitterException tex)
            {
                MessageBox.Show(tex.Message,
                    "Twitter Exception.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void listBox_TimeLine_Item_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem selected = sender as ListBoxItem;
            if (selected == null)
            {
                return;
            }

            if (e.ClickCount == 2)
            {
                // double click
            }
            else
            {
                if (selected.IsSelected)
                {
                    this.listBox_TimeLine.SelectedIndex = -1;
                    e.Handled = true;
                    return;
                }
            }
        }

        private async void listBox_TimeLine_ScrollBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.mode != TimeLineMode.UserTweet)
            {
                return;
            }

            ListBox lb = sender as ListBox;
            Border b1 = VisualTreeHelper.GetChild(lb, 0) as Border;
            ScrollViewer sv = VisualTreeHelper.GetChild(b1, 0) as ScrollViewer;

            double max = sv.ScrollableHeight;
            double now = sv.VerticalOffset;
            int per = (int)(now / max * 100);

            if (per < 95)
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
                await SetUserStatusesAsync(this.userId);
            }
            catch (TwitterException tex)
            {
                MessageBox.Show(tex.Message,
                    "Twitter Exception.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void listBox_TimeLine_KeyDown(object sender, KeyEventArgs e)
        {
            if (((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) != KeyStates.Down) &&
                ((Keyboard.GetKeyStates(Key.RightCtrl) & KeyStates.Down) != KeyStates.Down))
            {
                return;
            }

            if (e.Key == Key.C)
            {
                TimeLineItem target = this.listBox_TimeLine.SelectedItem as TimeLineItem;
                if (target == null)
                {
                    return;
                }

                string text = target.Text;
                Clipboard.SetDataObject(text, true);
            }
        }

        private void image_Icon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TimeLineItem target = ((sender as Image).TemplatedParent as ContentPresenter).Content as TimeLineItem;
            if (target == null)
            {
                return;
            }

            long id = (long)target.Tweet.User.Id;
            UserInfoWindow w = new UserInfoWindow(this.account, id);
            w.Owner = this.main;
            w.Show();
        }

        #endregion
    }
}
