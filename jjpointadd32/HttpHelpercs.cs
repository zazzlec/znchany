using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace znrsserver
{
    public class HttpHelpercs
    {

        public static void AddLgoToTXT(string logstring)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "logs/log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            if (!System.IO.File.Exists(path))
            {
                FileStream stream = System.IO.File.Create(path);
                stream.Close();
                stream.Dispose();
            }
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(logstring);
            }
        }
        public static string HttpGet(string url)
        {
            string result = "";
            try
            {
                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);
                wbRequest.Method = "GET";
                wbRequest.ContentType = "application/json";//链接类型
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                using (Stream responseStream = wbResponse.GetResponseStream())
                {
                    using (StreamReader sReader = new StreamReader(responseStream, System.Text.Encoding.Default))
                    {
                        result = sReader.ReadToEnd();
                    }
                }





                
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }


        public static string HttpGet(string url, System.Text.Encoding dd)
        {
            string result = "";
            try
            {
                HttpWebRequest wbRequest = (HttpWebRequest)WebRequest.Create(url);
                wbRequest.Method = "GET";
                wbRequest.ContentType = "application/x-www-form-urlencoded";//链接类型
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                using (Stream responseStream = wbResponse.GetResponseStream())
                {
                    using (StreamReader sReader = new StreamReader(responseStream, dd))
                    {
                        result = sReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public static string token="";
        public static string account = "znch003";//登录账号，需要赋值
        public static string password = "shglznch";//登录密码，需要赋值

        //public static int bid = 5;
        //public static string url = "http://132.0.5.80:8721";//接口地址，需要赋值

        public static int bid = 6;
        public static string url = "http://132.0.6.80:8721";//接口地址，需要赋值
        public static int ipindex = 0;

       
        public static string qz = "132.0";
        public static string dk = "8721";
        public static string[] host = { "80","81","82" };

        public static string gethost()
        {

            return "http://" + qz + "." + bid + "." + host[ipindex] + ":" + dk + "";
        }

        public static string geturl()
        {
            return "http://" + qz + "." + bid + "." + host[ipindex] + ":" + dk + "/account/login";
        }
        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        public static string gettoken()
        {
            if (string.IsNullOrEmpty(token))
            {
                string url_token = geturl();

                //JObject param1 = (JObject)JsonConvert.DeserializeObject("{\"account\": \""+account+"\",\"password\": \""+password+"\"}");
                string param1 = "{\"username\":\"" + account + "\",\"password\":\"" + password + "\"}";
                string data = HttpPostTk(url_token, param1);
                return data;
                //   JObject ret = JObject.Parse(data);
                //  token = ret["authorization"].ToString();
            }
            return token;
        }


        public static string HttpPostTk(string postUrl, string param)
        {
            string ret = "";
            try
            {

                //byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.Headers.Add("ContentType", "application/json");


                webReq.ContentType = "application/json";
                // byte[] btBodys = Encoding.UTF8.GetBytes(param.ToString());
                //string a = "{\"account\": \"ics_data\",\"password\": \"e10adc3949ba59abbe56e057f20f883e\"}";
                //  string a = param;
                byte[] btBodys = Encoding.UTF8.GetBytes(param);
                webReq.ContentLength = btBodys.Length;
                //webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(btBodys, 0, btBodys.Length);//写入参数
                newStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader sReader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
                        {
                            ret = sReader.ReadToEnd();
                        }
                    }
                    
                    JObject rt = (JObject)JsonConvert.DeserializeObject(ret);
                    ret = rt["data"]["token"].ToString();
                }

            }
            catch (Exception ex)
            {
                ret = ex.Message;
                if (ret.Equals("无法连接到远程服务器"))
                {
                    ipindex++;
                    if (ipindex > 2)
                    {
                        ipindex = 0;
                    }
                    return HttpPostTk(geturl(), param);
                }
            }

            return ret;
        }

        public static string HttpPost(string postUrl, string param)
        {
            string ret = "";
            try
            {
                string tk = gettoken();
                //byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(gethost() + postUrl));
                webReq.Method = "POST";
                webReq.Headers.Add("ContentType", "application/json");

                webReq.Headers.Add("Authorization",tk );
                
                webReq.ContentType = "application/json";
                // byte[] btBodys = Encoding.UTF8.GetBytes(param.ToString());
                //string a = "{\"account\": \"ics_data\",\"password\": \"e10adc3949ba59abbe56e057f20f883e\"}";
                //  string a = param;
                byte[] btBodys = Encoding.UTF8.GetBytes(param);
                webReq.ContentLength = btBodys.Length;
                //webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(btBodys, 0, btBodys.Length);//写入参数
                newStream.Close();
                using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader sReader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
                        {
                            ret = sReader.ReadToEnd();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ret = ex.Message;
                if (ret.Equals("无法连接到远程服务器"))
                {
                    ipindex++;
                    if (ipindex > 2)
                    {
                        ipindex = 0;
                    }
                    return HttpPost(geturl(), param);
                }
            }

          //  AddLgoToTXT(DateTime.Now.ToString() + ": "+ host.ToString());
            return ret;
        }
        //public static string HttpPost(string postUrl, string param,int k)
        //{
        //    string ret = "";
        //    try
        //    {

        //        //byte[] byteArray = Encoding.UTF8.GetBytes(paramData); //转化
        //        HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
        //        webReq.Method = "POST";
        //        webReq.Headers.Add("ContentType", "application/json");
        //        if (k==1)
        //        {
        //            if (string.IsNullOrEmpty(token))
        //            {
        //               token = gettoken();
        //            }
        //            webReq.Headers.Add("Authorization", token); 
        //        }              


        //         webReq.ContentType = "application/json";
        //         // byte[] btBodys = Encoding.UTF8.GetBytes(param.ToString());
        //         //string a = "{\"account\": \"ics_data\",\"password\": \"e10adc3949ba59abbe56e057f20f883e\"}";
        //        //  string a = param;
        //         byte[] btBodys = Encoding.UTF8.GetBytes(param);
        //        webReq.ContentLength = btBodys.Length;
        //        //webReq.ContentLength = byteArray.Length;
        //        Stream newStream = webReq.GetRequestStream();
        //        newStream.Write(btBodys, 0, btBodys.Length);//写入参数
        //        newStream.Close();
        //        using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
        //        {
        //            using (Stream responseStream = response.GetResponseStream())
        //            {
        //                using (StreamReader sReader = new StreamReader(responseStream, System.Text.Encoding.UTF8))
        //                {
        //                    ret = sReader.ReadToEnd();
        //                }
        //            }
        //            if (k == 0)
        //            {
        //                JObject rt = (JObject)JsonConvert.DeserializeObject(ret);

        //                ret = rt["data"]["token"].ToString();

        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ret = ex.Message;
        //        if (ret.Equals("无法连接到远程服务器"))
        //        {
        //            ipindex++;
        //            if (ipindex>2)
        //            {
        //                ipindex = 0;
        //            }
        //            return HttpPost(geturl(), param, k);
        //        }
        //    }

        //    return ret;
        //}


    }
}
