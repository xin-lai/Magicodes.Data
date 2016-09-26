// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : IMultitenantUser.cs
//          description :
//  
//          created by 李文强 at  2016/09/25 20:50
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using Microsoft.AspNet.Identity;

namespace Magicodes.Data.Multitenant
{
    /// <summary>
    ///     定义多租户接口
    /// </summary>
    /// <typeparam name="TKey">用户Id类型 <see cref="IUser.Id" /> </typeparam>
    /// <typeparam name="TTenantKey">租户Id类型<see cref="TenantId" /></typeparam>
    public interface IMultitenantUser<out TKey, TTenantKey> : IUser<TKey>
    {
        /// <summary>
        ///     多租户Id
        /// </summary>
        TTenantKey TenantId { get; set; }
    }
}