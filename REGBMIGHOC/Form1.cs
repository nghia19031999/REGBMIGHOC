using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace REGBMIGHOC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //Trung Chanh
        }

        private void test()
        {
            var danhsachmail = File.ReadAllLines("mail.txt").ToList();
            string mail = danhsachmail[0];
            string usernamemail = mail.Split('|')[0];
            string passwordmail = mail.Split('|')[1];
            string link1 = GetCodeHotmail(usernamemail, passwordmail);
        }
        private void button1_Click(object sender, EventArgs e)
        {

            var Account = File.ReadAllLines("Account.txt").ToList();
            int iThread = 0;
            int maxThread = (int)numericUpDown1.Value;
            int i = 0;
            KillChromeDriver();

            new Thread(() =>
            {
                while (i < Account.Count)
                {
                    if (iThread < maxThread)
                    {
                        int rowi = i;
                        Interlocked.Increment(ref iThread);
                        (new Thread(() =>
                        {
                            var account = Account[rowi];
                            var viasplit = account.Split('|');

                            string username = viasplit[0];
                            string password = viasplit[1];

                            int add = 0;
                            dgv1.Invoke(new Action(() =>
                            {

                                add = dgv1.Rows.Add((dgv1.RowCount + 1), username, password);

                            }));
                            DataGridViewRow row = dgv1.Rows[add];
                            LoginIG(row, username, password);


                            Interlocked.Decrement(ref iThread);
                        })).Start();
                        i++;
                        Thread.Sleep(100);
                    }
                    else
                    {
                        Thread.Sleep(300);
                    }
                    //get link all
                }
            }).Start();


        }


        private void LoginIG(DataGridViewRow row, string username, string password)
        {
            var chromedriver = Createchromedrive();
            try
            {
                
                chromedriver.Navigate().GoToUrl("https://www.instagram.com");
                Thread.Sleep(1000);
                ResizeWindow(chromedriver, 800, 800);
                row.Cells["cStatus"].Value = "Login hoc daica";
                IWebElement tbusername = chromedriver.FindElement(By.Name("username"));
                tbusername.SendKeys(username);
                Thread.Sleep(100);
                IWebElement tbpass = chromedriver.FindElement(By.Name("password"));
                tbpass.SendKeys(password);
                Thread.Sleep(100);
                var login = chromedriver.FindElement(By.XPath("//*[text()='Log in']"));
                login.Click();
                Thread.Sleep(10000);

            }
            catch { }

            var urlchrome = chromedriver.Url;
            if (!urlchrome.Contains("https://www.instagram.com/accounts/suspended")&&
                !urlchrome.Contains("https://www.instagram.com/challenge/action"))

            {


                row.Cells["cStatus"].Value = "Reg BM";
                chromedriver.Navigate().GoToUrl("https://business.facebook.com/");

                Thread.Sleep(5000);
                var loginwithInstagram = chromedriver.FindElement(By.XPath("//*[text()='Log in with Instagram']"));
                loginwithInstagram.Click();
                Thread.Sleep(5000);
                try
                {
                    var loginasinstagram = chromedriver.FindElements(By.XPath("//div[@role='button']"));
                    loginasinstagram[0].Click();
                    Thread.Sleep(5000);

                }
                catch { }

                try
                {
                    var chapnhan = chromedriver.FindElements(By.XPath("//*[text()='Chấp nhận']"));
                    chapnhan[1].Click();
                    Thread.Sleep(1000);
                }
                catch
                {
                    try
                    {
                        var Accept = chromedriver.FindElements(By.XPath("//*[text()='Accept']"));
                        Accept[1].Click();
                        Thread.Sleep(1000);
                    }
                    catch { }

                }

                chromedriver.Navigate().GoToUrl("https://business.facebook.com/latest/settings/business_users/?business_id=");
                var uidbm = Regex.Match(chromedriver.PageSource, "business_id=(.*?)\"}").Groups[1].Value;
                Thread.Sleep(1000);
                // tìm chữ InvalidID
                //IWebElement InvalidID = null;
                //InvalidID = chromedriver.FindElement(By.XPath("//*[text()='Invalid ID']"));
                // nếu ko có thì reg bm có thì thoát ra
                #region REG BM
                try
                {
                    var InvalidID = chromedriver.FindElement(By.XPath("//*[text()='Invalid ID']"));
                    row.Cells["cStatus"].Value = "IG KO REG ĐƯỢC BM";

                }
                catch
                {

                    ResizeWindow(chromedriver, 1200, 800);
                    IWebElement btninvitepeople = null;
                    try
                    {



                        try
                        {
                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Mời người khác']"));
                            Thread.Sleep(1000);
                            btninvitepeople.Click();
                            Thread.Sleep(1000);
                        }
                        catch
                        {
                            try
                            {
                                btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Invite people']"));
                                Thread.Sleep(1000);
                                btninvitepeople.Click();
                                Thread.Sleep(100);

                            }
                            catch { }

                        }

                        try
                        {
                            IWebElement btnnhapthongtinlh = chromedriver.FindElement(By.XPath("//*[text()='Nhập thông tin liên hệ']"));
                            btnnhapthongtinlh.Click();
                            Thread.Sleep(100);
                        }
                        catch
                        {
                            var btnnhapthongtinlh1 = chromedriver.FindElement(By.XPath("//*[text()='Enter contact info']"));
                            btnnhapthongtinlh1.Click();
                            Thread.Sleep(100);
                        }
                        Thread.Sleep(3000);
                        string mailbusiness = "vu1882168@gmail.com";
                        var nhapmailbusiness = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div[2]/div[2]/div/div[2]/div/div/div/div[1]/div[2]/div/div//input[@type='text']"));
                        nhapmailbusiness.SendKeys(mailbusiness);
                        Thread.Sleep(1000);
                        try
                        {
                            IWebElement tbLuu = chromedriver.FindElement(By.XPath("//*[text()='Lưu']"));
                            tbLuu.Click();
                            Thread.Sleep(3000);

                        }
                        catch
                        {
                            IWebElement Save = chromedriver.FindElement(By.XPath("//*[text()='Save']"));
                            Save.Click();
                            Thread.Sleep(3000);
                        }


                    }
                    catch { }


                    row.Cells["cStatus"].Value = "Reg BM thứ 2";
                    chromedriver.Navigate().GoToUrl($"https://business.facebook.com/billing_hub/accounts?business_id={uidbm}&placement=standalone&global_scope_id={uidbm}");
                    Thread.Sleep(1000);
                    try
                    {
                        // reg BM thứ 2
                        var Timkiem = chromedriver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/span/div[1]/div[1]/div/div[2]/div[1]/div[1]/div[3]/div/div/div/div/div/span[1]/span/span/div/div[2]/div"));
                        Timkiem.Click();
                        Thread.Sleep(1000);
                        try
                        {
                            var Taohosodoanhnghiep = chromedriver.FindElement(By.XPath("//*[text()='Tạo hồ sơ doanh nghiệp']"));
                            Taohosodoanhnghiep.Click();
                            Thread.Sleep(100);
                        }
                        catch
                        {
                            var Createabusinessprofile = chromedriver.FindElement(By.XPath("//*[text()='Create a business portfolio']"));
                            Createabusinessprofile.Click();
                            Thread.Sleep(100);

                        }
                        Thread.Sleep(5000);
                        Random rd = new Random();
                        int randomNumber = rd.Next(1, 1000000);
                        string Name = "VuThai95" + randomNumber;
                        var TenBM = chromedriver.FindElements(By.XPath("//input[@placeholder]"));
                        TenBM[2].SendKeys(Name);
                        Thread.Sleep(1000);
                        string Ten = "Thai";
                        string Ho = "Vu";
                        //                                           /html/body/div[5]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[1]/div[1]/div/div/div[2]/div/div[1]/div/div[1]/div[2]/div[1]/div/input

                        try
                        {
                            var firstname = chromedriver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[1]/div[1]/div/div/div[2]/div/div[1]/div/div[1]/div[2]/div[1]/div/input"));
                            firstname.SendKeys(Ten);
                            Thread.Sleep(200);

                        }
                        catch
                        {
                            var ten = chromedriver.FindElement(By.XPath("/html/body/div[6]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[1]/div[1]/div/div/div[2]/div/div[1]/div/div[1]/div[2]/div[1]/div/input"));
                            ten.SendKeys(Ten);
                            Thread.Sleep(200);


                        }
                        try
                        {
                            var Lastname = chromedriver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div/div[1]/div/div[1]/div[2]/div[1]/div/input"));
                            Lastname.SendKeys(Ho);
                            Thread.Sleep(200);

                        }
                        catch
                        {
                            var ho = chromedriver.FindElement(By.XPath("/html/body/div[6]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div/div[1]/div/div[1]/div[2]/div[1]/div/input"));
                            ho.SendKeys(Ho);
                            Thread.Sleep(200);

                        }


                        string emailbusiness = "vu1882168@gmail.com";
                        //                                                      /html/body/div[5]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[2]/div/div[2]/div/div/div/div[1]/div[2]/div/div/input

                        try
                        {
                            var emailbusiness1 = chromedriver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[2]/div/div[2]/div/div/div/div[1]/div[2]/div/div/input"));
                            emailbusiness1.SendKeys(emailbusiness);
                            Thread.Sleep(200);
                        }
                        catch
                        {
                            var EmailBusiness = chromedriver.FindElement(By.XPath("/html/body/div[6]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[2]/div/div[2]/div/div/div/div[1]/div[2]/div/div/input"));
                            EmailBusiness.SendKeys(emailbusiness);
                            Thread.Sleep(200);
                        }

                        try
                        {
                            var Tao = chromedriver.FindElement(By.XPath("//*[text()='Tạo']"));
                            Tao.Click();
                            Thread.Sleep(200);
                        }
                        catch
                        {
                            var Create = chromedriver.FindElement(By.XPath("//*[text()='Create']"));
                            Create.Click();
                            Thread.Sleep(200);
                        }
                        try
                        {
                            var Tiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                            Tiep.Click();
                            Thread.Sleep(200);
                        }
                        catch
                        {
                            var Next = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                            Next.Click();
                            Thread.Sleep(200);
                        }

                        try
                        {
                            var Tiep1 = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                            Tiep1.Click();
                            Thread.Sleep(200);
                        }
                        catch
                        {
                            var Next1 = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                            Next1.Click();
                            Thread.Sleep(200);
                        }

                        try
                        {
                            var Xacnhan = chromedriver.FindElement(By.XPath("//*[text()='Xác nhận']"));
                            Xacnhan.Click();
                            Thread.Sleep(200);
                        }
                        catch
                        {
                            var Confirm = chromedriver.FindElement(By.XPath("//*[text()='Confirm']"));
                            Confirm.Click();
                            Thread.Sleep(200);
                        }

                    }
                    catch
                    {
                        row.Cells["cStatus"].Value = "KO REG được BM Thứ 2 ";
                    }

                    row.Cells["cStatus"].Value = "Reg BM thứ 3";
                    try
                    {

                        chromedriver.Navigate().GoToUrl($"https://business.facebook.com/billing_hub/accounts?business_id={uidbm}&placement=standalone&global_scope_id={uidbm}");
                        Thread.Sleep(1000);

                        // reg BM thứ 3
                        var Timkiem = chromedriver.FindElement(By.XPath("/html/body/div[1]/div[1]/div/span/div[1]/div[1]/div/div[2]/div[1]/div[1]/div[3]/div/div/div/div/div/span[1]/span/span/div/div[2]/div"));
                        Timkiem.Click();
                        Thread.Sleep(1000);
                        try
                        {
                            var Taohosodoanhnghiep = chromedriver.FindElement(By.XPath("//*[text()='Tạo hồ sơ doanh nghiệp']"));
                            Taohosodoanhnghiep.Click();
                            Thread.Sleep(100);
                        }
                        catch
                        {
                            var Createabusinessprofile = chromedriver.FindElement(By.XPath("//*[text()='Create a business portfolio']"));
                            Createabusinessprofile.Click();
                            Thread.Sleep(100);

                        }
                        Thread.Sleep(5000);
                        Random rd = new Random();
                        int randomNumber = rd.Next(1, 1000000);
                        string Name = "VuThai95" + randomNumber;
                        var TenBM = chromedriver.FindElements(By.XPath("//input[@placeholder]"));
                        TenBM[2].SendKeys(Name);
                        Thread.Sleep(1000);
                        string Ten = "Thai";
                        string Ho = "Vu";
                        //                                           /html/body/div[5]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[1]/div[1]/div/div/div[2]/div/div[1]/div/div[1]/div[2]/div[1]/div/input

                        try
                        {
                            var firstname = chromedriver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[1]/div[1]/div/div/div[2]/div/div[1]/div/div[1]/div[2]/div[1]/div/input"));
                            firstname.SendKeys(Ten);
                            Thread.Sleep(200);

                        }
                        catch
                        {
                            var ten = chromedriver.FindElement(By.XPath("/html/body/div[6]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[1]/div[1]/div/div/div[2]/div/div[1]/div/div[1]/div[2]/div[1]/div/input"));
                            ten.SendKeys(Ten);
                            Thread.Sleep(200);


                        }
                        try
                        {
                            var Lastname = chromedriver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div/div[1]/div/div[1]/div[2]/div[1]/div/input"));
                            Lastname.SendKeys(Ho);
                            Thread.Sleep(200);

                        }
                        catch
                        {
                            var ho = chromedriver.FindElement(By.XPath("/html/body/div[6]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[1]/div[2]/div/div/div[2]/div/div[1]/div/div[1]/div[2]/div[1]/div/input"));
                            ho.SendKeys(Ho);
                            Thread.Sleep(200);

                        }


                        string emailbusiness = "vu1882168@gmail.com";
                        //                                                      /html/body/div[5]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[2]/div/div[2]/div/div/div/div[1]/div[2]/div/div/input

                        try
                        {
                            var emailbusiness1 = chromedriver.FindElement(By.XPath("/html/body/div[5]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[2]/div/div[2]/div/div/div/div[1]/div[2]/div/div/input"));
                            emailbusiness1.SendKeys(emailbusiness);
                            Thread.Sleep(200);
                        }
                        catch
                        {
                            var EmailBusiness = chromedriver.FindElement(By.XPath("/html/body/div[6]/div[1]/div[1]/div/div/div/div/div[2]/div[1]/div[2]/div/div[2]/div[2]/div/div[2]/div/div[2]/div/div/div/div[1]/div[2]/div/div/input"));
                            EmailBusiness.SendKeys(emailbusiness);
                            Thread.Sleep(200);
                        }

                        try
                        {
                            var Tao = chromedriver.FindElement(By.XPath("//*[text()='Tạo']"));
                            Tao.Click();
                            Thread.Sleep(200);
                        }
                        catch
                        {
                            var Create = chromedriver.FindElement(By.XPath("//*[text()='Create']"));
                            Create.Click();
                            Thread.Sleep(200);
                        }
                        try
                        {
                            var Tiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                            Tiep.Click();
                            Thread.Sleep(200);
                        }
                        catch
                        {
                            var Next = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                            Next.Click();
                            Thread.Sleep(200);
                        }

                        try
                        {
                            var Tiep1 = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                            Tiep1.Click();
                            Thread.Sleep(200);
                        }
                        catch
                        {
                            var Next1 = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                            Next1.Click();
                            Thread.Sleep(200);
                        }

                        try
                        {
                            var Xacnhan = chromedriver.FindElement(By.XPath("//*[text()='Xác nhận']"));
                            Xacnhan.Click();
                            Thread.Sleep(200);
                        }
                        catch
                        {
                            var Confirm = chromedriver.FindElement(By.XPath("//*[text()='Confirm']"));
                            Confirm.Click();
                            Thread.Sleep(200);
                        }

                    }
                    catch
                    {
                        row.Cells["cStatus"].Value = "KO Reg được BM thứ 3";
                    }


                    // lấy link tất cả bm
                    try
                    {
                        row.Cells["cStatus"].Value = "lấy IDBM";
                        chromedriver.Navigate().Refresh();
                        var manguon1 = chromedriver.PageSource;
                        Thread.Sleep(2000);
                        var tokenEAAG = Regex.Match(manguon1, "\\[{\"accessToken\":\"(.*?)\"").Groups[1].Value;
                        chromedriver.Navigate().GoToUrl($"https://graph.facebook.com/v15.0/me/businesses?access_token={tokenEAAG}&limit=100&fields=%5B%22id%22,%22allow_page_management_in_www%22,%22business_users%7Brole%7D%22,%22sharing_eligibility_status%22,%22owned_ad_accounts.limit(1)%7Bcurrency,adtrust_dsl%7D%22,%22created_time%22,%22name%22,%20%22status%22%5D");
                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(chromedriver.PageSource);


                        // Select the content inside <pre> tag
                        var preNode = doc.DocumentNode.SelectSingleNode("//pre");
                        if (preNode != null)
                        {
                            try
                            {
                                JObject jobject = JObject.Parse(preNode.InnerText);
                                var dataValues = jobject["data"];

                                if (dataValues != null)
                                {
                                    var dataValuesArr = dataValues.ToObject<JArray>();
                                    foreach (var itemObject in dataValuesArr)
                                    {
                                        var businessId = itemObject["id"].ToString();
                                        var typeBm = itemObject["sharing_eligibility_status"].ToString() == "enabled" ? "BM350" : "BM50";
                                        var statusBm = itemObject["allow_page_management_in_www"].ToString().ToLower() == "true" ? "BM Live" : "BM Die";
                                        row.Cells["cCheckBM"].Value += $"{businessId}|{typeBm}|{statusBm}" + Environment.NewLine;

                                        StreamWriter sw5 = new StreamWriter("checkBM.txt", true);
                                        sw5.WriteLine(businessId + "|" + typeBm + "|" + statusBm + "|" + username + "|" + password);
                                        sw5.Close();
                                        Thread.Sleep(100);

                                    }

                                    var ids = new List<string>();
                                    // lấy ra từng id lưu vào list 
                                    foreach (var item in dataValuesArr)
                                    {
                                        // lấy ra value của key id
                                        ids.Add(item["id"].ToString());

                                    }
                                    var url = $"https://business.facebook.com/billing_hub/accounts?business_id={uidbm}&placement=standalone&global_scope_id={uidbm}";
                                    chromedriver.Navigate().GoToUrl(url);
                                    // bm thứ 1
                                    string idbm = ids[0];
                                    if (idbm != null)
                                    {


                                        chromedriver.Navigate().GoToUrl($"https://business.facebook.com/latest/settings/business_users/?asset_id={idbm}&business_id={idbm}");
                                        Thread.Sleep(1000);
                                        try
                                        {
                                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Mời người khác']"));
                                            btninvitepeople.Click();
                                            Thread.Sleep(1000);
                                        }
                                        catch
                                        {
                                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Invite people']"));
                                            btninvitepeople.Click();
                                            Thread.Sleep(1000);
                                        }
                                        var danhsachmail1 = File.ReadAllLines("mail.txt").ToList();
                                        Random rnd1 = new Random();
                                        int indexRandom = rnd1.Next(0, danhsachmail1.Count);
                                        string mail1 = danhsachmail1[indexRandom];
                                        string usernamemail1 = mail1.Split('|')[0];
                                        string passwordmail1 = mail1.Split('|')[1];

                                        try
                                        {
                                            var nhapmailnahnbm = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div/div[1]/div[2]/div[2]/div/div/div[2]/div[1]/div[2]/div[2]/div/div/div[2]/div/div/div/div[1]/div[2]/div/div/div//input[@type='text']"));
                                            nhapmailnahnbm.SendKeys(usernamemail1);
                                            Thread.Sleep(1000);

                                        }
                                        catch
                                        {
                                            var nhapmailnahnbm1 = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div/div[1]/div[2]/div[2]/div/div/div[2]/div[1]/div[2]/div[2]/div/div/div[2]/div/div/div/div[1]/div[2]/div/div/div/input"));
                                            nhapmailnahnbm1.SendKeys(usernamemail1);
                                            Thread.Sleep(1000);

                                        }
                                        IWebElement tbTiep1 = null;
                                        try
                                        {
                                            tbTiep1 = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep1.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep1 = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep1.Click();
                                            Thread.Sleep(100);
                                        }
                                        var tbManage = chromedriver.FindElements(By.XPath("//input[@type='checkbox']"));
                                        tbManage[2].Click();
                                        Thread.Sleep(100);
                                        IWebElement tbTiep = null;
                                        try
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        try
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        try
                                        {
                                            var tbguiloimoi = chromedriver.FindElement(By.XPath("//*[text()='Gửi lời mời']"));
                                            tbguiloimoi.Click();
                                            Thread.Sleep(2000);

                                        }
                                        catch
                                        {
                                            var tbSendinvitation = chromedriver.FindElement(By.XPath("//*[text()='Send invitation']"));
                                            tbSendinvitation.Click();
                                            Thread.Sleep(2000);

                                        }
                                        try
                                        {
                                            var tbXong = chromedriver.FindElement(By.XPath("//*[text()='Xong']"));
                                            tbXong.Click();
                                            Thread.Sleep(1000);
                                        }
                                        catch
                                        {
                                            var Done = chromedriver.FindElement(By.XPath("//*[text()='Done']"));
                                            Done.Click();
                                            Thread.Sleep(1000);
                                        }
                                        Thread.Sleep(60000);
                                        row.Cells["cStatus"].Value = "lấy link1";
                                        string link = GetCodeHotmail(usernamemail1, passwordmail1);
                                        StreamWriter sw1 = new StreamWriter("link.txt", true);
                                        sw1.WriteLine($"{idbm}|{link}");
                                        sw1.Close();
                                        row.Cells["cStatus"].Value = "lấy link1 ok";

                                    }

                                    // bm thứ 2
                                    string idbm2 = ids[1];
                                    if (idbm2 != null)
                                    {


                                        chromedriver.Navigate().GoToUrl($"https://business.facebook.com/latest/settings/business_users/?asset_id={idbm2}&business_id={idbm2}");
                                        Thread.Sleep(1000);
                                        try
                                        {
                                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Mời người khác']"));
                                            btninvitepeople.Click();
                                            Thread.Sleep(1000);
                                        }
                                        catch
                                        {
                                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Invite people']"));
                                            btninvitepeople.Click();
                                            Thread.Sleep(1000);
                                        }
                                        var danhsachmail1 = File.ReadAllLines("mail.txt").ToList();
                                        Random rnd1 = new Random();
                                        int indexRandom = rnd1.Next(0, danhsachmail1.Count);
                                        string mail2 = danhsachmail1[indexRandom];
                                        string usernamemail1 = mail2.Split('|')[0];
                                        string passwordmail1 = mail2.Split('|')[1];

                                        try
                                        {
                                            var nhapmailnahnbm = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div/div[1]/div[2]/div[2]/div/div/div[2]/div[1]/div[2]/div[2]/div/div/div[2]/div/div/div/div[1]/div[2]/div/div/div//input[@type='text']"));
                                            nhapmailnahnbm.SendKeys(usernamemail1);
                                            Thread.Sleep(1000);

                                        }
                                        catch
                                        {
                                            var nhapmailnahnbm1 = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div/div[1]/div[2]/div[2]/div/div/div[2]/div[1]/div[2]/div[2]/div/div/div[2]/div/div/div/div[1]/div[2]/div/div/div/input"));
                                            nhapmailnahnbm1.SendKeys(usernamemail1);
                                            Thread.Sleep(1000);

                                        }
                                        IWebElement tbTiep1 = null;
                                        try
                                        {
                                            tbTiep1 = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep1.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep1 = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep1.Click();
                                            Thread.Sleep(100);
                                        }
                                        var tbManage = chromedriver.FindElements(By.XPath("//input[@type='checkbox']"));
                                        tbManage[2].Click();
                                        Thread.Sleep(100);
                                        IWebElement tbTiep = null;
                                        try
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        try
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        try
                                        {
                                            var tbguiloimoi = chromedriver.FindElement(By.XPath("//*[text()='Gửi lời mời']"));
                                            tbguiloimoi.Click();
                                            Thread.Sleep(2000);

                                        }
                                        catch
                                        {
                                            var tbSendinvitation = chromedriver.FindElement(By.XPath("//*[text()='Send invitation']"));
                                            tbSendinvitation.Click();
                                            Thread.Sleep(2000);

                                        }
                                        try
                                        {
                                            var tbXong = chromedriver.FindElement(By.XPath("//*[text()='Xong']"));
                                            tbXong.Click();
                                            Thread.Sleep(1000);
                                        }
                                        catch
                                        {
                                            var Done = chromedriver.FindElement(By.XPath("//*[text()='Done']"));
                                            Done.Click();
                                            Thread.Sleep(1000);
                                        }

                                        Thread.Sleep(60000);

                                        row.Cells["cStatus"].Value = "lấy link2";
                                        string link2 = GetCodeHotmail(usernamemail1, passwordmail1);
                                        StreamWriter sw1 = new StreamWriter("link.txt", true);
                                        sw1.WriteLine($"{idbm2}|{link2}");
                                        sw1.Close();
                                        row.Cells["cStatus"].Value = "lấy link2 ok";

                                    }

                                    // bm thứ 3
                                    string idbm3 = ids[2];
                                    if (idbm3 != null)
                                    {


                                        chromedriver.Navigate().GoToUrl($"https://business.facebook.com/latest/settings/business_users/?asset_id={idbm3}&business_id={idbm3}");
                                        Thread.Sleep(1000);
                                        try
                                        {
                                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Mời người khác']"));
                                            btninvitepeople.Click();
                                            Thread.Sleep(1000);
                                        }
                                        catch
                                        {
                                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Invite people']"));
                                            btninvitepeople.Click();
                                            Thread.Sleep(1000);
                                        }
                                        var danhsachmail1 = File.ReadAllLines("mail.txt").ToList();
                                        Random rnd1 = new Random();
                                        int indexRandom = rnd1.Next(0, danhsachmail1.Count);
                                        string mail3 = danhsachmail1[indexRandom];
                                        string usernamemail1 = mail3.Split('|')[0];
                                        string passwordmail1 = mail3.Split('|')[1];

                                        try
                                        {
                                            var nhapmailnahnbm = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div/div[1]/div[2]/div[2]/div/div/div[2]/div[1]/div[2]/div[2]/div/div/div[2]/div/div/div/div[1]/div[2]/div/div/div//input[@type='text']"));
                                            nhapmailnahnbm.SendKeys(usernamemail1);
                                            Thread.Sleep(1000);

                                        }
                                        catch
                                        {
                                            var nhapmailnahnbm1 = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div/div[1]/div[2]/div[2]/div/div/div[2]/div[1]/div[2]/div[2]/div/div/div[2]/div/div/div/div[1]/div[2]/div/div/div/input"));
                                            nhapmailnahnbm1.SendKeys(usernamemail1);
                                            Thread.Sleep(1000);

                                        }
                                        IWebElement tbTiep1 = null;
                                        try
                                        {
                                            tbTiep1 = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep1.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep1 = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep1.Click();
                                            Thread.Sleep(100);
                                        }
                                        var tbManage = chromedriver.FindElements(By.XPath("//input[@type='checkbox']"));
                                        tbManage[2].Click();
                                        Thread.Sleep(100);
                                        IWebElement tbTiep = null;
                                        try
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        try
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        try
                                        {
                                            var tbguiloimoi = chromedriver.FindElement(By.XPath("//*[text()='Gửi lời mời']"));
                                            tbguiloimoi.Click();
                                            Thread.Sleep(2000);

                                        }
                                        catch
                                        {
                                            var tbSendinvitation = chromedriver.FindElement(By.XPath("//*[text()='Send invitation']"));
                                            tbSendinvitation.Click();
                                            Thread.Sleep(2000);

                                        }
                                        try
                                        {
                                            var tbXong = chromedriver.FindElement(By.XPath("//*[text()='Xong']"));
                                            tbXong.Click();
                                            Thread.Sleep(1000);
                                        }
                                        catch
                                        {
                                            var Done = chromedriver.FindElement(By.XPath("//*[text()='Done']"));
                                            Done.Click();
                                            Thread.Sleep(1000);
                                        }

                                        Thread.Sleep(60000);

                                        row.Cells["cStatus"].Value = "lấy link3";
                                        string link3 = GetCodeHotmail(usernamemail1, passwordmail1);
                                        StreamWriter sw1 = new StreamWriter("link.txt", true);
                                        sw1.WriteLine($"{idbm3}|{link3}");
                                        sw1.Close();
                                        row.Cells["cStatus"].Value = "lấy link3 ok";

                                    }

                                    // bm thứ 4
                                    string idbm4 = ids[3];
                                    if (idbm4 != null)
                                    {


                                        chromedriver.Navigate().GoToUrl($"https://business.facebook.com/latest/settings/business_users/?asset_id={idbm4}&business_id={idbm4}");
                                        Thread.Sleep(1000);
                                        try
                                        {
                                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Mời người khác']"));
                                            btninvitepeople.Click();
                                            Thread.Sleep(1000);
                                        }
                                        catch
                                        {
                                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Invite people']"));
                                            btninvitepeople.Click();
                                            Thread.Sleep(1000);
                                        }
                                        var danhsachmail1 = File.ReadAllLines("mail.txt").ToList();
                                        Random rnd1 = new Random();
                                        int indexRandom = rnd1.Next(0, danhsachmail1.Count);
                                        string mail4 = danhsachmail1[indexRandom];
                                        string usernamemail1 = mail4.Split('|')[0];
                                        string passwordmail1 = mail4.Split('|')[1];

                                        try
                                        {
                                            var nhapmailnahnbm = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div/div[1]/div[2]/div[2]/div/div/div[2]/div[1]/div[2]/div[2]/div/div/div[2]/div/div/div/div[1]/div[2]/div/div/div//input[@type='text']"));
                                            nhapmailnahnbm.SendKeys(usernamemail1);
                                            Thread.Sleep(1000);

                                        }
                                        catch
                                        {
                                            var nhapmailnahnbm1 = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div/div[1]/div[2]/div[2]/div/div/div[2]/div[1]/div[2]/div[2]/div/div/div[2]/div/div/div/div[1]/div[2]/div/div/div/input"));
                                            nhapmailnahnbm1.SendKeys(usernamemail1);
                                            Thread.Sleep(1000);

                                        }
                                        IWebElement tbTiep1 = null;
                                        try
                                        {
                                            tbTiep1 = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep1.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep1 = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep1.Click();
                                            Thread.Sleep(100);
                                        }
                                        var tbManage = chromedriver.FindElements(By.XPath("//input[@type='checkbox']"));
                                        tbManage[2].Click();
                                        Thread.Sleep(100);
                                        IWebElement tbTiep = null;
                                        try
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        try
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        try
                                        {
                                            var tbguiloimoi = chromedriver.FindElement(By.XPath("//*[text()='Gửi lời mời']"));
                                            tbguiloimoi.Click();
                                            Thread.Sleep(2000);

                                        }
                                        catch
                                        {
                                            var tbSendinvitation = chromedriver.FindElement(By.XPath("//*[text()='Send invitation']"));
                                            tbSendinvitation.Click();
                                            Thread.Sleep(2000);

                                        }
                                        try
                                        {
                                            var tbXong = chromedriver.FindElement(By.XPath("//*[text()='Xong']"));
                                            tbXong.Click();
                                            Thread.Sleep(1000);
                                        }
                                        catch
                                        {
                                            var Done = chromedriver.FindElement(By.XPath("//*[text()='Done']"));
                                            Done.Click();
                                            Thread.Sleep(1000);
                                        }

                                        Thread.Sleep(60000);

                                        row.Cells["cStatus"].Value = "lấy link";
                                        string link4 = GetCodeHotmail(usernamemail1, passwordmail1);
                                        StreamWriter sw1 = new StreamWriter("link.txt", true);
                                        sw1.WriteLine($"{idbm4}|{link4}");
                                        sw1.Close();
                                        row.Cells["cStatus"].Value = "lấy link ok";

                                    }

                                    // bm thứ 5
                                    string idbm5 = ids[4];
                                    if (idbm5 != null)
                                    {


                                        chromedriver.Navigate().GoToUrl($"https://business.facebook.com/latest/settings/business_users/?asset_id={idbm5}&business_id={idbm5}");
                                        Thread.Sleep(1000);
                                        try
                                        {
                                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Mời người khác']"));
                                            btninvitepeople.Click();
                                            Thread.Sleep(1000);
                                        }
                                        catch
                                        {
                                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Invite people']"));
                                            btninvitepeople.Click();
                                            Thread.Sleep(1000);
                                        }
                                        var danhsachmail1 = File.ReadAllLines("mail.txt").ToList();
                                        Random rnd1 = new Random();
                                        int indexRandom = rnd1.Next(0, danhsachmail1.Count);
                                        string mail5 = danhsachmail1[indexRandom];
                                        string usernamemail1 = mail5.Split('|')[0];
                                        string passwordmail1 = mail5.Split('|')[1];

                                        try
                                        {
                                            var nhapmailnahnbm = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div/div[1]/div[2]/div[2]/div/div/div[2]/div[1]/div[2]/div[2]/div/div/div[2]/div/div/div/div[1]/div[2]/div/div/div//input[@type='text']"));
                                            nhapmailnahnbm.SendKeys(usernamemail1);
                                            Thread.Sleep(1000);

                                        }
                                        catch
                                        {
                                            var nhapmailnahnbm1 = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div/div[1]/div[2]/div[2]/div/div/div[2]/div[1]/div[2]/div[2]/div/div/div[2]/div/div/div/div[1]/div[2]/div/div/div/input"));
                                            nhapmailnahnbm1.SendKeys(usernamemail1);
                                            Thread.Sleep(1000);

                                        }
                                        IWebElement tbTiep1 = null;
                                        try
                                        {
                                            tbTiep1 = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep1.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep1 = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep1.Click();
                                            Thread.Sleep(100);
                                        }
                                        var tbManage = chromedriver.FindElements(By.XPath("//input[@type='checkbox']"));
                                        tbManage[2].Click();
                                        Thread.Sleep(100);
                                        IWebElement tbTiep = null;
                                        try
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        try
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        try
                                        {
                                            var tbguiloimoi = chromedriver.FindElement(By.XPath("//*[text()='Gửi lời mời']"));
                                            tbguiloimoi.Click();
                                            Thread.Sleep(2000);

                                        }
                                        catch
                                        {
                                            var tbSendinvitation = chromedriver.FindElement(By.XPath("//*[text()='Send invitation']"));
                                            tbSendinvitation.Click();
                                            Thread.Sleep(2000);

                                        }
                                        try
                                        {
                                            var tbXong = chromedriver.FindElement(By.XPath("//*[text()='Xong']"));
                                            tbXong.Click();
                                            Thread.Sleep(1000);
                                        }
                                        catch
                                        {
                                            var Done = chromedriver.FindElement(By.XPath("//*[text()='Done']"));
                                            Done.Click();
                                            Thread.Sleep(1000);
                                        }

                                        Thread.Sleep(60000);

                                        row.Cells["cStatus"].Value = "lấy link";
                                        string link5 = GetCodeHotmail(usernamemail1, passwordmail1);
                                        StreamWriter sw1 = new StreamWriter("link.txt", true);
                                        sw1.WriteLine($"{idbm5}|{link5}");
                                        sw1.Close();
                                        row.Cells["cStatus"].Value = "lấy link ok";

                                    }

                                    // bm thứ 6
                                    string idbm6 = ids[5];
                                    if (idbm6 != null)
                                    {
                                        chromedriver.Navigate().GoToUrl($"https://business.facebook.com/latest/settings/business_users/?asset_id={idbm6}&business_id={idbm6}");
                                        Thread.Sleep(1000);
                                        try
                                        {
                                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Mời người khác']"));
                                            btninvitepeople.Click();
                                            Thread.Sleep(1000);
                                        }
                                        catch
                                        {
                                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Invite people']"));
                                            btninvitepeople.Click();
                                            Thread.Sleep(1000);
                                        }
                                        var danhsachmail1 = File.ReadAllLines("mail.txt").ToList();
                                        Random rnd1 = new Random();
                                        int indexRandom = rnd1.Next(0, danhsachmail1.Count);
                                        string mail6 = danhsachmail1[indexRandom];
                                        string usernamemail1 = mail6.Split('|')[0];
                                        string passwordmail1 = mail6.Split('|')[1];

                                        try
                                        {
                                            var nhapmailnahnbm = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div/div[1]/div[2]/div[2]/div/div/div[2]/div[1]/div[2]/div[2]/div/div/div[2]/div/div/div/div[1]/div[2]/div/div/div//input[@type='text']"));
                                            nhapmailnahnbm.SendKeys(usernamemail1);
                                            Thread.Sleep(1000);

                                        }
                                        catch
                                        {
                                            var nhapmailnahnbm1 = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div/div[1]/div[2]/div[2]/div/div/div[2]/div[1]/div[2]/div[2]/div/div/div[2]/div/div/div/div[1]/div[2]/div/div/div/input"));
                                            nhapmailnahnbm1.SendKeys(usernamemail1);
                                            Thread.Sleep(1000);

                                        }
                                        IWebElement tbTiep1 = null;
                                        try
                                        {
                                            tbTiep1 = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep1.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep1 = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep1.Click();
                                            Thread.Sleep(100);
                                        }
                                        var tbManage = chromedriver.FindElements(By.XPath("//input[@type='checkbox']"));
                                        tbManage[2].Click();
                                        Thread.Sleep(100);
                                        IWebElement tbTiep = null;
                                        try
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        try
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        try
                                        {
                                            var tbguiloimoi = chromedriver.FindElement(By.XPath("//*[text()='Gửi lời mời']"));
                                            tbguiloimoi.Click();
                                            Thread.Sleep(2000);

                                        }
                                        catch
                                        {
                                            var tbSendinvitation = chromedriver.FindElement(By.XPath("//*[text()='Send invitation']"));
                                            tbSendinvitation.Click();
                                            Thread.Sleep(2000);

                                        }
                                        try
                                        {
                                            var tbXong = chromedriver.FindElement(By.XPath("//*[text()='Xong']"));
                                            tbXong.Click();
                                            Thread.Sleep(1000);
                                        }
                                        catch
                                        {
                                            var Done = chromedriver.FindElement(By.XPath("//*[text()='Done']"));
                                            Done.Click();
                                            Thread.Sleep(1000);
                                        }

                                        Thread.Sleep(60000);

                                        row.Cells["cStatus"].Value = "lấy link";
                                        string link6 = GetCodeHotmail(usernamemail1, passwordmail1);
                                        StreamWriter sw1 = new StreamWriter("link.txt", true);
                                        sw1.WriteLine($"{idbm6}|{link6}");
                                        sw1.Close();
                                        row.Cells["cStatus"].Value = "lấy link ok";

                                    }

                                    // bm thứ 7
                                    string idbm7 = ids[6];
                                    if (idbm7 != null)
                                    {


                                        chromedriver.Navigate().GoToUrl($"https://business.facebook.com/latest/settings/business_users/?asset_id={idbm7}&business_id={idbm7}");
                                        Thread.Sleep(1000);
                                        try
                                        {
                                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Mời người khác']"));
                                            btninvitepeople.Click();
                                            Thread.Sleep(1000);
                                        }
                                        catch
                                        {
                                            btninvitepeople = chromedriver.FindElement(By.XPath("//*[text()='Invite people']"));
                                            btninvitepeople.Click();
                                            Thread.Sleep(1000);
                                        }
                                        var danhsachmail1 = File.ReadAllLines("mail.txt").ToList();
                                        Random rnd1 = new Random();
                                        int indexRandom = rnd1.Next(0, danhsachmail1.Count);
                                        string mail3 = danhsachmail1[indexRandom];
                                        string usernamemail1 = mail3.Split('|')[0];
                                        string passwordmail1 = mail3.Split('|')[1];

                                        try
                                        {
                                            var nhapmailnahnbm = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div/div[1]/div[2]/div[2]/div/div/div[2]/div[1]/div[2]/div[2]/div/div/div[2]/div/div/div/div[1]/div[2]/div/div/div//input[@type='text']"));
                                            nhapmailnahnbm.SendKeys(usernamemail1);
                                            Thread.Sleep(1000);

                                        }
                                        catch
                                        {
                                            var nhapmailnahnbm1 = chromedriver.FindElement(By.XPath("/html/body/span/div/div[1]/div[1]/div/div/div/div/div/div[1]/div[2]/div[2]/div/div/div[2]/div[1]/div[2]/div[2]/div/div/div[2]/div/div/div/div[1]/div[2]/div/div/div/input"));
                                            nhapmailnahnbm1.SendKeys(usernamemail1);
                                            Thread.Sleep(1000);

                                        }
                                        IWebElement tbTiep1 = null;
                                        try
                                        {
                                            tbTiep1 = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep1.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep1 = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep1.Click();
                                            Thread.Sleep(100);
                                        }
                                        var tbManage = chromedriver.FindElements(By.XPath("//input[@type='checkbox']"));
                                        tbManage[2].Click();
                                        Thread.Sleep(100);
                                        IWebElement tbTiep = null;
                                        try
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        try
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Tiếp']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        catch
                                        {
                                            tbTiep = chromedriver.FindElement(By.XPath("//*[text()='Next']"));
                                            tbTiep.Click();
                                            Thread.Sleep(100);
                                        }
                                        try
                                        {
                                            var tbguiloimoi = chromedriver.FindElement(By.XPath("//*[text()='Gửi lời mời']"));
                                            tbguiloimoi.Click();
                                            Thread.Sleep(2000);

                                        }
                                        catch
                                        {
                                            var tbSendinvitation = chromedriver.FindElement(By.XPath("//*[text()='Send invitation']"));
                                            tbSendinvitation.Click();
                                            Thread.Sleep(2000);

                                        }
                                        try
                                        {
                                            var tbXong = chromedriver.FindElement(By.XPath("//*[text()='Xong']"));
                                            tbXong.Click();
                                            Thread.Sleep(1000);
                                        }
                                        catch
                                        {
                                            var Done = chromedriver.FindElement(By.XPath("//*[text()='Done']"));
                                            Done.Click();
                                            Thread.Sleep(1000);
                                        }

                                        Thread.Sleep(60000);

                                        row.Cells["cStatus"].Value = "lấy link";
                                        string link7 = GetCodeHotmail(usernamemail1, passwordmail1);
                                        StreamWriter sw1 = new StreamWriter("link.txt", true);
                                        sw1.WriteLine($"{idbm7}|{link7}");
                                        sw1.Close();
                                        row.Cells["cStatus"].Value = "lấy link ok";

                                    }
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }

                #endregion
            }
            else if (urlchrome.Contains("https://www.instagram.com/accounts/suspended"))
            {
                Thread.Sleep(1000);
                row.Cells["cStatus"].Value = "IG 282";

            } else if (urlchrome.Contains("https://www.instagram.com/challenge/action"))
            {
                Thread.Sleep(1000);
                row.Cells["cStatus"].Value = "IG checkpoint mail";

            }

            chromedriver.Close();
            chromedriver.Quit();
        }
        private static void ResizeWindow(IWebDriver driver, int width, int height)
        {
            driver.Manage().Window.Size = new System.Drawing.Size(width, height);
        }
        private static void SetZoom(IWebDriver driver, double zoomFactor)
        {
            // phóng to thu nhỏ 
            var jsExecutor = (IJavaScriptExecutor)driver;
            jsExecutor.ExecuteScript($"document.body.style.zoom = '{zoomFactor * 100}%';");
        }
        private ChromeDriver chromeDriver1andanh()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("incognito", "--disable-3d-apis", "--disable-background-networking", "--disable-bundled-ppapi-flash", "--disable-client-side-phishing-detection", "--disable-default-apps", "--disable-hang-monitor", "--disable-prompt-on-repost", "--disable-sync", "--disable-webgl", "--enable-blink-features=ShadowDOMV0", "--enable-logging", "--disable-notifications", "--no-sandbox", "--disable-gpu", "--disable-dev-shm-usage", "--disable-web-security", "--disable-rtc-smoothness-algorithm", "--disable-webrtc-hw-decoding", "--disable-webrtc-hw-encoding", "--disable-webrtc-multiple-routes", "--disable-webrtc-hw-vp8-encoding", "--enforce-webrtc-ip-permission-check", "--force-webrtc-ip-handling-policy", "--ignore-certificate-errors", "--disable-infobars", "--disable-blink-features=\"BlockCredentialedSubresources\"", "--disable-popup-blocking");
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.DisableBuildCheck = true;
            chromeDriverService.HideCommandPromptWindow = true;
            ChromeDriver ChromeDriver1 = new ChromeDriver(chromeDriverService, options);
            ChromeDriver1.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
            ChromeDriver1.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            return ChromeDriver1;
        }
        private ChromeDriver Createchromedrive()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--disable-3d-apis", "--disable-background-networking", "--disable-bundled-ppapi-flash", "--disable-client-side-phishing-detection", "--disable-default-apps", "--disable-hang-monitor", "--disable-prompt-on-repost", "--disable-sync", "--disable-webgl", "--enable-blink-features=ShadowDOMV0", "--enable-logging", "--disable-notifications", "--no-sandbox", "--disable-gpu", "--disable-dev-shm-usage", "--disable-web-security", "--disable-rtc-smoothness-algorithm", "--disable-webrtc-hw-decoding", "--disable-webrtc-hw-encoding", "--disable-webrtc-multiple-routes", "--disable-webrtc-hw-vp8-encoding", "--enforce-webrtc-ip-permission-check", "--force-webrtc-ip-handling-policy", "--ignore-certificate-errors", "--disable-infobars", "--disable-blink-features=\"BlockCredentialedSubresources\"", "--disable-popup-blocking");
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.DisableBuildCheck = true;
            chromeDriverService.HideCommandPromptWindow = true;
            ChromeDriver chromeDriver = new ChromeDriver(chromeDriverService, options);
            chromeDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
            chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            return chromeDriver;
        }

        private string GetCodeHotmail(string usernamemail, string passwordmail)
        {
            string server = "pop3.live.com";
            OpenPop.Pop3.Pop3Client client = new OpenPop.Pop3.Pop3Client();
            client.Connect(server, 995, true);
            client.Authenticate(usernamemail, passwordmail);

            var messageCount = client.GetMessageCount();
            for (var i = messageCount; i >= 0; i--)
            {
                var msg = client.GetMessage(i).ToMailMessage().Body;

                Thread.Sleep(1000);


                if (msg.Contains("Meta Business Suite") && msg.Contains("https://fb.me/"))
                {
                    var document = new HtmlAgilityPack.HtmlDocument();
                    document.LoadHtml(msg);

                    var codeNode = document.DocumentNode.SelectSingleNode("//a[contains(@href,'fb.me/')]");
                    var linkBM = codeNode.Attributes["href"].Value;

                    return linkBM;
                }
            }

            return "";
        }

        private string GetCodeHotmailgettoanbolinkbm(string usernamemail, string passwordmail)
        {
            string server = "pop3.live.com";
            var links = new List<string>();

            OpenPop.Pop3.Pop3Client client = new OpenPop.Pop3.Pop3Client();
            client.Connect(server, 995, true);
            client.Authenticate(usernamemail, passwordmail);
            var messageCount = client.GetMessageCount();
            for (var i = messageCount; i >= 0; i--)
            {
                var msg = client.GetMessage(i).ToMailMessage().Body;

                Thread.Sleep(1000);


                if (msg.Contains("Meta Business Suite") && msg.Contains("https://fb.me/"))
                {
                    var document = new HtmlAgilityPack.HtmlDocument();
                    document.LoadHtml(msg);

                    // Select all nodes with the link
                    var codeNodes = document.DocumentNode.SelectNodes("//a[contains(@href,'fb.me/')]");
                    if (codeNodes != null)
                    {
                        foreach (var codeNode in codeNodes)
                        {
                            var linkBM = codeNode.Attributes["href"].Value;
                            links.Add(linkBM);
                        }

                        
                    }

                    
                }
            }

            string ALLLINK = links.ToString();

            return ALLLINK;
        }
        private void KillChromeDriver()
        {
            Process[] processes = Process.GetProcessesByName("chromedriver");
            foreach (Process process in processes)
            {
                process.Kill();
            }
        }




    }
}
