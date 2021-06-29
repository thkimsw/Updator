using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Get_FileList
{
    public partial class Form1 : Form
    {
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
                                                        int size, string filePath);

        string Current_Path = System.Environment.CurrentDirectory;//현재 실행경로 가져오기

        public Form1()
        {
            InitializeComponent();
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Current_Path);

            DirFileSearch(Current_Path, "*");
            Application.ExitThread();
            Environment.Exit(0);
        }

        static void DirFileSearch(string path, string file)
        {
            int i = 0;
            string CP= System.Environment.CurrentDirectory;//현재 실행경로 가져오기
            try
            {
                string File_Path = Environment.CurrentDirectory;
                // \ 구분자로 키워드 구하기
                string spliter = Path.GetFullPath(Path.Combine(File_Path, @"..")).Split(@"\")[Path.GetFullPath(Path.Combine(File_Path, @"..")).Split(@"\").Length-1];
                string s_buf;
                string super_path = Environment.CurrentDirectory.ToString();
                string[] files = { "", };
                super_path = Directory.GetParent(super_path).ToString();
                files = Directory.GetFiles(super_path, "*.*", SearchOption.AllDirectories);
              
                int cnt=0;
                while (cnt < files.Length)
                {
                    s_buf = files[cnt].Split(spliter)[super_path.Split(spliter).Length-1];
                    WritePrivateProfileString("Files", "" + cnt.ToString(), s_buf, Environment.CurrentDirectory.ToString() + @"\" + "List.ini");
                    cnt++;
                }
                WritePrivateProfileString("Files","index",files.Length.ToString(), super_path + @"\" + "List.ini");
            }
            catch (Exception ex)
            {
            }
        }
    }
}
