using CoreTweet;
using CoreTweet.Core;
using Mik_Twit.Common;
using Mik_Twit.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mik_Twit.View.Control
{
    /// <summary>
    /// DirectMessageListViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class DirectMessageListViewer : UserControl
    {
        private MainWindow main = null;
        private Window parent = null;
        private Account account = null;

        private DispatchObservableCollection<DirectMessageItem> dmList = new DispatchObservableCollection<DirectMessageItem>();

        public DirectMessageListViewer()
        {
            InitializeComponent();
        }

        public void Initialize(Window w, Account ac)
        {
            this.main = Application.Current.MainWindow as MainWindow;
            this.parent = w;
            this.account = ac;

            this.listBox_DmList.DataContext = this.dmList;
        }

        public void SetDirectMessage()
        {
            try
            {
                this.dmList.Clear();
                List<DirectMessage> dm = getDm();
                if (dm.Count < 1)
                {
                    return;
                }

                var map = new Dictionary<long?, DirectMessage>();
                var map2 = new Dictionary<DirectMessage, bool>();
                foreach (DirectMessage msg in dm)
                {
                    bool sentByMe = (long)msg.Sender.Id == this.account.UserId;
                    if (sentByMe)
                    {
                        if (map.ContainsKey(msg.Recipient.Id))
                        {
                            continue;
                        }

                        map[msg.Recipient.Id] = msg;
                        map2[msg] = true;
                    }
                    else
                    {
                        if (map.ContainsKey(msg.Sender.Id))
                        {
                            continue;
                        }

                        map[msg.Sender.Id] = msg;
                        map2[msg] = false;
                    }
                }

                foreach (DirectMessage msg in map2.Keys)
                {
                    DirectMessageItem item = new DirectMessageItem(msg, map2[msg]);
                    this.dmList.Add(item);
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

        private List<DirectMessage> getDm()
        {
            ListedResponse<DirectMessage> received = this.account.TokensData.DirectMessages.Received(count => 100);
            ListedResponse<DirectMessage> sent = this.account.TokensData.DirectMessages.Sent(count => 100);

            List<DirectMessage> dm1 = new List<DirectMessage>();
            dm1.AddRange(received);
            dm1.AddRange(sent);

            var query = dm1.OrderByDescending(x => x.CreatedAt);
            List<DirectMessage> dm2 = query.ToList<DirectMessage>();

            return dm2;
        }

        #region Event Handler

        private void textBlock_Reply_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DirectMessageItem target = ((sender as Image).TemplatedParent as ContentPresenter).Content as DirectMessageItem;
            if (target == null)
            {
                return;
            }

            this.main.textBox_Tweet.Text.Insert(0, "D " + target.ScreenName + " ");
            this.main.textBox_Tweet.Focus();
        }

        private void textBlock_Talk_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DirectMessageItem target = ((sender as Image).TemplatedParent as ContentPresenter).Content as DirectMessageItem;
                if (target == null)
                {
                    return;
                }

                TalkDmWindow w = new TalkDmWindow(this.account, target.ScreenName);
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

        private void listBox_DmList_KeyDown(object sender, KeyEventArgs e)
        {
            if (((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) != KeyStates.Down) &&
                ((Keyboard.GetKeyStates(Key.RightCtrl) & KeyStates.Down) != KeyStates.Down))
            {
                return;
            }

            if (e.Key == Key.C)
            {
                DirectMessageItem target = this.listBox_DmList.SelectedItem as DirectMessageItem;
                if (target == null)
                {
                    return;
                }

                string text = target.Text;
                Clipboard.SetDataObject(text, true);
            }
        }

        private void listBox_DmList_Item_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem selected = sender as ListBoxItem;
            if (selected == null)
            {
                return;
            }

            if (e.ClickCount == 2)
            {
                // double click
                DirectMessageItem target = selected.Content as DirectMessageItem;
                this.main.textBox_Tweet.Text.Insert(0, "D " + target.ScreenName + " ");
                this.main.textBox_Tweet.Focus();
                e.Handled = true;
            }
            else
            {
                if (selected.IsSelected)
                {
                    this.listBox_DmList.SelectedIndex = -1;
                    e.Handled = true;
                    return;
                }
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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        #endregion
    }
}
