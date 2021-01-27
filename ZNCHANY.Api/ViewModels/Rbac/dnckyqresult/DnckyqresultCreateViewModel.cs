﻿using System;
using ZNCHANY.Api.Entities.Enums;
using static ZNCHANY.Api.Entities.Enums.CommonEnum;

namespace ZNCHANY.Api.ViewModels.Rbac.Dnckyqresult
{
	public class DnckyqresultCreateViewModel
	{
    
        public System.Int32 Id = 0;
        
    
        /// <summary>
    	/// 
    	/// </summary>
    	
	
    
        /// <summary>
    	/// 实际时间
    	/// </summary>
        public DateTime? RealTime { get; set; } 
    	
	
    
        /// <summary>
    	/// 备注
    	/// </summary>
        public System.String Remark { get; set; } 
    	
	
    
        /// <summary>
    	/// 锅炉ID
    	/// </summary>
    	public ZNCHANY.Api.Entities.Dncboiler DncBoiler { get; set; } 
    	
	
    
        /// <summary>
    	/// 锅炉描述
    	/// </summary>
        public System.String DncBoiler_Name { get; set; } 
    	
	
    
        /// <summary>
    	/// 支承轴承油温
    	/// </summary>
        public System.Single Zczc_Oil_Temp_Val { get; set; } 
    	
	
    
        /// <summary>
    	/// 导向轴承油温
    	/// </summary>
        public System.Single Dxzc_Oil_Temp_Val { get; set; } 
    	
	
    
        /// <summary>
    	/// 支承轴承润滑油压差
    	/// </summary>
        public System.Single Zczc_Oil_Dif_Val { get; set; } 
    	
	
    
        /// <summary>
    	/// 导向轴承润滑油压差
    	/// </summary>
        public System.Single Dxzc_Oil_Dif_Val { get; set; } 
    	
	
    
        /// <summary>
    	/// 空气出口温度
    	/// </summary>
        public System.Single Air_Temp_Out_Val { get; set; } 
    	
	
    
        /// <summary>
    	/// 空气入口温度
    	/// </summary>
        public System.Single Air_Temp_In_Val { get; set; } 
    	
	
    
        /// <summary>
    	/// 烟气进口温度
    	/// </summary>
        public System.Single Gas_Temp_In_Val { get; set; }

        /// <summary>
        /// 烟气出口温度
        /// </summary>
        public System.Single Gas_Temp_Out_Val { get; set; }

        /// <summary>
        /// 空气侧效率
        /// </summary>
        public System.Single Air_Radio_Val { get; set; }


        /// <summary>
        /// 烟气侧效率
        /// </summary>Gas_Radio_Val
        public System.Single Gas_Radio_Val { get; set; }

        /// <summary>
        /// 空预器Id
        /// </summary>

        public ZNCHANY.Api.Entities.Dnckyq DncKyq { get; set; }
        public System.String DncKyq_Name { get; set; }
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
