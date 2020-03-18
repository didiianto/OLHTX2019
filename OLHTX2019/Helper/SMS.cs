using OLHTX2019.Helper;
using OLHTX2019.Models.DB;
using OLHTX2019.Models.EntityManager;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

/// <summary>
/// Summary description for SMS
/// </summary>
public class SMS
{
    public static string SendSMSToAdmin(string mobile, string otpCode)
    {
        string rtnValue = "";
        string message = "";

        if (!String.IsNullOrEmpty(mobile))
        {
            UnitOfWork unitofWork = new UnitOfWork();
            Setting setting = GeneralHelper.GetSetting(unitofWork);

            string  paxTemplate = setting.WebLocation + "Storage\\Sms\\Template.txt";

            message = ReadSMSTemplate(paxTemplate);
            message = message.Replace("#code#", otpCode);
            rtnValue = SendSMS(message, mobile);
        }
        return rtnValue;
    }
    
    public static string ReadSMSTemplate(string filePath)
    {
        // Read the file as one string.
        string _fileContent = "";

        try
        {
            System.IO.StreamReader _file = new System.IO.StreamReader(filePath);
            _fileContent = _file.ReadToEnd();

            _file.Close();
        }
        catch (Exception err)
        {
            _fileContent = err.Message;
        }

        return (_fileContent);
    }

    public static string SendSMS(string message, string mobile)
    {
        // BULK SMS
        string server = System.Configuration.ConfigurationSettings.AppSettings["SMSServer2"];
        string username = System.Configuration.ConfigurationSettings.AppSettings["SMSUserID2"];
        string password = System.Configuration.ConfigurationSettings.AppSettings["SMSPwd2"];
        string sender = System.Configuration.ConfigurationSettings.AppSettings["SMSSender2"];
        string allowconcat = "1"; // 0 for disable, enable to allow message more than 160 chars
        //string mobile = "";

        string result = "";

        if (mobile.Length == 8)
        {
            if (mobile.Length == 8)
            {
                if (mobile.Substring(0, 2) != "65") mobile = "65" + mobile;
            }

            message = System.Web.HttpUtility.UrlEncode(message);

            // building the post data and attempt to send
            string strPost = String.Format("username={0}&password={1}&message={2}&msisdn={3}&sender={4}&allow_concat_text_sms={5}", username, password, message, mobile, sender, allowconcat);
            HttpWebRequest myHttpRequest = (HttpWebRequest)WebRequest.Create(server);

            // construct http post
            myHttpRequest.Method = "POST";
            myHttpRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpRequest.ContentLength = strPost.Length;

            // Send it
            StreamWriter StrWriter = new StreamWriter(myHttpRequest.GetRequestStream());
            StrWriter.Write(strPost);
            StrWriter.Close();

            // Get the response
            HttpWebResponse myHttpResponse = (HttpWebResponse)myHttpRequest.GetResponse();
            StreamReader StrReader = new StreamReader(myHttpResponse.GetResponseStream());
            result = StrReader.ReadToEnd();
            StrReader.Close();
                
        }

        return result;
    }
}

public class SMSRecipient
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public string Contact { get; set; }
}