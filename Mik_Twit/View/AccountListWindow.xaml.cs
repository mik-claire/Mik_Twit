using CoreTweet;
using MetroRadiance.Controls;
using Mik_Twit.Common;
using Mik_Twit.Model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace Mik_Twit.View
{
    /// <summary>
    /// AccountListWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AccountListWindow : MetroWindow
    {
        private Account currentAccount = null;
        private Account[] accounts = null;

        public AccountListWindow(Account account)
        {
            InitializeComponent();

            this.currentAccount = account;

            this.Activate();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AccountManager am = AccountManager.GetInstance();
            Account[] accounts = am.GetAllAccount();
            this.accounts = accounts;

            List<User> users = new List<User>();
            foreach (Account ac in accounts)
            {
                if (ac.UserData != null)
                {
                    users.Add(ac.UserData);
                }
            }
            
            this.userListViewer_AccountList.Initialize(this, this.currentAccount, UserListMode.Account);
            this.userListViewer_AccountList.SetUserList(users);

            this.Activate();
        }
    }
}
