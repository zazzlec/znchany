﻿
using System;
using System.Data.SqlClient;
using System.Linq;
using AutoMapper;
using ZNCHANY.Api.Entities;
using ZNCHANY.Api.Entities.Enums;
using ZNCHANY.Api.Extensions;
using ZNCHANY.Api.Extensions.AuthContext;
using ZNCHANY.Api.Extensions.CustomException;
using ZNCHANY.Api.Extensions.DataAccess;
using ZNCHANY.Api.Models.Response;
using ZNCHANY.Api.RequestPayload.Rbac.Role;
using ZNCHANY.Api.Utils;
using ZNCHANY.Api.ViewModels.Rbac.DncRole;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZNCHANY.Api.RequestPayload.Rbac.Chqpointdata;
using ZNCHANY.Api.ViewModels.Rbac.Dncchqpointdata;
using System.Transactions;
using System.Collections.Generic;
using ZNCHANY.Api.Utils;
using MySql.Data.MySqlClient;

namespace ZNCHANY.Api.Controllers.Api.ZNCHANY1
{
    /// <summary>
    /// 
    /// </summary>
    //[CustomAuthorize]
    [Route("api/ZNCHANY1/[controller]/[action]")]
    [ApiController]
    //[CustomAuthorize]
    public class DncchqpointdataController : ControllerBase
    {
        private readonly ZNCHANYDbContext _dbContext;
        private readonly IMapper _mapper;
        /// <summary>
        /// 构造control
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public DncchqpointdataController(ZNCHANYDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }


        [HttpGet]
        public IActionResult List()
        {
            using (_dbContext)
            {
                var list = _dbContext.Dncchqpointdata.ToList();
                list = list.FindAll(x => x.IsDeleted != CommonEnum.IsDeleted.Yes );
                var response = ResponseModelFactory.CreateInstance;
                response.SetData(list);
                return Ok(response);
            }
        }
        /// <summary>
        /// 查询请求
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult List(DncchqpointdataRequestPayload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            using (_dbContext)
            {
                var query = _dbContext.Dncchqpointdata.AsQueryable();
                //模糊查询
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x =>   x.Name_kw.Contains(payload.Kw.Trim())  );
                }
                
                //是否删除，是否启用
                if (payload.IsDeleted > CommonEnum.IsDeleted.All)
                {
                    query = query.Where(x => x.IsDeleted == payload.IsDeleted);
                }
                if (payload.Status > CommonEnum.Status.All)
                {
                    query = query.Where(x => x.Status == payload.Status);
                }
                
                if (payload.FirstSort != null)
                {
                    query = query.OrderBy(payload.FirstSort.Field, payload.FirstSort.Direct == "DESC");
                }
                var list = query.Paged(payload.CurrentPage, payload.PageSize).ToList();
                var totalCount = query.Count();
                var data = list.Select(_mapper.Map< Dncchqpointdata, DncchqpointdataJsonModel>);

                response.SetData(data, totalCount);
                return Ok(response);
            }
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="model">视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public IActionResult Create(DncchqpointdataCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            using (_dbContext)
            {
                var entity = _mapper.Map< DncchqpointdataCreateViewModel, Dncchqpointdata>(model);
                entity.DncBoiler = _dbContext.Dncboiler.FirstOrDefault(x => (x.K_Name_kw + "") == model.DncBoiler_Name);
                entity.DncBoiler_Name = entity.DncBoiler.K_Name_kw;
                entity.DncChstatus = _dbContext.Dncchstatus.FirstOrDefault(x => (x.K_Name_kw + "") == model.DncChstatus_Name);
                entity.DncChstatus_Name = entity.DncChstatus.K_Name_kw;
                entity.Status = CommonEnum.Status.Normal;
                _dbContext.Dncchqpointdata.Add(entity);
                _dbContext.SaveChanges();

                response.SetSuccess();
                return Ok(response);
            }
        }

        /// <summary>
        /// 编辑页获取实体
        /// </summary>
        /// <param name="code">惟一编码</param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [ProducesResponseType(200)]
        public IActionResult Edit(string code)
        {
            using (_dbContext)
            {
                var entity = _dbContext.Dncchqpointdata.FirstOrDefault(x => x.Id ==  int.Parse(code));
                var response = ResponseModelFactory.CreateInstance;
                response.SetData(_mapper.Map< Dncchqpointdata, DncchqpointdataCreateViewModel>(entity));
                return Ok(response);
            }
        }

