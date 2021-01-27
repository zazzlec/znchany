﻿using System;
using ZNCHANY.Api.Entities.Enums;
using static ZNCHANY.Api.Entities.Enums.CommonEnum;

namespace ZNCHANY.Api.ViewModels.Rbac.DncMessage
{
	public class DncMessageCreateViewModel
	{
    
        public System.Int32 Id = 0;
        
    
        /// <summary>
    	/// 
    	/// </summary>
    	
	
    
        /// <summary>
    	/// 消息内容
    	/// </summary>
        public System.String K_Msg_kw { get; set; } 
    	
	
    
        /// <summary>
    	/// 内容
    	/// </summary>
        public System.String Remark { get; set; } 
    	
	
    
        /// <summary>
    	/// 发消息人
    	/// </summary>
        public System.String FromWorkNum { get; set; } 
    	
	
    
        /// <summary>
    	/// 收消息人
    	/// </summary>
        public System.String ToWorkNum { get; set; } 
    	
	
    
        /// <summary>
    	/// 是否已读
    	/// </summary>
        public System.Int32 IsRead { get; set; } 
    	
	
    
        /// <summary>
    	/// 跳转url
    	/// </summary>
        public System.String ToUrl { get; set; } 
    	
	
    
        /// <summary>
    	/// 发送时间
    	/// </summary>
        public DateTime? SendTime { get; set; } 
    	
	
    
        /// <summary>
    	/// 类型：0提醒，1警告，2紧急通知）
    	/// </summary>
        public System.Int32 State { get; set; } 
    	
	
	
        /// <summary>
        /// 是否可用(0:禁用,1:可用)
        /// </summary>
        public Status Status { get; set; }
        /// <summary>
        /// 是否已删
        /// </summary>
        public IsDeleted IsDeleted { get; set; }
		
	}
}
