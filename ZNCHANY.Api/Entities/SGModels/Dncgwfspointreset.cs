﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static ZNCHANY.Api.Entities.Enums.CommonEnum;

namespace ZNCHANY.Api.Entities
{
	[Serializable]
	public class Dncgwfspointreset
	{
    
    
    
        /// <summary>
    	/// 序号
    	/// </summary>
        [Key,Required]
   
        public System.Int64 Id { get; set; }

        public System.Int64 BoilerStatus { get; set; }
        /// <summary>
        /// 实际时间
        /// </summary>


        public DateTime? RealTime { get; set; } 
        
    
        /// <summary>
    	/// 备注
    	/// </summary>
        
   
        public System.String Remarks { get; set; } 
        
    
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
    	/// 操作人
    	/// </summary>
        
   
        public System.String OperatePerson { get; set; } 
        
    
        /// <summary>
    	/// 点位ID
    	/// </summary>
        
    	public Dncgwfspoint DncGwfspoint { get; set; } 
        public System.Int32 DncGwfspointId { get; set; } 
	
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
