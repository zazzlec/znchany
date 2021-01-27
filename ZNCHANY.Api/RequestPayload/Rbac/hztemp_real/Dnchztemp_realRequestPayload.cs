
using static ZNCHANY.Api.Entities.Enums.CommonEnum;

namespace ZNCHANY.Api.RequestPayload.Rbac.Hztemp_real
{
    /// <summary>
    /// 
    /// </summary>
    public class Dnchztemp_realRequestPayload : RequestPayload
    {
        /// <summary>
        /// 是否已被删除
        /// </summary>
        public IsDeleted IsDeleted { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public Status Status { get; set; }
        public string d1 { get; set; }
        public string d2 { get; set; }
    }
}






