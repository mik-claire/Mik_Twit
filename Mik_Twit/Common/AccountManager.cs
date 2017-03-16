using CoreTweet;
using Mik_Twit.Model;
using Mik_Twit.Properties;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Mik_Twit.Common
{
    public sealed class AccountManager
    {
        private static AccountManager _am = new AccountManager();
        public static AccountManager GetInstance()
        {
            return _am;
        }

        private List<Account> accountList = new List<Account>();
        private int currentAccountIndex = -1;
        public int CurrentAccountIndex
        {
            get { return this.currentAccountIndex; }
            set { this.currentAccountIndex = value; }
        }

        public AccountManager()
        {
            List<string[]> acs = getAccounts();
            foreach (string[] ac in acs)
            {
                string userId = ac[0];
                string screenName = ac[1];
                string name = ac[2];
                string accessToken = ac[3];
                string accessTokenSecret = ac[4];

                Tokens token = Tokens.Create(
                    SystemSettings.ConsumerKey,
                    SystemSettings.ConsumerSecret,
                    accessToken,
                    accessTokenSecret);
                User user = token.Account.VerifyCredentials();
                Account account = new Account(token, user);
                this.accountList.Add(account);
            }

            if (0 < this.accountList.Count)
            {
                if (Settings.Default.CurrentAccountIndex < this.accountList.Count)
                {
                    this.currentAccountIndex = Settings.Default.CurrentAccountIndex;
                }
                else
                {
                    this.currentAccountIndex = 0;
                }
            }
        }

        public bool AddNewAccount(CoreTweet.OAuth.OAuthSession session, string pin)
        {
            Tokens tokens = OAuth.GetTokens(session, pin);
            var query = from x in this.accountList
                        where x.ScreenName == tokens.ScreenName
                        select x;
            if (0 < query.Count())
            {
                return false;
            }

            User user = tokens.Account.VerifyCredentials();
            Account account = new Account(tokens, user);

            this.accountList.Add(account);
            return true;
        }

        public Account GetAccount(string screenName)
        {
            foreach (Account account in this.accountList)
            {
                if (screenName != account.ScreenName)
                {
                    continue;
                }

                return account;
            }

            return null;
        }

        public Account GetAccount(int accountIndex)
        {
            if (accountIndex < 0 ||
                this.accountList.Count <= accountIndex)
            {
                return null;
            }

            return this.accountList[accountIndex];
        }

        public Account[] GetAllAccount()
        {
            return this.accountList.ToArray();
        }

        public void Save(Account account)
        {
            StringCollection tokens = Settings.Default.TokenList;
            for (int i = 0; i < tokens.Count; i++)
            {
                string[] token = tokens[i].Split(',');
                if (account.UserId.ToString() != token[0])
                {
                    continue;
                }

                string setting = string.Format("{0},{1},{2},{3},{4}",
                    account.UserId,
                    account.ScreenName,
                    account.Name,
                    account.AccessToken,
                    account.AccessTokenSecret);

                tokens[i] = setting;
                Settings.Default.TokenList = tokens;
                Settings.Default.CurrentAccountIndex = this.currentAccountIndex;
                break;
            }

            Settings.Default.Save();
        }

        public void SaveAll()
        {
            StringCollection col = new StringCollection();
            foreach (Account ac in this.accountList)
            {
                string setting = string.Format("{0},{1},{2},{3},{4}",
                    ac.UserId,
                    ac.ScreenName,
                    ac.Name,
                    ac.AccessToken,
                    ac.AccessTokenSecret);

                col.Add(setting);
            }

            Settings.Default.TokenList = col;
            Settings.Default.CurrentAccountIndex = this.currentAccountIndex;
            Settings.Default.Save();
        }

        private List<string[]> getAccounts()
        {
            List<string[]> accounts = new List<string[]>();
            if (Settings.Default.TokenList == null)
            {
                return accounts;
            }

            StringCollection tokens = Settings.Default.TokenList;
            foreach (string token in tokens)
            {
                string[] tokenData = token.Split(',');
                accounts.Add(tokenData);
            }

            return accounts;
        }

        public void SetLeastAddedAccountIndex()
        {
            int leastIndex = this.accountList.Count - 1;
            this.currentAccountIndex = leastIndex;
        }

        public void SetCurrentIndexFromScreenName(string screenName)
        {
            for (int i = 0; i < this.accountList.Count; i++)
            {
                if (this.accountList[i].ScreenName != screenName)
                {
                    continue;
                }

                this.currentAccountIndex = i;
                return;
            }
        }
    }
}
