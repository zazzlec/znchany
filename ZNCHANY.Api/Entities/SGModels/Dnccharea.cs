﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ZNCHANY.Api.Entities.Enums.CommonEnum;

namespace ZNCHANY.Api.Entities
{
	[Serializable]
	public class Dnccharea
	{
    
    
    
        /// <summary>
    	/// 序号
    	/// </summary>
        [Key,Required]
   
        public System.Int32 Id { get; set; }

        public DateTime? RealTime { get; set; }
        /// <summary>
        /// 区域描述
        /// </summary>


        public System.String K_Name_kw { get; set; } 
        
    
        /// <summary>
    	/// 备注
    	/// </summary>
        
   
        public System.String Remarks { get; set; }

        public System.String V1 { get; set; }
        public System.String V2 { get; set; }
        public System.String V3 { get; set; }
        /// <summary>
        /// 锅炉ID
        /// </summary>

        public Dncboiler DncBoiler { get; set; } 
        public System.Int32 DncBoilerId { get; set; } 
    
        /// <summary>
    	/// 锅炉名称
    	/// </summary>
        
   
        public System.String DncBoiler_Name { get; set; } 
        
    
        /// <summary>
    	/// 吹灰类型ID
    	/// </summary>
        
    	public Dncchtype DncChtype { get; set; } 
        public System.Int32 DncChtypeId { get; set; } 
    
        /// <summary>
    	/// 吹灰类型名称
    	/// </summary>
        
   
        public System.String DncChtype_Name { get; set; } 
        
    
        /// <summary>
    	/// 污染率
    	/// </summary>
        
   
        public System.Double Wrl_Val { get; set; }

        public System.Double Wrlhigh_Val { get; set; }


        /// <summary>
        /// 轮次
        /// </summary>


        public System.String NumberTo { get; set; }


        /// <summary>
        /// 
        /// </summary>


        public System.String TwoHour { get; set; }


        /// <summary>
        /// 当前序列
        /// </summary>


        public System.String NowNumber { get; set; }


        /// <summary>
        /// 总序列
        /// </summary>


        public System.String TotleNumber { get; set; }

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
