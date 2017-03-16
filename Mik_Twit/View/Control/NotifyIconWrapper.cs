using Mik_Twit.Common;
using Mik_Twit.Model;
using Mik_Twit.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Application = System.Windows.Application;
using WindowState = System.Windows.WindowState;

namespace Mik_Twit.View.Control
{
    public partial class NotifyIconWrapper : System.Windows.Forms.Control
    {
        public NotifyIconWrapper()
        {
            InitializeComponent();

            this.taskIcon.DoubleClick += taskIcon_DoubleClick;
            this.menuItem_Show.Click += menuItem_Show_Click;
            this.menuItem_Exit.Click += menuItem_Exit_Click;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public void ShowBaloon(string text, string userName, NotifyMode mode)
        {
            if (!ConfigManager.Config.Notification)
            {
                return;
            }

            string title = string.Empty;
            switch (mode)
            {
                case NotifyMode.Reply:
                    title = string.Format("You got a reply from {0}.", userName);
                    break;
                case NotifyMode.Rt:
                    title = string.Format("Retweeted by {0}.", userName);
                    break;
                case NotifyMode.Qt:
                    title = string.Format("Quoted by {0}.", userName);
                    break;
                case NotifyMode.Fav:
                    title = string.Format("Favorited by {0}.", userName);
                    break;
                case NotifyMode.Dm:
                    title = string.Format("You got a direct-message by {0}.", userName);
                    break;
                case NotifyMode.Follow:
                    title = string.Format("You followed by {0}.", userName);
                    break;
                default:
                    title = "Notification.";
                    break;
            }

            this.taskIcon.BalloonTipTitle = title;
            this.taskIcon.BalloonTipText = text;
            this.taskIcon.ShowBalloonTip(5);
        }

        #region Event Handler

        private void taskIcon_DoubleClick(object sender, EventArgs e)
        {
            MainWindow main = Application.Current.MainWindow as MainWindow;
            main.WindowState = WindowState.Normal;
            main.Show();
        }

        private void menuItem_Show_Click(object sender, EventArgs e)
        {
            MainWindow main = Application.Current.MainWindow as MainWindow;
            main.WindowState = WindowState.Normal;
            main.Show();
        }

        private void menuItem_Exit_Click(object sender, EventArgs e)
        {
            MainWindow main = Application.Current.MainWindow as MainWindow;
            main.Close();
        }

        #endregion
    }
}
