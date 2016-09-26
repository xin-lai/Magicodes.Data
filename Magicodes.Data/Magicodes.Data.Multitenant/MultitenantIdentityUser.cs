// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MultitenantIdentityUser.cs
//          description :
//  
//          created by 李文强 at  2016/09/25 20:50
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Magicodes.Data.Multitenant
{
    /// <summary>
    ///     多租户用户类
    /// </summary>
    /// <typeparam name="TKey">用户Id类型： <see cref="IUser.Id" /> </typeparam>
    /// <typeparam name="TTenantKey">租户Id类型： <see cref="IMultitenantUser{TKey, TTenantKey}.TenantId" /> </typeparam>
    /// <typeparam name="TLogin">用户登录类型</typeparam>
    /// <typeparam name="TRole">用户角色类型</typeparam>
    /// <typeparam name="TClaim">User claim类型</typeparam>
    public class MultitenantIdentityUser<TKey, TTenantKey, TLogin, TRole, TClaim>
        : IdentityUser<TKey, TLogin, TRole, TClaim>, IMultitenantUser<TKey, TTenantKey>
        where TLogin : MultitenantIdentityUserLogin<TKey, TTenantKey>
        where TRole : IdentityUserRole<TKey>
        where TClaim : IdentityUserClaim<TKey>
    {
        /// <summary>
        ///     租户Id
        /// </summary>
        [Display(Name = "租户")]
        public TTenantKey TenantId { get; set; }
    }
}