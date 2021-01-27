﻿
using static ZNCHANY.Api.Entities.Enums.CommonEnum;

namespace ZNCHANY.Api.RequestPayload.Rbac.Chhzpointnow
{
    /// <summary>
    /// 
    /// </summary>
    public class DncchhzpointnowRequestPayload : RequestPayload
    {
        /// <summary>
        /// 是否已被删除
        /// </summary>
        public IsDeleted IsDeleted { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }

        public int typeid { get; set; }
    }
}






