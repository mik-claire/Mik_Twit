using CoreTweet;
using MetroRadiance.Controls;
using Mik_Twit.Common;
using Mik_Twit.Model;
using Mik_Twit.View.Control;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Mik_Twit.View
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public NotifyIconWrapper TaskIcon = null;

        private Account account = null;
        private List<string> uploadFiles = new List<string>();
        private InputTextItem inputText = new InputTextItem();
        private TitleTextItem title = null;

        private MediaItems media = new MediaItems();

        private Status replyTo = null;
        public Status ReplyTo
        {
            get { return this.replyTo; }
            set { this.replyTo = value; }
        }

        private ReplyToItem replyToItem = null;
        public ReplyToItem ReplyToItem
        {
            get { return this.replyToItem; }
            set { this.replyToItem = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task tweetAsync(Tokens token, string text, string[] uploadFiles)
        {
            this.tweetGrid.IsEnabled = false;

            try
            {
                List<MediaUploadResult> results = new List<MediaUploadResult>();
                foreach (string filePath in uploadFiles)
                {
                    MediaUploadResult result = await token.Media.UploadAsync(media: new FileInfo(filePath));
                    results.Add(result);
                }

                var param = new Dictionary<string, object>();
                param.Add("status", text);
                if (0 < results.Count)
                {
                    param.Add("media_ids", results.Select(x => x.MediaId));
                }
                if (this.replyTo != null)
                {
                    param.Add("in_reply_to_status_id", this.replyTo.Id.ToString());
                }

                await token.Statuses.UpdateAsync(param);

                this.textBox_Tweet.Clear();
                this.media.Clear();
                this.replyTo = null;
                this.replyToItem.Clear();
            }
            catch (TwitterException tex)
            {
                MessageBox.Show(tex.Message,
                    "Twitter Exception.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            finally
            {
                this.tweetGrid.IsEnabled = true;
            }
        }

        public void ChangeAccount()
        {
            this.timeLineViewer_Home.StopStreaming();
            this.timeLineViewer_Mention.StopStreaming();
            this.timeLineViewer_Fav.StopStreaming();
            // this.timeLineViewer_DM.StopStreaming();

            AccountManager am = AccountManager.GetInstance();
            Account ac = am.GetAccount(am.CurrentAccountIndex);
            this.account = ac;
            this.image_UserIcon.DataContext = this.account;
            this.timeLineViewer_Home.Initialize(this, ac, TimeLineMode.Home);
            this.timeLineViewer_Home.SetTimeLine();
            this.timeLineViewer_Home.StartStreaming();

            this.timeLineViewer_Mention.Initialize(this, ac, TimeLineMode.Mention);
            this.timeLineViewer_Mention.SetTimeLine();
            this.timeLineViewer_Mention.StartStreaming();

            this.timeLineViewer_Fav.Initialize(this, ac, TimeLineMode.Fav);
            this.timeLineViewer_Fav.SetTimeLine();
            this.timeLineViewer_Fav.StartStreaming();

            this.DmListViewer_DM.Initialize(this, ac);
            this.DmListViewer_DM.SetDirectMessage();
            // this.timeLineViewer_DM.StartStreaming();
            
            string titleText = "Mik_Twit - @" + ac.ScreenName;
            this.title.Title = titleText;
        }

        public void ReloadTimeLine()
        {
            this.timeLineViewer_Home.StopStreaming();
            this.timeLineViewer_Mention.StopStreaming();
            this.timeLineViewer_Fav.StopStreaming();

            this.timeLineViewer_Home.SetTimeLine();
            this.timeLineViewer_Home.StartStreaming();

            this.timeLineViewer_Mention.SetTimeLine();
            this.timeLineViewer_Mention.StartStreaming();

            this.timeLineViewer_Fav.SetTimeLine();
            this.timeLineViewer_Fav.StartStreaming();

            this.DmListViewer_DM.SetDirectMessage();
            // this.timeLineViewer_DM.StartStreaming();
        }

        #region Event Handler

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigManager.LoadConf();

            this.replyToItem = new ReplyToItem();
            this.DataContext = this.replyToItem;
            this.textBlock_Remain.DataContext = this.inputText;
            this.textBlock_Media.DataContext = this.media;

            AccountManager am = AccountManager.GetInstance();
            if (am.CurrentAccountIndex < 0)
            {
                AuthorizeWindow w = new AuthorizeWindow();
                bool? success = w.ShowDialog();
                if (success == true)
                {

                }
            }

            Account ac = am.GetAccount(am.CurrentAccountIndex);
            if (ac == null)
            {
                return;
            }

            this.account = ac;
            this.image_UserIcon.DataContext = this.account;

            this.timeLineViewer_Home.Initialize(this, this.account, TimeLineMode.Home);
            this.timeLineViewer_Home.SetTimeLine();
            this.timeLineViewer_Home.StartStreaming();

            this.timeLineViewer_Mention.Initialize(this, this.account, TimeLineMode.Mention);
            this.timeLineViewer_Mention.SetTimeLine();
            this.timeLineViewer_Mention.StartStreaming();

            this.timeLineViewer_Fav.Initialize(this, ac, TimeLineMode.Fav);
            this.timeLineViewer_Fav.SetTimeLine();
            this.timeLineViewer_Fav.StartStreaming();

            this.DmListViewer_DM.Initialize(this, ac);
            this.DmListViewer_DM.SetDirectMessage();
            // this.timeLineViewer_DM.StartStreaming();
            
            string titleText = "Mik_Twit - @" + this.account.ScreenName;
            this.title = new TitleTextItem(titleText);
            this.textBlock_Title.DataContext = title;

            this.TaskIcon = new NotifyIconWrapper();
            
        }

        private void textBox_Tweet_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.inputText.Input = this.textBox_Tweet.Text;
        }

        private async void textBox_Tweet_KeyDown(object sender, KeyEventArgs e)
        {
            if (((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) != KeyStates.Down) &&
                ((Keyboard.GetKeyStates(Key.RightCtrl) & KeyStates.Down) != KeyStates.Down))
            {
                return;
            }

            if (e.Key == Key.Enter)
            {
                if (inputText.Remain < 0)
                {
                    return;
                }

                await tweetAsync(this.account.TokensData, this.inputText.Input, this.media.Files.ToArray());
                this.textBox_Tweet.Focus();
            }
        }

        private void textBox_Tweet_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop, true))
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void textBox_Tweet_Drop(object sender, DragEventArgs e)
        {
            string filePath = ((string[])e.Data.GetData(DataFormats.FileDrop, false))[0];

            this.media.AddMedia(filePath);
        }

        private async void Button_Tweet_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.textBox_Tweet.Text))
            {
                return;
            }

            if (inputText.Remain < 0)
            {
                return;
            }

            await tweetAsync(this.account.TokensData, this.inputText.Input, this.media.Files.ToArray());
            this.textBox_Tweet.Focus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) != KeyStates.Down) &&
                ((Keyboard.GetKeyStates(Key.RightCtrl) & KeyStates.Down) != KeyStates.Down))
            {
                return;
            }

            if (e.Key == Key.Space)
            {
                CommandWindow w = new CommandWindow();
                bool? success = w.ShowDialog();
                if (success == true)
                {

                }

                e.Handled = true;
            }
        }

        private void textBlock_Media_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount < 2)
            {
                return;
            }

            this.media.Clear();
        }

        private void textBlock_ReplyTo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount < 2)
            {
                return;
            }

            this.replyToItem.Clear();
            this.replyTo = null;
        }

        private void image_UserIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AccountListWindow w = new AccountListWindow(this.account);
            w.Owner = this;
            w.Show();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AccountManager am = AccountManager.GetInstance();
            am.SaveAll();

            ConfigManager.SaveConfig();
            this.TaskIcon.Dispose();
        }

        #endregion

        private void MetroWindow_StateChanged(object sender, System.EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
            else
            {
                this.ShowInTaskbar = true;
            }
        }
    }
}
