using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using xNet;
namespace REGBMIGHOC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void test()
        {
            // khai báo biến request
            HttpRequest http = new HttpRequest();
            // khai báo sử dụng cookie
            http.Cookies = new CookieDictionary();
            http.AddHeader("upgrade-insecure-requests", "1");
            // add AuserAgent 
            http.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36";

            var web = http.Get("https://www.instagram.com/accounts/login/");
            var cookie = http.Cookies.ToString();
            AddCookie(http, cookie);
            var Address = http.Address.AbsoluteUri;

            var webwww = http.Get("https://www.instagram.com/").ToString();
            Thread.Sleep(1000);
            string IG = "michellelillie08827";
            string passig = "iD68W3jWyTPhHBz";

            var data = $"enc_password={passig}&loginAttemptSubmissionCount=0&optIntoOneTap=false&queryParams=%7B%7D&trustedDeviceRecords=%7B%7D&username={IG}";
            var postlogin = http.Post("https://www.instagram.com/api/v1/web/accounts/login/ajax/", data, "application/json; charset=utf-8").ToString();
            Thread.Sleep(1000);


        }

        private void AddCookie(HttpRequest httpRequest, string cookie)
        {
            var cookieJar = cookie.Split(';');
            foreach (var cookieJarEntry in cookieJar)
            {
                var keyValuePair = cookieJarEntry.Split('=');
                try
                {
                    httpRequest.Cookies.Add(keyValuePair[0], keyValuePair[1]);
                }
                catch { }
            }
        }
        Dictionary<string, string> BMInfoDictionary = new Dictionary<string, string>();

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
                            string mail = viasplit[2];
                            string passmail = viasplit[3];
                            int add = 0;
                            dgv1.Invoke(new Action(() =>
                            {

                                add = dgv1.Rows.Add((dgv1.RowCount + 1), username, password);

                            }));
                            DataGridViewRow row = dgv1.Rows[add];
                            LoginIG(row, username, password, mail, passmail);

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


        private void LoginIG(DataGridViewRow row, string username, string password, string mail, string passmail)
        {
            var chromedriver = Createchromedrive();
            try
            {
                chromedriver.Navigate().GoToUrl("https://www.instagram.com/accounts/login/");
                Thread.Sleep(1000);
                row.Cells["cStatus"].Value = "Login";

                IWebElement tbusername = chromedriver.FindElement(By.Name("username"));
                tbusername.SendKeys(username);
                Thread.Sleep(100);
                IWebElement tbpass = chromedriver.FindElement(By.Name("password"));
                tbpass.SendKeys(password);
                Thread.Sleep(100);

                var login = chromedriver.FindElement(By.XPath("//*[text()='Log in']"));
                login.Click();
                Thread.Sleep(3000);

            }
            catch { }

            int solanlaplai = 0;
            bool loginthanhcong = true;
            var urlchrome = chromedriver.Url;
            while (urlchrome.Contains("https://www.instagram.com/accounts/login/"))
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                solanlaplai++;
                urlchrome = chromedriver.Url;

                if (solanlaplai == 10)
                {
                    loginthanhcong = false;
                    break;
                }
            }

            if (loginthanhcong == true)
            {
                if (urlchrome.Contains("https://www.instagram.com/auth_platform/codeentry") || urlchrome.Contains("https://www.instagram.com/challenge/action"))
                {
                    try
                    {
                        row.Cells["cStatus"].Value = "Checkpoint Mail=>Giải checkpoint";
                        var continue1 = chromedriver.FindElement(By.XPath("//div[text()='Continue']"));
                        continue1.Click();
                        Thread.Sleep(100);

                        // checkpoint mail
                        HttpRequest httpRequest = new HttpRequest();
                        httpRequest.AllowAutoRedirect = true;
                        httpRequest.Cookies = new CookieDictionary();
                        string mailgmx = $"{mail}|{passmail}";
                        Thread.Sleep(5000);
                        string codemailGMX = MailGMX(httpRequest, mailgmx);

                        while (string.IsNullOrEmpty(codemailGMX))
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                            codemailGMX = MailGMX(httpRequest, mailgmx);
                        }

                        urlchrome = chromedriver.Url;
                        var Securitycode = chromedriver.FindElement(By.Name("security_code"));
                        Securitycode.SendKeys(codemailGMX);
                        Thread.Sleep(100);

                        var submit = chromedriver.FindElement(By.XPath("//div[text()='Submit']"));
                        submit.Click();
                        Thread.Sleep(5000);

                        row.Cells["cStatus"].Value = "Đổi mật khẩu mới=>123123zz";
                        var tbNewPassword = chromedriver.FindElements(By.Name("new_password1"));
                        if (tbNewPassword.Count > 0)
                        {
                            tbNewPassword[0].SendKeys("123123zz");
                        }

                        var tbNewPassword2 = chromedriver.FindElements(By.Name("new_password2"));
                        if (tbNewPassword2.Count > 0)
                        {
                            tbNewPassword2[0].SendKeys("123123zz");
                        }

                        var btnNext = chromedriver.FindElements(By.XPath("//div[@role='button']"));
                        if (btnNext.Count > 0)
                        {
                            btnNext[0].Click();
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                        }

                    }
                    catch { }
                }


                if (urlchrome.Contains("https://www.instagram.com/accounts/suspended"))
                {
                    row.Cells["cStatus"].Value = "IG 282";
                }
                else
                {
                    row.Cells["cStatus"].Value = "Đăng nhập thành công=>Bật sáng tạo";

                    chromedriver.Navigate().GoToUrl("https://www.instagram.com/accounts/convert_to_professional_account/");

                    var scriptConvert = "(function() {\r\n    function getCSRFCookie(cookieString) {\r\n        const name = \"csrftoken=\";\r\n        const decodedCookie = decodeURIComponent(cookieString);\r\n        const cookieArray = decodedCookie.split(';');\r\n        for (let i = 0; i < cookieArray.length; i++) {\r\n            let cookie = cookieArray[i].trim();\r\n            if (cookie.indexOf(name) === 0) {\r\n                return cookie.substring(name.length, cookie.length);\r\n            }\r\n        }\r\n        return \"\";\r\n    }\r\n\r\n    const cookieString = document.cookie;\r\n    const csrfToken = getCSRFCookie(cookieString);\r\n\r\n    fetch(\"https://www.instagram.com/api/v1/business/account/convert_account/\", {\r\n        headers: {\r\n            \"content-type\": \"application/x-www-form-urlencoded\",\r\n            \"x-csrftoken\": csrfToken,\r\n        },\r\n        body: `category_id=180164648685982&create_business_id=true&entry_point=ig_web_settings&set_public=true&should_bypass_contact_check=true&should_show_category=0&to_account_type=2`,\r\n        method: \"POST\",\r\n        mode: \"cors\",\r\n        credentials: \"include\"\r\n    }).then(response => response.json()).then(data => {\r\n        console.log(\"Done\");\r\n        console.log(data);\r\n    }).catch(error => {\r\n        console.error(\"Error:\", error);\r\n    });\r\n})();\r\n";
                    chromedriver.ExecuteScript(scriptConvert);

                    Thread.Sleep(3000);


                    row.Cells["cStatus"].Value = "Reg BM";

                    chromedriver.Navigate().GoToUrl("https://business.facebook.com/");

                __RETRY:
                    var loginwithInstagram = chromedriver.FindElement(By.XPath("//*[text()='Log in with Instagram']"));
                    loginwithInstagram.Click();
                    Thread.Sleep(3000);

                    var uidbm = Regex.Match(chromedriver.PageSource, "business_id=(.*?)\"}").Groups[1].Value;
                    if (string.IsNullOrEmpty(uidbm))
                    {
                        try
                        {
                            var loginasinstagram = chromedriver.FindElements(By.XPath("//div[@role='button']"));
                            loginasinstagram[0].Click();
                            Thread.Sleep(5000);

                        }
                        catch { }

                        try
                        {
                            try
                            {
                                var btnLoginIG = chromedriver.FindElements(By.XPath("//*[text()='Log in with Instagram']"));
                                if (btnLoginIG.Count() > 0)
                                {
                                    goto __RETRY;
                                }

                                var chapnhan = chromedriver.FindElements(By.XPath("//*[text()='Chấp nhận']"));
                                if (chapnhan.Count() > 0)
                                {
                                    chapnhan[1].Click();
                                }
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
                        }
                        catch { }
                    }

                    chromedriver.Navigate().GoToUrl("https://business.facebook.com/latest/settings/business_users/?business_id=");
                    uidbm = Regex.Match(chromedriver.PageSource, "business_id=(.*?)\"}").Groups[1].Value;
                    var nameBM = Regex.Match(chromedriver.PageSource, "globalScopeName\":\"(.*?)\"").Groups[1].Value;

                    if (!string.IsNullOrEmpty(uidbm))
                    {
                        row.Cells["cStatus"].Value = "Add thông tin businessUser";
                        try
                        {
                            BMInfoDictionary.Add(nameBM, uidbm);
                        }
                        catch { }

                        var scriptAddThongTin = $"let token = require(\"DTSGInitialData\").token || document.querySelector('[name=\"fb_dtsg\"]').value,\r\n    uid = require(\"CurrentUserInitialData\").USER_ID || document.cookie.match(/c_user=([0-9]+)/)[1],\r\n    businessId = require(\"CurrentBusinessUser\").business_id,\r\n    businessUserID = require(\"CurrentBusinessUser\").business_user_id;\r\n\r\nfetch(\"https://business.facebook.com/api/graphql/?_callFlowletID=3860&_triggerFlowletID=3860\", {{\r\n    headers: {{\r\n        \"content-type\": \"application/x-www-form-urlencoded\",\r\n        \"x-fb-friendly-name\": \"BizKitSettingsUpdateBusinessUserForIGMAMutation\"\r\n    }},\r\n    body: `av=${{uid}}&__usid=6-Tsiqcgzf3m1eb:Psiqcib1f13xh9:0-Asiqcgzolcxdk-RV=6:F=&__aaid=0&__bid=${{businessId}}&__user=${{uid}}&__a=1&__req=2g&__hs=19959.HYP:bizweb_comet_pkg.2.1..0.0&dpr=1&__ccg=GOOD&__rev=1015950991&__s=cjs5u2:pbzma4:as9gve&__hsi=7406736738879158218&__dyn=7xeUmxa2C6onwn8K2Wmh0MBwCwpUnwgU7SbzEdF8ixy361twYwJw4BwHz8hw9-0r-qbwgEdo4to4C0KEswIwuo9oeUa8462mcw4JwgECu1vw9m1YwBgao6C1uwiUmw9O48W2a4p8aHwzzXwKwt8jwGzEaE8o4-222SU5G4E5yexfwjES1xwjokGvwOwem32fwLCyKbwzwea0Lo6-3u36iU9E2cwNwDwjouwqo4e220hi7E5y1rw&__csr=gfIpIQJiiRi9t8DbqPtqkXbh3h5t8_EB4qEhZGO9R8hnkAj94O8QBBmSiR8OluFllJlRFZ_SGJd6CqCBuWABiAqq9LqZA_K8yHCjzESL-iWiijGnpEDHg-cyokDzHy48AiJ4BAUlBGAWRp94ihkKiECKqvG44rhpaBAxibQcwA-EGiaAgiGm9KGxK4Ey9-al6gymayoPGh5yVp8ryt2F9EvAUyazU-i2KqcwQxG8xuq2JohAHAKcy8gAUDxqEy2G1axmag9Elz8OdzUDG6US3u0yQfxqEixm1UAxu04kE6ibgj9cUtg8QgE24w9u1-eFEB3egjuVVlGX4x29CGECUoCxFa3a6ryk9AAxmHOzeabxf-489pO2UaK498uwBx-2V1O3W1QU6S260xk1vwso184Ctrhk1hgoho-dwNlPkA9oly6dOpi1aUpUkw7IF022gG96gka2-2dg5i0lK0eYxS0oh0dKE0i3zo1fQ1kwwa0FAOUMF88FqBwby0UE8A321mxp00lao8E2H2xb61klw0xmU1-U0o8DBw1bO0vK5cMtw5oo5e480HWU6mi0qa0dq80oK2a589R7cawywl61pwqobQ028i4olS48a920rUeo88Oqq0FFU0TG2a02aiz01rd0yy9FXoc86a0c4Bg4d0d2po1w9QaCg1XFo0sEy80j5ws9UnDDgG0deBo1j80z-uu0qm0gO3O0KsjcoC4pQQ5Uvy8cEak4y97WjgO68&__comet_req=11&fb_dtsg=${{token}}&jazoest=25365&lsd=Z1d4jKYEGWH95IlAa2j0-A&__spin_r=1015950991&__spin_b=trunk&__spin_t=1724515282&__jssesw=1&fb_api_caller_class=RelayModern&fb_api_req_friendly_name=BizKitSettingsUpdateBusinessUserForIGMAMutation&variables={{\"businessUserID\":\"${{businessUserID}}\",\"firstName\":null,\"lastName\":null,\"email\":\"akajsjss111@hotmail.com\",\"roles\":null,\"business_account_task_ids\":null,\"expireTime\":null,\"clientTimezoneID\":null,\"businessID\":\"${{businessId}}\",\"nonce\":null,\"should_send_email_notif\":null,\"entryPoint\":\"BIZWEB_SETTINGS_ADD_CONTACT_INFO_UPDATE_BUSINESS_USER_FOR_IGMA\"}}&server_timestamps=true&doc_id=6412870625471134`,\r\n    method: \"POST\",\r\n    mode: \"cors\",\r\n    credentials: \"include\"\r\n}}).then(response => response.json()).then(data => {{\r\n    console.log(\"Done\");\r\n    console.log(data);\r\n}});";
                        chromedriver.ExecuteScript(scriptAddThongTin);
                        Thread.Sleep(3000);
                        chromedriver.Navigate().Refresh();

                        var danhsachmail1 = File.ReadAllLines("mail.txt").ToList();
                        string mail1 = danhsachmail1[0];
                        string usernamemail1 = mail1.Split('|')[0];
                        string passwordmail1 = mail1.Split('|')[1];

                        row.Cells["cStatus"].Value = "Share về mail "+ usernamemail1;
                        var scriptInvite = $"let token = require(\"DTSGInitialData\").token || document.querySelector('[name=\"fb_dtsg\"]').value,\r\n    uid = require(\"CurrentUserInitialData\").USER_ID || document.cookie.match(/c_user=([0-9]+)/)[1],\r\n    businessId = require(\"CurrentBusinessUser\").business_id;\r\n\r\nfetch(\"https://business.facebook.com/api/graphql/?_callFlowletID=3860&_triggerFlowletID=3860\", {{\r\n    headers: {{\r\n        \"content-type\": \"application/x-www-form-urlencoded\",\r\n        \"x-fb-friendly-name\": \"BizKitSettingsInvitePeopleModalMutation\"\r\n    }},\r\n    body: `av=${{uid}}&__usid=6-Tsiqcgzf3m1eb:Psiqcib1f13xh9:0-Asiqcgzolcxdk-RV=6:F=&__aaid=0&__bid=${{businessId}}&__user=${{uid}}&__a=1&__req=6s&__hs=19959.HYP:bizweb_comet_pkg.2.1..0.0&dpr=1&__ccg=GOOD&__rev=1015950991&__s=m2vooy:pbzma4:as9gve&__hsi=7406736738879158218&__dyn=7xeUmxa2C6onwn8K2Wmh0MBwCwpUnwgU7SbzEdF8ixy361twYwJw4BwHz8hw9-0r-qbwgEdo4to4C0KEswIwuo9oeUa8462mcw4JwgECu1vw9m1YwBgao6C1uwiUmwoEeogzE8EhAwGK2efK2W1Qxe2GewGwxwjU88brwmEiwm8W4-1ezo661dxiF-3a0Voc8-2-qaUK2e0UE2ZwrUdUcpbwCw8O362u1dxW1FwgU88158uwm85K&__csr=gfIpIQJiiRi9t8DbqPtqkXbh3h5t8_EB4qEhZGO9R8hnkAj94O8QBBmSiR8OluFllJlRFZ_SGJd6CqCBuWABiAqq9LqZA_K8yHCjzESL-iWiijGnpEDHg-cyokDzHy48AiJ4BAUlBGAWRp94ihkKiECKqvG44rhpaBAxibQcwA-EGiaAgiGm9KGxK4Ey9-al6gymayoPGh5yVp8ryt2F9EvAUyazU-i2KqcwQxG8xuq2JohAHAKcy8gAUDxqEy2G1axmag9Elz8OdzUDG6US3u0yQfxqEixm1UAxu04kE6ibgj9cUtg8QgE24w9u1-eFEB3egjuVVlGX4x29CGECUoCxFa3a6ryk9AAxmHOzeabxf-489pO2UaK498uwBx-2V1O3W1QU6S260xk1vwso184Ctrhk1hgoho-dwNlPkA9oly6dOpi1aUpUkw7IF022gG96gka2-2dg5i0lK0eYxS0oh0dKE0i3zo1fQ1kwwa0FAOUMF88FqBwby0UE8A321mxp00lao8E2H2xb61klw0xmU1-U0o8DBw1bO0vK5cMtw5oo5e480HWU6mi0qa0dq80oK2a589R7cawywl61pwqobQ028i4olS48a920rUeo88Oqq0FFU0TG2a02aiz01rd0yy9FXoc86a0c4Bg4d0d2po1w9QaCg1XFo0sEy80j5ws9UnDDgG0deBo1j80z-uu0qm0gO3O0KsjcoC4pQQ5Uvy8cEak4y97WjgO68&__comet_req=11&fb_dtsg=${{token}}&jazoest=25365&lsd=Z1d4jKYEGWH95IlAa2j0-A&__spin_r=1015950991&__spin_b=trunk&__spin_t=1724515282&__jssesw=1&qpl_active_flow_ids=558499583&fb_api_caller_class=RelayModern&fb_api_req_friendly_name=BizKitSettingsInvitePeopleModalMutation&variables={{\"input\":{{\"client_mutation_id\":\"5\",\"actor_id\":\"${{uid}}\",\"business_id\":\"${{businessId}}\",\"business_emails\":[\"{usernamemail1}\"],\"business_account_task_ids\":[\"926381894526285\",\"603931664885191\",\"1327662214465567\",\"862159105082613\",\"6161001899617846786\",\"1633404653754086\",\"967306614466178\",\"2848818871965443\",\"245181923290198\",\"388517145453246\"],\"invite_origin_surface\":\"MBS_INVITE_USER_FLOW\",\"assets\":[],\"expiry_time\":0,\"is_spark_permission\":false,\"client_timezone_id\":\"Asia/Jakarta\"}}}}&server_timestamps=true&doc_id=23919966164285762&fb_api_analytics_tags=[\"qpl_active_flow_ids=558499583\"]`,\r\n    method: \"POST\",\r\n    mode: \"cors\",\r\n    credentials: \"include\"\r\n}}).then(response => response.json()).then(data => {{\r\n    console.log(\"Done\");\r\n    console.log(data);\r\n}});";
                        var resultInvite = chromedriver.ExecuteScript(scriptInvite);
                        Thread.Sleep(3000);
                        if (resultInvite != null)
                        {
                            var resultInviteText = resultInvite.ToString();
                            if (resultInviteText.Contains("errors"))
                            {
                                row.Cells["cStatus"].Value = "MOI THAT BAI";
                            }
                            else
                            {
                                row.Cells["cStatus"].Value = "MOI THANH CONG";
                            }

                        }
                        try
                        {

                            var cookies = chromedriver.Manage().Cookies.AllCookies;
                            var cookieString = "";
                            foreach (var cookie in cookies)
                            {
                                cookieString += cookie.Name + "=" + cookie.Value + ";";
                            }

                            cookieString = cookieString.TrimEnd(';');

                            HttpRequest http = new HttpRequest();
                            http.Cookies = new CookieDictionary();
                            http.UserAgent = Http.ChromeUserAgent();
                            http.KeepAlive = true;

                            var cookieRaws = cookieString.Split(';');
                            foreach (var cookieRaw in cookieRaws)
                            {
                                var cookieJar = cookieRaw.Split('=');
                                try
                                {
                                    http.Cookies.Add(cookieJar[0], cookieJar[1]);
                                }
                                catch { }
                            }
                            for (int i = 0; i < 2; i++)
                            {
                                row.Cells["cStatus"].Value = "Reg BM thứ " + (i + 2);
                                chromedriver.Navigate().GoToUrl($"https://business.facebook.com/billing_hub/accounts?business_id={uidbm}&placement=standalone&global_scope_id={uidbm}");
                                var pageSourcebillinghub = chromedriver.PageSource;
                                var fbdtsgbilling = Regex.Match(pageSourcebillinghub, "DTSGInitialData\",\\[],{\"token\":\"(.*?)\"").Groups[1].Value;
                                var lsdbilling = Regex.Match(pageSourcebillinghub, "LSD\\\",\\[],{\\\"token\\\":\\\"(.*?)\\\"").Groups[1].Value;
                                var userIDbilling = Regex.Match(pageSourcebillinghub, "userID\":\"(.*?)\"").Groups[1].Value;
                                Random random = new Random();
                                int numberRandom = random.Next(0, 100000000);
                                string nameBusiness = "VuThai95" + numberRandom;
                                string Name = "Thai";
                                string Lastname = "Vu";
                                string mailbusiness = "vu1882168@gmail.com";

                                var databilling = $"av={userIDbilling}&__usid=6-Tsj2f3n10ciz3x:Psj2foz1pqge5l:0-Asj2f0rv8vjy2-RV=6:F=&__aaid=0&__bid={uidbm}&__user={userIDbilling}&__a=1&__req=w&__hs=19966.BP:DEFAULT.2.0..0.0&dpr=1&__ccg=EXCELLENT&__rev=1016131455&__s=ed5dxo:k8nyiz:bhvqcl&__hsi=7409159100372555872&__dyn=7xeUmxa2C5rgmwCwRyU8EKmhe2Om2q1DxuqErxSax21dxebzEdF98Sm4Euxa1MxKdwJzUmxe1kx20zEyaxG4o4B0l898888oe82xwCCwjFEK2Z17wJBGEpiwzlBwgrxK261UxO4VA48a8lwWxe4oeUa85Cdw9-0CE4a4ouyUd85W7o6eu2C2l0FgKi3a2i11grzUeUmwvC6UgzE8EhAwGK2efK2i9wAx25U9F8W6888dUnwj8888US1qxa3O6UW4UnwhFU2DxiaBwKwgocU4e2K7EOicG3qazo8U3yDwbm5E5y1FAK2q1bzEG2q362u1dxW6Uc8swn9UgxS0Vo7u1rwGw&__csr=&fb_dtsg={fbdtsgbilling}&jazoest=25634&lsd={lsdbilling}&__spin_r=1016131455&__spin_b=trunk&__spin_t=1725079282&__jssesw=1&qpl_active_flow_ids=1001920343,558502430&fb_api_caller_class=RelayModern&fb_api_req_friendly_name=useBusinessCreationMutationMutation&variables={{\"input\":{{\"client_mutation_id\":\"2\",\"actor_id\":\"{userIDbilling}\",\"business_name\":\"{nameBusiness}\",\"user_first_name\":\"{Name}\",\"user_last_name\":\"{Lastname}\",\"user_email\":\"{mailbusiness}\",\"creation_source\":\"BM_HOME_BUSINESS_CREATION_IN_SCOPE_SELECTOR\",\"entry_point\":\"UNIFIED_GLOBAL_SCOPE_SELECTOR\"}}}}&server_timestamps=true&doc_id=7780408488685584&fb_api_analytics_tags=[\"qpl_active_flow_ids=1001920343,558502430\"]";

                                var posttaobm2 = http.Post("https://business.facebook.com/api/graphql/?_callFlowletID=10444&_triggerFlowletID=10444", databilling, "application/x-www-form-urlencoded").ToString();

                                if (posttaobm2.Contains("profile_picture_url"))
                                {
                                    //status create thanh cong
                                    row.Cells["cStatus"].Value = "Reg BM thứ " + (i + 2) + " thành công";
                                    var uidBMNew = JObject.Parse(posttaobm2)["data"]["bizkit_create_business"]["id"].ToString();
                                    chromedriver.Navigate().GoToUrl($"https://business.facebook.com/latest/settings/business_users/?asset_id={uidBMNew}&business_id={uidBMNew}");

                                    var nameBMNew = Regex.Match(chromedriver.PageSource, "globalScopeName\":\"(.*?)\"").Groups[1].Value;


                                    try
                                    {
                                        BMInfoDictionary.Add(nameBMNew, uidBMNew);
                                    }
                                    catch { }


                                    chromedriver.ExecuteScript(scriptInvite);

                                    Thread.Sleep(3000);
                                }
                                else
                                {
                                    row.Cells["cStatus"].Value = "Reg BM thứ " + (i + 2) + " thất bại";
                                }
                            }

                            Thread.Sleep(TimeSpan.FromSeconds(20));


                        }
                        catch { row.Cells["cStatus"].Value = "IG KO REG ĐƯỢC BM"; }

                        row.Cells["cStatus"].Value = "Get toàn bộ link BM";
                        var links = GetCodeHotmailgettoanbolinkbm(usernamemail1, passwordmail1);
                        var listBM = new List<string>();

                        
                        lock (thisObject)
                        {
                            foreach (var link in links)
                            {
                                var nameBMMail = link.Key;
                                var BMInfomation = BMInfoDictionary.Where(x => nameBMMail.Contains(x.Key)).FirstOrDefault();

                                if (!BMInfomation.Equals(new KeyValuePair<string, string>()))
                                {
                                    listBM.Add($"{BMInfomation.Value}|{link.Value}");
                                    BMInfoDictionary.Remove(BMInfomation.Key);
                                }
                            }

                            row.Cells["cStatus"].Value = "Tìm thấy " + listBM.Count + " linkBM";
                            while (true)
                            {
                                try
                                {
                                    File.WriteAllLines("Business.txt", listBM);
                                    foreach (var BMData in listBM)
                                    {
                                        StreamWriter sw = new StreamWriter("Busiess.txt", true);
                                        sw.WriteLine(BMData);
                                        sw.Close();
                                    }
                                    row.Cells["cStatus"].Value = "Xong";
                                    break;
                                }
                                catch { }
                            }
                        }
                    }

                }
            }
            else
            {
                row.Cells["cStatus"].Value = "Login thất bại";
            }

            chromedriver.Close();
            chromedriver.Quit();
        }

        private object thisObject = new object();
        List<string> Names = new List<string>()
        {
    "James",
    "John",
    "Robert",
    "Michael",
    "William",
    "David",
    "Joseph",
    "Charles",
    "Thomas",
    "Christopher",
    "Daniel",
    "Matthew",
    "Anthony",
    "Mark",
    "Donald",
    "Steven",
    "Paul",
    "Andrew",
    "Joshua",
    "Kenneth",
    "Kevin",
    "Brian",
    "George",
    "Edward",
    "Ronald",
    "Timothy",
    "Jason",
    "Jeffrey",
    "Ryan",
    "Jacob",
    "Gary",
    "Nicholas",
    "Eric",
    "Stephen",
    "Jonathan",
    "Larry",
    "Justin",
    "Scott",
    "Brandon",
    "Frank",
    "Benjamin",
    "Gregory",
    "Samuel",
    "Raymond",
    "Patrick",
    "Alexander",
    "Jack",
    "Dennis",
    "Jerry",
    "Tyler",
    "Aaron",
    "Henry",
    "Douglas",
    "Peter",
    "Jose",
    "Adam",
    "Zachary",
    "Nathan",
    "Walter",
    "Harold",
    "Kyle",
    "Carl",
    "Arthur",
    "Gerald",
    "Roger",
    "Keith",
    "Lawrence",
    "Terry",
    "Sean",
    "Christian",
    "Albert",
    "Joe",
    "Ethan",
    "Austin",
    "Jesse",
    "Willie",
    "Billy",
    "Bryan",
    "Bruce",
    "Jordan",
    "Ralph",
    "Roy",
    "Noah",
    "Dylan",
    "Eugene",
    "Wayne",
    "Alan",
    "Juan",
    "Louis",
    "Russell",
    "Gabriel",
    "Randy",
    "Philip",
    "Harry",
    "Vincent",
    "Bobby",
    "Johnny"
};
        List<string> LastNames = new List<string>()
        {
    "Smith",
    "Johnson",
    "Williams",
    "Brown",
    "Jones",
    "Garcia",
    "Miller",
    "Davis",
    "Rodriguez",
    "Martinez",
    "Hernandez",
    "Lopez",
    "Gonzalez",
    "Wilson",
    "Anderson",
    "Thomas",
    "Taylor",
    "Moore",
    "Jackson",
    "Martin",
    "Lee",
    "Perez",
    "Thompson",
    "White",
    "Harris",
    "Sanchez",
    "Clark",
    "Ramirez",
    "Lewis",
    "Robinson",
    "Walker",
    "Young",
    "Allen",
    "King",
    "Wright",
    "Scott",
    "Torres",
    "Nguyen",
    "Hill",
    "Flores",
    "Green",
    "Adams",
    "Nelson",
    "Baker",
    "Hall",
    "Rivera",
    "Campbell",
    "Mitchell",
    "Carter",
    "Roberts",
    "Gomez",
    "Phillips",
    "Evans",
    "Turner",
    "Diaz",
    "Parker",
    "Cruz",
    "Edwards",
    "Collins",
    "Reyes",
    "Stewart",
    "Morris",
    "Morales",
    "Murphy",
    "Cook",
    "Rogers",
    "Gutierrez",
    "Ortiz",
    "Morgan",
    "Cooper",
    "Peterson",
    "Bailey",
    "Reed",
    "Kelly",
    "Howard",
    "Ramos",
    "Kim",
    "Cox",
    "Ward",
    "Richardson",
    "Watson",
    "Brooks",
    "Chavez",
    "Wood",
    "James",
    "Bennett",
    "Gray",
    "Mendoza",
    "Ruiz",
    "Hughes",
    "Price",
    "Alvarez",
    "Castillo",
    "Sanders",
    "Patel",
    "Myers",
    "Long",
    "Ross",
    "Foster",
    "Jimenez"
};


        private static void ResizeWindow(IWebDriver driver, int width, int height)
        {
            driver.Manage().Window.Size = new System.Drawing.Size(width, height);
        }

        private HttpRequest CreateHttpRequest(string user_agent)
        {
            HttpRequest request = new HttpRequest
            {
                Cookies = new CookieDictionary(),
                AllowAutoRedirect = true,
                UserAgent = user_agent,
                KeepAlive = true
            };
            request.AddHeader("Upgrade-Insecure-Requests", "1");

            return request;
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
            options.AddArgument("--window-size=320,480");
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
        private string MailGMX(HttpRequest httpRequest, string mailgmxlive)
        {
            string mailpassmail = mailgmxlive;
            string maill = mailpassmail.Split('|')[0];
            string tenmail = maill.Split('@')[0];
            string password = mailpassmail.Split('|')[1];
            var emailSrc = httpRequest.Get($"https://gmx.live/login/api.php?login={tenmail}@gmx.live|{password}").ToString();
            var letters = httpRequest.Get($"https://gmx.live/login/new.php?login={tenmail}@gmx.live|{password}").ToString();
            var codemail = Regex.Match(letters, "<font size=\"6\">(.*?)<\\/font>").Groups[1].Value;


            return codemail;
        }
        private Dictionary<string, string> GetCodeHotmailgettoanbolinkbm(string usernamemail, string passwordmail)
        {
            string server = "pop3.live.com";
            var linkDict = new Dictionary<string, string>();

            try
            {
                OpenPop.Pop3.Pop3Client client = new OpenPop.Pop3.Pop3Client();
                client.Connect(server, 995, true);
                client.Authenticate(usernamemail, passwordmail);
                var messageCount = client.GetMessageCount();
                for (var i = messageCount; i > 0; i--)
                {
                    var msg = client.GetMessage(i).ToMailMessage().Body;

                    Thread.Sleep(1000);

                    try
                    {
                        if (msg.Contains("Meta Business Suite") && msg.Contains("https://fb.me/"))
                        {
                            try
                            {
                                var document = new HtmlAgilityPack.HtmlDocument();
                                document.LoadHtml(msg);

                                // Select all nodes with the link
                                var codeNodes = document.DocumentNode.SelectSingleNode("//a[contains(@href,'fb.me/')]");
                                var name = document.DocumentNode.SelectSingleNode("/html[1]/table[1]/tr[1]/td[1]/table[1]/tr[2]/td[3]/table[1]/tr[1]/td[1]/table[1]/tr[3]/td[1]/center[1]/span[1]/span[2]");

                                var linkBM = codeNodes.Attributes["href"].Value;

                                linkDict.Add(name.InnerText, linkBM);
                            }
                            catch { }

                            client.DeleteMessage(i);
                        }
                    }
                    catch { }
                }
            }
            catch { }

            return linkDict;
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
