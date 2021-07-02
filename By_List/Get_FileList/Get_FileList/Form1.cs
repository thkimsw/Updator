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
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

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
            Double version = 0.0 ;
            StringBuilder Cur_V = new StringBuilder(255);
        
            string CP= System.Environment.CurrentDirectory;//현재 실행경로 가져오기
            try
            {
                string File_Path = Environment.CurrentDirectory;
                // \ 구분자로 키워드 구하기
                string spliter = Path.GetFullPath(Path.Combine(File_Path, @"..")).Split(@"\")[Path.GetFullPath(Path.Combine(File_Path, @"..")).Split(@"\").Length - 1];
                string s_buf, d_buf;
                string super_path = Path.GetDirectoryName(Environment.CurrentDirectory);
                string[] files = { "", }; //파일이름 목록
                string[] days = { "", }; //날짜 목록
                super_path = Directory.GetParent(File_Path).ToString();
                int cnt = 0;
                string masking = Path.GetFullPath(Path.Combine(File_Path, @".."));

                GetPrivateProfileString("Version", "Version", "0.0", Cur_V, 255, Environment.CurrentDirectory.ToString() + @"\" + "List.ini");

                version = Convert.ToDouble(Cur_V.ToString()); //버전정보 사전저장
                //파일 목록날짜 배열저장
                WritePrivateProfileString("Files", null, null, Environment.CurrentDirectory.ToString() + @"\" + "List.ini");

                string[] exceptions = new string[] { "", CP };
                files = Directory.GetFiles(super_path, "*.*", SearchOption.AllDirectories).Where(d => !d.StartsWith(CP)).ToArray();
                //불러와서 배열에 파일,날짜 각각 저장
                string[] old_files = new string[] {  };
                string[] n_fnm = new string[files.Length];
                string[] d_li = new string[files.Length];
                //기존파일 정보가져오기
                while (cnt < files.Length)
                {
                    try
                    {
                        d_buf = File.GetLastWriteTime(files[cnt]).ToString();
                        s_buf = files[cnt].ToString();
                        s_buf = s_buf.Replace(masking, "");
                        s_buf = s_buf.Substring(1, s_buf.Length - 1);
                        
                        n_fnm[cnt] = s_buf;
                        d_li[cnt] = d_buf;
                        
                        cnt++;
                    }
                    catch (Exception e)
                    {
                    }
                }

                int n_cnt = 0;
                //old ini 불러와서 위에 파일(키),날짜(값)랑 비교 후 files에 쓰기
                StringBuilder d_Builder = new StringBuilder(255);

                while (n_cnt < files.Length)
                {
                    GetPrivateProfileString("Old_Files", "" + n_fnm[n_cnt], "", d_Builder, 255, Environment.CurrentDirectory.ToString() + @"\" + "List.ini"); //날짜 불러오기

                    s_buf = files[n_cnt].ToString();
                
                    if (d_Builder.ToString() != File.GetLastWriteTime(files[n_cnt]).ToString())
                    {
                        s_buf = s_buf.Replace(masking, "");
                        WritePrivateProfileString("Files", "" + n_cnt.ToString(), s_buf, Environment.CurrentDirectory.ToString() + @"\" + "List.ini");
                        n_cnt++;
                    }
                }

                    WritePrivateProfileString("Files", "index", n_cnt.ToString(), Environment.CurrentDirectory.ToString() + @"\" + "List.ini");
                    version = Double.Parse(Cur_V.ToString()) + 0.1; //버전상승
                    version = Math.Truncate(version * 10) / 10;//trash 값 자르기
                    WritePrivateProfileString("Version", "Version", version.ToString(), Environment.CurrentDirectory.ToString() + @"\" + "List.ini");
                    WritePrivateProfileString("Old_Files", null, null, Environment.CurrentDirectory.ToString() + @"\" + "List.ini");

                    int o_cnt = 0;
                    //Old_files 갱신
                    while (o_cnt < files.Length)
                    {
                        try
                        {
                            s_buf = files[o_cnt].ToString();
                            d_buf = File.GetLastWriteTime(files[o_cnt]).ToString();
                            s_buf = s_buf.Replace(masking, "");
                            s_buf = s_buf.Substring(1, s_buf.Length - 1);

                            s_buf = files[o_cnt].ToString();
                            d_buf = File.GetLastWriteTime(files[o_cnt]).ToString();
                            s_buf = s_buf.Replace(masking, "");
                            s_buf = s_buf.Substring(1, s_buf.Length - 1);
                            WritePrivateProfileString("Old_Files", s_buf, d_buf, Environment.CurrentDirectory.ToString() + @"\" + "List.ini");//s_buf 버전체크해서 넣
                            o_cnt++;
                        }
                        catch (Exception e)
                        {

                        }
                    }
                
            }
            catch (Exception ex)
            {

            }
        }
    }
}
