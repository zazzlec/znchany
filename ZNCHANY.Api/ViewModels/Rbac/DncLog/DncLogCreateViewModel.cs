﻿using System;
using ZNCHANY.Api.Entities.Enums;
using static ZNCHANY.Api.Entities.Enums.CommonEnum;

namespace ZNCHANY.Api.ViewModels.Rbac.DncLog
{
	public class DncLogCreateViewModel
	{
    
        public System.Int32 Id = 0;
        
    
        /// <summary>
    	/// 
    	/// </summary>
    	
	
    
        /// <summary>
    	/// 日志说明
    	/// </summary>
        public System.String K_Log_kw { get; set; } 
    	
	
    
        /// <summary>
    	/// 日志时间
    	/// </summary>
        public System.DateTime LogTime { get; set; } 
    	
	
    
        /// <summary>
    	/// 业务类型
    	/// </summary>
        public System.Int32 Type { get; set; } 
    	
	
    
        /// <summary>
    	/// 操作人
    	/// </summary>
        public System.String WorkNum { get; set; } 
    	
	
	
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
