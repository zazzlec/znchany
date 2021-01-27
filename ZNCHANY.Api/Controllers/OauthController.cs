/******************************************
 * AUTHOR:          Rector
 * CREATEDON:       2018-09-26
 * OFFICIAL_SITE:    码友网(https://codedefault.com)--专注.NET/.NET Core
 * 版权所有，请勿删除
 ******************************************/

using ZNCHANY.Api.Entities;
using ZNCHANY.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using ZNCHANY.Api.Auth;
using static ZNCHANY.Api.Entities.Enums.CommonEnum;
using ZNCHANY.Api.Utils;

namespace ZNCHANY.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OauthController : ControllerBase
    {
        private readonly AppAuthenticationSettings _appSettings;
        private readonly ZNCHANYDbContext _dbContext;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appSettings"></param>
        public OauthController(IOptions<AppAuthenticationSettings> appSettings, ZNCHANYDbContext dbContext)
        {
            _appSettings = appSettings.Value;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Auth(string username, string password)
        {
            var response = ResponseModelFactory.CreateInstance;
            DncUser user;
            try
            {
                using (_dbContext)
                {
                    user = _dbContext.DncUser.FirstOrDefault(x => x.LoginName == username.Trim());
                    if (user == null || user.IsDeleted == IsDeleted.Yes)
                    {
                        response.SetFailed("用户不存在");
                        return Ok(response);
                    }
                    password = CryptoAES.DecryptByAES(password.Trim(), "11111111111111111111111111111112");
                    password = CryptoClass.RSAEncryption(password.Trim());
                    if (user.Password != password.Trim())
                    {
                        response.SetFailed("密码不正确");
                        return Ok(response);
                    }
                    if (user.IsLocked == IsLocked.Locked)
                    {
                        response.SetFailed("账号已被锁定");
                        return Ok(response);
                    }
                    if (user.Status == UserStatus.Forbidden)
                    {
                        response.SetFailed("账号已被禁用");
                        return Ok(response);
                    }
                    DncLog log = ToolService.Log(user.DisplayName + ",登录系统", user.LoginName, 1);
                    _dbContext.DncLog.Add(log);
                }
                var claimsIdentity = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("guid",user.Guid.ToString()),
                    new Claim("avatar",""),
                    new Claim("displayName",user.DisplayName),
                    new Claim("loginName",user.LoginName),
                    new Claim("emailAddress",""),
                    new Claim("guid",user.Guid.ToString()),
                    new Claim("userType",((int)user.UserType).ToString())
                    });
                var token = JwtBearerAuthenticationExtension.GetJwtAccessToken(_appSettings, claimsIdentity);


                

                response.SetData(token);
                return Ok(response);
            }
            catch (System.Exception ee)
            {

                response.SetError(ee.Message);
                return Ok(response);
            }
            
        }
    }
}