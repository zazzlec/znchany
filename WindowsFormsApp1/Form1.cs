using SynZnrs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //timer1.Interval = 500;
            //timer1.Start();
        }
        private System.Threading.Timer timerClose3;
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
        private void Form1_Load(object sender, EventArgs e)
        {
            //timerClose3 = new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", "888" }, 500, 0);
            //timerClose3 = new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", "888" }, 500, 0);
            //timerClose3 = new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", "888" }, 500, 0);

            new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", "12" }, 5000, 0);
            new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", "13" }, 5000, 0);



        }

        private void backcall_cg(object o)
        {
         //   AddLgoToTXT(DateTime.Now.ToString() + "：5秒钟后续操作");
            DBHelper db = new DBHelper();
            List<string> item = (List<string>)o;
            string DncChqpointId = item[0].ToString();
            int DncchareId = int.Parse(item[1].ToString());
         //   AddLgoToTXT(DateTime.Now.ToString() + "：吹灰器id：" + DncChqpointId + ",区域ID：" + item[1].ToString());
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
                        string sql_point = "select DncTypeId,Pvalue from dncchhzpointnow where  DncBoilerId=6 and DncTypeId in (73,74,75,76)";
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
        private string chmode = "0";
        public List<string> JoinInCH(DBHelper db, string id, string K_Name_kw, string lun)
        {
            DateTime realtime = DateTime.Now;
            List<string> xuliearr = new List<string>();
            string xuliesql = "select Name_kw,PointId from dncchareanumber where  DncBoilerId=6 and DncchareId=" + id + " and OrderNumber in (" + lun + ")   order by OrderNumber";
            DataTable xuliedt = db.GetCommand(xuliesql);
            foreach (DataRow item1 in xuliedt.Rows)
            {

                if (chmode.Equals("1"))
                {
                    xuliearr.Add("insert into dncchrunlist (Name_kw,AddTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + item1[0].ToString() + "','" + realtime + "','污染率超过高值',1,0," + item1[1].ToString() + ",'" + item1[0].ToString() + "',6,'6号锅炉'," + id + ",'" + K_Name_kw + "');");
                }
                else
                {
                    xuliearr.Add("insert into dncchrunlist (Name_kw,AddTime,RunTime,OffTime,Remarks,Status,IsDeleted,DncChqpointId,DncChqpoint_Name,DncBoilerId,DncBoiler_Name,DncchareId,Dncchare_Name) values ('" + item1[0].ToString() + "','" + realtime + "','" + realtime + "','" + realtime + "','常规吹灰',0,1," + item1[1].ToString() + ",'" + item1[0].ToString() + "',6,'6号锅炉'," + id + ",'" + K_Name_kw + "');");
                    new System.Threading.Timer(new TimerCallback(backcall_cg), new List<string>() { "", id }, 10 * 60000, 0);
                    //timerClose2 = new System.Threading.Timer(new TimerCallback(backcall2), arrparam, 10 * 60000, 0);
                    //xuliearr.Add("update dncchqpoint set Lastchtime=now() where DncBoilerId=" + bid + " and Name_kw='" + item1[0].ToString() + "';");
                    //xuliearr.Add("update dncboiler set Qp_bh_update='0'  where Id=" + bid + ";");
                }
            }
            return xuliearr;
        }

        public bool DD(string id, double dg_dbkd_left, double dg_dbkd_right, double dz_dbkd_left, double dz_dbkd_right)
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
        private void timer1_Tick(object sender, EventArgs e)
        {
           
        }
        //private void backcall_cg(object o)
        //{
        //    //AddLgoToTXT(DateTime.Now.ToString() + "：10分钟后续操作");
        //    MessageBox.Show("Test");
        //}
    }
}
