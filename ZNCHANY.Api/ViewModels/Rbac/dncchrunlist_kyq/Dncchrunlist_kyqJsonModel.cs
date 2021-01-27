﻿using System;
using ZNCHANY.Api.Entities.Enums;
using static ZNCHANY.Api.Entities.Enums.CommonEnum;

namespace ZNCHANY.Api.ViewModels.Rbac.Dncchrunlist_kyq
{
	public class Dncchrunlist_kyqJsonModel
	{
    
    
    
        /// <summary>
    	/// 序号
    	/// </summary>
        public System.Int32 Id { get; set; } 
	
    
        /// <summary>
    	/// 吹灰器描述
    	/// </summary>
        public System.String Name_kw { get; set; } 
	
    
        /// <summary>
    	/// 实际时间
    	/// </summary>
        public DateTime? AddTime { get; set; } 
	
    
        /// <summary>
    	/// 吹灰时间
    	/// </summary>
        public DateTime? RunTime { get; set; } 
	
    
        /// <summary>
    	/// 吹灰结束时间
    	/// </summary>
        public DateTime? OffTime { get; set; } 
	
    
        /// <summary>
    	/// 备注
    	/// </summary>
        public System.String Remarks { get; set; } 
	
    
        /// <summary>
    	/// 吹灰器ID
    	/// </summary>
    	public ZNCHANY.Api.Entities.Dncchqpoint DncChqpoint { get; set; } 
        public int DncChqpointId { get; set; } 
	
    
        /// <summary>
    	/// 吹灰器名称
    	/// </summary>
        public System.String DncChqpoint_Name { get; set; } 
	
    
        /// <summary>
    	/// 锅炉ID
    	/// </summary>
    	public ZNCHANY.Api.Entities.Dncboiler DncBoiler { get; set; } 
        public int DncBoilerId { get; set; } 
	
    
        /// <summary>
    	/// 锅炉名称
    	/// </summary>
        public System.String DncBoiler_Name { get; set; } 
	
	
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
