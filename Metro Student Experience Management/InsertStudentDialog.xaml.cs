using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Metro_Student_Experience_Management
{
    /// <summary>
    /// InsertStudentDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InsertStudentDialog : Window
    {
        public InsertStudentDialog()
        {
            InitializeComponent();
        }
        private Student std = null;
        private void MessageWindowMouseLButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }
        public Student GetStudent()
        {
            return std;
        }
        private void btnYes_Click_1(object sender, RoutedEventArgs e)
        {
            if (tbExp.Text != "" && tbName.Text != "" && tbNum.Text != "" &&
                tbExp.Text != null && tbName.Text != null && tbNum.Text != null)
            {
                std = new Student(tbName.Text, tbNum.Text, int.Parse(tbExp.Text));

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageWindow.Show("警告", "姓名，学号，经验不能为空");
            }
            
        }

        private void btnNo_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void TBTextChanged(object sender, TextChangedEventArgs e)
        {
            long _num = -1;
            TextBox tb = (TextBox)sender;
            if(long.TryParse(tb.Text,out _num)==false)
            {
                tb.Text = "";
            }
        }
    }
}
