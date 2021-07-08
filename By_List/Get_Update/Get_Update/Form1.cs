using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Get_Update
{
    public partial class Form1 : Form
    {
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        string Current_Path;//현재실행경로,전역변수 선언
        Uri uri; //서버경로
        string save_name; //저장경로

        string Cur_Day = DateTime.Now.ToString("yyyy-MM-dd");//현재날짜 가져오기
        string S_Cur_Day = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");//현재날짜 가져오기

        StringBuilder SR;
        StringBuilder TR;
        StringBuilder C_Path;
        StringBuilder M_CNT;
        StringBuilder B_Version = new StringBuilder(255);
        StringBuilder A_Version = new StringBuilder(255);
        DirectoryInfo di;
        FileInfo fi;

        string[] F_List;
        int e_cnt;
        Double B_Cnt=0.0,A_Cnt = 0.0;

        public Form1()
        {
            //this.WindowState = FormWindowState.Minimized; //폼최소화
            InitializeComponent();

            this.Visible = true;

            Current_Path = System.Environment.CurrentDirectory;//현재 실행경로 가져오기
            //로그경로
            di = new DirectoryInfo(Current_Path.ToString() + @"\LOG");
            fi = new FileInfo(di + @"\" + Cur_Day + ".Log");

            try
            {
                GetPrivateProfileString("Version", "Version", "0.0", B_Version, 255, Current_Path + @"\List.ini"); 
            }
            catch(Exception e)
            {
             //   B_Cnt = 0.0;
            }
        
            Get_Files_List();//파일 리스트 가져오기
            try
            {
                GetPrivateProfileString("Version", "Version", "0.0", A_Version, 255, Current_Path + @"\List.ini");
            }
            catch (Exception e)
            {
               // A_Cnt = 0.0;
            }
            // MessageBox.Show("A: " + A_Cnt.ToString());

            Get_Update();//업데이트
          
            //업데이트 후 파일실행 및 폼종료 처리
            // Process.Start(C_Path.ToString()); // 실행 될 프로그램 설정\
            Application.Exit();
        }
        public void Get_Files_List()
        {
            int count = 0;
            SR = new StringBuilder(255);
            TR = new StringBuilder(255);
            C_Path = new StringBuilder(255);
            M_CNT = new StringBuilder(255);
            StringBuilder F_NM = new StringBuilder(255);
            Current_Path = System.Environment.CurrentDirectory;//현재 실행경로 가져오기
            
            GetPrivateProfileString("PATH", "S_Resource", "", SR, 255, Current_Path + @"\path.ini"); //서버경로
            GetPrivateProfileString("PATH", "T_Resource", "", TR, 255, Current_Path + @"\path.ini"); //대상경로
            GetPrivateProfileString("PATH", "C_Path", "", C_Path, 255, Current_Path + @"\path.ini"); //업데이트 후 실행프로그램 경로

            DirectoryInfo d_main = new DirectoryInfo(TR.ToString());

            if (!d_main.Exists)
            {
                Directory.CreateDirectory(d_main.ToString());
            }

            WebClient mywebClient = new WebClient();
            uri = new Uri(SR.ToString()+"/updator"+ "/List.ini");//서버리소스 경로 및 이름
            save_name = TR.ToString(); //저장경로;
            mywebClient.DownloadFile(uri, Current_Path + @"\List.ini");
            GetPrivateProfileString("Files", "index", "", M_CNT, 255, Current_Path + @"\List.ini"); //ini에서 파일갯수가져오기

            e_cnt = Convert.ToInt32(M_CNT.ToString());
            count = 0;
            F_List = new string[e_cnt];
            //배열에 파일이름 저장
            while (count < (e_cnt))
            {
                {
                    try
                    {
                        GetPrivateProfileString("Files", count.ToString(), "", F_NM, 255, Current_Path + @"\List.ini"); 
                        
                        F_NM = F_NM.Replace(@"\", "/");
                        F_List[count] = F_NM.ToString();

                        if (count == (e_cnt - 2)) {
                            Logging(Cur_Day, 0, "\nList_Up");
                        }//마지막 로깅      
                    }
                    catch (Exception e)
                    {
                        Logging(Cur_Day, 1, "@"+"\nListUp_ERROR" + e.ToString());
                    }
                    count++;
                }
            }
        }
        public void Get_Update()
        {
            int cnt;
            WebClient DownLoadClient = new WebClient();
            GetPrivateProfileString("PATH", "S_Resource", "", SR, 255, Current_Path + @"\path.ini"); //서버경로
            GetPrivateProfileString("PATH", "T_Resource", "", TR, 255, Current_Path + @"\path.ini"); //대상경로
            cnt = 0;
            string F_buf;
            string[] fol_buf;
            string fo_name;
            while (cnt <= e_cnt)
              {
                try
                {

                    F_buf = F_List[cnt].Replace("/", @"\");
                    save_name = @"" + TR.ToString() + F_buf; //저장경로 지정 
                    fol_buf = F_buf.Split(@"\"); //폴더경로 구분위한 파일이름 구하기
                    uri = new Uri(SR.ToString() + F_List[cnt]);   //서버리소스 경로 및 이름

                    fo_name = save_name.Replace(fol_buf[fol_buf.Length - 1], "");
                    fo_name = fo_name.Replace(@"\", "/");
                    fo_name = fo_name.Substring(0, fo_name.Length - 1);
                    DirectoryInfo Creater = new DirectoryInfo(fo_name);
                    if (Creater.Exists == false) {
                        Creater.Create();
                    }
                    DownLoadClient.DownloadFile(uri, save_name);
                    progressBar1.Value = ((cnt * 100) / e_cnt);
                   }
                  catch (Exception e)
                  {
                      Logging(Cur_Day, 1, e.ToString());
                  }
                  cnt++;
            }
        }

        //로깅 함수
        public void Logging(string curday, int checker, string e_msg)
        {
            String temp;
            try
            {
                if (!di.Exists)
                    Directory.CreateDirectory(di.ToString());
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(fi.ToString()))
                    {
                        //정상업데이트 오류 로그
                        if (checker == 0)
                        {
                            temp = string.Format("[{0}] {1}", DateTime.Now, "[updated]" + e_msg);
                            sw.WriteLine(temp);
                            sw.Close();
                        }
                        //에러 로그 메시지
                        else if (checker == 1)
                        {
                            temp = string.Format("[{0}] {1}", DateTime.Now, "\n [failed] " + "\n" + e_msg);
                            sw.WriteLine(temp);
                            sw.Close();
                        }
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(fi.ToString()))
                    {
                        //정상업데이트 오류 로그
                        if (checker == 0)
                        {
                            temp = string.Format("[{0}] {1}", DateTime.Now, "[updated] " + e_msg);
                            sw.WriteLine(temp);
                            sw.Close();
                        }
                        //에러 로그 메시지
                        else if (checker == 1)
                        {
                            temp = string.Format("[{0}] {1}", DateTime.Now, "\n [failed] " + "\n" + e_msg);
                            sw.WriteLine(temp);
                            sw.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
