// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MultitenantUserStore.cs
//          description :
//  
//          created by 李文强 at  2016/09/25 20:50
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Magicodes.Data.Multitenant
{
    /// <summary>
    ///     多租户UserStore
    /// </summary>
    /// <typeparam name="TUser">user类型</typeparam>
    /// <typeparam name="TRole">role类型</typeparam>
    /// <typeparam name="TKey">用户<see cref="IUser.Id" />类型</typeparam>
    /// <typeparam name="TTenantKey">用户 <see cref="IMultitenantUser{TKey,TTenantKey}.TenantId" /> 类型</typeparam>
    /// <typeparam name="TUserLogin">UserLogin类型</typeparam>
    /// <typeparam name="TUserRole">UserRole类型</typeparam>
    /// <typeparam name="TUserClaim">UserClaim类型</typeparam>
    public class MultitenantUserStore<TUser, TRole, TKey, TTenantKey, TUserLogin, TUserRole, TUserClaim>
        : UserStore<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>
        where TUser : MultitenantIdentityUser<TKey, TTenantKey, TUserLogin, TUserRole, TUserClaim>
        where TRole : IdentityRole<TKey, TUserRole>
        where TKey : IEquatable<TKey>
        where TTenantKey : IEquatable<TTenantKey>
        where TUserLogin : MultitenantIdentityUserLogin<TKey, TTenantKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserClaim : IdentityUserClaim<TKey>, new()
    {
        /// <summary>
        ///     标记该对象是否已被释放
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     <see cref="Logins" />
        /// </summary>
        private DbSet<TUserLogin> _logins;

        /// <summary>
        ///     初始化
        ///     <see cref="MultitenantUserStore{TUser, TRole, TKey, TTenantKey, TUserLogin, TUserRole, TUserClaim}" /> 的实例
        /// </summary>
        /// <param name="context">
        ///     <see cref="DbContext" />
        /// </param>
        public MultitenantUserStore(DbContext context)
            : base(context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
        }

        /// <summary>
        ///     获取或设置<see cref="IMultitenantUser{TKey, TTenantKey}.TenantId" />以在查询时使用
        /// </summary>
        public virtual TTenantKey TenantId { get; set; }

        /// <summary>
        ///     获取TUserLogin的实体集合
        /// </summary>
        private DbSet<TUserLogin> Logins
        {
            get { return _logins ?? (_logins = Context.Set<TUserLogin>()); }
        }

        /// <summary>
        ///     创建账号
        /// </summary>
        /// <param name="user">待创建的用户信息</param>
        /// <returns>
        ///     <see cref="Task" />
        /// </returns>
        public override Task CreateAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            ThrowIfInvalid();

            user.TenantId = TenantId;

            return base.CreateAsync(user);
        }

        /// <summary>
        ///     根据<see cref="IUser{TKey}.UserName" /> 查询 <typeparamref name="TUser" />
        /// </summary>
        /// <param name="userName">The <see cref="IUser{TKey}.UserName" /> of a <typeparamref name="TUser" />.</param>
        /// <returns>The <typeparamref name="TUser" /> if found; otherwise <c>null</c>.</returns>
        public override Task<TUser> FindByNameAsync(string userName)
        {
            return EqualityComparer<TTenantKey>.Default.Equals(TenantId, default(TTenantKey))
                ? GetUserAggregateAsync(u => u.UserName == userName)
                : GetUserAggregateAsync(u => (u.UserName == userName) && u.TenantId.Equals(TenantId));
        }

        /// <summary>
        ///     Adds the external login for the <paramref name="user" />.
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <param name="login">登录信息</param>
        /// <returns>An <see cref="Task" /> from which the operation can be awaited.</returns>
        public override Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (login == null)
                throw new ArgumentNullException("login");

            ThrowIfInvalid();

            var userLogin = new TUserLogin
            {
                TenantId = TenantId,
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider
            };

            user.Logins.Add(userLogin);
            return Task.FromResult(0);
        }

        /// <summary>
        ///     根据登录信息获取用户
        /// </summary>
        /// <param name="login">登录信息</param>
        /// <returns>如果存在，则返回 <typeparamref name="TUser" /> ，否则返回<c>null</c></returns>
        public override async Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
                throw new ArgumentNullException("login");

            ThrowIfInvalid();

            var userId = await
                (from l in Logins
                        where (l.LoginProvider == login.LoginProvider)
                              && (l.ProviderKey == login.ProviderKey)
                              && l.TenantId.Equals(TenantId)
                        select l.UserId)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

            if (EqualityComparer<TKey>.Default.Equals(userId, default(TKey)))
                return null;

            return await GetUserAggregateAsync(u => u.Id.Equals(userId));
        }

        /// <summary>
        ///     通过邮箱查找账户
        /// </summary>
        /// <param name="email"><typeparamref name="TUser" />邮箱</param>
        /// <returns>如果存在，则返回 <typeparamref name="TUser" /> ，否则返回<c>null</c></returns>
        public override Task<TUser> FindByEmailAsync(string email)
        {
            ThrowIfInvalid();
            return GetUserAggregateAsync(u => (u.Email.ToUpper() == email.ToUpper()) && u.TenantId.Equals(TenantId));
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            _disposed = true;
            _logins = null;
        }

        /// <summary>
        ///     如果验证未通过，则抛出异常
        /// </summary>
        private void ThrowIfInvalid()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            if (EqualityComparer<TTenantKey>.Default.Equals(TenantId, default(TTenantKey)))
                throw new InvalidOperationException("TenantId必须赋值！");
        }
    }
}