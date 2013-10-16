using System.Windows;
using System.Windows.Input;

namespace Metro_Student_Experience_Management
{
    /// <summary>
    /// MessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow()
        {
            InitializeComponent();
        }
        public new string Title
        {

            get { return this.lblTitle.Text; }

            set { this.lblTitle.Text = value; }

        }



        public string Message
        {

            get { return this.lblMsg.Text; }

            set { this.lblMsg.Text = value; }

        }

        /// <summary>

        /// 静态方法 模拟MESSAGEBOX.Show方法

        /// </summary>

        /// <param name="title">窗口标题</param>

        /// <param name="msg">窗口消息</param>

        /// <returns></returns>

        public static bool? Show(string title, string msg)
        {

            var msgBox = new MessageWindow();

            msgBox.Title = title;

            msgBox.Message = msg;

            return msgBox.ShowDialog();

        }



        private void Yes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            this.DialogResult = true;

            this.Close();

        }





        private void No_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            this.DialogResult = false;

            this.Close();

        }

        private void MessageWindowMouseLButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }
    }
}
