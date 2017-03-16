using CoreTweet;
using MetroRadiance.Controls;
using Mik_Twit.Model;
using Mik_Twit.Util;
using System.Collections.Generic;
using System.Windows;

namespace Mik_Twit.View
{
    /// <summary>
    /// TalkWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TalkWindow : MetroWindow
    {
        private List<Status> talk = new List<Status>();
        private Account account = null;
        private Status target = null;

        public TalkWindow(List<Status> talk)
        {
            InitializeComponent();
            this.talk = talk;

            this.Activate();
        }

        public TalkWindow(Account ac, Status target)
        {
            InitializeComponent();
            this.account = ac;
            this.target = target;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.timeLineViewer.Initialize(this.account, TimeLineMode.Talk);

                this.talk = await TwitterUtil.GetTalkAsync(this.account.TokensData, this.target);
                this.timeLineViewer.SetStatuses(this.talk);
            }
            catch (TwitterException tex)
            {
                MessageBox.Show(tex.Message,
                    "Twitter Exception.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            this.Activate();
        }
    }
}
