using CoreTweet;
using Mik_Twit.Common;
using Mik_Twit.Model;
using Mik_Twit.Util;
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
    /// TalkDmViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class TalkDmViewer : UserControl
    {
        private MainWindow main = null;
        private Window parent = null;
        private Account account = null;

        private bool isLoading = false;

        private string target = string.Empty;
        private long maxId = 0;

        private DispatchObservableCollection<DirectMessageItem> talkDm = new DispatchObservableCollection<DirectMessageItem>();

        public TalkDmViewer()
        {
            InitializeComponent();
        }

        public void Initialize(Window w, Account ac)
        {
            this.main = Application.Current.MainWindow as MainWindow;
            this.parent = w;
            this.account = ac;

            this.listBox_TalkDmList.DataContext = this.talkDm;
        }

        public void SetTalkDm(List<DirectMessage> dmList)
        {
            foreach (DirectMessage msg in dmList)
            {
                DirectMessageItem item = new DirectMessageItem(msg);
                this.talkDm.Add(item);
            }
        }

        public async Task SetTalkDmAsync(string screenName)
        {
            var talk = await TwitterUtil.GetTalkDmAsync(this.account.TokensData, screenName, this.maxId);
            if (talk.Count < 1)
            {
                return;
            }

            this.maxId = talk[talk.Count - 1].Id - 1;
            this.target = screenName;

            SetTalkDm(talk);
            this.isLoading = false;
        }

        #region Event Handler

        private async void listBox_TalkDmList_ScrollBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
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
                await SetTalkDmAsync(this.target);
            }
            catch (TwitterException tex)
            {
                MessageBox.Show(tex.Message,
                    "Twitter Exception.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void listBox_TalkDmList_KeyDown(object sender, KeyEventArgs e)
        {
            if (((Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) != KeyStates.Down) &&
                ((Keyboard.GetKeyStates(Key.RightCtrl) & KeyStates.Down) != KeyStates.Down))
            {
                return;
            }

            if (e.Key == Key.C)
            {
                DirectMessageItem target = this.listBox_TalkDmList.SelectedItem as DirectMessageItem;
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
            DirectMessageItem target = ((sender as Image).TemplatedParent as ContentPresenter).Content as DirectMessageItem;
            if (target == null)
            {
                return;
            }

            long id = (long)target.UserData.Id;
            UserInfoWindow w = new UserInfoWindow(this.account, id);
            w.Owner = this.main;
            w.Show();
        }

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

        #endregion
    }
}
