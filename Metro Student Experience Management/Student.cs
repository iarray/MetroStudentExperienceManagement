using System;

namespace Metro_Student_Experience_Management
{
    [Serializable]
    public class Student
    {
        //_truantCount逃课次数
        private int _truantCount = 0;
        //学生经验值
        private uint _exp;
        //学生姓名
        private string _strName;
        //学生学号
        private string _strNum;

        public string Name
        {
            get
            {
                return _strName;
            }
        }

        public string Num
        {
            get
            {
                return _strNum;
            }
        }

        public uint Experience
        {
            get
            {
                if (_exp < 0) _exp = 0;
                return _exp;
            }

        }

        public int CountOfTruant
        {
            get
            {
                if (_truantCount < 0) _truantCount = 0;
                return _truantCount;
            }
        }

        public Student(string _tmpname, string _tmpnum)
        {
            _strName = _tmpname;
            _strNum = _tmpnum;
            _exp = 1000;
        }

        public Student(string _tmpname, string _tmpnum, int exp)
        {
             _strName = _tmpname;
            _strNum = _tmpnum;
            _exp = (uint)exp;
        }

        public Student()
        {
        }

        public void AddExperience(uint i)
        {
            _exp += i;
        }

        public void DeductExperience(uint i)
        {
            if (i > Experience) _exp = 0;
            else _exp -= i;
        }

        public void AddTruantCount()
        {
            _truantCount++;
        }

        public void ReduceTruantCount()
        {
            _truantCount--;
        }
    }
}