        /// <summary>
        /// 保存编辑后的信息
        /// </summary>
        /// <param name="model">视图实体</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200)]
        public IActionResult Edit(DncchqpointdataEditViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            using (_dbContext)
            {


                var entity = _dbContext.Dncchqpointdata.FirstOrDefault(x => x.Id == model.Id);





                entity.Name_kw = model.Name_kw;
                entity.Remarks = model.Remarks;
                entity.Status = model.Status;
                entity.IsDeleted = model.IsDeleted;
                entity.DncBoiler = _dbContext.Dncboiler.FirstOrDefault(x => x.K_Name_kw == model.DncBoiler_Name);
                entity.DncBoiler_Name = entity.DncBoiler.K_Name_kw;
                entity.DncBoiler_Name = model.DncBoiler_Name;
                entity.DncChstatus = _dbContext.Dncchstatus.FirstOrDefault(x => x.K_Name_kw == model.DncChstatus_Name);
                entity.DncChstatus_Name = entity.DncChstatus.K_Name_kw;
                entity.DncChstatus_Name = model.DncChstatus_Name;
                entity.Lastchtime = model.Lastchtime;
                entity.Position = model.Position;
                entity.Last_temp_dif_Val = model.Last_temp_dif_Val;
                entity.Now_temp_dif_Val = model.Now_temp_dif_Val;
                entity.Slb_floor_Val = model.Slb_floor_Val;
                entity.Slb_circle_num = model.Slb_circle_num;
                entity.Now_temp_qp_Val = model.Now_temp_qp_Val;
                entity.Now_temp_bh_Val = model.Now_temp_bh_Val;
                entity.Realtime = model.Realtime;

                _dbContext.SaveChanges();
                return Ok(response);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet("{ids}")]
        [ProducesResponseType(200)]
        public IActionResult Delete(string ids)
        {
            var response = ResponseModelFactory.CreateInstance;

            response = UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
            return Ok(response);
        }
        
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="isDeleted"></param>
        /// <param name="ids">ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private ResponseModel UpdateIsDelete(CommonEnum.IsDeleted isDeleted, string ids)
        {
            using (_dbContext)
            {
                if (ToolService.DbType.Equals("mysql")){
                    var parameters = ids.Split(",").Select((id, index) => new MySqlParameter(string.Format("@p{0}", index), id)).ToList();
                    var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                    var sql = string.Format("UPDATE Dncchqpointdata SET IsDeleted=@IsDeleted WHERE id IN ({0})", parameterNames);
                    parameters.Add(new MySqlParameter("@IsDeleted", (int)isDeleted));
                    _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                    var response = ResponseModelFactory.CreateInstance;
                    return response;
                }else{
                    var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                    var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                    var sql = string.Format("UPDATE Dncchqpointdata SET IsDeleted=@IsDeleted WHERE id IN ({0})", parameterNames);
                    parameters.Add(new SqlParameter("@IsDeleted", (int)isDeleted));
                    _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                    var response = ResponseModelFactory.CreateInstance;
                    return response;
                }
                
            }
        }

        /// <summary>
        /// 恢复
        /// </summary>
        /// <param name="ids">ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet("{ids}")]
        [ProducesResponseType(200)]
        public IActionResult Recover(string ids)
        {
            var response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
            return Ok(response);
        }
        
        
        /// <summary>
        /// 批量更新状态
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="ids">ID字符串,多个以逗号隔开</param>
        /// <returns></returns>
        private ResponseModel UpdateStatus(UserStatus status, string ids)
        {
            using (_dbContext)
            {
                if (ToolService.DbType.Equals("mysql")){
                    var parameters = ids.Split(",").Select((id, index) => new MySqlParameter(string.Format("@p{0}", index), id)).ToList();
                    var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                    var sql = string.Format("UPDATE Dncchqpointdata SET Status=@Status WHERE id IN ({0})", parameterNames);
                    parameters.Add(new MySqlParameter("@Status", (int)status));
                    _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                    var response = ResponseModelFactory.CreateInstance;
                    return response;
                }else{
                    var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                    var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                    var sql = string.Format("UPDATE Dncchqpointdata SET Status=@Status WHERE id IN ({0})", parameterNames);
                    parameters.Add(new SqlParameter("@Status", (int)status));
                    _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                    var response = ResponseModelFactory.CreateInstance;
                    return response;
                }
                
            }
        }

        /// <summary>
        /// 批量操作
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ids">ID,多个以逗号分隔</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult Batch(string command, string ids)
        {
            var response = ResponseModelFactory.CreateInstance;
            switch (command)
            {
                case "delete":
                    response = UpdateIsDelete(CommonEnum.IsDeleted.Yes, ids);
                    break;
                case "recover":
                    response = UpdateIsDelete(CommonEnum.IsDeleted.No, ids);
                    break;
                case "forbidden":
                    response = UpdateStatus(UserStatus.Forbidden, ids);
                    break;
                case "normal":
                    response = UpdateStatus(UserStatus.Normal, ids);
                    break;
                default:
                    break;
            }
            return Ok(response);
        }


        
        

        /// <summary>
        /// 批量创建
        /// </summary>
        [HttpPost]
        [ProducesResponseType(200)]
        public IActionResult BatchCreate(string fsts)
        {
            var response = ResponseModelFactory.CreateInstance;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    using (_dbContext)
                    {
                        KeyValuePair<string, List< DncchqpointdataCreateViewModel>> res = ValidateJson.Validation< DncchqpointdataCreateViewModel>(fsts);

                        if (res.Key.Equals("ok"))
                        {
                            List< DncchqpointdataCreateViewModel> arr = res.Value;
                            foreach ( DncchqpointdataCreateViewModel item in arr)
                            {

      
                                
                                
                                
                                
                                
                                
                                var entity = _mapper.Map< DncchqpointdataCreateViewModel, Dncchqpointdata>(item);
                                
                                entity.DncBoiler = _dbContext.Dncboiler.FirstOrDefault(x => x.K_Name_kw == item.DncBoiler_Name);
                                entity.DncChstatus = _dbContext.Dncchstatus.FirstOrDefault(x => x.K_Name_kw == item.DncChstatus_Name);
                                
                                entity.Status = CommonEnum.Status.Normal;
                                _dbContext.Dncchqpointdata.Add(entity);
                            }
                        }
                        else
                        {
                            response.SetFailed(res.Key + " 数据格式有误.");
                            return Ok(response);
                        }
                        _dbContext.SaveChanges();
                    }
                    // 如果所有的操作都执行成功，则Complete()会被调用来提交事务
                    // 如果发生异常，则不会调用它并回滚事务
                    scope.Complete();
                }
                response.SetSuccess();
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.SetFailed(ex.Message);
                return Ok(response);
            }
        }
        

    }
}









