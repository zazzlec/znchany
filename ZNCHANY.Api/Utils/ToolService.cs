using Microsoft.Extensions.Configuration;
using ZNCHANY.Api.Entities;
using ZNCHANY.Api.Entities.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.Json;

namespace ZNCHANY.Api.Utils
{
    public class ToolService
    {


        /// <summary>
        /// db 类型
        /// </summary>
        /// <returns>mssql  mysql</returns>
        public static string DbType = "mysql";

        //public static string DbType = "sql";

        /// <summary>
        /// 1 登录
        /// </summary>
        /// <param name="contant"></param>
        /// <param name="by"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DncLog Log(string contant, string by, int type)
        {
            var entity = new DncLog();
            entity.LogTime = DateTime.Now;
            entity.Type = type;
            entity.K_Log_kw = contant;
            entity.WorkNum = by;
            entity.IsDeleted = CommonEnum.IsDeleted.No;
            entity.Status = CommonEnum.Status.Normal;
            return entity;
        }

        /// <summary>
        /// 发消息
        /// </summary>
        /// <param name="from">发消息人</param>
        /// <param name="to">收消息人</param>
        /// <param name="mas">消息</param>
        /// <param name="content">消息</param>
        /// <param name="state">0提醒，1警告，2紧急通知</param>
        public static DncMessage SendMessage(string from,string to,string mas,string content,int state)
        {
            
            var entity = new DncMessage();
            entity.FromWorkNum = from;
            entity.ToWorkNum = to;
            entity.IsRead = 0;
            entity.K_Msg_kw = mas;
            entity.SendTime = DateTime.Now;
            entity.State = state;
            entity.Remark = content;
            entity.IsDeleted = CommonEnum.IsDeleted.No;
            entity.Status = CommonEnum.Status.Normal;
            return entity;
        }
    }
}
