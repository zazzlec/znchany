﻿using System;
using ZNCHANY.Api.Entities.Enums;
using static ZNCHANY.Api.Entities.Enums.CommonEnum;

namespace ZNCHANY.Api.ViewModels.Rbac.Dnchzpoint_type
{
	public class Dnchzpoint_typeJsonModel
	{
    
    
    
        /// <summary>
    	/// 序号
    	/// </summary>
        public System.Int32 Id { get; set; } 
	
    
        /// <summary>
    	/// 测点类型
    	/// </summary>
    	public System.String DncType { get; set; } 
	
    
        /// <summary>
    	/// 
    	/// </summary>
        public System.String DncType_Name { get; set; } 
	
    
        /// <summary>
    	/// 测点编号
    	/// </summary>
        public System.Int32 PointId { get; set; } 
	
    
        /// <summary>
    	/// 测点名称
    	/// </summary>
        public System.String K_Name_kw { get; set; } 
	
    
        /// <summary>
    	/// DCS编码
    	/// </summary>
        public System.String TagCode { get; set; } 
	
    
        /// <summary>
    	/// 
    	/// </summary>
    	public System.String DncBoiler { get; set; } 
	
    
        /// <summary>
    	/// 
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
