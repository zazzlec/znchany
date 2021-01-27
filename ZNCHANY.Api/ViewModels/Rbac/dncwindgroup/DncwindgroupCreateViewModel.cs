﻿
using System;
using ZNCHANY.Api.Entities.Enums;
using static ZNCHANY.Api.Entities.Enums.CommonEnum;

namespace ZNCHANY.Api.ViewModels.Rbac.Dncwindgroup
{
	public class DncwindgroupCreateViewModel
	{
    
        public System.Int32 Id = 0;
        
    
        /// <summary>
    	/// 编号
    	/// </summary>
    	
	
    
        /// <summary>
    	/// 组别名称
    	/// </summary>
        public System.String Group_Name_kw { get; set; } 
    	
	
    
        /// <summary>
    	/// 百分比（基础工况）
    	/// </summary>
        public System.Double Base_percent { get; set; } 
    	
	
    
        /// <summary>
    	/// 实际时间
    	/// </summary>
        public DateTime? RealTime { get; set; } 
    	
	
    
        /// <summary>
    	/// 百分比（实际工况）
    	/// </summary>
        public System.Double Real_percent { get; set; }



        /// <summary>
        /// 锅炉ID
        /// </summary>
        public ZNCHANY.Api.Entities.Dncboiler DncBoiler { get; set; }
        public System.Int32 DncBoilerId { get; set; }
        public System.String DncBoiler_Name { get; set; } 
    	
	
    
        /// <summary>
    	/// 备注
    	/// </summary>
        public System.String Remarks { get; set; } 
    	
	
	
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
