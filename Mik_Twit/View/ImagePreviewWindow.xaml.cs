using MetroRadiance.Controls;
using Microsoft.Win32;
using Mik_Twit.Model;
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Mik_Twit.View
{
    /// <summary>
    /// ImagePreviewWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ImagePreviewWindow : MetroWindow
    {
        private ContentsItem item = null;
        private Point start;
        private Point origin;

        private SaveFileDialog sfd = new SaveFileDialog();

        public ImagePreviewWindow(ContentsItem item)
        {
            InitializeComponent();

            this.item = item;
            this.sfd.Filter = "JPGファイル(*.jpg)|*.jpg";

            this.Activate();
        }

        #region Event Handler

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.image_Preview.DataContext = this.item;

            this.Activate();
        }

        private void textBlock_Close_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void image_Preview_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var st = (ScaleTransform)this.image_Preview.RenderTransform;
            double zoom = e.Delta > 0 ? .2 : -.2;
            st.ScaleX += zoom;
            st.ScaleY += zoom;
        }

        private void image_Preview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.image_Preview.CaptureMouse();
            var tt = (TranslateTransform)((TransformGroup)this.image_Preview.RenderTransform)
                .Children.First(tr => tr is TranslateTransform);
            this.start = e.GetPosition(this.border);
            this.origin = new Point(tt.X, tt.Y);
        }

        private void image_Preview_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.image_Preview.IsMouseCaptured)
            {
                var tt = (TranslateTransform)((TransformGroup)this.image_Preview.RenderTransform)
                    .Children.First(tr => tr is TranslateTransform);
                Vector v = this.start - e.GetPosition(this.border);
                tt.X = this.origin.X - v.X;
                tt.Y = this.origin.Y - v.Y;
            }
        }

        private void image_Preview_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.image_Preview.ReleaseMouseCapture();
        }

        private void image_Download_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string fileName = this.item.ContentsUrl.Split('/').Last();
            this.sfd.FileName = fileName;
            bool? save = sfd.ShowDialog();
            if (save != true)
            {
                return;
            }

            try
            {
                using (WebClient client = new WebClient())
                {
                    string url = this.item.ContentsUrl;
                    client.DownloadFile(url, this.sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Web Exception.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        #endregion
    }
}
