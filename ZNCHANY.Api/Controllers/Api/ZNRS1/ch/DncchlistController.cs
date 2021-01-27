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
using ZNCHANY.Api.RequestPayload.Rbac.Chlist;
using ZNCHANY.Api.ViewModels.Rbac.Dncchlist;
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
    public class DncchlistController : ControllerBase
    {
        private readonly ZNCHANYDbContext _dbContext;
        private readonly IMapper _mapper;
        /// <summary>
        /// 构造control
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="mapper"></param>
        public DncchlistController(ZNCHANYDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }


        [HttpGet]
        public IActionResult List()
        {
            using (_dbContext)
            {
                var list = _dbContext.Dncchlist.ToList();
                list = list.FindAll(x => x.IsDeleted != CommonEnum.IsDeleted.Yes );
                var response = ResponseModelFactory.CreateInstance;
                response.SetData(list);
                return Ok(response);
            }
        }

        [HttpGet]
        public IActionResult RunPush(int bid,int type)
        {
            var response = ResponseModelFactory.CreateInstance;
            //ZNCHANY.Api.dataoperate.Chcompute.ZNCHANYrun(bid, DateTime.Now, type);
            response.SetSuccess("ok");
            return Ok(response);
        }
        /// <summary>
        /// 查询请求
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult List(DncchlistRequestPayload payload)
        {
            var response = ResponseModelFactory.CreateResultInstance;
            using (_dbContext)
            {
                var query = _dbContext.Dncchlist.Include(x=>x.DncChqpoint.DncChtype).AsQueryable();
                //模糊查询
                if (!string.IsNullOrEmpty(payload.Kw))
                {
                    query = query.Where(x =>   x.K_Name_kw.Contains(payload.Kw.Trim())  );
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

                if (payload.boilerid != -1)
                {
                    query = query.Where(x => x.DncBoilerId == payload.boilerid);
                }


                if (payload.FirstSort != null)
                {
                    query = query.OrderBy(payload.FirstSort.Field, payload.FirstSort.Direct == "DESC");
                }
                var list = query.Paged(payload.CurrentPage, payload.PageSize).ToList();
                var totalCount = query.Count();
                foreach (var item in list)
                {
                    item.Remarks = item.DncChqpoint.DncChtype.Ch_time_Val+"";
                }
                var data = list.Select(_mapper.Map< Dncchlist, DncchlistJsonModel>);

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
        public IActionResult Create(DncchlistCreateViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            if (model.K_Name_kw.Trim().Length <= 0)
            {
                response.SetFailed("请输入吹灰器描述");
                return Ok(response);
            }
            using (_dbContext)
            {
                //3 只有未删除的有重复才提示
                if (_dbContext.Dncchlist.Count(x => x.K_Name_kw == model.K_Name_kw && x.IsDeleted == CommonEnum.IsDeleted.No) > 0)
                {
                    response.SetFailed(model.K_Name_kw+"已存在");
                    return Ok(response);
                }
                //4 删除的有重复就真删掉
                if (_dbContext.Dncchlist.Count(x => x.K_Name_kw == model.K_Name_kw && x.IsDeleted == CommonEnum.IsDeleted.Yes) > 0)
                {
                    var entity2 = _dbContext.Dncchlist.FirstOrDefault(x => x.K_Name_kw == model.K_Name_kw  && x.IsDeleted == CommonEnum.IsDeleted.Yes);
                    _dbContext.Dncchlist.Remove(entity2);
                }
                var entity = _mapper.Map< DncchlistCreateViewModel, Dncchlist>(model);
                entity.DncChqpoint = _dbContext.Dncchqpoint.FirstOrDefault(x => (x.Name_kw + "") == model.DncChqpoint_Name);
                entity.DncChqpoint_Name = entity.DncChqpoint.Name_kw;
                entity.DncBoiler = _dbContext.Dncboiler.FirstOrDefault(x => (x.K_Name_kw + "") == model.DncBoiler_Name);
                entity.DncBoiler_Name = entity.DncBoiler.K_Name_kw;
                entity.Status = CommonEnum.Status.Normal;
                _dbContext.Dncchlist.Add(entity);
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
                var entity = _dbContext.Dncchlist.FirstOrDefault(x => x.Id ==  int.Parse(code));
                var response = ResponseModelFactory.CreateInstance;
                response.SetData(_mapper.Map< Dncchlist, DncchlistCreateViewModel>(entity));
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
        public IActionResult Edit(DncchlistEditViewModel model)
        {
            var response = ResponseModelFactory.CreateInstance;
            using (_dbContext)
            {


                var entity = _dbContext.Dncchlist.FirstOrDefault(x => x.Id == model.Id);


                
                //1 未删除的其他记录重复校验 K_Name_kw
                if (_dbContext.Dncchlist.Count(x => x.K_Name_kw == model.K_Name_kw && x.Id != model.Id && x.IsDeleted == CommonEnum.IsDeleted.No) > 0 )
                {
                    response.SetFailed(model.K_Name_kw + "已存在");
                    return Ok(response);
                }
                //2 已删除的其他记录重复的真删 K_Name_kw
                if (_dbContext.Dncchlist.Count(x => x.K_Name_kw == model.K_Name_kw && x.Id != model.Id && x.IsDeleted == CommonEnum.IsDeleted.Yes) > 0)
                {
                    var entity2 = _dbContext.Dncchlist.FirstOrDefault(x => x.K_Name_kw == model.K_Name_kw && x.Id != model.Id && x.IsDeleted == CommonEnum.IsDeleted.Yes);
                    _dbContext.Dncchlist.Remove(entity2);
                }
                



                entity.K_Name_kw = model.K_Name_kw;
                entity.AddTime = model.AddTime;
                entity.RunTime = model.RunTime;
                entity.Remarks = model.Remarks;
                entity.DncChqpoint = _dbContext.Dncchqpoint.FirstOrDefault(x => x.Name_kw == model.DncChqpoint_Name);
                entity.DncChqpoint_Name = entity.DncChqpoint.Name_kw;
                entity.DncChqpoint_Name = model.DncChqpoint_Name;
                entity.Wrl_Val = model.Wrl_Val;
                entity.Wrlhigh_Val = model.Wrlhigh_Val;
                entity.AddReason = model.AddReason;
                entity.DncBoiler = _dbContext.Dncboiler.FirstOrDefault(x => x.K_Name_kw == model.DncBoiler_Name);
                entity.DncBoiler_Name = entity.DncBoiler.K_Name_kw;
                entity.DncBoiler_Name = model.DncBoiler_Name;
                entity.Status = model.Status;
                entity.IsDeleted = model.IsDeleted;

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
                    var sql = string.Format("UPDATE Dncchlist SET IsDeleted=@IsDeleted WHERE id IN ({0})", parameterNames);
                    parameters.Add(new MySqlParameter("@IsDeleted", (int)isDeleted));
                    _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                    var response = ResponseModelFactory.CreateInstance;
                    return response;
                }else{
                    var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                    var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                    var sql = string.Format("UPDATE Dncchlist SET IsDeleted=@IsDeleted WHERE id IN ({0})", parameterNames);
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
                    var sql = string.Format("UPDATE Dncchlist SET Status=@Status WHERE id IN ({0})", parameterNames);
                    parameters.Add(new MySqlParameter("@Status", (int)status));
                    _dbContext.Database.ExecuteSqlCommand(sql, parameters);
                    var response = ResponseModelFactory.CreateInstance;
                    return response;
                }else{
                    var parameters = ids.Split(",").Select((id, index) => new SqlParameter(string.Format("@p{0}", index), id)).ToList();
                    var parameterNames = string.Join(", ", parameters.Select(p => p.ParameterName));
                    var sql = string.Format("UPDATE Dncchlist SET Status=@Status WHERE id IN ({0})", parameterNames);
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
                        KeyValuePair<string, List< DncchlistCreateViewModel>> res = ValidateJson.Validation< DncchlistCreateViewModel>(fsts);

                        if (res.Key.Equals("ok"))
                        {
                            List< DncchlistCreateViewModel> arr = res.Value;
                            foreach ( DncchlistCreateViewModel item in arr)
                            {

                                if (item.K_Name_kw.Trim().Length <= 0)
                                {
                                    response.SetFailed("请输入吹灰器描述");
                                    return Ok(response);
                                }
      
                                
                                
                                //5 只有未删除的有重复才提示
                                if (_dbContext.Dncchlist.Count(x => x.K_Name_kw == item.K_Name_kw && x.IsDeleted == CommonEnum.IsDeleted.No) > 0)
                                {
                                    response.SetFailed(item.K_Name_kw+"已存在");
                                    return Ok(response);
                                }
                                //6 删除的有重复就真删掉
                                if (_dbContext.Dncchlist.Count(x => x.K_Name_kw == item.K_Name_kw && x.IsDeleted == CommonEnum.IsDeleted.Yes) > 0)
                                {
                                    var entity2 = _dbContext.Dncchlist.FirstOrDefault(x => x.K_Name_kw == item.K_Name_kw  && x.IsDeleted == CommonEnum.IsDeleted.Yes);
                                    _dbContext.Dncchlist.Remove(entity2);
                                }
                                
                                
                                
                                
                                var entity = _mapper.Map< DncchlistCreateViewModel, Dncchlist>(item);
                                
                                entity.DncChqpoint = _dbContext.Dncchqpoint.FirstOrDefault(x => x.Name_kw == item.DncChqpoint_Name);
                                entity.DncBoiler = _dbContext.Dncboiler.FirstOrDefault(x => x.K_Name_kw == item.DncBoiler_Name);
                                
                                entity.Status = CommonEnum.Status.Normal;
                                _dbContext.Dncchlist.Add(entity);
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









