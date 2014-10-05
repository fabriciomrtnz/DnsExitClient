using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.IO;
using System.Web;

namespace DnsExitClient
{
    class Program
    {
        static void Main(string[] args)
        {
            
            try
            {
                //1.Check Web Ip.
                string myIp = GetMyIp();

                //2.Compare to temp file.
                if (File.Exists(Properties.Settings.Default.IpTempFile))
                {
                    //3. If change update web else exit
                    //string ipFile = File.ReadAllText(Properties.Settings.Default.IpTempFile);
                    string ipFile = File.ReadAllText(ConfigurationManager.AppSettings["IpTempFile"]);
                    if (myIp.Trim() != ipFile.Trim())
                    {
                        UpdateMyIp(myIp);
                        File.WriteAllText(ConfigurationManager.AppSettings["IpTempFile"], myIp, Encoding.UTF8);
                    }
                    else
                    {
                        Console.WriteLine("System Ip Has Not Changed");
                    }
                }
                else
                {
                    //4. Never updated or file lost, update.
                    UpdateMyIp(myIp);
                    File.WriteAllText(ConfigurationManager.AppSettings["IpTempFile"], myIp, Encoding.UTF8);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }

        static private string GetMyIp()
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            Stream data = client.OpenRead(ConfigurationManager.AppSettings["IpWhoIsUrl"]);
            StreamReader reader = new StreamReader(data);
            string myIp = reader.ReadToEnd();
            data.Close();
            reader.Close();

            return myIp.Trim();
        }

        static private string UpdateMyIp(string ip)
        {
            //http://update.dnsexit.com/RemoteUpdate.sv?login=$MyLogin&amp;password=$MyPassword&amp;host=$MyDomains&amp;myip=$MyIp
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            string login = ConfigurationManager.AppSettings["Login"];
            string pass = ConfigurationManager.AppSettings["Password"];
            string myDomains = ConfigurationManager.AppSettings["MyDomains"];
            string updateUrl = ConfigurationManager.AppSettings["UpdateUrl"];
            updateUrl = updateUrl.Replace("$MyIp", ip).Replace("$MyDomains", HttpUtility.UrlEncode(ConfigurationManager.AppSettings["MyDomains"]))
                                 .Replace("$MyLogin", HttpUtility.UrlEncode(login)).Replace("$MyPassword", HttpUtility.UrlEncode(pass));

            Stream data = client.OpenRead(updateUrl);
            StreamReader reader = new StreamReader(data);
            string result = reader.ReadToEnd();
            data.Close();
            reader.Close();

            return result;
        }
    }
}
