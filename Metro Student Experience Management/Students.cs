using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Metro_Student_Experience_Management
{
    enum SortDepend
    {
        StudentID,
        Experience,
        TruantCount
    }
    [Serializable]
    class Students
    {
        protected int _size;
        private Student[] _students;
        public int Length
        {
            get
            {
                if (_size < 0) _size = 0;
                return _size;
            }
        }
        //设置索引器
        public Student this[int _index]
        {
            set
            {
                if (_index >= 0 && _index < _size)
                {
                    this._students[_index] = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
                }
            }
            get
            {
                if (_index >= _size)
                {
                    throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");
                }
                return this._students[_index];
            }
        }
        public Students()
        {
            _students = new Student[0];
            _size = 0;
        }
        //为集合添加元素
        public int Add(Student _tmpsd)
        {
            if (_size == 0)
            {
                _students = new Student[_size + 1];
                _students[0] = new Student();
                _students[0] = _tmpsd;
            }
            else if (_size > 0)
            {
                Student[] _tmpstds = _students;
                _students = new Student[_size + 1];
                for (int i = 0; i < _size; i++)
                {
                    _students[i] = _tmpstds[i];
                }
                _students[_size] = _tmpsd;
            }
            return _size++;
        }
        //移除集合中的某一项
        public void RemoveAt(int _index)
        {
            if (_index >= 0 && _index < _size)
            {
                Student[] _tmpstd = _students;
                _students = new Student[--_size];
                for (int i = 0; i < _index; i++)
                {
                    _students[i] = _tmpstd[i];
                }
                for (int i1 = _index; i1 < _size; i1++)
                {
                    _students[i1] = _tmpstd[i1 + 1];
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_Index");

            }
        }
        //对集合插入一个元素
        public void Insert(int index, Student value)
        {
            if (index == _size)
            {
                Add(value);
                return;
            }
            else if (index < _size && index >= 0)
            {
                Student[] tmpstd = _students;
                _students = new Student[++_size];
                for (int i = 0, j = 0; i < _size; i++, j++)
                {
                    if (i == index)
                    {
                        _students[i] = value;
                        j = i - 1;
                    }
                    else _students[i] = tmpstd[j];
                }
            }
            else if (index < 0 || index > _size)
            {
                throw new ArgumentOutOfRangeException("index", "ArgumentOutOfRange_ArrayListInsert");
            }
        }
        //查找某一项返回该项索引，找不到则返回-1
        public int IndexOf(Student value)
        {
            for (int index = 0; index < _size; index++)
            {
                if (string.Compare(_students[index].Name, value.Name) == 0 &&
                    _students[index].Num == value.Num)
                {
                    return index;
                }
            }
            return -1;
        }
        //按名字或学号查找
        public int IndexOf(string value)
        {
            for (int index = 0; index < _size; index++)
            {
                if (string.Compare(_students[index].Name, value) == 0)
                {
                    return index;
                }
                if (string.Compare(_students[index].Num, value) == 0) return index;
            }
            return -1;
        }
        public void RemoveAll()
        {
            _students = new Student[0];
            _size = 0;
        }
        public Student[] ReturnList()
        {
            return this._students;
        }
        public void SaveDataTo(string filePath)
        {
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, this);
                fs.Close();
            }
            catch
            {
                throw new Exception("保存文件失败！");
            }
        }
        public void SaveData()
        {
            SaveDataTo(@"info.dat");
        }
        public static void LoadData(ref Students sds)
        {
            LoadDataTo(@"info.dat", ref sds);
        }
        public static void LoadDataTo(string filePath, ref Students sds)
        {
            try
            {
                if (File.Exists(filePath) == false) return;
                FileStream fs = new FileStream(filePath, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                sds = (Students)formatter.Deserialize(fs);
                fs.Close();
            }
            catch
            {
                return;
            }
        }
        public int CompareOfExp(Student std1, Student std2)
        {
            return (int)std1.Experience - (int)std2.Experience;
        }
        public long CompareOfStudentID(Student std1, Student std2)
        {
            long std1_id = long.Parse(std1.Num);
            long std2_id = long.Parse(std2.Num);
            return std1_id - std2_id;
        }
        public int CompareOfTruant(Student std1, Student std2)
        {
            return std1.CountOfTruant - std2.CountOfTruant;
        }
        public void Swap(ref Student std1, ref Student std2)
        {
            Student tmp = std1;
            std1 = std2;
            std2 = tmp;
        }
        public void Sort(SortDepend sortdep)
        {
            switch (sortdep)
            {
                case SortDepend.StudentID:
                    for (int gap = _students.Length / 2; gap > 0; gap /= 2)
                        for (int i = gap; i < _students.Length; i++)
                            for (int j = i - gap; j >= 0 && CompareOfStudentID(_students[j], _students[j + gap]) > 0; j -= gap)
                            {
                                Swap(ref _students[j], ref _students[j + gap]);
                            }  
                    break;
                case SortDepend.Experience:
                    for (int gap = _students.Length / 2; gap > 0; gap /= 2)
                        for (int i = gap; i < _students.Length; i++)
                            for (int j = i - gap; j >= 0 && CompareOfExp(_students[j], _students[j + gap]) < 0; j -= gap)
                            {
                                Swap(ref _students[j],ref  _students[j + gap]);
                            }  
                    break;
                case SortDepend.TruantCount:
                    for (int gap = _students.Length / 2; gap > 0; gap /= 2)
                        for (int i = gap; i < _students.Length; i++)
                            for (int j = i - gap; j >= 0 && CompareOfTruant(_students[j], _students[j + gap]) < 0; j -= gap)
                            {
                                Swap(ref _students[j], ref  _students[j + gap]);
                            }  
                    break;
            }
        }
        public Students GetDuplicate()
        {
            return (Students)this.MemberwiseClone();
        }
    }
}
