// ======================================================================
//  
//          Copyright (C) 2016-2020 湖南心莱信息科技有限公司    
//          All rights reserved
//  
//          filename : MultitenantIdentityDbContext.cs
//          description :
//  
//          created by 李文强 at  2016/09/25 20:50
//          Blog：http://www.cnblogs.com/codelove/
//          GitHub ： https://github.com/xin-lai
//          Home：http://xin-lai.com
//  
// ======================================================================

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Validation;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Magicodes.Data.Multitenant
{
    /// <summary>
    ///     多租户 <see cref="IdentityDbContext" />
    /// </summary>
    /// <typeparam name="TUser">MultitenantIdentityUser<TKey, TTenantKey, TUserLogin, TUserRole, TUserClaim>的派生类类型</typeparam>
    /// <typeparam name="TRole">IdentityRole<TKey, TUserRole>的派生类类型</typeparam>
    /// <typeparam name="TKey">TUser的主键<see cref="IUser.Id" /> 的类型</typeparam>
    /// <typeparam name="TTenantKey"><see cref="IMultitenantUser{TKey,TTenantKey}.TenantId" />的类型</typeparam>
    /// <typeparam name="TUserLogin">UserLogin类型</typeparam>
    /// <typeparam name="TUserRole">UserRole类型</typeparam>
    /// <typeparam name="TUserClaim">UserClaim类型</typeparam>
    public class MultitenantIdentityDbContext<TUser, TRole, TKey, TTenantKey, TUserLogin, TUserRole, TUserClaim>
        : IdentityDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>
        where TUser : MultitenantIdentityUser<TKey, TTenantKey, TUserLogin, TUserRole, TUserClaim>
        where TRole : IdentityRole<TKey, TUserRole>
        where TUserLogin : MultitenantIdentityUserLogin<TKey, TTenantKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserClaim : IdentityUserClaim<TKey>, new()
    {
        public MultitenantIdentityDbContext()
            : this("DefaultConnection")
        {
        }

        /// <summary>
        ///     初始化
        ///     <see cref="MultitenantIdentityDbContext{TUser, TRole, TKey, TTenantKey, TUserLogin, TUserRole, TUserClaim}" />
        ///     实例
        /// </summary>
        /// <param name="nameOrConnectionString">连接字符串名称或者连接字符串</param>
        public MultitenantIdentityDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        /// <summary>
        ///     自定义验证
        /// </summary>
        /// <param name="entityEntry">验证实体：<see cref="DbEntityEntry" /></param>
        /// <param name="items">
        ///     自定义验证
        /// </param>
        /// <returns>实体验证结果</returns>
        protected override DbEntityValidationResult ValidateEntity(
            DbEntityEntry entityEntry,
            IDictionary<object, object> items)
        {
            if ((entityEntry == null) || (entityEntry.State != EntityState.Added))
                return base.ValidateEntity(entityEntry, items);
            var user = entityEntry.Entity as TUser;
            return user != null ? new DbEntityValidationResult(entityEntry, Enumerable.Empty<DbValidationError>()) : base.ValidateEntity(entityEntry, items);
        }

        /// <summary>
        ///     应用自定义模型映射
        /// </summary>
        /// <param name="modelBuilder">模型架构映射定义</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUserLogin>()
                .HasKey(e => new {e.TenantId, e.LoginProvider, e.ProviderKey, e.UserId});
            
            //modelBuilder.Entity<TUser>()
            //    .Property(u => u.UserName)
            //    .HasColumnAnnotation(
            //        "Index",
            //        new IndexAnnotation(new IndexAttribute("UserNameIndex", 1)
            //        {
            //            IsUnique = true
            //        }));

            //modelBuilder.Entity<TUser>()
            //   .Property(e => e.TenantId)
            //   .IsRequired()
            //   .HasColumnAnnotation(
            //       "Index",
            //       new IndexAnnotation(new IndexAttribute("UserNameIndex", order: 0)
            //       {
            //           IsUnique = true
            //       }));
        }
    }
}