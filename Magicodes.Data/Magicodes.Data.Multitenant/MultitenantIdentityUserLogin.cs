// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MultitenantIdentityUserLogin.cs
//          description :
//  
//          created by 李文强 at  2016/09/25 20:50
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using Microsoft.AspNet.Identity.EntityFramework;

namespace Magicodes.Data.Multitenant
{
    /// <summary>
    ///     多租户UserLogin定义
    /// </summary>
    /// <typeparam name="TKey"> <see cref="IdentityUserLogin.UserId" /> 类型</typeparam>
    /// <typeparam name="TTenantKey"><see cref="TenantId" /> 类型</typeparam>
    public class MultitenantIdentityUserLogin<TKey, TTenantKey> : IdentityUserLogin<TKey>
    {
        /// <summary>
        ///     获取或设置多租户唯一标示Id.
        /// </summary>
        public virtual TTenantKey TenantId { get; set; }
    }
}