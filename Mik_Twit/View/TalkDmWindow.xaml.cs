using CoreTweet;
using MetroRadiance.Controls;
using Mik_Twit.Model;
using Mik_Twit.Util;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Mik_Twit.View
{
    /// <summary>
    /// TalkDmWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TalkDmWindow : MetroWindow
    {
        private Account account = null;
        private string target = string.Empty;

        public TalkDmWindow(Account ac, string target)
        {
            InitializeComponent();

            this.account = ac;
            this.target = target;

            this.Activate();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.talkDmViewer.Initialize(this, this.account);

                List<DirectMessage> talk = await TwitterUtil.GetTalkDmAsync(this.account.TokensData, this.target);
                this.talkDmViewer.SetTalkDm(talk);

                await this.talkDmViewer.SetTalkDmAsync(this.target);
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
