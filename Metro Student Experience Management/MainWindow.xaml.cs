using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using DotNetSpeech;
using System.Collections.Generic;
using System.Text;


namespace Metro_Student_Experience_Management
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Students students = new Students();
        private BinaryFormatter formatter = new BinaryFormatter();
        private Config config = new Config();
        private Stack<Students> _revocation = new Stack<Students>();
        //声明托盘图标
        private System.Windows.Forms.NotifyIcon notifyIcon = null;
        private SpeechVoiceSpeakFlags speechFlags = SpeechVoiceSpeakFlags.SVSFlagsAsync;
        private SpVoice speechVoice = new SpVoice();
        private List<string> log=new List<string>();
        private double wWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ShowInTaskbar = false;
            //设置托盘的各个属性
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Text = "Student Experience Management";
            notifyIcon.Icon = Properties.Resources.AppIco;
            notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(notifyIcon_MouseClick);
            notifyIcon.Visible = true;
            //得到屏幕工作区域宽度
            wWindow = SystemParameters.WorkArea.Width;
            Students.LoadData(ref students); 
            Config.LoadConfig(ref config);
            Log.LoadLog(log);
            listviewStudents.ItemsSource = students.ReturnList();
            sliderRate.Value = config.Rate;
            sliderVoice.Value = config.Volume;
            sliderOpacity.Value = 1 - config.Opacity;
            checkboxHide.IsChecked = config.IsAutoHide;
        }

        private void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    this.Activate();
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (MessageWindow.Show("提示", "确定退出程序？") == true)
                {
                    this.Close();
                }
                else return;
            }
        }

        private void Window_Drop_1(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;
            else e.Effects = DragDropEffects.None;
            string _filePath = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            if (_filePath.LastIndexOf(".xls") != -1)
            {
                students.RemoveAll();
                Excel excel = new Excel(_filePath);
                excel.ContentToStudents(ref students);
                listviewStudents.ItemsSource = students.ReturnList();
            }
            else if (_filePath.LastIndexOf(".dat") != -1)
            {
                students.RemoveAll();
                Students.LoadDataTo(_filePath, ref students);
                listviewStudents.ItemsSource = students.ReturnList();
            }
            else
            {
                return;
            }
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            students.SaveData();
            config.SaveConfig();
            Log.SaveLog(log);
            notifyIcon.Visible = false;
        }

        private void Window_MouseEnter_1(object sender, MouseEventArgs e)
        {
            if (checkboxHide.IsChecked == false) return;
            if (this.Top > 0) return;
            if (this.Top < 0)
            {
                this.Top = -8;
                this.Topmost = false;
            }
        }

        private void Window_MouseLeave_1(object sender, MouseEventArgs e)
        {
            if (checkboxHide.IsChecked == false) return;
            if (this.Top > 5) return;
            if (this.Top <= 5)
            {
                this.Top = -400;
                this.Topmost = true;
            }
        }

        private void Window_LocationChanged_1(object sender, EventArgs e)
        {
            if (this.Left < 0)
            {
                this.Left = 0;
            }
            if (this.Left > wWindow - 253)
            {
                this.Left = wWindow - 253;
            }
        }

        private void WindowLButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }

        private void BtnChek_Click_1(object sender, RoutedEventArgs e)
        {
            speechVoice.Rate = (int)sliderRate.Value;
            StringBuilder skippingStudents = new StringBuilder(200);
            int countOfSkipping=0;
            for (int i = 0; i < students.Length; i++)
            {
                speechVoice.Speak(students[i].Name, speechFlags);
                if (MessageWindow.Show("提示", students[i].Name + "是否到了") == true)
                {
                    students[i].AddExperience(100);
                }
                else
                {
                    students[i].DeductExperience(100);
                    students[i].AddTruantCount();
                    countOfSkipping++;
                    skippingStudents.Append(students[i].Name+"  ");
                }
                
            }
            listviewStudents.ItemsSource = null;
            listviewStudents.ItemsSource = students.ReturnList();
            if (countOfSkipping > 0)
            {
                string tmp = "本次考勤未到同学有" + countOfSkipping.ToString() + "人,分别是:\t" + skippingStudents.ToString();
                log.Add(DateTime.Today.ToShortDateString()+"  "+ tmp);
                speechVoice.Speak(tmp, speechFlags);
                MessageWindow.Show("提示", tmp);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnOpenSave_Click_1(object sender, RoutedEventArgs e)
        {
            flyoutOpenSave.IsOpen = !flyoutOpenSave.IsOpen;
            tbResult.Text = "";
        }

        private void BtnMan_Click(object sender, RoutedEventArgs e)
        {
            flyoutMan.IsOpen = !flyoutMan.IsOpen;
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            int _index = listviewStudents.SelectedIndex;
            if (_index == -1) return;
            _revocation.Push(students.GetDuplicate());
            students[_index].AddExperience(uint.Parse(tbExpIncremental.Text));
            listviewStudents.ItemsSource = null;
            listviewStudents.ItemsSource = students.ReturnList();
        }

        private void ReduceClick(object sender, RoutedEventArgs e)
        {
            int _index = listviewStudents.SelectedIndex;
            if (_index == -1) return;
            _revocation.Push(students.GetDuplicate());
            students[_index].DeductExperience(uint.Parse(tbExpReduction.Text));
            listviewStudents.ItemsSource = null;
            listviewStudents.ItemsSource = students.ReturnList();
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            int _index = listviewStudents.SelectedIndex;
            if (_index == -1) return;
            _revocation.Push(students.GetDuplicate());
            students.RemoveAt(_index);
            listviewStudents.ItemsSource = null;
            listviewStudents.ItemsSource = students.ReturnList();
        }

        private void InsertClick(object sender, RoutedEventArgs e)
        {
            InsertStudentDialog isd = new InsertStudentDialog();
            if (isd.ShowDialog() == true)
            {
                int _index = listviewStudents.SelectedIndex;
                if (_index == -1) return;
                _revocation.Push(students.GetDuplicate());
                students.Insert(_index, isd.GetStudent());
                listviewStudents.ItemsSource = null;
                listviewStudents.ItemsSource = students.ReturnList();
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (students.Length != 0)
            {
                int _index = students.IndexOf(txtboxSearch.Text);
                listviewStudents.SelectedIndex = _index;
                listviewStudents.ScrollIntoView(listviewStudents.Items[_index]);
            }
        }

        private void SaveBtnStyle_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "数据文件|*.dat";
            if (sfd.ShowDialog() == true)
            {
                students.SaveDataTo(sfd.FileName);
                tbResult.Text = " 导出文件成功！";
            }
        }

        private void OpenBthStyle_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "支持格式|*.xls;*.dat|Excel文件|*.xls|数据文件|*.dat";
            ofd.FilterIndex = 0;
            if (ofd.ShowDialog() == true)
            {
                students.RemoveAll();
                if (ofd.FileName.LastIndexOf(".xls") != -1)
                {
                    Excel excel = new Excel(ofd.FileName);
                    excel.ContentToStudents(ref students);
                    listviewStudents.ItemsSource = students.ReturnList();
                }
                else
                {
                    Students.LoadDataTo(ofd.FileName, ref students);
                    listviewStudents.ItemsSource = students.ReturnList();
                }
                tbResult.Text = " 导入文件成功！";
            }
        }

        private void tbResult_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            tbResult.Text = "";
        }

        private void BtnSet_Click_1(object sender, RoutedEventArgs e)
        {
            flyoutSetting.IsOpen = !flyoutSetting.IsOpen;
        }

        private void BtnLog_Click_1(object sender, RoutedEventArgs e)
        {
            flyoutLog.IsOpen = !flyoutLog.IsOpen;
            listboxLog.ItemsSource = null;
            listboxLog.ItemsSource = this.log;
        }

        private void sliderOpacityValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            flyoutMan.Opacity =1 - sliderOpacity.Value;
            flyoutOpenSave.Opacity =1 - sliderOpacity.Value;
            flyoutLog.Opacity = 1 - sliderOpacity.Value;
            config.Opacity = flyoutSetting.Opacity = 1 - sliderOpacity.Value;
        }

        private void sliderVoice_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           config.Volume= speechVoice.Volume =(int) sliderVoice.Value;
        }

        private void sliderRate_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           config.Rate= speechVoice.Rate = (int)sliderRate.Value;
        }

        private void checkboxHide_Click(object sender, RoutedEventArgs e)
        {
            config.IsAutoHide = checkboxHide.IsChecked;
        }

        private void AddSkipClick(object sender, RoutedEventArgs e)
        {
            int _index = listviewStudents.SelectedIndex;
            if (_index == -1) return;
            _revocation.Push(students.GetDuplicate());
            students[_index].DeductExperience(100);
            students[_index].AddTruantCount();
            listviewStudents.ItemsSource = null;
            listviewStudents.ItemsSource = students.ReturnList();
        }

        private void ReduceSkipClick(object sender, RoutedEventArgs e)
        {
            int _index = listviewStudents.SelectedIndex;
            if (_index == -1) return;
            _revocation.Push(students.GetDuplicate());
            students[_index].AddExperience(100);
            students[_index].ReduceTruantCount();
            listviewStudents.ItemsSource = null;
            listviewStudents.ItemsSource = students.ReturnList();
        }

        private void RemoveLogClick(object sender, RoutedEventArgs e)
        {
            int _index = listboxLog.SelectedIndex;
            if (_index == -1) return;
            log.RemoveAt(_index);
            listboxLog.ItemsSource = null;
            listboxLog.ItemsSource = log;
        }

        private void RemoveAllLogClick(object sender, RoutedEventArgs e)
        {
            log.Clear();
            listboxLog.ItemsSource = null;
            listboxLog.ItemsSource = log;
        }

        private void IDSortClick(object sender, RoutedEventArgs e)
        {
            this.students.Sort(SortDepend.StudentID);
            listviewStudents.ItemsSource = null;
            listviewStudents.ItemsSource = students.ReturnList();
        }

        private void ExpSortClick(object sender, RoutedEventArgs e)
        {
            this.students.Sort(SortDepend.Experience);
            listviewStudents.ItemsSource = null;
            listviewStudents.ItemsSource = students.ReturnList();
        }

        private void TruantSortClick(object sender, RoutedEventArgs e)
        {
            this.students.Sort(SortDepend.TruantCount);
            listviewStudents.ItemsSource = null;
            listviewStudents.ItemsSource = students.ReturnList();
        }

        private void UndoClick(object sender, RoutedEventArgs e)
        {
            if (_revocation.Count == 0) return;
            students = _revocation.Pop();
            listviewStudents.ItemsSource = null;
            listviewStudents.ItemsSource = students.ReturnList();
        }
    }
}
