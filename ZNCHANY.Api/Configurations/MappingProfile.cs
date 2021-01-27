/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using AutoMapper;
using ZNCHANY.Api.Entities;
using ZNCHANY.Api.Models.Menu;
using ZNCHANY.Api.ViewModels.Rbac.Dncboiler;
using ZNCHANY.Api.ViewModels.Rbac.Dncerror_parameter;
using ZNCHANY.Api.ViewModels.Rbac.Dncfireerror_advice;
using ZNCHANY.Api.ViewModels.Rbac.Dncfireerror_mid;
using ZNCHANY.Api.ViewModels.Rbac.Dnchzpoint_type;
using ZNCHANY.Api.ViewModels.Rbac.Dnchztemp_mid;
using ZNCHANY.Api.ViewModels.Rbac.Dnchztemp_point;
using ZNCHANY.Api.ViewModels.Rbac.Dnchztemp_real;
using ZNCHANY.Api.ViewModels.Rbac.DncIcon;
using ZNCHANY.Api.ViewModels.Rbac.DncLog;
using ZNCHANY.Api.ViewModels.Rbac.DncMessage;
using ZNCHANY.Api.ViewModels.Rbac.Dnco2nox_point;
using ZNCHANY.Api.ViewModels.Rbac.DncPermission;
using ZNCHANY.Api.ViewModels.Rbac.DncRole;
using ZNCHANY.Api.ViewModels.Rbac.Dncsrm_parameter;
using ZNCHANY.Api.ViewModels.Rbac.Dnctype;
using ZNCHANY.Api.ViewModels.Rbac.DncUser;

namespace ZNCHANY.Api.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public MappingProfile()
        {
            #region DncUser
            CreateMap<DncUser, UserJsonModel>();
            CreateMap<UserCreateViewModel, DncUser>();
            CreateMap<UserEditViewModel, DncUser>();
            #endregion

            #region DncRole
            CreateMap<DncRole, RoleJsonModel>();
            CreateMap<RoleCreateViewModel, DncRole>(); 
            #endregion

            #region DncMenu
            CreateMap<DncMenu, MenuJsonModel>();
            CreateMap<ViewModels.Rbac.DncMenu.MenuCreateViewModel, DncMenu>();
            CreateMap<ViewModels.Rbac.DncMenu.MenuEditViewModel, DncMenu>();
            CreateMap<DncMenu, ViewModels.Rbac.DncMenu.MenuEditViewModel>();
            #endregion

            #region DncIcon
            CreateMap<DncIcon, IconCreateViewModel>();
            CreateMap<IconCreateViewModel, DncIcon>();
            #endregion

            #region DncPermission
            CreateMap<DncPermission, PermissionJsonModel>()
                .ForMember(d=>d.MenuName,s=>s.MapFrom(x=>x.Menu.Name))
                .ForMember(d => d.PermissionTypeText, s => s.MapFrom(x => x.Type.ToString()));
            CreateMap<PermissionCreateViewModel, DncPermission>();
            CreateMap<PermissionEditViewModel, DncPermission>();
            CreateMap<DncPermission,PermissionEditViewModel>();
            #endregion





            #region DncLog
            CreateMap<DncLog, DncLogJsonModel>();
            CreateMap<DncLogCreateViewModel, DncLog>();
            CreateMap<DncLogEditViewModel, DncLog>();
            #endregion



            #region DncMessage
            CreateMap<DncMessage, DncMessageJsonModel>();
            CreateMap<DncMessageCreateViewModel, DncMessage>();
            CreateMap<DncMessageEditViewModel, DncMessage>();
            #endregion



            #region dncboiler
            CreateMap<Dncboiler, DncboilerJsonModel>();
            CreateMap<DncboilerCreateViewModel, Dncboiler>();
            CreateMap<DncboilerEditViewModel, Dncboiler>();
            #endregion


            #region dncerror_parameter
            CreateMap<Dncerror_parameter, Dncerror_parameterJsonModel>();
            CreateMap<Dncerror_parameterCreateViewModel, Dncerror_parameter>();
            CreateMap<Dncerror_parameterEditViewModel, Dncerror_parameter>();
            #endregion


            #region dncfireerror_advice
            CreateMap<Dncfireerror_advice, Dncfireerror_adviceJsonModel>();
            CreateMap<Dncfireerror_adviceCreateViewModel, Dncfireerror_advice>();
            CreateMap<Dncfireerror_adviceEditViewModel, Dncfireerror_advice>();
            #endregion


            #region dncfireerror_mid
            CreateMap<Dncfireerror_mid, Dncfireerror_midJsonModel>();
            CreateMap<Dncfireerror_midCreateViewModel, Dncfireerror_mid>();
            CreateMap<Dncfireerror_midEditViewModel, Dncfireerror_mid>();
            #endregion


            #region dnchzpoint_type
            CreateMap<Dnchzpoint_type, Dnchzpoint_typeJsonModel>();
            CreateMap<Dnchzpoint_typeCreateViewModel, Dnchzpoint_type>();
            CreateMap<Dnchzpoint_typeEditViewModel, Dnchzpoint_type>();
            #endregion


            #region dnchztemp_mid
            CreateMap<Dnchztemp_mid, Dnchztemp_midJsonModel>();
            CreateMap<Dnchztemp_midCreateViewModel, Dnchztemp_mid>();
            CreateMap<Dnchztemp_midEditViewModel, Dnchztemp_mid>();
            #endregion


            #region dnchztemp_point
            CreateMap<Dnchztemp_point, Dnchztemp_pointJsonModel>();
            CreateMap<Dnchztemp_pointCreateViewModel, Dnchztemp_point>();
            CreateMap<Dnchztemp_pointEditViewModel, Dnchztemp_point>();
            #endregion


            #region dnchztemp_real
            CreateMap<Dnchztemp_real, Dnchztemp_realJsonModel>();
            CreateMap<Dnchztemp_realCreateViewModel, Dnchztemp_real>();
            CreateMap<Dnchztemp_realEditViewModel, Dnchztemp_real>();
            #endregion

            #region dnco2nox_point
            CreateMap<Dnco2nox_point, Dnco2nox_pointJsonModel>();
            CreateMap<Dnco2nox_pointCreateViewModel, Dnco2nox_point>();
            CreateMap<Dnco2nox_pointEditViewModel, Dnco2nox_point>();
            #endregion

            #region dncsrm_parameter
            CreateMap<Dncsrm_parameter, Dncsrm_parameterJsonModel>();
            CreateMap<Dncsrm_parameterCreateViewModel, Dncsrm_parameter>();
            CreateMap<Dncsrm_parameterEditViewModel, Dncsrm_parameter>();
            #endregion

            #region dnctype
            CreateMap<Dnctype, DnctypeJsonModel>();
            CreateMap<DnctypeCreateViewModel, Dnctype>();
            CreateMap<DnctypeEditViewModel, Dnctype>();
            #endregion
        }
    }
}
