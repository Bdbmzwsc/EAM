using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using EAM.API;
using EAM.Stringbase64;
using System.Net.Mail;
using System.IO;
using System.Net.Mime;
using System.Net;


namespace EAM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // string RegexStr = string.Empty;
        public string vcode;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (vcode == "")//没有验证码
            {
                MessageBox.Show("请先获取验证码");
                return;
            }
           
            
            // Dictionary dic = new Dictionary();
            //获取文件原文件内容
            JArray jar_file = JArray.Parse(ED.Md5Decrypt(GetFileContent()));

            for (int i = 0; i <= jar_file.Count - 1; i++)
            {
                JObject job_users = JObject.Parse(jar_file[i].ToString());
                if (Email.Text == job_users["EMAIL"].ToString() || UserName_reg.Text == job_users["NAME"].ToString()) //email或用户名重复
                {
                    MessageBox.Show("已有人使用 " + Email.Text + "邮箱或已有人使用名称 " + UserName_reg.Text);
                    return;
                }    
            }

            if (Vcode_reg.Password != vcode)//验证码错误
            {
                Vcode_reg.Password = "";
                MessageBox.Show("验证码错误");
                return;
            }

            //    MessageBox.Show(jar_file.ToString());
            //导出新Accout的json字符串
            JObject job = new JObject();
            job.Add("NAME", UserName_reg.Text);
            job.Add("PASS", Password.Password);
            job.Add("EMAIL", Email.Text);

            /*
             * 创建验证码
             * 此步打包为方法
             */


            //将内容导入jar
            jar_file.Add(job);
            //将jar加密并转base64
            string content = base64.EncodeBase64(ED.Md5Encrypt(jar_file.ToString()));
 
            
            JObject sendjob = new JObject();
            sendjob.Add("message", "Up " + UserName_reg.Text + " Accout");
            sendjob.Add("content", content);
            sendjob.Add("sha", GetFileID());
            // MessageBox.Show(sendjob.ToString());
            try
            {
                WebApi.GetHttpResponse("https://api.github.com/repos/Bdbmzwsc/EAM/contents/users.json", "put", Config.User_Agent, Config.JSONcon, "Authorization: token " + Config.GithubToken, sendjob.ToString());
                MessageBox.Show("注册成功");
            }
            catch(Exception ex)
            {
                MessageBox.Show("注册失败，错误:" + ex.Message);
            }
        }
        public static string GetFileID()
        {
            JObject job= JObject.Parse(WebApi.GetHttpResponse("https://api.github.com/repos/Bdbmzwsc/EAM/contents/users.json", "GET", Config.User_Agent, Config.TEXTcon, "Authorization: token " + Config.GithubToken, ""));
            return job["sha"].ToString();
        }
        public static string GetFileContent()
        {
            /*
            return WebApi.GetHttpResponse("https://raw.githubusercontent.com/Bdbmzwsc/EAM/Application/users.json",
                "GET",
                Config.User_Agent,
                Config.TEXTcon,
                "Authorization: token " + Config.GithubToken,
                "");
            */
            JObject job= JObject.Parse(WebApi.GetHttpResponse("https://api.github.com/repos/Bdbmzwsc/EAM/contents/users.json",
                "GET",
                Config.User_Agent,
                Config.TEXTcon,
                "Authorization: token " + Config.GithubToken,
                ""));
            return base64.DecodeBase64(job["content"].ToString());
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            

            //获取用户名和密码
            string emailaccount = Email.Text;
            string pass = Password.Password;

            //获得流并解析
          //  MessageBox.Show(GetFileContent());
            JArray jar_file = JArray.Parse(ED.Md5Decrypt(GetFileContent()));

            //遍历查看有无此账户
            for(int i = 0; i <= jar_file.Count - 1; i++)
            {
                JObject job_user = JObject.Parse(jar_file[i].ToString());
                if (job_user["EMAIL"].ToString() == emailaccount) //查到账户
                { 
                    if (job_user["PASS"].ToString() == pass)//密码正确
                    {
                        MessageBox.Show("登录成功");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("密码错误");
                        return;
                    }
                }
            }
            //没有账号
            MessageBox.Show("不存在账号 " + emailaccount);
        }

        private void Login_Copy_Click(object sender, RoutedEventArgs e)
        {
            JArray jar = new JArray();
            Email.Text = ED.Md5Encrypt(jar.ToString());
        }
        public static string Send_Email(string reciver,string content)
        {
            try
            {
                MailMessage message = new MailMessage();
                //设置发件人,发件人需要与设置的邮件发送服务器的邮箱一致
                MailAddress fromAddr = new MailAddress(Config.account_email);
                message.From = fromAddr;
                //设置收件人,可添加多个,添加方法与下面的一样
                message.To.Add(reciver);
                //设置抄送人
                message.CC.Add("EH Send@EH");
                //设置邮件标题
                message.Subject = "This is a Verification code email";
                //设置邮件内容
                message.Body = content;
                //设置邮件发送服务器,服务器根据你使用的邮箱而不同,可以到相应的 邮箱管理后台查看,下面是QQ的
                SmtpClient client = new SmtpClient("smtp.qq.com", 25);
                //设置发送人的邮箱账号和密码
                client.Credentials = new NetworkCredential(Config.account_email, Config.password_email);
                //启用ssl,也就是安全发送
                client.EnableSsl = true;
                //发送邮件
                client.Send(message);
                return "ok";
            }
            catch (Exception ex)
            {
                return "Error " + ex.Message;
            }
        }

        private void te_Click(object sender, RoutedEventArgs e)
        {
          //  Loading.Visibility = Visibility.Visible;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void To_Reg_Click(object sender, RoutedEventArgs e)
        {
            AccoutTab.SelectedIndex = 1;
        }

        private void To_Login_Click(object sender, RoutedEventArgs e)
        {
            AccoutTab.SelectedIndex = 0;
        }

        private void GetVCode_Click(object sender, RoutedEventArgs e)
        {

            vcode = getvco();

            //发送
            try
            {
                Send_Email(Email.Text, vcode);
                GetVCode.Content = "验证码已发送";

            }
            catch
            {
                GetVCode.Content = "发送失败";
            }
        }
        public static string getvco()
        {
            Random random = new Random();
            int num1 = random.Next(1, 100);
            var num2 = num1 * int.Parse(DateTime.Now.Second.ToString());
            return ED.Md5Encrypt(num2.ToString());
        }
    }
}
