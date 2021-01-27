﻿
using static ZNCHANY.Api.Entities.Enums.CommonEnum;

namespace ZNCHANY.Api.RequestPayload.Rbac.Hztemp_point
{
    /// <summary>
    /// 
    /// </summary>
    public class Dnchztemp_pointRequestPayload : RequestPayload
    {
        /// <summary>
        /// 是否已被删除
        /// </summary>
        public IsDeleted IsDeleted { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }

        public string btime { get; set; }
        public string etime { get; set; }
    }
}






