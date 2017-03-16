using CoreTweet;
using MetroRadiance.Controls;
using Mik_Twit.Common;
using mshtml;
using System;
using System.Windows;

namespace Mik_Twit.View
{
    /// <summary>
    /// AuthorizeWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AuthorizeWindow : MetroWindow
    {
        #region Fields

        private CoreTweet.OAuth.OAuthSession session = null;

        #endregion

        public AuthorizeWindow()
        {
            InitializeComponent();

            this.session = OAuth.Authorize(SystemSettings.ConsumerKey, SystemSettings.ConsumerSecret);
            Uri uri = this.session.AuthorizeUri;

            this.webBrowser.Source = uri;
        }

        private bool load = false;

        #region Event Handler

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // this.load = true;
            this.Activate();
        }

        private void webBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!this.load)
            {
                this.load = true;
                return;
            }
            if (!this.load ||
                this.webBrowser.Document == null)
            {
                return;
            }
            HTMLDocument doc = this.webBrowser.Document as HTMLDocument;
            if (doc == null)
            {
                return;
            }

            IHTMLElementCollection collection = doc.getElementsByTagName("code");
            if (collection.length < 1)
            {
                return;
            }

            foreach (IHTMLElement element in collection)
            {
                string code = element.innerText;
                var am = AccountManager.GetInstance();
                bool success = am.AddNewAccount(this.session, code);
                if (!success)
                {
                    MessageBox.Show("You have already added this account.");
                    break;
                }
                else
                {
                    am.SetLeastAddedAccountIndex();
                    am.SaveAll();
                }
            }

            this.Close();
        }

        #endregion
    }
}
