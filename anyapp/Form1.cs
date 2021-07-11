using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZNRS.Api.dataoperate;
using znrsserver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SynZnrs;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace anyapp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        List<int> irarr = new List<int>();
        List<int> yesarr = new List<int>();
        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 1; i <= 96; i++)
            {
                irarr.Add(i);
            }
        }

        #region 基础方法

        /// <summary>
        /// 1、登录
        /// 2、接口地址通了  3，通了但是数据读取出错
        /// 101、接口不通
        /// 102、数据读取返回错误（999）
        /// 103、数据异常
        /// 104、单吹指令发送不成功
        /// 105、单吹指令未执行
        /// 106、疏水指令发送不成功
        /// 107、疏水指令未执行
        /// 108、空预器汽源选择指令发送不成功
        /// 109、空预器汽源选择指令未执行
        /// 110、空预器程控吹灰指令发送不成功
        /// 111、空预器程控吹灰未执行
        /// 112、数据表更新错误
        /// 113、数据库插入错误
        /// 114  吹灰器状态异常
        /// </summary>
        /// <param name="db"></param>
        /// <param name="contant"></param>
        /// <param name="type"></param>
        public static void AddLog(DBHelper db, string contant, int type)
        {
            db.CommandExecuteNonQuery("insert into dnclog(K_Log_kw,LogTime,Type,WorkNum,Status,IsDeleted) values('" + contant + "',NOW()," + type + ",'miniapp',1,0);");
        }

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

        class point
        {
            public string name { get; set; }
            public double pvalue { get; set; }
            public double typeid { get; set; }
            public string typename { get; set; }
        }


        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public string GetTimeStamp()
        {
            //TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            //return Convert.ToInt64(ts.TotalSeconds).ToString();

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            //Ticks表示自 0001 年 1 月 1 日午夜 12:00:00 以来已经过的时间的以 100 毫微秒为间隔的间隔数
            //除10000000调整为10位  1 毫秒 = 10^-3 秒，1 微秒 = 10 ^ -6 秒，1 毫微秒 = 10 ^ -9 秒，100 毫微秒 = 10 ^ -7 秒
            long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000000;
            return t + "";
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
        //public static int bid = 6;//6号炉
        //public static string nspace = "unit06";//6号炉
        public static int bid = 5;//5号炉
        public static string nspace = "unit05";//5号炉
        public static DateTime realtime = DateTime.MinValue;


        public double yl_fh_out;
        public int all_boilerid;
        public string boiler_name;
        public DateTime all_syntime;

        private double dg_zwxs_design = 0;
        private double pg_zwxs_design = 0;
        private double mg_zwxs_design = 0;
        private double dz_zwxs_design = 0;
        private double gz_zwxs_design = 0;
        private double fs_zwxs_design = 0;
        private double zs_zwxs_design = 0;

        private Dictionary<int, string> dic_type = new Dictionary<int, string>();
        private DBHelper Dbobj = null;
        private DBHelper DB()
        {
            if (Dbobj == null)
            {
                Dbobj = new DBHelper();
            }
            return Dbobj;
        }

        public string UnicodeToString(string value)
        {
            return new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled).Replace(
                  value, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }
        #endregion

        #region 请求封装
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

            string url_realdata = "/macs/v1/realtime/read/findPoints";//锦界
            //string url_write = url + "/macs/v1/realtime/write/writePoints";//锦界
            String r = HttpHelpercs.HttpPost(url_realdata, para);
            JObject rt = (JObject)JsonConvert.DeserializeObject(r);

            JToken ja = rt["data"];
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
           // string para2 = "[{\"tags\":[{\"items\": [{\"item\": \"" + pmode + "\",\"value\": 1}],\"namespace\": \"" + nspace + "\",\"tag\":\"" + dt_pk + "\"}]";

            string para2 = "[{\"items\": [{\"item\": \"" + pmode + "\",\"value\": 1}],\"namespace\": \"" + nspace + "\",\"tag\":\"" + dt_pk + "\"}]";
            string url_realdata = "/macs/v1/realtime/write/writePoints";//锦界
            String r2 = HttpHelpercs.HttpPost(url_realdata, para2);
            JObject rt2 = (JObject)JsonConvert.DeserializeObject(r2);
            JToken ja2 = rt2["status"];
            return ja2.ToString().Equals("0");
        }


        private bool JinJieHttpDo(string dt_pk, string pmode, string value)
        {
            //  string para2 = "[{\"tags\":[{\"items\": [{\"item\": \"" + pmode + "\",\"value\": " + value + "}],\"namespace\": \"" + nspace + "\",\"tag\":\"" + dt_pk + "\"}]";

            string para2 = "[{\"items\": [{\"item\": \"" + pmode + "\",\"value\": " + value + "}],\"namespace\": \"" + nspace + "\",\"tag\":\"" + dt_pk + "\"}]";
            string url_realdata = "/macs/v1/realtime/write/writePoints";//锦界
            String r2 = HttpHelpercs.HttpPost(url_realdata, para2);
            JObject rt2 = (JObject)JsonConvert.DeserializeObject(r2);
            JToken ja2 = rt2["status"];
            return ja2.ToString().Equals("0");
        }

        #endregion

        #region 常用函数
        /// <summary>
        /// 受热面焓增计算公式
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="press"></param>
        /// <returns></returns>
        public static double Steamhz(double temp, double press)
        {
            temp = temp + 273.15;//℃转成K

            return 2022.7 + 1.6675 * temp + 0.00029593 * Math.Pow(temp, 2) - 1269000000 * press / Math.Pow(temp, 2.7984) - 1.0185 * Math.Pow(10, 23) * Math.Pow(press, 2) / Math.Pow(temp, 8.3207);
        }

        /// <summary>
        /// 空气比热公式
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public static double Airc(double temp)
        {
            double airc = -2 * Math.Pow(10, -15) * Math.Pow(temp, 5) + 2 * Math.Pow(10, -12) * Math.Pow(temp, 4) - 9 * Math.Pow(10, -10) * Math.Pow(temp, 3) + 3 * Math.Pow(10, -7) * Math.Pow(temp, 2) + 8 * Math.Pow(10, -6) * temp + 1.005;
            return airc;
        }

        /// <summary>
        /// 烟气比热公式
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public static double Gasc(double temp)
        {
            double gasc = 3 * Math.Pow(10, -17) * Math.Pow(temp, 5) - 9 * Math.Pow(10, -14) * Math.Pow(temp, 4) + 3 * Math.Pow(10, -11) * Math.Pow(temp, 3) + 4 * Math.Pow(10, -8) * Math.Pow(temp, 2) + Math.Pow(10, -4) * temp + 0.9837;
            return gasc;
        }

        /// <summary>
        /// 减温水焓增计算公式
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="press"></param>
        /// <returns></returns>

        public static double Whz(double temp, double press)
        {

            // return 130.06 + 0.947711 * t ^ 1.2521 + p * (0.7234 - 9.2384 * (10 ^ -10) * t ^ 3.6606);
            // var whz = 130.06 + 0.947711 * Math.Pow(temp, 1.2521) + press * (0.7234 - 9.2384 * (10e-10) * Math.Pow(temp, 3.6606));
            var whz = 130.06 + 0.947711 * Math.Pow(temp, 1.2521) + press * (0.7234 - 9.2384 * Math.Pow(10, -10) * Math.Pow(temp, 3.6606));
            return whz;
        }

        /// <summary>
        /// 计算水冷壁一层非周期性吹灰逻辑
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Chpoint> Chlist(List<Chpoint> list)
        {
            var q = from u in list.ToArray() select u;
            int num = list.Count;
            double dif_sum = 0d;
            double dx_sum = 0d;
            double dx = 0d;
            List<Chpoint> rtlist = new List<Chpoint>();
            for (int i = 0; i < num; i++)
            {
                dif_sum += list[i].Now_temp_dif_Val;
            }
            double avg = Math.Round(dif_sum * 1.0 / num, 4);

            for (int j = 0; j < num; j++)
            {
                double minus = list[j].Now_temp_dif_Val - avg;
                dx_sum += Math.Pow(minus, 2);
            }
            dx = dx_sum / num;

            if (dx > 60)
            {
                rtlist = q.OrderByDescending(x => x.Last_now_dif).Take(4).ToList();

            }
            else if (dx > 40 && dx <= 60)
            {
                rtlist = q.OrderByDescending(x => x.Last_now_dif).Take(6).ToList();
            }
            else if (dx >= 20 && dx <= 40)
            {
                rtlist = q.OrderByDescending(x => x.Last_now_dif).Take(8).ToList();
            }
            else
            {
                rtlist = list;
            }

            return rtlist;

        }
        #endregion

        #region 获取吹灰模式  15秒
        private string chmode = "0";
        private string GetChMode()
        {

            JToken ja2 = JinJieHttp("DMSBWZNSEL", "DV");
            string mode = ja2[0]["item"]["DV"].ToString();
            AddLgoToTXT(DateTime.Now.ToString() + ":吹灰模式：" + mode);
            return mode;
        }
        #endregion

        #region 取负荷数据 1分钟1次
        private string fhtag = "AMGENMW";
        private void GetFH(DBHelper db)
        {

            JToken jt = JinJieHttp(fhtag, "AV");
            double value2 = 0d;
            var item = jt[0];
            var name = item["tag"].ToString();
            DateTime nowtime = DateTime.Now;
            if (item["item"]["AV"] != null)
            {
                value2 = double.Parse(item["item"]["AV"].ToString());
                if (value2 < 0)
                {
                    value2 = 0;
                }
                var timestamp = long.Parse(item["item"]["timestamp"].ToString());//锦界环境
                nowtime = ConvertLongToDateTime(timestamp);//锦界环境

            }
            else
            {
                value2 = 999;
            }
            yl_fh_out = value2;
            string rsqyqpwd = "insert into dncfhdata(RealTime, Fh_Val, Status, IsDeleted, DncBoilerId, DncBoiler_Name) values('" + nowtime + "', " + value2 + ", 1, 0," + bid + ", '" + bid + "号机组')";
            string rsqyqpwd2 = "update DncBoiler set Fh_Chlist="+ value2 + " where id="+bid+";";

            db.ExecuteTransaction(new List<string> { rsqyqpwd , rsqyqpwd2 });


        }
        #endregion

        #region 吹灰器状态更新  15秒
        private void RefreshState(DBHelper db)
        {
            AddLgoToTXT(DateTime.Now.ToString() + ":吹灰器状态更新");
            //string sql = "select kkscode from dncchqkks where DncBoilerId=" + bid;
            string sql = "select kkscode from dncchqkks";
            DataTable dt_pk = db.GetCommand(sql);
            JToken ja = JinJieHttp(dt_pk, "DV");
            int value = 0;
            List<string> arr = new List<string>();
            long timestamp = 0;
            DateTime nowtime = DateTime.Now;
            for (int i = 0; i < ja.Count(); i++)
            {
                try
                {
                    var item = ja[i];
                    var name = item["tag"].ToString();
                    if (item["item"]["DV"] != null)
                    {
                        // value = int.Parse(item["item"]["DV"].ToString());
                        value = ((int.Parse(item["item"]["DV"].ToString())) & 8) / 8;//取出来的数值是2和10，需要取二进制第4位的值
                        timestamp = long.Parse(item["item"]["timestamp"].ToString());//锦界环境
                        nowtime = ConvertLongToDateTime(timestamp);//锦界环境
                    }
                    else
                    {
                        AddLog(db, "tag:" + name + " 吹灰器状态异常", 114);
                        AddLog(db, "tag:" + name + " 数据读取返回错误", 102);
                        value = 999;
                    }


                    string sql_up_pvalue = "update dncchqkks set Pvalue=" + value + ",RealTime='" + nowtime + "' where kkscode='" + name + "' and DncBoilerId=" + bid;
                    arr.Add(sql_up_pvalue);
                }
                catch (Exception  ee)
                {

                    AddLog(db, "通了但是数据读取出错:"+ee.Message, 103);
                }
                

            }
            db.ExecuteTransaction(arr);

            string sqlupdown = "update dncchqpoint t1 inner join  dncchqkks t2 on t1.Name_kw=t2.chq_name   set t1.DncChstatusId=t2.chstatus,t1.DncChstatus_Name=t2.chstatus_des,t1.updown=t2.updown where t2.Pvalue=1 and t1.DncBoilerId=" + bid + " and t2.DncBoilerId=" + bid;
            db.CommandExecuteNonQuery(sqlupdown);

        }
        #endregion

        #region 96个吹灰点  5分钟
        //12个背火侧温度tag  abd   a左 a前 a右 a后  b左 b前 b右 b后  d左 d前 d右 d后
        public List<string> b_tags_12 = new List<string>
        {
            //5号炉
            "SH02ZNRTD14",
            "SH01ZNRTD14",
            "SH01ZNRTD07",
            "SH02ZNRTD07",
            "SH06ZNRTD19",
            "SH05ZNRTD19",
            "SH04ZNRTD19",
            "SH03ZNRTD19",
            "SH06ZNRTD20",
            "SH05ZNRTD20",
            "SH04ZNRTD20",
            "SH03ZNRTD20"

            ////6号炉
            //"SH02ZNRTD22",
            //"SH01ZNRTD11",
            //"SH01ZNRTD22",
            //"SH02ZNRTD09",
            //"SH06ZNRTD09",
            //"SH05ZNRTD11",
            //"SH04ZNRTD08",
            //"SH03ZNRTD13",
            //"SH06ZNRTD04",
            //"SH05ZNRTD03",
            //"SH04ZNRTD18",
            //"SH03ZNRTD18"

        };
        //16个背火侧温度值   a左 a前 a右 a后  b左 b前 b右 b后  d左 d前 d右 d后  c左 c前 c右 c后
        public double[] bhcwd = new double[16];


        // 5号炉
        //50SH01ZNRTD15	631	前墙第1层第1个鳍片温度
        //50SH01ZNRTD16	632	前墙第1层第2个鳍片温度
        //50SH01ZNRTD17	633	前墙第1层第3个鳍片温度
        //50SH01ZNRTD18	634	前墙第1层第4个鳍片温度
        //50SH01ZNRTD19	641	前墙第2层第1个鳍片温度
        //50SH01ZNRTD20	642	前墙第2层第2个鳍片温度
        //50SH01ZNRTD21	643	前墙第2层第3个鳍片温度
        //50SH01ZNRTD22	644	前墙第2层第4个鳍片温度
        //50SH02ZNRTD15	611	后墙第1层第1个鳍片温度
        //50SH02ZNRTD16	612	后墙第1层第2个鳍片温度
        //50SH02ZNRTD17	613	后墙第1层第3个鳍片温度
        //50SH02ZNRTD18	614	后墙第1层第4个鳍片温度
        //50SH02ZNRTD19	621	后墙第2层第1个鳍片温度
        //50SH02ZNRTD20	622	后墙第2层第2个鳍片温度
        //50SH02ZNRTD21	623	后墙第2层第3个鳍片温度
        // 50SH02ZNRTD22	624	后墙第2层第4个鳍片温度


        //燃烧区域鳍片温度
        //60SH01ZNRTD04	631	前墙第1层第1个鳍片温度
        //60SH01ZNRTD03	632	前墙第1层第2个鳍片温度
        //60SH01ZNRTD12	633	前墙第1层第3个鳍片温度
        //60SH01ZNRTD13	634	前墙第1层第4个鳍片温度
        //60SH01ZNRTD01	641	前墙第2层第1个鳍片温度
        //60SH01ZNRTD02	642	前墙第2层第2个鳍片温度
        //60SH01ZNRTD14	643	前墙第2层第3个鳍片温度
        //60SH01ZNRTD15	644	前墙第2层第4个鳍片温度
        //60SH02ZNRTD04	611	后墙第1层第1个鳍片温度
        //60SH02ZNRTD03	612	后墙第1层第2个鳍片温度
        //60SH02ZNRTD12	613	后墙第1层第3个鳍片温度
        //60SH02ZNRTD13	614	后墙第1层第4个鳍片温度
        //60SH02ZNRTD01	621	后墙第2层第1个鳍片温度
        //60SH02ZNRTD02	622	后墙第2层第2个鳍片温度
        //60SH02ZNRTD15	623	后墙第2层第3个鳍片温度
        //60SH02ZNRTD14	624	后墙第2层第4个鳍片温度
        public List<string> rsqy_tags_16 = new List<string>
        {

            //5号炉
            "SH01ZNRTD15",
            "SH01ZNRTD16",
            "SH01ZNRTD17",
            "SH01ZNRTD18",
            "SH01ZNRTD19",
            "SH01ZNRTD20",
            "SH01ZNRTD21",
            "SH01ZNRTD22",
            "SH02ZNRTD15",
            "SH02ZNRTD16",
            "SH02ZNRTD17",
            "SH02ZNRTD18",
            "SH02ZNRTD19",
            "SH02ZNRTD20",
            "SH02ZNRTD21",
            "SH02ZNRTD22"


            ////6号炉
            //"SH01ZNRTD04",
            //"SH01ZNRTD03",
            //"SH01ZNRTD12",
            //"SH01ZNRTD13",
            //"SH01ZNRTD01",
            //"SH01ZNRTD02",
            //"SH01ZNRTD14",
            //"SH01ZNRTD15",
            //"SH02ZNRTD04",
            //"SH02ZNRTD03",
            //"SH02ZNRTD12",
            //"SH02ZNRTD13",
            //"SH02ZNRTD01",
            //"SH02ZNRTD02",
            //"SH02ZNRTD15",
            //"SH02ZNRTD14"
        };
        public void CHPoint(DBHelper db)
        {
            List<string> arr = new List<string>();
            Dictionary<string, string> dicindexname = new Dictionary<string, string>();
            //获取96个吹灰点
            string sql_pointkkscode = "SELECT kkscode,Name_kw,bhcindex from dncchqpoint WHERE DncBoilerId=" + bid + " and position=1";
            DataTable dt_pk = db.GetCommand(sql_pointkkscode);

            JToken ja = JinJieHttp(dt_pk, "AV", ref dicindexname);

            JToken ja2 = JinJieHttp(b_tags_12, "AV");

            double value2 = 0d;
            for (int i = 0; i < ja2.Count(); i++)
            {
                var item = ja2[i];
                var name = item["tag"].ToString();
                if (item["item"]["AV"] != null)
                {
                    value2 = (double.Parse(item["item"]["AV"].ToString())) / 10;//新增测点数据需要除以10
                }
                else
                {
                    value2 = 999;
                }

                int xh = b_tags_12.FindIndex(x => x.Equals(name));
                bhcwd[xh] = value2;
            }
            //16个背火侧温度值   a左 a前 a右 a后  b左 b前 b右 b后  d左 d前 d右 d后  c左 c前 c右 c后
            bhcwd[12] = (bhcwd[4] + bhcwd[8]) / 2;
            bhcwd[13] = (bhcwd[5] + bhcwd[9]) / 2;
            bhcwd[14] = (bhcwd[6] + bhcwd[10]) / 2;
            bhcwd[15] = (bhcwd[7] + bhcwd[11]) / 2;

            double value = 0d;
            string sql_up_pvalue = "";
            for (int i = 0; i < ja.Count(); i++)
            {
                var item = ja[i];
                var name = item["tag"].ToString();
                if (item["item"]["AV"] != null)
                {
                    value = (double.Parse(item["item"]["AV"].ToString())) / 10;//新增测点数据需要除以10
                }
                else
                {
                    value = 999;
                }
                //string kwname = dickks[name];
                string indexval = dicindexname[name];
                double bhcwdval = bhcwd[int.Parse(indexval)];
                sql_up_pvalue = "update dncchqpoint set now_temp_qp_Val=" + value + ",RealTime='" + realtime + "',now_temp_bh_Val=" + bhcwdval + ",now_temp_dif_Val=" + (value - bhcwdval) + "  where kkscode='" + name + "' and DncBoilerId=" + bid;
                arr.Add(sql_up_pvalue);
            }



            double[] ret = new double[16];
            JToken ja3 = JinJieHttp(rsqy_tags_16, "AV");
            double value3 = 0d;
            for (int i = 0; i < ja3.Count(); i++)
            {
                var item = ja3[i];
                var name = item["tag"].ToString();
                if (item["item"]["AV"] != null)
                {
                    value3 = (double.Parse(item["item"]["AV"].ToString())) / 10;//新增测点数据需要除以10
                }
                else
                {
                    value3 = 999;
                }
                int xh = rsqy_tags_16.FindIndex(x => x.Equals(name));
                ret[xh] = value3;
            }
            //rsqy_tags_16

            string rsqyqpwd = "insert into dncchhzpoint(DncTypeId, DncType_Name, RealTime, Pvalue, Status, IsDeleted, DncBoilerId, DncBoiler_Name) values(85, '燃烧区域鳍片温度',  '" + realtime + "','[" + string.Join(",", ret) + "]', 1, 0," + bid + ", '" + bid + "号机组')";
            arr.Add(rsqyqpwd);

            string sql = "update dncchhzpointnow set RealTime='" + realtime + "',Pvalue='[" + string.Join(",", ret) + "]' where DncBoilerId=" + bid + " and DncTypeId=85;";
            arr.Add(sql);


            db.ExecuteTransaction(arr);

            // arr.Clear();
            string sql_data = "insert  dncchqpointdata (Name_kw,DncBoilerId,DncBoiler_Name,DncChstatusId,DncChstatus_Name,Lastchtime,position,last_temp_dif_Val,now_temp_dif_Val,slb_floor_Val,slb_circle_num,now_temp_qp_Val,now_temp_bh_Val,realtime) select Name_kw,DncBoilerId,DncBoiler_Name,DncChstatusId,DncChstatus_Name,Lastchtime,position,last_temp_dif_Val,now_temp_dif_Val,slb_floor_Val,slb_circle_num,now_temp_qp_Val,now_temp_bh_Val,realtime from dncchqpoint where DncChtypeId=1 and DncBoilerId=" + bid;
            int pt = db.CommandExecuteNonQuery(sql_data);
            AddLgoToTXT(DateTime.Now.ToString() + ":" + pt + "条插入dncchqpointdata成功");
        }
        #endregion

        #region 污染率所需测点读取  5
        public void WRLPoint(DBHelper db)
        {
            List<string> arr = new List<string>();
            //获取锅炉信息
            //string sql_pointkks = "select DISTINCT Name_kw from dncchpointkks_6 where Status=1 and  IsDeleted=0 and DncBoilerId=" + bid;//6号炉
           string sql_pointkks = "select DISTINCT Name_kw from dncchpointkks_5 where Status=1 and  IsDeleted=0 and DncBoilerId=" + bid;//5号炉

            DataTable dt_pk = db.GetCommand(sql_pointkks);

            JToken ja = JinJieHttp(dt_pk, "AV");
            double value = 0d;
            DateTime d1 = DateTime.MinValue;

            string sql_up_pvalue = "";
            for (int i = 0; i < ja.Count(); i++)
            {
                var item = ja[i];
                var name = item["tag"].ToString();
                if (item["item"]["AV"] != null)
                {
                    value = double.Parse(double.Parse(item["item"]["AV"].ToString()).ToString("0.00"));//保留2位小数
                }
                else
                {
                    value = 999;
                }

                //  var timestamp = long.Parse(item["timestamp"].ToString());//和利时环境
                // DateTime up_date = ConvertLongToDateTime(timestamp);//和利时环境
                // DateTime up_date = DateTime.Parse(item["dateTime"].ToString());//锦界现场环境
                var timestamp = long.Parse(item["item"]["timestamp"].ToString());//锦界环境
                DateTime up_date = ConvertLongToDateTime(timestamp);//锦界环境
                d1 = up_date;
                //sql_up_pvalue = "update dncchpointkks_6 set Pvalue=" + value + ",RealTime='" + up_date + "' where Name_kw='" + name + "' and DncBoilerId=" + bid + ";";//6号炉
                sql_up_pvalue = "update dncchpointkks_5 set Pvalue=" + value + ",RealTime='" + up_date + "' where Name_kw='" + name + "' and DncBoilerId=" + bid + ";";//5号炉
                arr.Add(sql_up_pvalue);
            }
            //*/
            db.ExecuteTransaction(arr);
            realtime = d1;
            arr.Clear();
            //string sql_point_all = "select  Name_kw,DncTypeId,DncType_Name,Pvalue from dncchpointkks_6 where Status=1 and IsDeleted=0 and DncBoilerId=" + bid;//6号炉
            string sql_point_all = "select  Name_kw,DncTypeId,DncType_Name,Pvalue from dncchpointkks_5 where Status=1 and IsDeleted=0 and DncBoilerId=" + bid;//5号炉
            DataTable dt_pvalue = db.GetCommand(sql_point_all);
            List<point> vl = new List<point>();
            foreach (DataRow item in dt_pvalue.Rows)
            {
                point p = new point();
                p.name = item[0].ToString();
                p.typeid = int.Parse(item[1].ToString());
                p.typename = item[2].ToString();
                p.pvalue = double.Parse(item[3].ToString());
                vl.Add(p);
            }
            // List<int> typelist = new List<int>();
            int typeid = 0;
            string typename = "";
            string sql_pgroup = "select Id,K_Name_kw from dncchhztype where Status=1 and IsDeleted=0 and Id<=76";
            DataTable dt_pgroup = db.GetCommand(sql_pgroup);
            DateTime dtnow = DateTime.Now;
            foreach (DataRow item in dt_pgroup.Rows)
            {
                //typelist.Add(int.Parse(item[0].ToString()));
                typeid = int.Parse(item[0].ToString());
                typename = item[1].ToString();
                JArray jar = JArray.Parse("[]");
                var f = vl.FindAll(x => x.typeid == typeid);
                foreach (var i in f)
                {
                    jar.Add(i.pvalue);
                }
                //jar.ToString()
                System.Text.RegularExpressions.MatchEvaluator matchEvalu = delegate (System.Text.RegularExpressions.Match m)
                {
                    return "";
                };
                string jarstr = System.Text.RegularExpressions.Regex.Replace(jar.ToString(), @"\r\n", matchEvalu);
                jarstr = System.Text.RegularExpressions.Regex.Replace(jarstr.ToString(), @" ", matchEvalu);
                arr.Add("insert into dncchhzpoint(DncTypeId,DncType_Name,RealTime,Pvalue,Status,IsDeleted,DncBoilerId,DncBoiler_Name) values(" + typeid + ",'" + typename + "','" + realtime + "','" + jarstr + "',1,0," + bid + ",'" + bid.ToString() + "号机组');");


                string sql = "update dncchhzpointnow set RealTime='" + realtime + "',Pvalue='" + jarstr + "' where DncBoilerId=" + bid + " and DncTypeId=" + typeid + ";";
                arr.Add(sql);

            }
            //foreach (var item in arr)
            //{
            //    AddLgoToTXT(item.ToString());
            //}

            db.ExecuteTransaction(arr);

        }
        #endregion

        

        #region 计算污染率加入待吹灰列表，空预器吹灰器加入执行列表  5
        private void JSWRL(DBHelper db)
        {
            //realtime = DateTime.Parse("2020-12-28 20:42:46");
            AddLgoToTXT(DateTime.Now.ToString() + ":开始计算污染率");

            #region 获取参数

            //获取智能吹灰参数配置表信息
            string sql_ch_para = "select dg_zwxs_design_Val,pg_zwxs_design_Val,mg_zwxs_design_Val,dz_zwxs_design_Val,gz_zwxs_design_Val,fs_zwxs_design_Val,zs_zwxs_design_Val,ctylxs_Val,xs_py_x_modify_Val,wind_temp_in_design_Val,py_temp_in_wind_temp_modify_xs_Val,kyq_yq_ratio_low_Val,mg_wrl_low_Val,gz_wrl_low_Val from dncch_parameter where Status=1 and IsDeleted=0 and DncBoilerId=" + bid;
            DataTable dt_ch_para = db.GetCommand(sql_ch_para);

            //if (dt_srm_para != null && dt_srm_para.Rows.Count > 0)
            //{
            //}
            dg_zwxs_design = double.Parse(dt_ch_para.Rows[0][0].ToString());//低过设计沾污系数
            pg_zwxs_design = double.Parse(dt_ch_para.Rows[0][1].ToString());//屏过设计沾污系数
            mg_zwxs_design = double.Parse(dt_ch_para.Rows[0][2].ToString());//末过设计沾污系数
            dz_zwxs_design = double.Parse(dt_ch_para.Rows[0][3].ToString());//低再设计沾污系数
            gz_zwxs_design = double.Parse(dt_ch_para.Rows[0][4].ToString());//高再设计沾污系数
            fs_zwxs_design = double.Parse(dt_ch_para.Rows[0][5].ToString());//分级省煤器设计沾污系数
            zs_zwxs_design = double.Parse(dt_ch_para.Rows[0][6].ToString());//主省煤器设计沾污系数
            double ctylxs = double.Parse(dt_ch_para.Rows[0][7].ToString());//给水泵中间抽头压力系数
            double xs_py_x_modify = double.Parse(dt_ch_para.Rows[0][8].ToString());//经X比修正后排烟温度变化值系数
            double wind_temp_in_design = double.Parse(dt_ch_para.Rows[0][9].ToString());//设计进口风温
            double py_temp_in_wind_temp_modify_xs = double.Parse(dt_ch_para.Rows[0][10].ToString());//经进口风温修正后排烟温度变化值系数
            double kyq_yq_ratio_low_Val = double.Parse(dt_ch_para.Rows[0][11].ToString());//空预器烟气侧效率下限
            double mg_wrl_low = double.Parse(dt_ch_para.Rows[0][12].ToString());//高过污染率下限
            double gz_wrl_low = double.Parse(dt_ch_para.Rows[0][13].ToString());//高再污染率上限



            //获取dncchhztype表数据
            //string sql_typet = "select Id,K_Name_kw from dncchhztype where  Status=1 and IsDeleted=0";
            //DataTable dt_type = db.GetCommand(sql_typet);
            //Dictionary<int, string> dic_type = new Dictionary<int, string>();   //数据存入词典，方便查找 
            //foreach (DataRow item in dt_type.Rows)
            //{
            //    dic_type.Add(int.Parse(item[0].ToString()), item[1].ToString());

            //}
            #endregion

            //获取测点数据
            string sql_point = "select DncTypeId,Pvalue from dncchhzpointnow where  DncBoilerId=" + bid;
            DataTable dt_point = db.GetCommand(sql_point);
            //  AddLgoToTXT(sql_point);
            Dictionary<string, string> dic = new Dictionary<string, string>();   //数据存入词典，方便查找           
            if (dt_point != null && dt_point.Rows.Count > 0)
            {
                foreach (DataRow item in dt_point.Rows)
                {
                    dic.Add(item[0].ToString(), item[1].ToString());

                }

                #region 污染率计算
                yl_fh_out = Compute.Avgdata(dic["1"]);//实际负荷
                double flq_press = Compute.Avgdata(dic["2"]);//分离器压力
                double mg_out_press_left = Compute.Avgdata(dic["3"]);//末过左侧出口压力
                double mg_out_press_right = Compute.Avgdata(dic["4"]);//末过右侧出口压力
                double dg_out_qw_left = Compute.Avgdata(dic["5"]);//低过左侧出口汽温测点
                double dg_out_qw_right = Compute.Avgdata(dic["6"]);//低过右侧出口汽温测点
                double dg_bw_left = Compute.Avgdata(dic["7"]);//低过左侧壁温测点
                double dg_bw_right = Compute.Avgdata(dic["8"]);//低过右侧壁温测点
                double dg_in_temp_left = Compute.Avgdata(dic["9"]);//低过左侧进口温度
                double dg_in_temp_right = Compute.Avgdata(dic["10"]);//低过右侧进口温度
                double pg_out_qw_left = Compute.Avgdata(dic["11"]);//屏过左侧出口汽温测点
                double pg_out_qw_right = Compute.Avgdata(dic["12"]);//屏过右侧出口汽温测点
                double pg_bw_left = Compute.Avgdata(dic["13"]);//屏过左侧壁温测点
                double pg_bw_right = Compute.Avgdata(dic["14"]);//屏过右侧壁温测点
                double pg_in_temp_left = Compute.Avgdata(dic["15"]);//屏过左侧进口温度
                double pg_in_temp_right = Compute.Avgdata(dic["16"]);//屏过右侧进口温度
                double mg_out_qw_left = Compute.Avgdata(dic["17"]);//末过左侧出口汽温测点
                double mg_out_qw_right = Compute.Avgdata(dic["18"]);//末过右侧出口汽温测点
                double mg_bw_left = Compute.Avgdata(dic["19"]);//末过左侧壁温测点
                double mg_bw_right = Compute.Avgdata(dic["20"]);//末过右侧壁温测点
                double mg_in_temp_left = Compute.Avgdata(dic["21"]);//末过左侧进口温度
                double mg_in_temp_right = Compute.Avgdata(dic["22"]);//末过右侧进口温度
                double dz_in_press_left = Compute.Avgdata(dic["23"]);//低再左侧入口压力值
                double dz_in_press_right = Compute.Avgdata(dic["24"]);//低再右侧入口压力值
                double gz_out_press_left = Compute.Avgdata(dic["25"]);//高再左侧出口压力值
                double gz_out_press_right = Compute.Avgdata(dic["26"]);//高再右侧出口压力值
                double dz_out_qw_left = Compute.Avgdata(dic["27"]);//低再左侧出口汽温测点
                double dz_out_qw_right = Compute.Avgdata(dic["28"]);//低再右侧出口汽温测点
                double dz_bw_left = Compute.Avgdata(dic["29"]);//低再左侧壁温测点
                double dz_bw_right = Compute.Avgdata(dic["30"]);//低再右侧壁温测点
                double dz_in_temp_left = Compute.Avgdata(dic["31"]);//低再左侧进口温度 
                double dz_in_temp_right = Compute.Avgdata(dic["32"]);//低再右侧进口温度 
                double gz_out_qw_left = Compute.Avgdata(dic["33"]);//高再左侧出口汽温测点
                double gz_out_qw_right = Compute.Avgdata(dic["34"]);//高再右侧出口汽温测点
                double gz_bw_left = Compute.Avgdata(dic["35"]);//高再左侧壁温测点
                double gz_bw_right = Compute.Avgdata(dic["36"]);//高再右侧壁温测点
                double gz_in_temp_left = Compute.Avgdata(dic["37"]);//高再左侧进口温度
                double gz_in_temp_right = Compute.Avgdata(dic["38"]);//高再右侧进口温度
                                                                     // double yq_in_press = Compute.Avgdata(dic["39"]);//烟气侧进口压力
                double fs_out_temp = Compute.Avgdata(dic["40"]);//分级省煤器出口温度
                double fs_in_press = Compute.Avgdata(dic["41"]);//分级省煤器进口压力
                double fs_in_temp = Compute.Avgdata(dic["42"]);//分级省煤器进口温度
                                                               // double yq_out_press = Compute.Avgdata(dic["43"]);//烟气侧出口压力
                double zs_out_temp = Compute.Avgdata(dic["44"]);//主省煤器出口温度


                double fs_out_press = fs_in_press - 0.04; //分级省煤器出口压力
                double zs_out_press = fs_in_press - 0.12;//主省煤器出口压力
                double zs_in_press = fs_out_press;//主省煤器进口压力
                double zs_in_temp = Compute.Avgdata(dic["46"]);//主省煤器进口温度
                double yrq_in_yw = Compute.Avgdata(dic["47"]);//预热器进口烟温
                                                              // double yrq_py_temp = Compute.Avgdata(dic["48"]);//排烟温度
                                                              //double yrq_in_wt1 = Compute.Avgdata(dic["49"]);//预热器进口一次风温
                                                              // double yrq_in_wt2 = Compute.Avgdata(dic["50"]);//预热器进口二次风温
                double yrq_yq_pd = Compute.Avgdata(dic["51"]);//实际烟气侧压降
                double gsll = Compute.Avgdata(dic["52"]);//给水流量
                double gspress = Compute.Avgdata(dic["53"]);//给水压力
                double gstemp = Compute.Avgdata(dic["54"]);//给水温度
                double sgjwq_front_temp_left = Compute.Avgdata(dic["55"]);//事故减温器前温度（左侧）
                double sgjwq_front_temp_right = Compute.Avgdata(dic["56"]);//事故减温器前温度（右侧）
                //double cql = Compute.Avgdata(dic["57"]);//总抽气量
                double cql = 0;//锦界先用0
                double kyq_in_fw_1 = Compute.Avgdata(dic["58"]);//空预器进口一次风温
                double kyq_out_fw_1 = Compute.Avgdata(dic["59"]);//空预器出口一次风温
                double kyq_in_fw_2 = Compute.Avgdata(dic["60"]);//空预器进口二次风温
                double kyq_out_fw_2 = Compute.Avgdata(dic["61"]);//空预器出口二次风温
                double kyq_wind_2 = Compute.Sumdata(dic["62"]) / 3; //空预器二次风量，6个测点求和除以3
                //double kyq_wind_2 = Compute.Avgdata(dic["62"]);//测试环境只有一个测点，直接用
                double kyq_in_yw = Compute.Avgdata(dic["63"]);//空预器进口烟温
                double kyq_py_temp = Compute.Avgdata(dic["64"]);//实测空预器排烟温度
                double kyq_in_o2 = Compute.Avgdata(dic["65"]);//空预器进口氧量
                double kyq_out_o2 = Compute.Avgdata(dic["66"]);//空预器出口氧量
                double kyq_wind_1 = Compute.Sumdata(dic["67"]) + Compute.Sumdata(dic["68"]) + Compute.Sumdata(dic["69"]) + Compute.Sumdata(dic["70"]) + Compute.Sumdata(dic["71"]) + Compute.Sumdata(dic["72"]);//一次风量=A磨进口一次风量+B磨进口一次风量+……+F磨进口一次风量
                double dg_dbkd_left = Compute.Avgdata(dic["73"]);//低过挡板开度（左侧）
                double dg_dbkd_right = Compute.Avgdata(dic["74"]);//低过挡板开度（右侧）
                double dz_dbkd_left = Compute.Avgdata(dic["75"]);//低再挡板开度（左侧）
                double dz_dbkd_right = Compute.Avgdata(dic["76"]);//低再挡板开度（右侧）


                double dg_xrl_design = (0.0336 * Math.Pow(yl_fh_out / 6.6, 2) - 2.2052 * (yl_fh_out / 6.6) + 107.85) / 2;//低过设计吸热量
                double pg_xrl_design = (-0.0106 * Math.Pow(yl_fh_out / 6.6, 2) + 6.4571 * (yl_fh_out / 6.6) + 39.429) / 2;//屏过设计吸热量
                double mg_xrl_design = (0.004 * Math.Pow(yl_fh_out / 6.6, 2) + 2.4461 * (yl_fh_out / 6.6) + 4.1171) / 2;//末过设计吸热量
                double dz_xrl_design = (0.0045 * Math.Pow(yl_fh_out / 6.6, 2) + 4.721 * (yl_fh_out / 6.6) + 18.6) / 2;//低再设计吸热量
                double gz_xrl_design = (0.0215 * Math.Pow(yl_fh_out / 6.6, 2) + 2.1836 * (yl_fh_out / 6.6) + 1.9571) / 2;//高再设计吸热量
                double fs_xrl_design = 0.009 * Math.Pow(yl_fh_out / 6.6, 2) - 1.01 * (yl_fh_out / 6.6) + 69.4;//分省设计吸热量
                double zs_xrl_design = 0.0411 * Math.Pow(yl_fh_out / 6.6, 2) - 2.7678 * (yl_fh_out / 6.6) + 171.51; //主省设计吸热量


                double dg_out_press_left = flq_press - (flq_press - mg_out_press_left) / 4 * 2;//低过左侧出口压力
                double dg_out_temp_left = dg_out_qw_left * 0.95 + dg_bw_left * 0.05;//低过左侧出口温度
                double dg_out_hz_left = Steamhz(dg_out_temp_left, dg_out_press_left);//低过左侧出口焓
                double dg_in_press_left = flq_press - (flq_press - mg_out_press_left) / 4 * 1;//低过左侧进口压力
                double dg_in_hz_left = Steamhz(dg_in_temp_left, dg_in_press_left);//低过左侧进口焓
                double dg_hz_left = dg_out_hz_left - dg_in_hz_left;//低过左侧焓增



                double dg_out_press_right = flq_press - (flq_press - mg_out_press_right) / 4 * 2;//低过右侧出口压力
                double dg_out_temp_right = dg_out_qw_right * 0.95 + dg_bw_right * 0.05;//低过右侧出口温度
                double dg_out_hz_right = Steamhz(dg_out_temp_right, dg_out_press_right);//低过右侧出口焓
                double dg_in_press_right = flq_press - (flq_press - mg_out_press_right) / 4 * 1;//低过右侧进口压力
                double dg_in_hz_right = Steamhz(dg_in_temp_right, dg_in_press_right);//低过右侧进口焓
                double dg_hz_right = dg_out_hz_right - dg_in_hz_right;//低过右侧焓增


                double pg_out_press_left = flq_press - (flq_press - mg_out_press_left) / 4 * 3;//屏过左侧出口压力
                double pg_out_temp_left = pg_out_qw_left * 0.95 + pg_bw_left * 0.05;//屏过左侧出口温度
                double pg_out_hz_left = Steamhz(pg_out_temp_left, pg_out_press_left);//屏过左侧出口焓
                double pg_in_press_left = flq_press - (flq_press - mg_out_press_left) / 4 * 2;//屏过左侧进口压力
                double pg_in_hz_left = Steamhz(pg_in_temp_left, pg_in_press_left);//屏过左侧进口焓
                double pg_hz_left = pg_out_hz_left - pg_in_hz_left;//屏过左侧焓增


                double pg_out_press_right = flq_press - (flq_press - mg_out_press_right) / 4 * 3;//屏过右侧出口压力
                double pg_out_temp_right = pg_out_qw_right * 0.95 + pg_bw_right * 0.05;//屏过右侧出口温度
                double pg_out_hz_right = Steamhz(pg_out_temp_right, pg_out_press_right);//屏过右侧出口焓
                double pg_in_press_right = flq_press - (flq_press - mg_out_press_right) / 4 * 2;//屏过右侧进口压力
                double pg_in_hz_right = Steamhz(pg_in_temp_right, pg_in_press_right);//屏过右侧进口焓
                double pg_hz_right = pg_out_hz_right - pg_in_hz_right;//屏过右侧焓增


                double mg_out_temp_left = mg_out_qw_left * 0.9 + mg_bw_left * 0.1;//末过左侧出口温度
                double mg_out_hz_left = Steamhz(mg_out_temp_left, mg_out_press_left);//末过左侧出口焓
                double mg_in_press_left = flq_press - (flq_press - mg_out_press_left) / 4 * 3;//末过左侧进口压力
                double mg_in_hz_left = Steamhz(mg_in_temp_left, mg_in_press_left);//末过左侧进口焓
                double mg_hz_left = mg_out_hz_left - mg_in_hz_left;//末过左侧焓增


                double mg_out_temp_right = mg_out_qw_right * 0.9 + mg_bw_right * 0.1;//末过右侧出口温度
                double mg_out_hz_right = Steamhz(mg_out_temp_right, mg_out_press_right);//末过右侧出口焓
                double mg_in_press_right = flq_press - (flq_press - mg_out_press_right) / 4 * 3;//末过右侧进口压力
                double mg_in_hz_right = Steamhz(mg_in_temp_right, mg_in_press_right);//末过右侧进口焓
                double mg_hz_right = mg_out_hz_right - mg_in_hz_right;//末过右侧焓增


                double dz_out_press_left = dz_in_press_left - (dz_in_press_left - gz_out_press_left) / 2;//低再左侧出口压力
                double dz_out_temp_left = dz_out_qw_left * 0.95 + dz_bw_left * 0.05;//低再左侧出口温度
                double dz_out_hz_left = Steamhz(dz_out_temp_left, dz_out_press_left);//低再左侧出口焓
                double dz_in_hz_left = Steamhz(dz_in_temp_left, dz_in_press_left); //低再左侧进口焓
                double dz_hz_left = dz_out_hz_left - dz_in_hz_left;//低再左侧焓增


                double dz_out_press_right = dz_in_press_right - (dz_in_press_right - gz_out_press_right) / 2;//低再右侧出口压力
                double dz_out_temp_right = dz_out_qw_right * 0.95 + dz_bw_right * 0.05;//低再右侧出口温度
                double dz_out_hz_right = Steamhz(dz_out_temp_right, dz_out_press_right);//低再右侧出口焓
                double dz_in_hz_right = Steamhz(dz_in_temp_right, dz_in_press_right); //低再右侧进口焓
                double dz_hz_right = dz_out_hz_right - dz_in_hz_right;//低再右侧焓增


                double gz_out_temp_left = gz_out_qw_left * 0.9 + gz_bw_left * 0.1;//高再左侧出口温度
                double gz_out_hz_left = Steamhz(gz_out_temp_left, gz_out_press_left);//高再左侧出口焓
                double gz_in_press_left = dz_out_press_left;//高再左侧进口压力
                double gz_in_hz_left = Steamhz(gz_in_temp_left, gz_in_press_left);//高再左侧进口焓
                double gz_hz_left = gz_out_hz_left - gz_in_hz_left;//高再左侧焓增


                double gz_out_temp_right = gz_out_qw_right * 0.9 + gz_bw_right * 0.1;//高再右侧出口温度
                double gz_out_hz_right = Steamhz(gz_out_temp_right, gz_out_press_right);//高再右侧出口焓
                double gz_in_press_right = dz_out_press_right;//高再右侧进口压力
                double gz_in_hz_right = Steamhz(gz_in_temp_right, gz_in_press_right);//高再右侧进口焓
                double gz_hz_right = gz_out_hz_right - gz_in_hz_right;//高再右侧焓增


                double fs_out_hz = Whz(fs_out_temp, fs_out_press);//分省出口焓
                double fs_in_hz = Whz(fs_in_temp, fs_in_press);//分省进口焓
                double fs_hz = fs_out_hz - fs_in_hz;//分省焓增


                double zs_out_hz = Whz(zs_out_temp, zs_out_press);//主省出口焓
                double zs_in_hz = Whz(zs_in_temp, zs_in_press);//主省进口焓
                double zs_hz = zs_out_hz - zs_in_hz;//主省焓增


                double whz_jws = Whz(gstemp, gspress);//一级减温水焓值、二级减温水焓值
                double hz_jwq_front_1_left = Steamhz(dg_out_temp_left, pg_in_press_left); //一级减温器前过热蒸汽焓值（左侧）
                double hz_jwq_back_1_left = Steamhz(pg_in_temp_left, pg_in_press_left); //一级减温器后过热蒸汽焓值（左侧）
                double hz_jwq_front_1_right = Steamhz(dg_out_temp_right, pg_in_press_right); //一级减温器前过热蒸汽焓值（右侧）
                double hz_jwq_back_1_right = Steamhz(pg_in_temp_right, pg_in_press_right); //一级减温器后过热蒸汽焓值（右侧）

                double jws_1_left = (hz_jwq_front_1_left - hz_jwq_back_1_left) * gsll / 2 / (hz_jwq_back_1_left - whz_jws);//一级减温水量（左侧）
                if (jws_1_left < 0)
                {
                    jws_1_left = 0;
                }
                double jws_1_right = (hz_jwq_front_1_right - hz_jwq_back_1_right) * gsll / 2 / (hz_jwq_back_1_right - whz_jws);//一级减温水量（右侧）
                if (jws_1_right < 0)
                {
                    jws_1_right = 0;
                }
                double jws_1 = jws_1_left + jws_1_right;//一级减温水总量


                double hz_jwq_front_2_left = Steamhz(pg_out_temp_left, mg_in_press_left); //二级减温器前过热蒸汽焓值（左侧）
                double hz_jwq_back_2_left = Steamhz(mg_in_temp_left, mg_in_press_left); //二级减温器后过热蒸汽焓值（左侧）
                double hz_jwq_front_2_right = Steamhz(pg_out_temp_right, mg_in_press_right); //二级减温器前过热蒸汽焓值（右侧）
                double hz_jwq_back_2_right = Steamhz(mg_in_temp_right, mg_in_press_right); //二级减温器后过热蒸汽焓值（右侧）

                double jws_2_left = (hz_jwq_front_2_left - hz_jwq_back_2_left) * (gsll + jws_1) / 2 / (hz_jwq_back_2_left - whz_jws);//二级减温水量（左侧）
                if (jws_2_left < 0)
                {
                    jws_2_left = 0;
                }
                double jws_2_right = (hz_jwq_front_2_right - hz_jwq_back_2_right) * (gsll + jws_1) / 2 / (hz_jwq_back_2_right - whz_jws);//二级减温水量（右侧）
                if (jws_2_right < 0)
                {
                    jws_2_right = 0;
                }
                double jws_2 = jws_2_left + jws_2_right;//二级减温水总量


                db.CommandExecuteNonQuery("update dncboiler set jws_1="+ jws_1 + ",jws_2=" + jws_2 + " where id=" + bid);

                double sjzr_grzq = 0.0017 * Math.Pow(yl_fh_out / 100, 3) - 0.0175 * Math.Pow(yl_fh_out / 100, 2) + 0.0381 * (yl_fh_out / 100) + 0.8318;//设计再热/过热蒸汽流量比
                double grq_jws = jws_1 + jws_2;//过热器减温水总量
                double gp_out_zqll = sjzr_grzq * (gsll + grq_jws);//高排出口蒸汽流量

                double sgjws_press = ctylxs * gspress;//事故减温水压力
                double whz_sgjws = Whz(gstemp, sgjws_press);//事故减温水焓值
                double hz_sgjwq_front_left = Steamhz(sgjwq_front_temp_left, dz_in_press_left);//事故减温器前再热蒸汽焓值（左侧）
                double hz_sgjwq_back_left = Steamhz(dz_in_temp_left, dz_in_press_left);//事故减温器后再热蒸汽焓值（左侧）
                double hz_sgjwq_front_right = Steamhz(sgjwq_front_temp_right, dz_in_press_right);//事故减温器前再热蒸汽焓值（右侧）
                double hz_sgjwq_back_right = Steamhz(dz_in_temp_right, dz_in_press_right);//事故减温器后再热蒸汽焓值（右侧）

                double zrq_zqll_single = gp_out_zqll / 2;//当前单侧再热蒸汽流量
                double zrq_sgjws_left = (hz_sgjwq_front_left - hz_sgjwq_back_left) * (zrq_zqll_single - cql / 2) / (hz_sgjwq_back_left - whz_sgjws);//再热器事故减温水量（左侧）
                if (zrq_sgjws_left < 0)
                {
                    zrq_sgjws_left = 0;
                }
                double zrq_sgjws_right = (hz_sgjwq_front_right - hz_sgjwq_back_right) * (zrq_zqll_single - cql / 2) / (hz_sgjwq_back_right - whz_sgjws);//再热器事故减温水量（右侧）
                if (zrq_sgjws_right < 0)
                {
                    zrq_sgjws_right = 0;
                }
                double dzzqll_single_left = (gp_out_zqll - cql) / 2 + zrq_sgjws_left;//单侧低再蒸汽流量（左侧）
                double dzzqll_single_right = (gp_out_zqll - cql) / 2 + zrq_sgjws_right;//单侧低再蒸汽流量（右侧）

                double hz_wljwq_front_left = Steamhz(dz_out_temp_left, gz_in_press_left);//微量减温器前再热蒸汽焓值（左侧）
                double hz_wljwq_back_left = Steamhz(gz_in_temp_left, gz_in_press_left);//微量减温器后再热蒸汽焓值（左侧）
                double hz_wljwq_front_right = Steamhz(dz_out_temp_right, gz_in_press_right);//微量减温器前再热蒸汽焓值（右侧）
                double hz_wljwq_back_right = Steamhz(gz_in_temp_right, gz_in_press_right);//微量减温器后再热蒸汽焓值（右侧）

                double zrq_wljws_left = (hz_wljwq_front_left - hz_wljwq_back_left) * dzzqll_single_left / (hz_wljwq_back_left - whz_sgjws);//再热器微量减温水量（左侧）

                double zrq_wljws_right = (hz_wljwq_front_right - hz_wljwq_back_right) * dzzqll_single_right / (hz_wljwq_back_right - whz_sgjws);//再热器微量减温水量（右侧）


                double gzzqll_single_left = dzzqll_single_left + zrq_wljws_left;//单侧高再蒸汽流量（左侧）
                double gzzqll_single_right = dzzqll_single_right + zrq_wljws_right;//单侧高再蒸汽流量（右侧）



                double dg_xrl_real_left = dg_hz_left * (gsll / 2) / 1000;//低过左侧实际吸热量
                double dg_wrl_left = dg_xrl_design / dg_xrl_real_left;//低过左侧污染率
                double dg_xrl_real_right = dg_hz_right * (gsll / 2) / 1000;//低过右侧实际吸热量
                double dg_wrl_right = dg_xrl_design / dg_xrl_real_right;//低过右侧污染率

                double pg_xrl_real_left = pg_hz_left * (gsll / 2 + jws_1_left) / 1000;//屏过左侧实际吸热量
                double pg_wrl_left = pg_xrl_design / pg_xrl_real_left;//屏过左侧污染率
                double pg_xrl_real_right = pg_hz_right * (gsll / 2 + jws_1_right) / 1000;//屏过右侧实际吸热量
                double pg_wrl_right = pg_xrl_design / pg_xrl_real_right;//屏过右侧污染率

                double mg_xrl_real_left = mg_hz_left * (gsll / 2 + grq_jws) / 1000;//末过左侧实际吸热量
                double mg_wrl_left = mg_xrl_design / mg_xrl_real_left;//末过左侧污染率
                double mg_xrl_real_right = mg_hz_right * (gsll / 2 + grq_jws) / 1000;//末过右侧实际吸热量
                double mg_wrl_right = mg_xrl_design / mg_xrl_real_right;//末过右侧污染率

                double dz_xrl_real_left = dz_hz_left * dzzqll_single_left / 1000;//低再左侧实际吸热量
                double dz_wrl_left = dz_xrl_design / dz_xrl_real_left;//低再左侧污染率
                double dz_xrl_real_right = dz_hz_right * dzzqll_single_right / 1000;//低再右侧实际吸热量
                double dz_wrl_right = dz_xrl_design / dz_xrl_real_right;//低再右侧污染率

                double gz_xrl_real_left = gz_hz_left * gzzqll_single_left / 1000;//高再左侧实际吸热量
                double gz_wrl_left = gz_xrl_design / gz_xrl_real_left;//高再左侧污染率
                double gz_xrl_real_right = gz_hz_right * gzzqll_single_right / 1000;//高再右侧实际吸热量
                double gz_wrl_right = gz_xrl_design / gz_xrl_real_right;//高再右侧污染率

                double zs_xrl_real = zs_hz * gsll / 1000;//主省煤器实际吸热量
                double zs_wrl = zs_xrl_design / zs_xrl_real;//主省污染率
                double fs_xrl_real = fs_hz * gsll / 1000;//分级省煤器实际吸热量
                double fs_wrl = fs_xrl_design / fs_xrl_real;//分级省煤器污染率



                //double yrq_wrl = (1 + (0.7134 - yq_ratio_real) / 0.1 + (yrq_yq_pd - yq_pd_design) / yq_pd_design)*0.6;//预热器污染率
                // double yrq_in_wind_temp = 0.3 * yrq_in_wt1 + 0.7 * yrq_in_wt2;//预热器进口风温
                // double yq_pd_design = Math.Pow(yl_fh_out / 700, 2) * 1015;//设计烟气侧压降
                //以下为空气预热器实际烟气侧效率计算逻辑

                double kyq_in_xs_kqgl = 21 / (21 - kyq_in_o2);//空预器进口过量空气系数
                double kyq_out_xs_kqgl = 21 / (21 - kyq_out_o2);//空预器出口过量空气系数

                double kyq_in_wind_temp_jq = (kyq_in_fw_1 * kyq_wind_1 + kyq_in_fw_2 * kyq_wind_2) / (kyq_wind_1 + kyq_wind_2);//加权平均后空预器进口风温
                double kyq_out_wind_temp_jq = (kyq_out_fw_1 * kyq_wind_1 + kyq_out_fw_2 * kyq_wind_2) / (kyq_wind_1 + kyq_wind_2);//加权平均后空预器出口风温

                double kyq_wind_temp_inout_avg = (kyq_in_wind_temp_jq + kyq_out_wind_temp_jq) / 2;//空预器进出口风温平均值
                double kyq_gas_temp_inout_avg = 240d;//空预器进出口烟温平均值
                double kyq_airc = Airc(kyq_wind_temp_inout_avg);  //空预器进出口平均风温对应比热
                double kyq_gasc = 0d;//空预器进出口平均烟温（入口与零漏风）对应比热
                double px_temp_0_modify = 0d;//零漏风修正后的排烟温度
                double kyq_lfl = (kyq_out_xs_kqgl - kyq_in_xs_kqgl) / kyq_in_xs_kqgl * 0.9 * 100;//空预器漏风率
                double kyq_gas_temp_inout_avg_modify = 0d;//修正后的空预器进出口烟温平均值

                // DateTime now_time = DateTime.Now;

                for (int i = 0; i < 20; i++)
                {
                    kyq_gasc = Gasc(kyq_gas_temp_inout_avg);
                    px_temp_0_modify = kyq_lfl * kyq_airc * (kyq_py_temp - kyq_in_wind_temp_jq) / (100 * kyq_gasc) + kyq_py_temp;
                    kyq_gas_temp_inout_avg_modify = (kyq_in_yw + px_temp_0_modify) / 2;
                    if (Math.Abs(kyq_gas_temp_inout_avg_modify - kyq_gas_temp_inout_avg) < 0.1)
                    {

                        break;
                    }
                    else
                    {
                        kyq_gas_temp_inout_avg = kyq_gas_temp_inout_avg_modify;
                        continue;
                    }

                }
                kyq_gasc = Gasc(kyq_gas_temp_inout_avg_modify);
                px_temp_0_modify = kyq_lfl * kyq_airc * (kyq_py_temp - kyq_in_wind_temp_jq) / (100 * kyq_gasc) + kyq_py_temp;
                double x_ratio_design = 0.0039 * Math.Pow(yl_fh_out / 100, 3) - 0.0565 * Math.Pow(yl_fh_out / 100, 2) + 0.2725 * yl_fh_out / 100 + 0.2979;//设计X比
                double x_ratio_real = (kyq_in_yw - px_temp_0_modify) / (kyq_out_wind_temp_jq - kyq_in_wind_temp_jq);//实测X比
                double py_temp_x_modify = (x_ratio_design - x_ratio_real) * xs_py_x_modify;//经X比修正后排烟温度变化值
                double wind_temp_in_real = kyq_wind_1 / (kyq_wind_1 + kyq_wind_2) * kyq_in_fw_1 + kyq_wind_1 / (kyq_wind_1 + kyq_wind_2) * kyq_in_fw_2;//实测进口风温

                double py_temp_in_wind_temp_modify = (wind_temp_in_design - wind_temp_in_real) * py_temp_in_wind_temp_modify_xs;//经进口风温修正后排烟温度变化值
                double py_temp_modify = kyq_py_temp + py_temp_x_modify + py_temp_in_wind_temp_modify;//修正后的排烟温度
                double yq_ratio_real = (kyq_in_yw - py_temp_modify) / (kyq_in_yw - wind_temp_in_real);//实际烟气侧效率
                #endregion

                List<string> arr = new List<string>();

                AddLgoToTXT(DateTime.Now.ToString() + ":计算污染率结束！");
                #region 污染率更新

                arr.Add("update dnccharea set Wrl_Val=" + pg_wrl_left + ",RealTime='" + realtime + "'where DncBoilerId=" + bid + " and K_Name_kw='屏式过热器（左侧）';");
                arr.Add("update dnccharea set Wrl_Val=" + pg_wrl_right + ",RealTime='" + realtime + "' where DncBoilerId=" + bid + " and K_Name_kw='屏式过热器（右侧）';");
                arr.Add("update dnccharea set Wrl_Val=" + mg_wrl_left + ",RealTime='" + realtime + "' where DncBoilerId=" + bid + " and K_Name_kw='高温过热器（左侧）';");
                arr.Add("update dnccharea set Wrl_Val=" + mg_wrl_right + ",RealTime='" + realtime + "' where DncBoilerId=" + bid + " and K_Name_kw='高温过热器（右侧）';");
                arr.Add("update dnccharea set Wrl_Val=" + gz_wrl_left + ",RealTime='" + realtime + "' where DncBoilerId=" + bid + " and K_Name_kw='高温再热器（左侧）';");
                arr.Add("update dnccharea set Wrl_Val=" + gz_wrl_right + ",RealTime='" + realtime + "' where DncBoilerId=" + bid + " and K_Name_kw='高温再热器（右侧）';");
                arr.Add("update dnccharea set Wrl_Val=" + dz_wrl_left + ",RealTime='" + realtime + "' where DncBoilerId=" + bid + " and K_Name_kw='低温再热器（左侧）';");
                arr.Add("update dnccharea set Wrl_Val=" + dz_wrl_right + ",RealTime='" + realtime + "' where DncBoilerId=" + bid + " and K_Name_kw='低温再热器（右侧）';");
                arr.Add("update dnccharea set Wrl_Val=" + dg_wrl_left + ",RealTime='" + realtime + "' where DncBoilerId=" + bid + " and K_Name_kw='低温过热器（左侧）';");
                arr.Add("update dnccharea set Wrl_Val=" + dg_wrl_right + ",RealTime='" + realtime + "' where DncBoilerId=" + bid + " and K_Name_kw='低温过热器（右侧）';");
                arr.Add("update dnccharea set Wrl_Val=" + zs_wrl + ",RealTime='" + realtime + "' where DncBoilerId=" + bid + " and K_Name_kw='主省煤器（左侧）';");
                arr.Add("update dnccharea set Wrl_Val=" + zs_wrl + ",RealTime='" + realtime + "' where DncBoilerId=" + bid + " and K_Name_kw='主省煤器（右侧）';");
                arr.Add("update dnccharea set Wrl_Val=" + fs_wrl + ",RealTime='" + realtime + "' where DncBoilerId=" + bid + " and K_Name_kw='分级省煤器';");
                arr.Add("update dnccharea set Wrl_Val=" + yq_ratio_real + ",RealTime='" + realtime + "' where DncBoilerId=" + bid + " and K_Name_kw='空气预热器';");
                db.ExecuteTransaction(arr);


                string sql_his = " insert  dnccharea_his (Status,IsDeleted,DncBoilerId,DncBoiler_Name,DncChareaId,DncCharea_Name,Wrl_Val,Wrlhigh_Val,RealTime,NumberTo,TwoHour,NowNumber,TotleNumber,PreTime,All_sta) select Status,IsDeleted,DncBoilerId,DncBoiler_Name,Id,K_Name_kw,Wrl_Val,Wrlhigh_Val,RealTime,NumberTo,TwoHour,NowNumber,TotleNumber,PreTime,All_sta from dnccharea where DncBoilerId=" + bid + " and Id>1";
               // AddLgoToTXT(DateTime.Now.ToString() + ":"+ sql_his);
                db.CommandExecuteNonQuery(sql_his);

                AddLgoToTXT(DateTime.Now.ToString() + ":更新污染率结束！");
                #endregion

                #region 空预器吹灰器加入执行列表
                //判断空预器的效率是否低于下限，如果低于，将吹灰器加入执行列表
                if (yq_ratio_real < kyq_yq_ratio_low_Val)
                {
                    AddLgoToTXT(DateTime.Now.ToString() + ":空预器吹灰器加入执行列表！");
                    StringBuilder sql_ky_runlist_add = new StringBuilder();
                    string sql_kyq_run = "select Id,Name_kw,DncBoiler_Name,Lastchtime from dncchqpoint where DncChtypeId=4 and DncBoilerId=" + bid;
                    string sql_exe_ky = "select * from dncchrunlist_kyq where OffTime is null and DncBoilerId=" + bid;
                    int runnum_ky = db.GetCommand(sql_exe_ky).Rows.Count;
                    DataTable dt_kyq = db.GetCommand(sql_kyq_run);
                    DateTime lastruntime = DateTime.Parse("2021-1-1 00:00:00");
                    int kyqid = 0;
                    string kyqname = "";
                    string bname = "";

                   
                    foreach (DataRow item in dt_kyq.Rows)
                    {
                        kyqid = int.Parse(item[0].ToString());
                        kyqname = item[1].ToString();
                        bname = item[2].ToString();

                        if (chmode.Equals("1"))
                        {
                            arr.Add("insert into dncchrunlist_kyq(Name_kw, AddTime, Remarks, Status, IsDeleted, DncChqpointId, DncChqpoint_Name, DncBoilerId, DncBoiler_Name) values('" + kyqname + "', '" + realtime + "', '" + kyqname + "', 1, 0, " + kyqid + ", '" + kyqname + "', " + bid + ", '" + bname + "'); ");
                        }
                        else
                        {
                            arr.Add("insert into dncchrunlist_kyq(Name_kw, AddTime, RunTime,OffTime,Remarks, Status, IsDeleted, DncChqpointId, DncChqpoint_Name, DncBoilerId, DncBoiler_Name) values('" + kyqname + "', '" + realtime + "', '" + realtime + "', '" + realtime + "', '常规吹灰', 0, 1, " + kyqid + ", '" + kyqname + "', " + bid + ", '" + bname + "'); ");
                        }

                        lastruntime = DateTime.Parse(item[3].ToString());
                    }
                    if (runnum_ky == 0 && realtime.Subtract(lastruntime).TotalMinutes > 30)
                    {
                        arr.Add("update dnccharea set Wrl_Val=" + yq_ratio_real + ",RealTime='" + realtime + "' where DncBoilerId=" + bid + " and K_Name_kw='空气预热器';");
                        db.ExecuteTransaction(arr);
                        arr.Clear();

                    }
                }

                #endregion


                #region 长吹半吹（除分级省煤器），按区域计算污染率，加入执行列表
                if (yl_fh_out > 300)
                {
                    
                    string sql_fh = "select Fh_Val from dncfhdata ORDER BY RealTime DESC LIMIT 11";
                    DataTable dt_fh = db.GetCommand(sql_fh);
                    bool a = true;
                    if (dt_fh != null && dt_fh.Rows.Count > 0)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (Math.Abs(int.Parse(dt_fh.Rows[i + 1][0].ToString()) - int.Parse(dt_fh.Rows[i][0].ToString())) > 5)
                            {
                                a = false;
                                break;
                            }
                        }

                        if (a)
                        {
                            


                            //#region 水冷壁非周期性吹灰
                            ////水冷壁非周期性
                            //double mg_wrl = (mg_wrl_left + mg_wrl_right) / 2;
                            //double gz_wrl = (gz_wrl_left + gz_wrl_right) / 2;
                            ////if (mg_wrl < 0.825 && gz_wrl < 0.9)
                            //if (mg_wrl < mg_wrl_low && gz_wrl < gz_wrl_low)
                            //{
                            //    // string sql_chlist = "select Name_kw,Wrl_Val,Wrlhigh_Val,last_temp_dif_Val,now_temp_dif_Val,dx_temp_dif_Val,slb_circle_num,slb_floor_Val from V_chwrl where Status=1  and DncBoilerId=" + boilerid;
                            //    string sql_chlist = "select Id,Name_kw,slb_floor_Val,last_temp_dif_Val,now_temp_dif_Val,DncBoiler_Name from dncchqpoint where DncChtypeId=1 and DncBoilerId=" + bid;
                            //    DataTable dt_chlist = db.GetCommand(sql_chlist);
                            //    var list1 = new List<Chpoint>();
                            //    var list2 = new List<Chpoint>();
                            //    var list3 = new List<Chpoint>();
                            //    var list4 = new List<Chpoint>();
                            //    foreach (DataRow item in dt_chlist.Rows)
                            //    {
                            //        var obj = new Chpoint();
                            //        obj.Id = int.Parse(item[0].ToString());
                            //        obj.Name_kw = item[1].ToString();
                            //        obj.Slb_floor_Val = int.Parse(item[2].ToString());
                            //        obj.Last_temp_dif_Val = double.Parse(item[3].ToString());
                            //        obj.Now_temp_dif_Val = double.Parse(item[4].ToString());
                            //        obj.Last_now_dif = double.Parse(item[3].ToString()) - double.Parse(item[4].ToString());
                            //        obj.DncBoilerId = bid;
                            //        obj.DncBoiler_Name = item[5].ToString();
                            //        if (obj.Slb_floor_Val == 1)
                            //        {
                            //            list1.Add(obj);
                            //        }
                            //        if (obj.Slb_floor_Val == 2)
                            //        {
                            //            list2.Add(obj);
                            //        }
                            //        if (obj.Slb_floor_Val == 3)
                            //        {
                            //            list3.Add(obj);
                            //        }
                            //        if (obj.Slb_floor_Val == 4)
                            //        {
                            //            list4.Add(obj);
                            //        }

                            //    }
                            //    List<Chpoint> chlist = new List<Chpoint>();
                            //    chlist = Chlist(list1).Concat(Chlist(list2)).Concat(Chlist(list3)).Concat(Chlist(list4)).ToList();
                            //    int cl_num = chlist.Count;


                            //    List<string> arrsql = new List<string>();
                            //    for (int j = 0; j < cl_num; j++)
                            //    {
                            //        int id = chlist[j].Id;

                            //        string chq_name = chlist[j].Name_kw;
                            //        int bid = chlist[j].DncBoilerId;
                            //        string bname = chlist[j].DncBoiler_Name;
                            //        if (chmode.Equals("1"))
                            //        {
                            //            arrsql.Add("insert into dncchrunlist (Name_kw,AddTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + chq_name + "','" + realtime + "','短吹非周期性吹灰',1,0," + id + ",'" + chq_name + "'," + bid + ",'" + bid + "号锅炉',1,'水冷壁');");
                            //        }
                            //        else
                            //        {
                            //            arrsql.Add("insert into dncchrunlist (Name_kw,AddTime,RunTime,OffTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + chq_name + "','" + realtime + "','" + realtime + "','" + realtime + "','短吹非周期性吹灰',0,1," + id + ",'" + chq_name + "'," + bid + ",'" + bid + "号锅炉',1,'水冷壁');");
                            //            //arrsql.Add("update dncchqpoint set Lastchtime=now() where DncBoilerId=" + bid + " and Name_kw='" + chq_name + "';");
                            //            //arrsql.Add("update dncboiler set Qp_bh_update='0'  where Id=" + bid + ";");
                            //        }

                                    
                            //    }
                            //    db.ExecuteTransaction(arrsql);
                            //}


                            //#endregion


                            #region 水平烟道超过10天或者尾部烟道超过20天未吹

                            //水平烟道超过10天或者尾部烟道超过20天未吹
                            string sql_yd = "select Id,Name_kw,DncBoilerId,DncBoiler_Name from dncchqpoint where DncBoilerId=" + bid + " and  ((position=2 and TIMESTAMPDIFF(DAY,Lastchtime,DATE_FORMAT(NOW(), '%Y-%m-%d %H:%i:%S'))>=10) or (position=3 and TIMESTAMPDIFF(DAY,Lastchtime,DATE_FORMAT(NOW(), '%Y-%m-%d %H:%i:%S'))>=20))";
                            DataTable dt_yd = db.GetCommand(sql_yd);


                            List<string> sql_chlist_add = new List<string>();
                            if (dt_yd != null && dt_yd.Rows.Count > 0)
                            {
                                foreach (DataRow item in dt_yd.Rows)
                                {
                                    if (chmode.Equals("1"))
                                    {
                                        sql_chlist_add.Add("insert into dncchrunlist (Name_kw,AddTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + item[1].ToString() + "','" + realtime + "','尾部烟道多天未吹',1,0," + item[0].ToString() + ",'" + item[1].ToString() + "'," + bid + ",'" + bid + "号锅炉',0,'');");
                                    }
                                    else
                                    {
                                        sql_chlist_add.Add("insert into dncchrunlist (Name_kw,AddTime,RunTime,OffTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + item[1].ToString() + "','" + realtime + "','" + realtime + "','" + realtime + "','尾部烟道多天未吹(常规)',0,1," + item[0].ToString() + ",'" + item[1].ToString() + "'," + bid + ",'" + bid + "号锅炉',0,'');");
                                        //sql_chlist_add.Add("update dncchqpoint set Lastchtime=now() where DncBoilerId=" + bid + " and Name_kw='" + item[1].ToString() + "';");
                                        //sql_chlist_add.Add("update dncboiler set Qp_bh_update='0'  where Id=" + bid + ";");
                                    }
                                }
                            }
                            db.ExecuteTransaction(sql_chlist_add);


                            //负荷低于330MV超过6小时，将IK21和IK22加入列表
                            string sql_fh_6 = "select TIMESTAMPDIFF(HOUR, RealTime, now())  from dncfhdata where Fh_Val>=330 order by RealTime desc LIMIT 1";
                            DataTable dt_fh_6 = db.GetCommand(sql_fh_6);
                            int hour_fh6 = 0;
                            if (dt_fh_6.Rows.Count > 0)
                            {
                                hour_fh6 = int.Parse(dt_fh_6.Rows[0][0].ToString());
                            }

                            List<string> sql_chlist_add2 = new List<string>();

                            if (hour_fh6 >= 6)
                            {
                                string sql_fh6_point = "select Id,Name_kw,DncBoilerId,DncBoiler_Name from dncchqpoint where DncBoilerId=" + bid + " and (Name_kw='IK21' or Name_kw='IK22')";
                                DataTable dt_fh6_p = db.GetCommand(sql_fh6_point);
                                foreach (DataRow item in dt_fh6_p.Rows)
                                {
                                    if (chmode.Equals("1"))
                                    {
                                        sql_chlist_add2.Add("insert into dncchrunlist (Name_kw,AddTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + item[1].ToString() + "','" + realtime + "','低负荷超时',1,0," + item[0].ToString() + ",'" + item[1].ToString() + "'," + bid + ",'" + bid + "号锅炉',0,'');");
                                    }
                                    else
                                    {
                                        sql_chlist_add2.Add("insert into dncchrunlist (Name_kw,AddTime,RunTime,OffTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + item[1].ToString() + "','" + realtime + "','" + realtime + "','" + realtime + "','低负荷超时(常规)',0,1," + item[0].ToString() + ",'" + item[1].ToString() + "'," + bid + ",'" + bid + "号锅炉',0,'');");
                                        //sql_chlist_add2.Add("update dncchqpoint set Lastchtime=now() where DncBoilerId=" + bid + " and Name_kw='" + item[1].ToString() + "';");
                                        //sql_chlist_add2.Add("update dncboiler set Qp_bh_update='0'  where Id=" + bid + ";");
                                    }
                                }
                            }
                            db.ExecuteTransaction(sql_chlist_add2);
                            #endregion


                            #region 长吹、半长吹逻辑
                            string sqlwrl = "select id,K_Name_kw,Wrl_Val,RealTime,Wrlhigh_Val,NumberTo,TwoHour,NowNumber,TotleNumber,PreTime,v1,v2,v3,vup from dnccharea where DncBoilerId=" + bid + " and DncChtypeId in (2,3) and K_Name_kw <> '分级省煤器'";
                            DataTable dt = db.GetCommand(sqlwrl);
                            foreach (DataRow item in dt.Rows)
                            {
                                string id = item[0].ToString();
                                string K_Name_kw = item[1].ToString();
                                string Wrl_Val = item[2].ToString();
                                string RealTime = item[3].ToString();
                                string Wrlhigh_Val = item[4].ToString();
                                string NumberTo = item[5].ToString();
                                string TwoHour = item[6].ToString();
                                string NowNumber = item[7].ToString();
                                string TotleNumber = item[8].ToString();
                               // string PreTime = item[9].ToString();
                                string v1 = item[10].ToString();
                                string v2 = item[11].ToString();
                                string v3 = item[12].ToString();
                                string vup = item[13].ToString();
                                string hasrun = "select * from dncchrunlist where DncBoilerId=" + bid + " and DncchareId=" + id + " and OffTime is null;";
                                if (db.GetCommand(hasrun).Rows.Count == 0)
                                {
                                    hasrun = "select TIMESTAMPDIFF(MINUTE,OffTime,now()) from dncchrunlist where DncBoilerId=" + bid + " and DncchareId=" + id + " order by OffTime desc;";

                                    int min = 10;
                                    if (db.GetCommand(hasrun).Rows.Count > 0)
                                    {
                                        min = int.Parse(db.GetCommand(hasrun).Rows[0][0].ToString());
                                    }
                                   
                                    if (min >= 10)
                                    {
                                        List<string> xuliearr = new List<string>();
                                        double aval = double.Parse(Wrlhigh_Val);
                                        double nval = double.Parse(Wrl_Val);
                                        double vv1 = double.Parse(v1);
                                        double vv2 = double.Parse(v2);
                                        double vv3 = double.Parse(v3);
                                        double vvup = double.Parse(vup);

                                        int nownumber_v = int.Parse(NowNumber);
                                        int totlenumber_v = int.Parse(TotleNumber);


                                        //double dg_dbkd_left = Compute.Avgdata(dic["73"]);//低过挡板开度（左侧）
                                        //double dg_dbkd_right = Compute.Avgdata(dic["74"]);//低过挡板开度（右侧）
                                        //double dz_dbkd_left = Compute.Avgdata(dic["75"]);//低再挡板开度（左侧）
                                        //double dz_dbkd_right = Compute.Avgdata(dic["76"]);//低再挡板开度（右侧）

                                        if (nval > aval && DD(id, dg_dbkd_left, dg_dbkd_right, dz_dbkd_left, dz_dbkd_right))
                                        {

                                            //是否到两小时了
                                            int timein = 0;
                                            //轮次为0时，先更新轮次为1，时间为当前时间
                                            if (NumberTo.Equals("0"))
                                            {
                                                string luncissql = "update dnccharea set NumberTo=1,PreTime=now() where id=" + id;
                                                db.CommandExecuteNonQuery(luncissql);
                                            }
                                            //轮次为1时，看时间有没到2小时
                                            else if (NumberTo.Equals("1"))
                                            {
                                                hasrun = "select TIMESTAMPDIFF(MINUTE,PreTime,now()) from dnccharea where id=" + id;
                                                min = int.Parse(db.GetCommand(hasrun).Rows[0][0].ToString());
                                                if (min >= 120)
                                                {
                                                    string luncissq2 = "update dnccharea set TwoHour=1 where id=" + id;
                                                    db.CommandExecuteNonQuery(luncissq2);
                                                    timein = 1;
                                                }
                                            }



                                            if (TwoHour.Equals("0"))
                                            {
                                                if (nval > aval + vv3)
                                                {
                                                    // 全序列吹灰
                                                    AddLgoToTXT(DateTime.Now.ToString() + ":" + id + "区域全序列吹灰");
                                                    string xuliesql = "select Name_kw,PointId from dncchareanumber where  DncBoilerId=" + bid + " and DncchareId=" + id + "  order by OrderNumber";
                                                    DataTable xuliedt = db.GetCommand(xuliesql);
                                                    xuliearr.Add("update dnccharea set All_sta=1 where id=" + id);
                                                    foreach (DataRow item1 in xuliedt.Rows)
                                                    {
                                                      
                                                        if (chmode.Equals("1"))
                                                        {
                                                           
                                                            xuliearr.Add("insert into dncchrunlist (Name_kw,AddTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + item1[0].ToString() + "','" + realtime + "','污染率超过高值',1,0," + item1[1].ToString() + ",'" + item1[0].ToString() + "'," + bid + ",'" + bid + "号锅炉'," + id + ",'" + K_Name_kw + "');");
                                                        }
                                                        else
                                                        {
                                                            xuliearr.Add("insert into dncchrunlist (Name_kw,AddTime,RunTime,OffTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + item1[0].ToString() + "','" + realtime + "','" + realtime + "','" + realtime + "','常规吹灰，全序列吹灰',0,1," + item1[1].ToString() + ",'" + item1[0].ToString() + "'," + bid + ",'" + bid + "号锅炉'," + id + ",'" + K_Name_kw + "');");
                                                           // xuliearr.Add("update dnccharea set All_sta=0,NumberTo=0 where id=" + id);

                                                            new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", id}, 10 * 60000, 0);

                                                            //xuliearr.Add("update dnccharea set NowNumber=0,All_sta=0,TwoHour=0,PreTime=null,NumberTo=0,Wrlhigh_Val= Wrlhigh_Val+Vup  where id=" + id);
                                                            //xuliearr.Add("update dncchqpoint set Lastchtime=now() where DncBoilerId=" + bid + " and Name_kw='" + item1[0].ToString() + "';");
                                                            //xuliearr.Add("update dncboiler set Qp_bh_update='0'  where Id=" + bid + ";");
                                                        }
                                                    }


                                                    //if (chmode.Equals("1"))
                                                    //{
                                                    //    xuliearr.Add("update dnccharea set All_sta=1 where id=" + id);
                                                    //}
                                                }

                                                if (!DD(id, dg_dbkd_left, dg_dbkd_right, dz_dbkd_left, dz_dbkd_right))
                                                {
                                                    if (nval <= aval + vv3 && nval > aval + vv2)
                                                    {
                                                        string cc = "";
                                                        if (totlenumber_v == 3)
                                                        {
                                                            cc = ",All_sta=1";
                                                        }
                                                        string luncissq6 = "update dnccharea set NowNumber=0" + cc + "  where id=" + id;
                                                        db.CommandExecuteNonQuery(luncissq6);
                                                        xuliearr.AddRange(JoinInCH(db, id, K_Name_kw, "1,2,3"));
                                                    }
                                                    else if (nval <= aval + vv2 && nval > aval + vv1)
                                                    {
                                                        string cc = "";
                                                        if (totlenumber_v == 2)
                                                        {
                                                            cc = ",All_sta=1";
                                                        }
                                                        string luncissq6 = "update dnccharea set NowNumber=0" + cc + "  where id=" + id;
                                                        db.CommandExecuteNonQuery(luncissq6);
                                                        xuliearr.AddRange(JoinInCH(db, id, K_Name_kw, "1,2"));
                                                    }
                                                    else if (nval <= aval + vv1)
                                                    {
                                                        string luncissq6 = "update dnccharea set NumberTo=0  where id=" + id;
                                                        db.CommandExecuteNonQuery(luncissq6);
                                                        xuliearr.AddRange(JoinInCH(db, id, K_Name_kw, "1"));
                                                    }
                                                }


                                            }
                                            //timein==1 就是刚到2小时
                                            if (TwoHour.Equals("1") || timein == 1)
                                            {

                                                if (nval <= aval + vv3 && nval > aval + vv2)
                                                {
                                                    string cc = "";
                                                    if (totlenumber_v == 3)
                                                    {
                                                        cc = ",All_sta=1";
                                                    }
                                                    string luncissq6 = "update dnccharea set NowNumber=3" + cc + "  where id=" + id;
                                                    db.CommandExecuteNonQuery(luncissq6);
                                                    xuliearr.AddRange(JoinInCH(db, id, K_Name_kw, "1,2,3"));

                                                }
                                                else if (nval <= aval + vv2 && nval > aval + vv1)
                                                {
                                                    string cc = "";
                                                    if (totlenumber_v == 2)
                                                    {
                                                        cc = ",All_sta=1";
                                                    }
                                                    string luncissq6 = "update dnccharea set NowNumber=2" + cc + "  where id=" + id;
                                                    db.CommandExecuteNonQuery(luncissq6);
                                                    xuliearr.AddRange(JoinInCH(db, id, K_Name_kw, "1,2"));
                                                    // 第一，二序列吹灰
                                                    //if (nownumber_v < totlenumber_v)
                                                    //{
                                                    //    if (nval >= aval - vv1)
                                                    //    {
                                                    //        string cc = "";
                                                    //        if (totlenumber_v == nownumber_v + 1)
                                                    //        {
                                                    //            cc = ",All_sta=1";
                                                    //        }
                                                    //        string luncissq6 = "update dnccharea set NowNumber=" + (nownumber_v + 1) + "" + cc + "  where id=" + id;
                                                    //        db.CommandExecuteNonQuery(luncissq6);
                                                    //        if (nownumber_v == 0)
                                                    //        {
                                                    //            xuliearr.AddRange(JoinInCH(db, id, K_Name_kw, "1,2"));
                                                    //        }
                                                    //        else
                                                    //        {
                                                    //            xuliearr.AddRange(JoinInCH(db, id, K_Name_kw, (nownumber_v + 1) + ""));
                                                    //        }
                                                    //    }
                                                    //}
                                                }
                                                else if (nval <= aval + vv1)
                                                {
                                                    string luncissq6 = "update dnccharea set NowNumber=1  where id=" + id;
                                                    db.CommandExecuteNonQuery(luncissq6);
                                                    xuliearr.AddRange(JoinInCH(db, id, K_Name_kw, "1"));
                                                    // 第一序列吹灰
                                                    //if (nval >= aval - vv1)
                                                    //{
                                                    //    string cc = "";
                                                    //    if (totlenumber_v == nownumber_v + 1)
                                                    //    {
                                                    //        cc = ",All_sta=1";
                                                    //    }
                                                    //    string luncissq6 = "update dnccharea set NowNumber=" + (nownumber_v + 1) + ""+ cc + "  where id=" + id;
                                                    //    db.CommandExecuteNonQuery(luncissq6);
                                                    //    if (nownumber_v == 0)
                                                    //    {
                                                    //        xuliearr.AddRange(JoinInCH(db, id, K_Name_kw, "1"));
                                                    //    }
                                                    //    else
                                                    //    {
                                                    //        xuliearr.AddRange(JoinInCH(db, id, K_Name_kw, (nownumber_v + 1) + ""));
                                                    //    }
                                                    //}
                                                }
                                            }

                                            db.ExecuteTransaction(xuliearr);



                                        }

                                    }
                                }

                            }
                            #endregion

                            #region 分级省煤器
                            sqlwrl = "select id,K_Name_kw,Wrl_Val,RealTime,Wrlhigh_Val,NumberTo,TwoHour,NowNumber,TotleNumber,PreTime,v1,v2,v3,vup from dnccharea where DncBoilerId=" + bid + "  and K_Name_kw = '分级省煤器'";
                            dt = db.GetCommand(sqlwrl);
                            foreach (DataRow item in dt.Rows)
                            {
                                string id = item[0].ToString();
                                string K_Name_kw = item[1].ToString();
                                string Wrl_Val = item[2].ToString();
                                string RealTime = item[3].ToString();
                                string Wrlhigh_Val = item[4].ToString();
                                //string NumberTo = item[5].ToString();
                                //string TwoHour = item[6].ToString();
                                //string NowNumber = item[7].ToString();
                                //string TotleNumber = item[8].ToString();
                                //string PreTime = item[9].ToString();
                                //string v1 = item[10].ToString();
                                //string v2 = item[11].ToString();
                                //string v3 = item[12].ToString();
                                //string vup = item[13].ToString();
                                string hasrun = "select * from dncchrunlist where DncBoilerId=" + bid + " and DncchareId=" + id + " and OffTime is null;";
                                if (db.GetCommand(hasrun).Rows.Count == 0)
                                {
                                    hasrun = "select TIMESTAMPDIFF(MINUTE,OffTime,now()) from dncchrunlist where DncBoilerId=" + bid + " and DncchareId=" + id + " order by OffTime desc;";
                                    DataTable dt_run = db.GetCommand(hasrun);
                                    int min = 10;
                                    if (dt_run.Rows.Count>0)
                                    {
                                        min= int.Parse(dt_run.Rows[0][0].ToString());
                                    }
                                   
                                    double aval = double.Parse(Wrlhigh_Val);
                                    double nval = double.Parse(Wrl_Val);
                                    if (min >= 10 && nval > aval)
                                    {
                                        
                                        //double vv1 = double.Parse(v1);
                                        //double vv2 = double.Parse(v2);
                                        //double vv3 = double.Parse(v3);
                                        //double vvup = double.Parse(vup);

                                        string sql = "select Name_kw,PointId from dncchareanumber where Dncchare_Name ='分级省煤器' and DncBoilerId=" + bid;
                                        List<string> xuliearr = new List<string>();
                                        foreach (DataRow item1 in db.GetCommand(sql).Rows)
                                        {
                                            if (chmode.Equals("1"))
                                            {
                                                xuliearr.Add("insert into dncchrunlist (Name_kw,AddTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + item1[0].ToString() + "','" + realtime + "','污染率超过高值',1,0," + item1[1].ToString() + ",'" + item1[0].ToString() + "'," + bid + ",'" + bid + "号锅炉'," + id + ",'" + K_Name_kw + "');");
                                            }
                                            else
                                            {
                                                xuliearr.Add("insert into dncchrunlist (Name_kw,AddTime,RunTime,OffTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + item1[0].ToString() + "','" + realtime + "','" + realtime + "','" + realtime + "','常规吹灰',0,1," + item1[1].ToString() + ",'" + item1[0].ToString() + "'," + bid + ",'" + bid + "号锅炉'," + id + ",'" + K_Name_kw + "');");
                                                
                                            }
                                        }
                                        db.ExecuteTransaction(xuliearr);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
                #endregion

            }
        }

        /// <summary>
        /// 挡板条件
        /// </summary>
        /// <param name="id">areid</param>
        /// <param name="dg_dbkd_left">低过挡板开度（左侧）</param>
        /// <param name="dg_dbkd_right">低过挡板开度（右侧）</param>
        /// <param name="dz_dbkd_left">低再挡板开度（左侧）</param>
        /// <param name="dz_dbkd_right">低再挡板开度（右侧）</param>
        /// <returns></returns>
        public bool DD(string id,double dg_dbkd_left, double dg_dbkd_right, double dz_dbkd_left, double dz_dbkd_right)
        {
            bool flag = true;
            //低温再热器（左侧）
            if (id.Equals("8"))
            {
                if (dz_dbkd_left < 60)
                {
                    flag = false;
                }
            }
            //低温再热器（右侧）
            else if (id.Equals("9"))
            {
                if (dz_dbkd_right < 60)
                {
                    flag = false;
                }
            }
            //低温过热器（左侧）
            else if (id.Equals("10"))
            {
                if (dg_dbkd_left < 50)
                {
                    flag = false;
                }
            }
            //低温过热器（右侧）
            else if (id.Equals("11"))
            {
                if (dg_dbkd_right < 50)
                {
                    flag = false;
                }
            }
            return flag;
        }
        #endregion

        #region 短吹
        private void DCUI(DBHelper db)
        {
            #region 短吹
            //凌晨0点重置数据
            if (DateTime.Now.Hour == 0 )
            {
                irarr.Clear();
                for (int i = 1; i <= 96; i++)
                {
                    irarr.Add(i);
                }

                // 昨天没吹完的加入到没吹完列表
                string sql = "select DncChqpointId from dncchrunlist where (TO_DAYS(NOW()) - TO_DAYS(AddTime)) <= 1  and Remarks='短吹常规' and DncBoilerId=" + bid + " and DncchareId=1 and Status=1 and IsDeleted=0";
                DataTable dt = db.GetCommand(sql);

                yesarr.Clear();
                foreach (DataRow item in dt.Rows)
                {
                    yesarr.Add(int.Parse(item[0].ToString()));
                }

            }
            //每个小时30分，从每层短吹选一个  1-24    25-48   49-72    73-96
            int[] c1 = { 0, 0, 0, 0 };
            for (int i = 0; i < 4; i++)
            {
                if (HasIr(24 * i + 1, 24 * (i + 1)))
                {
                    IEnumerable<int> dd = irarr.Except(yesarr).Where(x => x >= 24 * i + 1 && x <= 24 * (i + 1));
                    if (dd.Count()==0)
                    {
                        dd = irarr.Where(x => x >= 24 * i + 1 && x <= 24 * (i + 1));
                    }
                    if (dd.Count() == 0)
                    {
                        
                    }
                    else
                    {
                        c1[i] = dd.OrderBy(d => Guid.NewGuid()).FirstOrDefault();
                        irarr.Remove(c1[i]);
                    }
                    
                }
            }


            List<string> arrsql = new List<string>();

            foreach (var item in c1)
            {
                if (item != 0)
                {
                    if (chmode.Equals("1"))
                    {
                        arrsql.Add("insert into dncchrunlist (Name_kw,AddTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('IR" + item + "','" + realtime + "','周期性短吹',1,0," + item + ",'IR" + item + "'," + bid + ",'" + bid + "号锅炉',1,'水冷壁');");
                    }
                    else
                    {
                        arrsql.Add("insert into dncchrunlist (Name_kw,AddTime,RunTime,OffTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('IR" + item + "','" + realtime + "','" + realtime + "','" + realtime + "','短吹常规吹灰',0,1," + item + ",'IR" + item + "'," + bid + ",'" + bid + "号锅炉',1,'水冷壁');");
                        AddLgoToTXT(DateTime.Now.ToString() + ":IR" + item + "吹灰器加入列表");
                       // timerClose3 = new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "IR" + item, "1" }, 10*60000, 0);
                        //arrsql.Add("update dncchqpoint set Lastchtime=now() where DncBoilerId=" + bid + " and Name_kw='IR" + item + "';");
                        //arrsql.Add("update dncboiler set Qp_bh_update='0'  where Id=" + bid + ";");
                    }
                }
            }
            db.ExecuteTransaction(arrsql);

            //int c1 = new Random().Next(24) + 1;
            #endregion
        }

        public bool HasIr(int min, int max)
        {
            bool s1 = false;
            foreach (int item in irarr)
            {
                if (item >= min && item <= max)
                {
                    s1 = true;
                    break;
                }
            }
            return s1;
        }
        #endregion
        private System.Threading.Timer timerClose3;

        #region 加入吹灰
        public List<string> JoinInCH(DBHelper db, string id,string K_Name_kw,string lun)
        {
            List<string> xuliearr = new List<string>();
            string xuliesql = "select Name_kw,PointId from dncchareanumber where  DncBoilerId=" + bid + " and DncchareId=" + id + " and OrderNumber in ("+lun+")   order by OrderNumber";
            DataTable xuliedt = db.GetCommand(xuliesql);
            foreach (DataRow item1 in xuliedt.Rows)
            {

                if (chmode.Equals("1"))
                {
                    xuliearr.Add("insert into dncchrunlist (Name_kw,AddTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + item1[0].ToString() + "','" + realtime + "','污染率超过高值',1,0," + item1[1].ToString() + ",'" + item1[0].ToString() + "'," + bid + ",'" + bid + "号锅炉'," + id + ",'" + K_Name_kw + "');");
                }
                else
                {
                    xuliearr.Add("insert into dncchrunlist (Name_kw,AddTime,RunTime,OffTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + item1[0].ToString() + "','" + realtime + "','" + realtime + "','" + realtime + "','常规吹灰',0,1," + item1[1].ToString() + ",'" + item1[0].ToString() + "'," + bid + ",'" + bid + "号锅炉'," + id + ",'" + K_Name_kw + "');");
                    new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", id }, 10 * 60000, 0);
                    //timerClose2 = new System.Threading.Timer(new TimerCallback(backcall2), arrparam, 10 * 60000, 0);
                    //xuliearr.Add("update dncchqpoint set Lastchtime=now() where DncBoilerId=" + bid + " and Name_kw='" + item1[0].ToString() + "';");
                    //xuliearr.Add("update dncboiler set Qp_bh_update='0'  where Id=" + bid + ";");
                }
            }
            return xuliearr;
        }
        #endregion

        #region 锅炉本体吹灰列表执行  30秒
        private void pauserun(DBHelper db)//非周期性暂停吹灰
        {
            string sql = "select 1 from dncchrunlist where DncBoilerId=" + bid + " and Remarks='周期性短吹' and pause <> 1 and  OffTime is null";
            DataTable dto = db.GetCommand(sql);
            if (dto.Rows.Count > 0)
            {
                sql = "select jws_1,jws_2 from dncboiler where Id=" + bid + "";
                DataTable jwsdata = db.GetCommand(sql);
                double jws1 = double.Parse(jwsdata.Rows[0][0].ToString());
                double jws2 = double.Parse(jwsdata.Rows[0][1].ToString());

                sql = "select Wrl_Val,Wrl_zqx from dnccharea where DncBoilerId=" + bid + " and K_Name_kw in ('屏式过热器（左侧）','屏式过热器（右侧）')";
                DataTable wrldata = db.GetCommand(sql);
                double Wrl_Val = double.Parse(wrldata.Rows[0][0].ToString());
                double Wrl_zqx = double.Parse(wrldata.Rows[0][1].ToString());
                double Wrl_Val2 = double.Parse(wrldata.Rows[1][0].ToString());
                double Wrl_zqx2 = double.Parse(wrldata.Rows[1][1].ToString());


                sql = "select Wrl_Val,Wrl_zqx from dnccharea where DncBoilerId=" + bid + " and K_Name_kw in ('高温过热器（左侧）','高温过热器（右侧）')";
                DataTable ggdata = db.GetCommand(sql);
                double ggWrl_Val = double.Parse(ggdata.Rows[0][0].ToString());
                double ggWrl_zqx = double.Parse(ggdata.Rows[0][1].ToString());
                double ggWrl_Val2 = double.Parse(ggdata.Rows[1][0].ToString());
                double ggWrl_zqx2 = double.Parse(ggdata.Rows[1][1].ToString());

                bool b = false;
                int k = 0;
                if (jws1 < 5 && (Wrl_Val - Wrl_zqx > 0.1 || Wrl_Val2 - Wrl_zqx2 > 0.1) && jws2 < 5 && (ggWrl_Val - ggWrl_zqx > 0.075 || ggWrl_Val2 - ggWrl_zqx2 > 0.075))
                {
                    b = true;
                }
                else
                {
                    k++;
                }
                if (jws1 < 5 && (Wrl_Val - Wrl_zqx > 0.15 || Wrl_Val2 - Wrl_zqx2 > 0.15))
                {
                    b = true;
                }
                else
                {
                    k++;
                }
                if (jws2 < 5 && (ggWrl_Val - ggWrl_zqx > 0.1 || ggWrl_Val2 - ggWrl_zqx2 > 0.1))
                {
                    b = true;
                }
                else
                {
                    k++;
                }
                if (b)
                {
                    //select OffTime from dncchrunlist where DncBoilerId=" + bid + " and Remarks='周期性短吹' and OffTime is not null order by OffTime desc limit 1";
                    sql = "update dncchrunlist set pause=1 where DncBoilerId=" + bid + " and Remarks='周期性短吹' and OffTime is  null";
                    db.CommandExecuteNonQuery(sql);
                }


                string s = "select 1 from dncchrunlist where DncBoilerId=" + bid + " and Remarks='周期性短吹' and pause=1  and OffTime is  null";
                DataTable dtt = db.GetCommand(s);
                if (k == 3 && dtt.Rows.Count > 0)
                {
                    sql = "update dncchrunlist set pause=0 where DncBoilerId=" + bid + " and Remarks='周期性短吹' and OffTime is  null  and pause=1";
                    db.CommandExecuteNonQuery(sql);
                }

            }
        }

        private void ChRun(DBHelper db)
        {
            DateTime nowtime = DateTime.Now;
            if (chmode.Equals("0"))
            {
                string sqlrun = "update dncchrunlist set OffTime='" + nowtime + "',RunTime='" + nowtime + "',remarks='切换常规吹灰，强行停止执行',status=0,isdelete=1 where OffTime is null and RunTime is null and DncBoilerId=" + bid;
                db.CommandExecuteNonQuery(sqlrun);
            }


            // string sql = "select q.DncChqpointId,q.DncChqpoint_Name,p.DncChstatusId,q.Id,q.Dncchare_Name,q.DncchareId,a.TwoHour,a.NowNumber,a.TotleNumber,a.PreTime,a.Wrl_Val,a.Wrlhigh_Val,a.NumberTo,a.All_sta,a.V1,a.V2,a.V3,a.Vup  from dncchrunlist q inner join dncchqpoint p on q.DncChqpointId=p.Id inner join dnccharea a on a.id=q.DncchareId where q.DncBoilerId=" + bid + " and q.OffTime is null and q.RunTime is not null";
            string sql = "select q.DncChqpointId,q.DncChqpoint_Name,p.DncChstatusId,q.Id,q.Dncchare_Name,q.DncchareId  from dncchrunlist q inner join dncchqpoint p on q.DncChqpointId=p.Id  where q.DncBoilerId=" + bid + " and q.OffTime is null and q.RunTime is not null";
            var rdt = db.GetCommand(sql);
            //有正在执行吹灰的吹灰器
            if (rdt.Rows.Count > 0)
            {
                foreach (DataRow item in rdt.Rows)
                {
                    string DncChqpointId = item[0].ToString();
                    string DncChqpoint_Name = item[1].ToString();
                    string DncChstatusId = item[2].ToString();
                    string dncchrunlistId = item[3].ToString();
                    string Name_kw = item[4].ToString();
                    int DncchareId = 0;
                    if  (!string.IsNullOrEmpty(item[5].ToString()))
                    {
                        DncchareId= int.Parse(item[5].ToString());
                    }
                     

                    //string TwoHour = item[6].ToString();
                    //string NowNumber = item[7].ToString();
                    //string TotleNumber = item[8].ToString();
                    //string PreTime = item[9].ToString();
                    //string Wrl_Val = item[10].ToString();
                    //string Wrlhigh_Val = item[11].ToString();
                    //string NumberTo = item[12].ToString();
                    //string All_sta = item[13].ToString();

                    //double vv1 = double.Parse(item[14].ToString());
                    //double vv2 = double.Parse(item[15].ToString());
                    //double vv3 = double.Parse(item[16].ToString());
                    //double vvup = double.Parse(item[17].ToString());

                    //double nval = double.Parse(Wrl_Val);
                    //double aval = double.Parse(Wrlhigh_Val);

                    List<string> arrparam = new List<string>();
                    foreach (var item1 in item.ItemArray)
                    {
                        arrparam.Add(item1.ToString());
                    }

                    if (item[2].ToString().Equals("1"))
                    {
                        sql = "update dncchrunlist set OffTime='" + nowtime + "',Status=0 where Id=" + DncChstatusId;
                        db.CommandExecuteNonQuery(sql);
                        sql = "update dncchqpoint set Lastchtime='" + nowtime + "' where DncBoilerId=" + bid + " and Name_kw='" + DncChqpoint_Name + "'";
                        db.CommandExecuteNonQuery(sql);

                        //string ss = "select 1 from dncchrunlist where ";
                        if (DncchareId >= 2 && DncchareId <= 13)
                        {
                            string hasrun = "select * from dncchrunlist where DncBoilerId=" + bid + " and DncchareId=" + DncchareId + " and Status=1 and IsDeleted=0;";
                            if (db.GetCommand(hasrun).Rows.Count == 0)
                            {
                                timerClose2 = new System.Threading.Timer(new TimerCallback(backcall2), arrparam, 10 * 60000, 0);
                            }
                        }
                        else if (DncchareId == 1)
                        {
                            timerClose2 = new System.Threading.Timer(new TimerCallback(backcall2), arrparam, 10 * 60000, 0);
                        }
                    }
                }
            }
            //沒有正在执行吹灰的吹灰器
            else
            {

                //刚加入未执行的
                sql = "select q.Name_kw,p.Kks,p.Pmode,q.Id,q.DncChqpointId,q.Remarks from dncchrunlist q inner join dncchqpoint_zt  p on q.DncChqpointId=p.DncChqpointId where q.DncBoilerId=" + bid + " and q.OffTime is null and q.RunTime is null and p.Kind=1 and q.pause<>1 order by q.DncchareId,q.Id  limit 1";
                DataTable dt = db.GetCommand(sql);
                if (dt.Rows.Count > 0)
                {
                    string tag = dt.Rows[0][1].ToString();
                    string pmode = dt.Rows[0][2].ToString();
                    string Name_kw = dt.Rows[0][0].ToString();
                    string id = dt.Rows[0][3].ToString();
                    string pid = dt.Rows[0][4].ToString();
                    string Remarks = dt.Rows[0][5].ToString();

                    if (Remarks.Equals("周期性短吹"))
                    {
                        sql = "select OffTime from dncchrunlist where DncBoilerId=" + bid + " and Remarks='周期性短吹' and OffTime is not null order by OffTime desc limit 1";
                        DataTable dto = db.GetCommand(sql);
                        bool gx = false;
                        if (dto.Rows.Count > 0)
                        {
                            DateTime t1 = DateTime.Parse(dt.Rows[0][1].ToString());
                            int yy = t1.AddMinutes(50).CompareTo(DateTime.Now);
                            if (yy == -1 && yy == 0)
                            {
                                gx = true;
                            }
                        }
                        else
                        {
                            gx = true;
                        }
                        if (gx)
                        {
                            sql = "update dnccharea set Wrl_zqx=Wrl_Val where DncBoilerId=" + bid;
                            db.CommandExecuteNonQuery(sql);
                        }

                    }

                    if (yl_fh_out >= 400)
                    {
                        //智能化吹灰调用
                        DoChui(db, tag, pmode, id, Name_kw, pid);
                    }




                }
            }
        }



        #region dochui
        private void DoChui(DBHelper db, string tag, string pmode, string id, string Name_kw, string pid)
        {
            //智能化吹灰调用
            bool b = JinJieHttpDo(tag, pmode);
            int donum = 0;
            while (!b)
            {
                b = JinJieHttpDo(tag, pmode);
                ++donum;
                if (donum >= 5)
                {
                    break;
                }
            }


            if (donum >= 5)
            {
                AddLog(db, "tag:" + tag + " 单吹指令未执行", 105);
                throw new Exception(Name_kw + "吹灰器自启调用失败！");
            }
            else
            {
                bool b2 = JinJieHttpDo(tag, pmode, "0");
                donum = 0;
                while (!b2)
                {
                    b2 = JinJieHttpDo(tag, pmode, "0");
                    ++donum;
                    if (donum >= 5)
                    {
                        break;
                    }
                }
                if (donum >= 5)
                {
                    throw new Exception(Name_kw + "吹灰器自启信号解除失败！");
                }
                else
                {
                    //string sql = "update dncchrunlist set RunTime=now()  where Id=" + id;
                    //db.CommandExecuteNonQuery(sql);

                    db.CommandExecuteNonQuery("update dncchqpoint set Lastchtime=now() where DncBoilerId=" + bid + " and Name_kw='" + Name_kw + "';");
                    //30秒后看看，没吹继续调吹
                    var timerClose = new System.Threading.Timer(new TimerCallback(backcall), new List<string>() { pid, tag, pmode, id, Name_kw }, 30000, 0);
                }
            }
        }


        private System.Threading.Timer timerClose;
        private System.Threading.Timer timerClose2;
        private void backcall(object o)
        {
            List<string> arr = (List<string>)o;
            DBHelper db = new DBHelper();
            var sql = "select DncChstatusId from DncChqpoint where Id=" + arr[0] + " ";
            DataTable dt = db.GetCommand(sql);
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0][0].ToString().Equals("1"))
                {
                    DoChui(db, arr[1], arr[2], arr[3], arr[4], arr[0]);
                    //string sqloff = "update dncchrunlist set OffTime='" + DateTime.Now + "'  where Id=" + arr[3];
                    //db.CommandExecuteNonQuery(sqloff);
                }
                else if (dt.Rows[0][0].ToString().Equals("2"))
                {
                    string sql1 = "update dncchrunlist set RunTime='" + DateTime.Now + "'  where Id=" + arr[3];
                    db.CommandExecuteNonQuery(sql);
                }
                else
                {
                    AddLgoToTXT(DateTime.Now.ToString() + "：" + arr[4] + "吹灰器故障！");
                }
            }
            timerClose.Dispose();
        }

        #endregion

        
        private void backcall2(object o)
        {
            DBHelper db = new DBHelper();
            List<string> item = (List<string>)o;
            string DncChqpointId = item[0].ToString();
            string DncChqpoint_Name = item[1].ToString();
            string DncChstatusId = item[2].ToString();
            string dncchrunlistId = item[3].ToString();
            string Name_kw = item[4].ToString();
            int DncchareId = int.Parse( item[5].ToString());

            //string TwoHour = item[6].ToString();
            //string NowNumber = item[7].ToString();
            //string TotleNumber = item[8].ToString();
            //string PreTime = item[9].ToString();
            //string Wrl_Val = item[10].ToString();
            //string Wrlhigh_Val = item[11].ToString();
            //string NumberTo = item[12].ToString();
            //string All_sta = item[13].ToString();

            //double vv1 = double.Parse(item[14].ToString());
            //double vv2 = double.Parse(item[15].ToString());
            //double vv3 = double.Parse(item[16].ToString());
            //double vvup = double.Parse(item[17].ToString());

            //double nval = double.Parse(Wrl_Val);
            //double aval = double.Parse(Wrlhigh_Val);

            //string csql = "select Wrl_Val,Wrlhigh_Val  from dnccharea where id=" + DncchareId;
            //DataTable dt = db.GetCommand(csql);
            //nval = double.Parse(dt.Rows[0][0].ToString());
            //aval = double.Parse(dt.Rows[0][1].ToString());

            if (DncchareId>=2 && DncchareId<=13 )
            {
                string csql = "select Wrl_Val,Wrlhigh_Val,K_Name_kw,NowNumber,TotleNumber,NumberTo,All_sta,v1  from dnccharea where id=" + DncchareId;
                DataTable dt = db.GetCommand(csql);
                double nval = double.Parse(dt.Rows[0][0].ToString());
                double aval = double.Parse(dt.Rows[0][1].ToString());
                //string Name_kw = dt.Rows[0][2].ToString();
                string NowNumber = dt.Rows[0][3].ToString();
                string TotleNumber = dt.Rows[0][4].ToString();
                string NumberTo = dt.Rows[0][5].ToString();
                string All_sta = dt.Rows[0][6].ToString();
                double vv1 = double.Parse(dt.Rows[0][7].ToString());
                if (All_sta.Equals("1"))
                {
                    string luncissq6 = "update dnccharea set NowNumber=0,All_sta=0,TwoHour=0,PreTime=null,NumberTo=0,Wrlhigh_Val= Wrl_Val+Vup  where id=" + DncchareId;
                    db.CommandExecuteNonQuery(luncissq6);
                }
                else if(NumberTo.Equals("1") && int.Parse(NowNumber)< int.Parse(TotleNumber) )
                {
                    if (nval <= aval - vv1)
                    {
                        string luncissq6 = "update dnccharea set NowNumber=0,All_sta=0,TwoHour=0,PreTime=null,NumberTo=0  where id=" + DncchareId;
                        db.CommandExecuteNonQuery(luncissq6);
                    }
                    else
                    {
                        string sql_point = "select DncTypeId,Pvalue from dncchhzpointnow where  DncBoilerId=" + bid+" and DncTypeId in (73,74,75,76)";
                        DataTable dt_point = db.GetCommand(sql_point);

                        double dg_dbkd_left = Compute.Avgdata(dt_point.Rows[0][1].ToString());//低过挡板开度（左侧）
                        double dg_dbkd_right = Compute.Avgdata(dt_point.Rows[1][1].ToString());//低过挡板开度（右侧）
                        double dz_dbkd_left = Compute.Avgdata(dt_point.Rows[2][1].ToString());//低再挡板开度（左侧）
                        double dz_dbkd_right = Compute.Avgdata(dt_point.Rows[3][1].ToString());//低再挡板开度（右侧）

                        if (DD(DncchareId+"", dg_dbkd_left, dg_dbkd_right, dz_dbkd_left, dz_dbkd_right))
                        {
                            string luncissq6 = "update dnccharea set NowNumber=" + (int.Parse(NowNumber) + 1) + " where id=" + DncchareId;
                            db.CommandExecuteNonQuery(luncissq6);
                            List<string> xuliearr = new List<string>();
                            xuliearr.AddRange(JoinInCH(db, DncchareId + "", Name_kw, (int.Parse(NowNumber) + 1) + ""));

                            if ((int.Parse(NowNumber) + 1) == int.Parse(TotleNumber))
                            {
                                xuliearr.Add("update dnccharea set All_sta=1  where id=" + DncchareId);

                            }
                            db.ExecuteTransaction(xuliearr);
                        }
                    }
                }
            }
            else if (DncchareId == 1)
            {
                string luncissq6 = "update dncchqpoint set last_temp_dif_Val=now_temp_dif_Val  where id=" + DncChqpointId;
                db.CommandExecuteNonQuery(luncissq6);
            }

            timerClose2.Dispose();
        }
        private void backcall_cg(object o)
        {
            //AddLgoToTXT(DateTime.Now.ToString() + "：10分钟后续操作");
            DBHelper db = new DBHelper();
            List<string> item = (List<string>)o;
            string DncChqpointId = item[0].ToString(); 
            int DncchareId = int.Parse(item[1].ToString());
           // AddLgoToTXT(DateTime.Now.ToString() + "：吹灰器id："+ DncChqpointId+",区域ID："+ item[1].ToString());
            if (DncchareId >= 2 && DncchareId <= 13)
            {
                string csql = "select Wrl_Val,Wrlhigh_Val,K_Name_kw,NowNumber,TotleNumber,NumberTo,All_sta,v1  from dnccharea where id=" + DncchareId;
                DataTable dt = db.GetCommand(csql);
                double nval = double.Parse(dt.Rows[0][0].ToString());
                double aval = double.Parse(dt.Rows[0][1].ToString());
                string Name_kw = dt.Rows[0][2].ToString();
                string NowNumber = dt.Rows[0][3].ToString();
                string TotleNumber = dt.Rows[0][4].ToString();
                string NumberTo = dt.Rows[0][5].ToString();
                string All_sta = dt.Rows[0][6].ToString();
                double vv1 = double.Parse(dt.Rows[0][7].ToString());
                if (All_sta.Equals("1"))
                {
                    string luncissq6 = "update dnccharea set NowNumber=0,All_sta=0,TwoHour=0,PreTime=null,NumberTo=0,Wrlhigh_Val= Wrl_Val+Vup  where id=" + DncchareId;
                    db.CommandExecuteNonQuery(luncissq6);
                }
                else if (NumberTo.Equals("1") && int.Parse(NowNumber) < int.Parse(TotleNumber))
                {
                    if (nval <= aval - vv1)
                    {
                        string luncissq6 = "update dnccharea set NowNumber=0,All_sta=0,TwoHour=0,PreTime=null,NumberTo=0  where id=" + DncchareId;
                        db.CommandExecuteNonQuery(luncissq6);
                    }
                    else
                    {
                        string sql_point = "select DncTypeId,Pvalue from dncchhzpointnow where  DncBoilerId=" + bid + " and DncTypeId in (73,74,75,76)";
                        DataTable dt_point = db.GetCommand(sql_point);

                        double dg_dbkd_left = Compute.Avgdata(dt_point.Rows[0][1].ToString());//低过挡板开度（左侧）
                        double dg_dbkd_right = Compute.Avgdata(dt_point.Rows[1][1].ToString());//低过挡板开度（右侧）
                        double dz_dbkd_left = Compute.Avgdata(dt_point.Rows[2][1].ToString());//低再挡板开度（左侧）
                        double dz_dbkd_right = Compute.Avgdata(dt_point.Rows[3][1].ToString());//低再挡板开度（右侧）

                        if (DD(DncchareId + "", dg_dbkd_left, dg_dbkd_right, dz_dbkd_left, dz_dbkd_right))
                        {
                            string luncissq6 = "update dnccharea set NowNumber=" + (int.Parse(NowNumber) + 1) + " where id=" + DncchareId;
                            db.CommandExecuteNonQuery(luncissq6);
                            List<string> xuliearr = new List<string>();
                            xuliearr.AddRange(JoinInCH(db, DncchareId + "", Name_kw, (int.Parse(NowNumber) + 1) + ""));

                            if ((int.Parse(NowNumber) + 1) == int.Parse(TotleNumber))
                            {
                                xuliearr.Add("update dnccharea set All_sta=1  where id=" + DncchareId);

                            }
                            db.ExecuteTransaction(xuliearr);
                        }
                    }
                }
            }
            else if (DncchareId == 1)
            {
                string luncissq6 = "update dncchqpoint set last_temp_dif_Val=now_temp_dif_Val  where id=" + DncChqpointId;
                db.CommandExecuteNonQuery(luncissq6);
            }

           // timerClose3.Dispose();
        }




        #endregion

        #region 空预器吹灰列表执行  30秒
        private void KyqChRun(DBHelper db)
        {


            string sql = "select q.DncChqpointId,q.DncChqpoint_Name,p.DncChstatusId,q.Id from dncchrunlist_kyq q inner join dncchqpoint p on q.DncChqpointId=p.Id where q.DncBoilerId=" + bid + " and q.OffTime is null and q.RunTime is not null";
            var rdt = db.GetCommand(sql);
            DateTime nowtime = DateTime.Now;
            //有正在执行吹灰的吹灰器
            if (rdt.Rows.Count > 0)
            {

                foreach (DataRow item in rdt.Rows)
                {
                    string DncChstatusId = item[2].ToString();
                    if (DncChstatusId.Equals("1"))
                    {
                        sql = "update dncchrunlist_kyq set OffTime='" + nowtime + "',Status=0 where Id=" + item[3].ToString();
                        db.CommandExecuteNonQuery(sql);
                        sql = "update dncchqpoint set Lastchtime='" + nowtime + "' where DncBoilerId=" + bid + " and Name_kw='" + item[1].ToString() + "'";
                        db.CommandExecuteNonQuery(sql);
                    }


                }
                string ss = "select id from dncchrunlist_kyq where DncBoilerId=" + bid + " and OffTime is null;";
                if (db.GetCommand(ss).Rows.Count == 0)
                {
                    sql = "update dncchrunlist_kyq set IsDeleted=1  where DncBoilerId=" + bid;
                    db.CommandExecuteNonQuery(sql);
                }
            }
            //沒有正在执行吹灰的吹灰器
            else
            {
                //剛加入未執行的有没有
                sql = "select q.DncChqpointId,q.DncChqpoint_Name,p.DncChstatusId,q.Id from dncchrunlist_kyq q inner join dncchqpoint p on q.DncChqpointId=p.Id where q.DncBoilerId=" + bid + " and q.OffTime is null and q.RunTime is null order by q.Id";
                if (db.GetCommand(sql).Rows.Count > 0)
                {

                    //DMAPHSBWFQPUTINPB2
                    JToken ja = JinJieHttp("HCB10CP101", "AV");
                    double d = double.Parse(ja[0]["item"]["AV"].ToString());
                    bool b1 = false;
                    int ii = 0;
                    do
                    {
                        if (d < 1)
                        {
                            b1 = JinJieHttpDo("DMAPHSBWFQPUTINPB2", "DI", "1");

                        }
                        else
                        {
                            b1 = JinJieHttpDo("DMAPHSBWFQPUTINPB2", "DI", "0");
                        }
                        ii++;
                        if (ii >= 5)
                        {
                            AddLog(db, "空预器吹灰选择汽源失败", 109);
                            throw new Exception("空预器吹灰选择汽源失败！");
                        }
                    } while (!b1);



                    //智能化空预器吹灰调用程控
                    bool b = JinJieHttpDo("ZHAPHSBWSQR", "DI");
                    int donum = 0;
                    while (!b)
                    {
                        b = JinJieHttpDo("ZHAPHSBWSQR", "DI");
                        ++donum;
                        if (donum >= 5)
                        {
                            break;
                        }
                    }
                    if (donum >= 5)
                    {
                        AddLog(db, "空预器程控吹灰未执行", 111);
                        throw new Exception("空预器吹灰程控信号发送失败！");
                    }
                    else
                    {
                        bool b2 = JinJieHttpDo("ZHAPHSBWSQR", "DI", "0");
                        donum = 0;
                        while (!b2)
                        {
                            b2 = JinJieHttpDo("ZHAPHSBWSQR", "DI", "0");
                            ++donum;
                            if (donum >= 5)
                            {
                                break;
                            }
                        }
                        if (donum >= 5)
                        {
                            throw new Exception("空预器吹灰程控信号解除失败！");
                        }
                        else
                        {

                            sql = "update dncchrunlist_kyq set RunTime='" + realtime + "'  where DncBoilerId=" + bid + " and RunTime is null";
                            db.CommandExecuteNonQuery(sql);

                        }

                    }

                }
            }
        }

        #endregion

       
       



        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DBHelper db = DB();
            JSWRL(db);
        }

        private int c = 1;
        private void timer1_Tick(object sender, EventArgs e)
        {
            c++;
            try
            {
                //todo
                DBHelper db = DB();


                string ret = HttpHelpercs.gettoken();


                //15秒 更新吹灰器状态
                if (c % (1 * 3) == 0)
                {
                    chmode = GetChMode();
                    RefreshState(db);
                }

                //1分钟一次 获取机组负荷
                if (c % (1 * 12) == 0)
                {
                    GetFH(db);
                }
                //5分钟一次 获取测点数据
                if (c % (5 * 12) == 0)
                {
                    WRLPoint(db);//realtime 赋值

                    // AddLgoToTXT("2169: "+realtime.ToString("yyyy-MM-dd HH:mm:ss"));
                    //调接口读取并更新96个吹灰点的鳍片温度值和背火侧温度值 和 燃烧区域漆片温度
                    CHPoint(db);

                    // AddLgoToTXT("2173: " + realtime.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                //5分钟一次  计算污染率，加入待吹灰列表，空预器吹灰器加入执行列表
                if (c % (5 * 12) == 0)
                {
                    // AddLgoToTXT("2186: " + realtime.ToString("yyyy-MM-dd HH:mm:ss"));
                   JSWRL(db);
                    //  AddLgoToTXT("2187: " + realtime.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                //短吹 1小时一次
                if (c% (60*12)==0)
                {
                    DCUI(db);
                }

                //30秒一次   吹灰列表执行
                if (c % (1 * 6) == 0)
                {
                    if (chmode == "1")
                    {
                        //pauserun(db);
                        //ChRun(db);
                        //KyqChRun(db);
                    }
                    
                }




            }
            catch (Exception rrr)
            {

                AddLgoToTXT(rrr.Message + "\n " + rrr.StackTrace);
            }

        }














        #region doshui 弃用
        #region 待吹灰加入执行列表  1
        /// <summary>
        /// 判断是否加入执行列表
        /// </summary>
        private void Znchrun(DBHelper db)
        {
            int chtime_sum = 0;
            int reason = 0;
            string sql_chlist_sum = "select count(*),Ch_time_Val from v_chlist where DncBoilerId=" + bid + " GROUP BY DncChtypeId,Ch_time_Val";
            DataTable dt_chtime = db.GetCommand(sql_chlist_sum);
            if (dt_chtime != null && dt_chtime.Rows.Count > 0)
            {
                foreach (DataRow item in dt_chtime.Rows)
                {
                    int num = int.Parse(item[0].ToString());
                    int chtime = int.Parse(item[1].ToString());

                    //以下为对吹计算方法
                    //if (num % 2 == 0)
                    //{
                    //    chtime_sum += num / 2 * chtime;
                    //}
                    //else
                    //{
                    //    chtime_sum += (num + 1) / 2 * chtime;
                    //}

                    //以下为单吹计算方法
                    chtime_sum += num * chtime;

                }

                string sql_wrl_cx = "select * from dnccharea where DncBoilerId=" + bid + " and (DncChtypeId=2 or DncChtypeId=3) and K_Name_kw<>'分级省煤器' and ((Wrl_Val-Wrlhigh_Val)>=0.25 or ((K_Name_kw='高温过热器（左侧）' or K_Name_kw='高温过热器（右侧）') and Wrl_Val>0 and Wrl_Val<0.7 ) or ((K_Name_kw='高温再热器（左侧）' or K_Name_kw='高温再热器（右侧）') and Wrl_Val>0 and Wrl_Val<0.75 ))";
                DataTable dt_wrl_cx = db.GetCommand(sql_wrl_cx);
                string sql_norun = "select * from dncchrunlist  where DATE_FORMAT(AddTime,'%Y-%m-%d')= DATE_FORMAT(" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",'%Y-%m-%d') ";
                DataTable dt_norun = db.GetCommand(sql_norun);
                string rname = "";
                if (chtime_sum > 7200)
                {
                    reason = 1;
                }
                else if (dt_wrl_cx != null && dt_wrl_cx.Rows.Count > 0)
                {
                    reason = 2;
                }
                else if (dt_norun != null && dt_norun.Rows.Count > 0 && realtime > DateTime.Parse(DateTime.Now.ToShortDateString() + " 20:00:00"))
                {
                    reason = 3;

                }

                switch (reason)
                {

                    case 1:
                        rname = "待吹灰列表达7200s";
                        break;
                    case 2:
                        rname = "污染率超限";
                        break;
                    case 3:
                        rname = "晚20点一天未吹";
                        break;

                }
                if (reason > 0)
                {
                    string sql_chlist = "select  DISTINCT DncChqpointId,DncChqpoint_Name,DncBoiler_Name from dncchlist  where Status=1 and IsDeleted=0 and DncBoilerId=" + bid + " ";
                    if (reason == 2)
                    {
                        sql_chlist += "order by AddTime desc,DncChqpointId asc";
                    }
                    else
                    {
                        sql_chlist += "order by DncChqpointId asc";
                    }



                    DataTable dt_chlist = db.GetCommand(sql_chlist);
                    if (dt_chlist != null && dt_chlist.Rows.Count > 0)
                    {

                        StringBuilder sql_runlist = new StringBuilder();
                        int chqid = 0;
                        string chqname = "";
                        string bname = "";

                        DateTime nowtime = DateTime.Now;
                        foreach (DataRow item1 in dt_chlist.Rows)
                        {
                            chqid = int.Parse(item1[0].ToString());
                            chqname = item1[1].ToString();
                            bname = item1[2].ToString();

                            if (chmode.Equals("1"))
                            {
                                sql_runlist.Append("insert into dncchrunlist (Name_kw,AddTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name) values ('" + chqname + "','" + realtime + "','" + rname + "',1,0," + chqid + ",'" + chqname + "'," + bid + ",'" + bname + "');");
                            }
                            else
                            {
                                sql_runlist.Append("insert into dncchrunlist (Name_kw,AddTime,RunTime,OffTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name) values ('" + chqname + "','" + realtime + "','" + realtime + "','" + realtime + "','常规吹灰:" + rname + "',0,1," + chqid + ",'" + chqname + "'," + bid + ",'" + bname + "');");
                                sql_runlist.Append("update dncchqpoint set Lastchtime='" + nowtime + "' where DncBoilerId=" + bid + " and Name_kw='" + chqname + "';");
                                sql_runlist.Append("update dncboiler set Qp_bh_update='0'  where Id=" + bid + ";");
                            }


                        }
                        AddLgoToTXT(DateTime.Now.ToString() + ":需加入执行列表数量" + dt_chlist.Rows.Count);
                        //  sql_runlist.Append();


                        JToken jt = JinJieHttp(fhtag, "AV");
                        double value2 = 0d;
                        var item = jt[0];
                        if (item["item"]["AV"] != null)
                        {
                            value2 = double.Parse(item["item"]["AV"].ToString());
                        }
                        else
                        {
                            value2 = 999;
                        }
                        int zhnum = 0;
                        if (!string.IsNullOrEmpty(sql_runlist.ToString()) && value2 > 400)
                        {
                            // AddLgoToTXT(DateTime.Now.ToString() + ":"+ sql_runlist.ToString());
                            zhnum = db.CommandExecuteNonQuery(sql_runlist.ToString());

                        }
                        // AddLgoToTXT(DateTime.Now.ToString() + ":已加入执行列表数量" + zhnum);
                        db.CommandExecuteNonQuery("update dncchlist set Status=0 where DncBoilerId=" + bid + ";");


                    }

                }
            }


        }


        #endregion

        //private void SS(DBHelper db)
        //{
        //    string c = "select Ss_sta from dncboiler where id=" + bid;
        //    DataTable dtc = db.GetCommand(c);
        //    if (dtc.Rows.Count > 0)
        //    {
        //        string ss_st = dtc.Rows[0][0].ToString();
        //        if (!ss_st.Equals("2"))
        //        {
        //            //疏水
        //            bool b = DoShui();
        //            c = "update dncboiler set Ss_sta='1' where id=" + bid;
        //        }
        //    }
        //}



        //private bool DoShui()
        //{
        //    //智能化吹灰调用
        //    bool b = JinJieHttpDo("ZHDRHSBWSQR", "DI");
        //    int donum = 0;
        //    while (!b)
        //    {
        //        b = JinJieHttpDo("ZHDRHSBWSQR", "DI");
        //        ++donum;
        //        if (donum >= 5)
        //        {
        //            break;
        //        }
        //    }
        //    if (donum >= 5)
        //    {
        //        throw new Exception("疏水程控调用失败！");
        //    }
        //    else
        //    {
        //        bool b2 = JinJieHttpDo("ZHDRHSBWSQR", "DI", "0");
        //        donum = 0;
        //        while (!b2)
        //        {
        //            b2 = JinJieHttpDo("ZHDRHSBWSQR", "DI", "0");
        //            ++donum;
        //            if (donum >= 5)
        //            {
        //                break;
        //            }
        //        }
        //        if (donum >= 5)
        //        {
        //            throw new Exception("疏水信号解除失败！");
        //        }
        //    }
        //    return b;
        //}

        #region 执行吹灰30分钟后更新上次鳍片温度和背火侧温差  3  
        private void AfterCH30(DBHelper db)
        {
            AddLgoToTXT(DateTime.Now.ToString() + ":更新鳍片温度和背火侧温差");
            DateTime d1 = DateTime.MinValue;
            string sql_blid = "select Qp_bh_update from dncboiler where  id=" + bid;
            string sql_last_ch = "select OffTime from dncchrunlist where DncBoilerId=" + bid + " ORDER BY OffTime desc LIMIT 1";
            DataTable dt_blid = db.GetCommand(sql_blid);
            DataTable dt_last_ch = db.GetCommand(sql_last_ch);
            if (dt_last_ch != null && dt_last_ch.Rows.Count != 0)
            {

                d1 = DateTime.Parse(dt_last_ch.Rows[0][0].ToString());
            }



            DateTime d2 = DateTime.Now;
            TimeSpan d3 = d2.Subtract(d1);
            if (d3.TotalMinutes > 30 && (dt_blid.Rows[0][0] == null || dt_blid.Rows[0][0].Equals("0")))
            {
                string uplast_temp_dif_Val = "UPDATE dncchqpoint set last_temp_dif_Val=now_temp_dif_Val WHERE DncChtypeId=1 and  DncBoilerId=" + bid;
                db.CommandExecuteNonQuery(uplast_temp_dif_Val);

                string sql = "update dncboiler set Qp_bh_update='1'  where Id=" + bid;
                db.CommandExecuteNonQuery(sql);
            }
        }
        #endregion

        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            DBHelper db = new DBHelper();
            string sql_chlist = "select Id,Name_kw,slb_floor_Val,last_temp_dif_Val,now_temp_dif_Val,DncBoiler_Name from dncchqpoint where DncChtypeId=1 and DncBoilerId=" + bid;
            DataTable dt_chlist = db.GetCommand(sql_chlist);
            var list1 = new List<Chpoint>();
            var list2 = new List<Chpoint>();
            var list3 = new List<Chpoint>();
            var list4 = new List<Chpoint>();
            foreach (DataRow item in dt_chlist.Rows)
            {
                var obj = new Chpoint();
                obj.Id = int.Parse(item[0].ToString());
                obj.Name_kw = item[1].ToString();
                obj.Slb_floor_Val = int.Parse(item[2].ToString());
                obj.Last_temp_dif_Val = double.Parse(item[3].ToString());
                obj.Now_temp_dif_Val = double.Parse(item[4].ToString());
                obj.Last_now_dif = double.Parse(item[3].ToString()) - double.Parse(item[4].ToString());
                obj.DncBoilerId = bid;
                obj.DncBoiler_Name = item[5].ToString();
                if (obj.Slb_floor_Val == 1)
                {
                    list1.Add(obj);
                }
                if (obj.Slb_floor_Val == 2)
                {
                    list2.Add(obj);
                }
                if (obj.Slb_floor_Val == 3)
                {
                    list3.Add(obj);
                }
                if (obj.Slb_floor_Val == 4)
                {
                    list4.Add(obj);
                }

            }
            List<Chpoint> chlist = new List<Chpoint>();
            chlist = Chlist(list1).Concat(Chlist(list2)).Concat(Chlist(list3)).Concat(Chlist(list4)).ToList();
            int cl_num = chlist.Count;


            List<string> arrsql = new List<string>();
            for (int j = 0; j < cl_num; j++)
            {
                int id = chlist[j].Id;

                string chq_name = chlist[j].Name_kw;
                int bid = chlist[j].DncBoilerId;
                string bname = chlist[j].DncBoiler_Name;
                if (chmode.Equals("1"))
                {
                    arrsql.Add("insert into dncchrunlist (Name_kw,AddTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + chq_name + "','" + realtime + "','短吹非周期性吹灰',1,0," + id + ",'" + chq_name + "'," + bid + ",'" + bid + "号锅炉',1,'水冷壁');");
                }
                else
                {
                    arrsql.Add("insert into dncchrunlist (Name_kw,AddTime,RunTime,OffTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + chq_name + "','" + realtime + "','" + realtime + "','" + realtime + "','短吹非周期性吹灰',0,1," + id + ",'" + chq_name + "'," + bid + ",'" + bid + "号锅炉',1,'水冷壁');");
                    //arrsql.Add("update dncchqpoint set Lastchtime=now() where DncBoilerId=" + bid + " and Name_kw='" + chq_name + "';");
                    //arrsql.Add("update dncboiler set Qp_bh_update='0'  where Id=" + bid + ";");
                }


            }
          //  db.ExecuteTransaction(arrsql);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string  id = "2";
            new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", id }, 5000, 0);
        }
    }
}
