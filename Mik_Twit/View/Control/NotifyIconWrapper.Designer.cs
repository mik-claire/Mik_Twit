namespace Mik_Twit.View.Control
{
    partial class NotifyIconWrapper
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotifyIconWrapper));
            this.taskIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.taskMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItem_Show = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.taskMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // taskIcon
            // 
            this.taskIcon.ContextMenuStrip = this.taskMenu;
            this.taskIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("taskIcon.Icon")));
            this.taskIcon.Text = "Mik_Twit";
            this.taskIcon.Visible = true;
            // 
            // taskMenu
            // 
            this.taskMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem_Show,
            this.menuItem_Exit});
            this.taskMenu.Name = "taskMenu";
            this.taskMenu.Size = new System.Drawing.Size(99, 48);
            // 
            // menuItem_Show
            // 
            this.menuItem_Show.Name = "menuItem_Show";
            this.menuItem_Show.Size = new System.Drawing.Size(98, 22);
            this.menuItem_Show.Text = "表示";
            // 
            // menuItem_Exit
            // 
            this.menuItem_Exit.Name = "menuItem_Exit";
            this.menuItem_Exit.Size = new System.Drawing.Size(98, 22);
            this.menuItem_Exit.Text = "終了";
            this.taskMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon taskIcon;
        private System.Windows.Forms.ContextMenuStrip taskMenu;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Show;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Exit;
    }
}
