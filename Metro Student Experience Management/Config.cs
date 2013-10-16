using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Metro_Student_Experience_Management
{
    [Serializable]
    class Config
    {
        private int rate;
        private int volume;
        private double opacity;
        private bool? isAutoHide;

        public int Rate
        {
            set
            {
                if (value > 0 && value <= 10) rate = value;
            }
            get
            {
                return rate;
            }
        }
        public int Volume
        {
            set
            {
                if (value >= 0 && value <= 100) volume = value;
            }
            get
            {
                return volume;
            }
        }
        public double Opacity
        {
            set
            {
                if (value >= 0 && value <= 1) opacity = value;
            }
            get
            {
                return opacity;
            }
        }
        public bool? IsAutoHide
        {
            set
            {
                isAutoHide = value;
            }
            get
            {
                return isAutoHide;
            }
        }
        public Config()
        {
            rate = 3;
            volume = 100;
            opacity = 1;
            isAutoHide = true;
        }
        public void SaveConfig()
        {
            try
            {
                FileStream fs = new FileStream(@"config.cfg", FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, this);
                fs.Close();
            }
            catch
            {
                throw new Exception("保存文件失败！");
            }
        }
        public static bool LoadConfig(ref Config cfg)
        {
            try
            {
                if (File.Exists(@"config.cfg") == false) return false;
                FileStream fs = new FileStream(@"config.cfg", FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                cfg = (Config)formatter.Deserialize(fs);
                fs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
