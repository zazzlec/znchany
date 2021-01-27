using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SynZnrs;
using znrsserver;

namespace jjpointadd32
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            timer2.Start();
         //   AddLgoToTXT(DateTime.Now.ToString()+":程序启动");
        }
        #region 请求封装

        //public static int bid = 5;//5号炉
        //public static string nspace = "unit05";//5号炉
        public static int bid = 6;//6号炉
        public static string nspace = "unit06";//6号炉

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

        public static void AddLgoToTXT2(string logstring)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "logs/test_log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
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
        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name=”timeStamp”></param>        
        /// <returns></returns>        
        public static DateTime ConvertLongToDateTime(long timeStamp)
        {
            DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 8, 0, 0, 0), TimeZoneInfo.Local);
            long lTime = timeStamp * 10000;
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        private JToken JinJieHttp(DataTable dt_pk, string ptype, ref Dictionary<string, string> dicindexname)
        {
            string para = "{\"tags\":[";
            foreach (DataRow item in dt_pk.Rows)
            {
                para += "{\"items\":[\"" + ptype + "\"],\"namespace\": \"" + nspace + "\",\"tag\":\"" + item[0].ToString() + "\"},";
                dicindexname.Add(item[0].ToString(), item[2].ToString());
            }
            para = para.Substring(0, para.Length - 1) + "]}";

            string url_realdata = "/macs/v1/realtime/read/findPoints";//锦界
            //string url_write = url + "/macs/v1/realtime/write/writePoints";//锦界
            String r = HttpHelpercs.HttpPost(url_realdata, para);
            JObject rt = (JObject)JsonConvert.DeserializeObject(r);
            JToken ja = rt["data"];
            return ja;
        }

        private JToken JinJieHttp(string para)
        {
            string url_realdata = "/macs/v1/realtime/read/findPoints";//锦界
            //string url_write = url + "/macs/v1/realtime/write/writePoints";//锦界
            String r = HttpHelpercs.HttpPost(url_realdata, para);
            JObject rt = (JObject)JsonConvert.DeserializeObject(r);

            JToken ja = rt["data"];
            return ja;
        }

        private List<JToken> JinJieHttpMore(DataTable dt_pk, string ptype)
        {
            //string sql_pointkkscode = "SELECT kkscode,Name_kw,bhcindex,badkks,backkks,frontkks,stopkks from dncchqpoint WHERE DncBoilerId=" + bid + " and position=1";
            List<JToken> arr = new List<JToken>();
            for (int i = 3; i < 7; i++)
            {
                string para = "{\"tags\":[";
                foreach (DataRow item in dt_pk.Rows)
                {
                    para += "{\"items\":[\"" + ptype + "\"],\"namespace\": \"" + nspace + "\",\"tag\":\"" + item[i].ToString() + "\"},";
                }
                para = para.Substring(0, para.Length - 1) + "]}";

                string url_realdata = "/macs/v1/realtime/read/findPoints";//锦界
                                                                          //string url_write = url + "/macs/v1/realtime/write/writePoints";//锦界
                String r = HttpHelpercs.HttpPost(url_realdata, para);
                JObject rt = (JObject)JsonConvert.DeserializeObject(r);
                JToken ja = rt["data"];
                arr.Add(ja);
            }
            return arr;
        }

        private JToken JinJieHttp(DataTable dt_pk, string ptype)
        {
            string para = "{\"tags\":[";
            foreach (DataRow item in dt_pk.Rows)
            {
                para += "{\"items\":[\"" + ptype + "\"],\"namespace\": \"" + nspace + "\",\"tag\":\"" + item[0].ToString() + "\"},";
            }
            para = para.Substring(0, para.Length - 1) + "]}";
          //  AddLgoToTXT("para:" + para);
            string url_realdata = "/macs/v1/realtime/read/findPoints";//锦界
            //string url_write = url + "/macs/v1/realtime/write/writePoints";//锦界
            String r = HttpHelpercs.HttpPost(url_realdata, para);
          //  AddLgoToTXT("r:" + r);
            JObject rt = (JObject)JsonConvert.DeserializeObject(r);
           // AddLgoToTXT("rt:" + rt);
            JToken ja = rt["data"];
         //   AddLgoToTXT("ja:" + ja);
            return ja;
        }


        private JToken JinJieHttp(DataTable dt_pk)
        {
            string para = "{\"tags\":[";
            foreach (DataRow item in dt_pk.Rows)
            {
                para += "{\"items\":[\"" + item[1].ToString() + "\"],\"namespace\": \"" + nspace + "\",\"tag\":\"" + item[0].ToString() + "\"},";
            }
            para = para.Substring(0, para.Length - 1) + "]}";
            //  AddLgoToTXT("para:" + para);
            string url_realdata = "/macs/v1/realtime/read/findPoints";//锦界
            //string url_write = url + "/macs/v1/realtime/write/writePoints";//锦界
            String r = HttpHelpercs.HttpPost(url_realdata, para);
            //  AddLgoToTXT("r:" + r);
            JObject rt = (JObject)JsonConvert.DeserializeObject(r);
            // AddLgoToTXT("rt:" + rt);
            JToken ja = rt["data"];
            //   AddLgoToTXT("ja:" + ja);
            return ja;
        }

        private JToken JinJieHttp(List<string> dt_pk, string ptype)
        {
            string para2 = "{\"tags\":[";
            foreach (string item in dt_pk)
            {
                para2 += "{\"items\":[\"" + ptype + "\"],\"namespace\": \"" + nspace + "\",\"tag\":\"" + item + "\"},";
            }
            para2 = para2.Substring(0, para2.Length - 1) + "]}";
            //  MessageBox.Show(para2);
            string url_realdata = "/macs/v1/realtime/read/findPoints";//锦界
            String r2 = HttpHelpercs.HttpPost(url_realdata, para2);
            //  MessageBox.Show(r2);
            JObject rt2 = (JObject)JsonConvert.DeserializeObject(r2);

            JToken ja2 = rt2["data"];
            return ja2;
        }

        private JToken JinJieHttp(string dt_pk, string ptype)
        {
            string para2 = "{\"tags\":[{\"items\":[\"" + ptype + "\"],\"namespace\": \"" + nspace + "\",\"tag\":\"" + dt_pk + "\"}]}";
            string url_realdata = "/macs/v1/realtime/read/findPoints";//锦界
            String r2 = HttpHelpercs.HttpPost(url_realdata, para2);
            JObject rt2 = (JObject)JsonConvert.DeserializeObject(r2);
            JToken ja2 = rt2["data"];
            return ja2;
        }

        private bool JinJieHttpDo(string dt_pk, string pmode)
        {
            string para2 = "{\"tags\":[{\"items\": [{\"item\": \"" + pmode + "\",\"value\": 1}],\"namespace\": \"" + nspace + "\",\"tag\":\"" + dt_pk + "\"}]";
            string url_realdata = "/macs/v1/realtime/write/writePoints";//锦界
            String r2 = HttpHelpercs.HttpPost(url_realdata, para2);
            JObject rt2 = (JObject)JsonConvert.DeserializeObject(r2);
            JToken ja2 = rt2["status"];
            return ja2.ToString().Equals("0");
        }


        private bool JinJieHttpDo(string dt_pk, string pmode, string value)
        {
            string para2 = "{\"tags\":[{\"items\": [{\"item\": \"" + pmode + "\",\"value\": " + value + "}],\"namespace\": \"" + nspace + "\",\"tag\":\"" + dt_pk + "\"}]";
            string url_realdata = "/macs/v1/realtime/write/writePoints";//锦界
            String r2 = HttpHelpercs.HttpPost(url_realdata, para2);
            JObject rt2 = (JObject)JsonConvert.DeserializeObject(r2);
            JToken ja2 = rt2["status"];
            return ja2.ToString().Equals("0");
        }

        #endregion

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }

        private void timer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            AddLgoToTXT(DateTime.Now.ToString() + ":进入timer程序");
            try
            {
               // AddLgoToTXT("192行开始");
                DBHelper db = new DBHelper();
                string sql_point = "select Pkks,Pmode from dncchpointadd_js order by Id";
                DataTable dt_pk = db.GetCommand(sql_point);
               // AddLgoToTXT( "197行dt_pk:" + dt_pk);
               // if (dt_pk!=null)
                //{
                //    AddLgoToTXT("200行:" + dt_pk.Rows.Count);
                //}
                JToken ja = JinJieHttp(dt_pk);
              //  AddLgoToTXT( "ja:" + ja);
                double value = 0d;
                List<string> arr = new List<string>();
                long timestamp = 0;
                DateTime nowtime = DateTime.Now;
                for (int i = 0; i < ja.Count(); i++)
                {
                  //  AddLgoToTXT(DateTime.Now.ToString()+"进入循环"+i);
                    var item = ja[i];
                    var name = item["tag"].ToString();
                    int avindex = item["item"].ToString().IndexOf("AV");
                    int dvindex = item["item"].ToString().IndexOf("DV");
                    if (avindex != -1)
                    {

                        value = double.Parse(item["item"]["AV"].ToString()) / 10;
                        timestamp = long.Parse(item["item"]["timestamp"].ToString());//锦界环境
                        nowtime = ConvertLongToDateTime(timestamp);//锦界环境
                    }
                    else if (dvindex != -1)
                    {
                        value = ((int.Parse(item["item"]["DV"].ToString())) & 8) / 8;//取出来的数值是2和10，需要取二进制第4位的值
                        timestamp = long.Parse(item["item"]["timestamp"].ToString());//锦界环境
                        nowtime = ConvertLongToDateTime(timestamp);//锦界环境
                    }
                    else
                    {
                        value = 0;
                    }
                   // AddLgoToTXT(i+"行value:"+value );

                    string sql_up_pvalue = "update dncchpointadd_js set Pvalue=" + value + ",Realtime='" + nowtime + "' where Pkks='" + name + "';";
                    arr.Add(sql_up_pvalue);

                }
             //   AddLgoToTXT(DateTime.Now.ToString() + ":" + arr.ToString());
                bool a = db.ExecuteTransaction(arr);
               // AddLgoToTXT(DateTime.Now.ToString() + "223行:" + a);

                sql_point = "select Pvalue from dncchpointadd_js order by Id";
                dt_pk = db.GetCommand(sql_point);

                string sql_insert = "insert into dncchpointadddata (Realtime,P1,P2,P3,P4,P5,P6,P7,P8,P9,P10,P11,P12,P13,P14,P15,P16,P17,P18,P19,P20,P21,P22,P23,P24,P25,P26,P27,P28,P29,P30,P31,P32,Fh,AMFWF,AMTFFACT,HAH74CT601,HAH74CT603,HAH75CT601,HAH75CT603,HAH74CT602,HAH74CT604,HAH75CT602,HAH75CT604,LAE51CF102,LAE52CF102,LAE51CF101,LAE52CF101,LAE41CF102,LAE42CF102,LAE41CF101,LAE42CF101,HAH61CT602,HAH62CT602,HAH61CT603,HAH62CT603,HAH61CT601,HAH62CT601,LAB30CT608,HAH12CP101,HAH14CP101,HAH11CT601,HAH13CT601,HAH12CT601,HAH14CT601,LBA11CP101,LBA12CP101,LBA12CP102,LBA11CT601,LBA12CT601,LBA11CT602,LBA12CT602,LBA11CT603,LBA12CT603,LAF21CF102,LAF22CF102,LAF21CF101,LAF22CF101,LBC81CT601,LBC81CT603,LBC82CT601,LBC82CT603,LBC81CT602,LBC81CT604,LBC82CT602,LBC82CT604,LBC81CP101,LAF31CF101,LAF32CF101,LAF31CF102,LAF32CF102,HAJ11CT601,HAJ12CT601,HAJ11CT602,HAJ12CT602,LAF11CP101,LAB31CT601,LBB11CP101,LBB12CP101,HAJ11CT603,HAJ11CT604,HAJ12CT603,HAJ12CT604,LBB11CT601,LBB12CT601,LBB11CT602,LBB12CT602,LBB11CT603,LBB12CT603,HAC11CT601,HAC11CT602,HAC21CT601,HAC21CT602,HAC22CT601,AMLAB80CT601S,LAB80CP102,CDB62AI001,CDB61AI001,HBK05CT601,HBK05CT602,HBK06CT601,HBK06CT602,HBK05CT603,HBK05CT604,HBK06CT603,HBK06CT604,HBK07CT601,HBK07CT603,HBK07CT602,HBK07CT604,HSA01CT301,HSA01CQ101,HSA01CT302,HSA01CQ104,HNA01CQ101,HNA01CQ102,HNA01CQ103,HFE30CP101,HNA10CP101,HLA40CP102,HLACP102,HCB12AT001ZT,HCB12AT002ZT,HCB12AT003ZT,HCB12AT004ZT,HCB12AT005ZT,HCB12AT006ZT,HCB12AT007ZT,HCB12AT008ZT,HCB12AT009ZT,HCB12AT010ZT,HCB12AT011ZT,HCB12AT012ZT,HCB12AT013ZT,HCB12AT014ZT,HCB12AT015ZT,HCB12AT016ZT,HCB12AT017ZT,HCB12AT018ZT,HCB12AT019ZT,HCB12AT020ZT,HCB12AT021ZT,HCB12AT022ZT,HCB12AT023ZT,HCB12AT024ZT,HCB12AT025ZT,HCB12AT026ZT,HCB12AT027ZT,HCB12AT028ZT,HCB12AT029ZT,HCB12AT030ZT,HCB12AT031ZT,HCB13AT001ZT,HCB13AT002ZT,HCB13AT003ZT,HCB13AT004ZT,HCB13AT005ZT,HCB13AT006ZT,HCB13AT007ZT,HCB13AT008ZT,HCB13AT009ZT,HCB13AT010ZT,HCB13AT011ZT,HCB13AT012ZT,HCB13AT013ZT,HCB13AT014ZT,HCB13AT015ZT,HCB13AT016ZT,HCB13AT017ZT,HCB13AT018ZT,HCB13AT019ZT,HCB13AT020ZT,HCB13AT021ZT,HCB13AT022ZT,HCB13AT023ZT,HCB13AT024ZT,HCB13AT025ZT,HCB13AT026ZT,HCB13AT027ZT,HCB13AT028ZT,HCB13AT029ZT,HCB13AT030ZT,HCB13AT031ZT) values ('" + nowtime + "',";
                foreach (DataRow item in dt_pk.Rows)
                {
                    value = double.Parse(item[0].ToString());
                    sql_insert += value + ",";
                }
                sql_insert = sql_insert.Substring(0, sql_insert.Length - 1) + ")";
                //AddLgoToTXT(DateTime.Now.ToString() + ":" + sql_insert);
                int b = db.CommandExecuteNonQuery(sql_insert);
               // AddLgoToTXT(DateTime.Now.ToString() + "237行:" + b.ToString());
            }
            catch (Exception e1)
            {
                AddLgoToTXT(DateTime.Now.ToString() + "254行:" + e1.Message);
               // throw;
            }
            



        }
    }
}
