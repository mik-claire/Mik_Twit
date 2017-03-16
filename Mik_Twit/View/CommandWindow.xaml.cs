using MetroRadiance.Controls;
using Mik_Twit.Common;
using Mik_Twit.Model;
using MikLib.Util;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Mik_Twit.View
{
    /// <summary>
    /// CommandWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CommandWindow : MetroWindow
    {
        public CommandWindow()
        {
            InitializeComponent();
        }

        private bool analyzeCommand(ArgumentMap amap)
        {
            List<string> main = amap.GetMainArgs();
            if (main[0] == "ac" ||
                main[0] == "account")
            {
                if (amap.HasSwitch("-n"))
                {
                    AuthorizeWindow w = new AuthorizeWindow();
                    bool? success = w.ShowDialog();
                    if (success == true)
                    {

                    }

                    AccountManager am = AccountManager.GetInstance();
                    Account ac = am.GetAccount(am.CurrentAccountIndex);
                    if (ac == null)
                    {
                        return false;
                    }

                    MainWindow mw = Application.Current.MainWindow as MainWindow;
                    if (mw != null)
                    {
                        mw.ChangeAccount();
                    }

                    return true;
                }

                if (amap.HasSwitch("-a"))
                {
                    AccountManager am = AccountManager.GetInstance();
                    Account ac = am.GetAccount(am.CurrentAccountIndex);
                    if (ac == null)
                    {
                        return false;
                    }

                    AccountListWindow w = new AccountListWindow(ac);
                    MainWindow mw = Application.Current.MainWindow as MainWindow;
                    if (mw != null)
                    {
                        w.Owner = mw;
                    }
                    w.Show();
                    return true;
                }

                int i = amap.GetOptionInt("-i", -1);
                if (i != -1)
                {
                    AccountManager am = AccountManager.GetInstance();
                    am.CurrentAccountIndex = i;
                    MainWindow mw = Application.Current.MainWindow as MainWindow;
                    if (mw != null)
                    {
                        mw.ChangeAccount();
                    }

                    return true;
                }

                if (main.Count == 2)
                {
                    AccountManager am = AccountManager.GetInstance();
                    am.SetCurrentIndexFromScreenName(main[1]);
                    MainWindow mw = Application.Current.MainWindow as MainWindow;
                    if (mw != null)
                    {
                        mw.ChangeAccount();
                    }

                    return true;
                }
            }
            else if (main[0] == "info")
            {
                AccountManager am = AccountManager.GetInstance();
                Account ac = am.GetAccount(am.CurrentAccountIndex);
                if (ac == null)
                {
                    return false;
                }

                string screenName = ac.ScreenName;
                if (main.Count == 2)
                {
                    screenName = main[1];
                }

                UserInfoWindow w = new UserInfoWindow(ac, screenName);
                MainWindow mw = Application.Current.MainWindow as MainWindow;
                if (mw != null)
                {
                    w.Owner = mw;
                }
                w.Show();
                return true;
            }
            else if (main[0] == "reload")
            {
                MainWindow mw = Application.Current.MainWindow as MainWindow;
                if (mw != null)
                {
                    mw.ReloadTimeLine();
                }

                return true;
            }
            else if (main[0] == "notification")
            {
                if (main.Count == 1)
                {
                    string message = string.Format("Notification: {0}",
                        ConfigManager.Config.Notification ? "On" : "Off");
                    MessageBox.Show(message,
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Asterisk);
                    return true;
                }

                if (main.Count != 2)
                {
                    return false;
                }

                if (main[1] == "on")
                {
                    ConfigManager.Config.Notification = true;
                    MessageBox.Show("Notification: On",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Asterisk);
                    return true;
                }
                else if (main[1] == "off")
                {
                    ConfigManager.Config.Notification = false;
                    MessageBox.Show("Notification: Off",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Asterisk);
                    return true;
                }
            }
            else if (main[0] == "face")
            {
                if (main.Count < 2)
                {
                    string message =
@"0: ｜ω・)
1: ( ˘ω˘)
2: ( ꒪ ω꒪)
3: (;꒪ ω꒪)
4: ~(=^･ω･^)ﾉ ﾆｬｰ!";
                    MessageBox.Show(message);
                    return false;
                }

                int index = 0;
                if (!int.TryParse(main[1], out index))
                {
                    return false;
                }

                MainWindow mw = Application.Current.MainWindow as MainWindow;
                if (mw == null)
                {
                    return false;
                }

                if (index == 0)
                {
                    mw.textBox_Tweet.Text += "｜ω・)";
                }
                else if (index == 1)
                {
                    mw.textBox_Tweet.Text += "( ˘ω˘)";
                }
                else if (index == 2)
                {
                    mw.textBox_Tweet.Text += "( ꒪ ω꒪)";
                }
                else if (index == 3)
                {
                    mw.textBox_Tweet.Text += "(;꒪ ω꒪)";
                }
                else if (index == 4)
                {
                    mw.textBox_Tweet.Text += "~(=^･ω･^)ﾉ ﾆｬｰ!";
                }
                mw.textBox_Tweet.Focus();

                return true;
            }

            return false;
        }

        #region Event Handler

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.textBox_Command.Focus();
        }

        private void textBox_Command_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }

            if (e.Key != Key.Enter)
            {
                return;
            }

            string[] args = this.textBox_Command.Text.Trim().Split(' ');
            ArgumentMap amap = new ArgumentMap();
            amap.Init(args);

            bool close = analyzeCommand(amap);
            e.Handled = true;
            if (close)
            {
                this.Close();
            }
        }

        #endregion
    }
}
