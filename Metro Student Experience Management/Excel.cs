using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Metro_Student_Experience_Management
{
    class Excel
    {
        private string _filePath = null;
        private int _columnsOfName = -1;
        private int _columnsOfId = -1;
        private int _columnsOfExp = -1;
        public Excel(string FilePath)
        {
            _filePath = FilePath;
        }
        public bool ContentToStudents(ref Students stds)
        {
            bool _isSuccess = true;
            if (File.Exists(_filePath) == false)
            {
                throw new Exception("文件路径错误或文件不存在");
            }
            try
            {
                string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _filePath + ";Extended Properties='Excel 8.0;HDR=False;IMEX=1'";
                OleDbConnection odc = new OleDbConnection(strConn);
                odc.Open();
                String sql = "SELECT * FROM  [Sheet1$]";
                OleDbDataAdapter OleDaExcel = new OleDbDataAdapter(sql, odc);
                DataSet OleDsExcle = new DataSet();
                OleDaExcel.Fill(OleDsExcle, "Sheet1");
                odc.Close();
                DataTable dt = OleDsExcle.Tables[0];
                //获取姓名，学号，经验值所对应的列编号
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (string.Compare(dt.Columns[i].ToString(), "姓名") == 0) _columnsOfName = i;
                    else if (string.Compare(dt.Columns[i].ToString(), "学号") == 0) _columnsOfId = i;
                    else if (string.Compare(dt.Columns[i].ToString(), "经验值") == 0) _columnsOfExp = i;
                }
                #region 处理数据，存放至Students类中
                foreach (DataRow dc in dt.Rows)
                {
                    if (dc[0].ToString() == "") continue;
                    if (_columnsOfExp == -1)
                    {
                        if (_columnsOfName != -1 && _columnsOfId != -1)
                        {
                            stds.Add(new Student(dc[_columnsOfName].ToString(), dc[_columnsOfId].ToString()));
                        }
                        else if (_columnsOfName != -1)
                        {
                            stds.Add(new Student(dc[_columnsOfName].ToString(), "None"));
                        }
                        else
                        {
                            stds.Add(new Student("None", "0"));
                        }
                    }
                    else
                    {
                        int tmpExp = 1000;
                        try
                        {
                            tmpExp = Convert.ToInt32(dc[_columnsOfExp].ToString());
                        }
                        catch
                        {
                            tmpExp = 1000;
                        }

                        if (_columnsOfName != -1 && _columnsOfId != -1)
                        {
                            stds.Add(new Student(dc[_columnsOfName].ToString(), dc[_columnsOfId].ToString(), tmpExp));
                        }
                        else if (_columnsOfName != -1)
                        {
                            stds.Add(new Student(dc[_columnsOfName].ToString(), "None", tmpExp));
                        }
                        else
                        {
                            stds.Add(new Student("None", "0", tmpExp));
                        }
                    }
                }
            }
                #endregion
            catch
            {
                _isSuccess = false;
            }

            return _isSuccess;
        }
    }
    class Log
    {
        public static void SaveLog(List<string> tmplist)
        {
            StreamWriter sw = new StreamWriter(@"Log.txt", false);
            foreach (string tmp in tmplist)
            {
                sw.WriteLine(tmp);
            }
            sw.Close();
        }
        public static void LoadLog(List<string> tmplist)
        {
            if (File.Exists(@"Log.txt") == false) return;
            StreamReader sr = new StreamReader(@"Log.txt");
            while (!sr.EndOfStream)
            {
                tmplist.Add(sr.ReadLine());
            }
            sr.Close();
        }
    }
}
