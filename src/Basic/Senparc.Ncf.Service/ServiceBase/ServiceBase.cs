﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Senparc.CO2NET;
using Senparc.Ncf.Core.Enums;
using Senparc.Ncf.Core.Models;
using Senparc.Ncf.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Query;
using Senparc.Ncf.Core.Models.DataBaseModel;
using Senparc.Ncf.Core.MultiTenant;

namespace Senparc.Ncf.Service
{
    public class ServiceBase<T> : ServiceDataBase, IServiceBase<T> where T : class, IEntityBase// global::System.Data.Objects.DataClasses.EntityObject, new()
    {
        public IMapper Mapper { get; set; } //TODO: add in to Wapper

        public IRepositoryBase<T> RepositoryBase { get; set; }
        protected IServiceProvider _serviceProvider => base.ServiceProvider;

        public ServiceBase(IRepositoryBase<T> repo, IServiceProvider serviceProvider)
            : base(repo, serviceProvider)
        {
            //_serviceProvider = serviceProvider;
            RepositoryBase = repo;
            Mapper = _serviceProvider.GetService<IMapper>();//确保 Mapper 中有值
        }

        #region Insert & DetectChange

        /// <summary>
        /// 强制将实体设置为Modified状态
        /// </summary>
        /// <param name="obj"></param>
        public virtual void TryDetectChange(T obj)
        {
            if (!IsInsert(obj))
            {
                RepositoryBase.BaseDB.BaseDataContext.Entry(obj).State = EntityState.Modified;
            }
        }

        public virtual bool IsInsert(T obj)
        {
            return RepositoryBase.IsInsert(obj);
        }

        #endregion


        #region GetObject

        public virtual T GetObject(Expression<Func<T, bool>> where, params string[] includes)
        {
            return RepositoryBase.GetFirstOrDefaultObject(where, includes);
        }

        public virtual async Task<T> GetObjectAsync(Expression<Func<T, bool>> where, params string[] includes)
        {
            return await RepositoryBase.GetFirstOrDefaultObjectAsync(where, includes);
        }

        public virtual T GetObject<TIncludesProperty>(Expression<Func<T, bool>> where, Expression<Func<DbSet<T>, IIncludableQueryable<T, TIncludesProperty>>> includesNavigationPropertyPathFunc)
        {
            return RepositoryBase.GetFirstOrDefaultObject(where, includesNavigationPropertyPathFunc);
        }

        public virtual async Task<T> GetObjectAsync<TIncludesProperty>(Expression<Func<T, bool>> where, Expression<Func<DbSet<T>, IIncludableQueryable<T, TIncludesProperty>>> includesNavigationPropertyPathFunc)
        {
            return await RepositoryBase.GetFirstOrDefaultObjectAsync(where, includesNavigationPropertyPathFunc);
        }

        public virtual T GetObject<TK>(Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, params string[] includes)
        {
            return RepositoryBase.GetFirstOrDefaultObject(where, orderBy, orderingType, includes);
        }

        public virtual async Task<T> GetObjectAsync<TK>(Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, params string[] includes)
        {
            return await RepositoryBase.GetFirstOrDefaultObjectAsync(where, orderBy, orderingType, includes);
        }

        public virtual T GetObject<TK, TIncludesProperty>(Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, Expression<Func<DbSet<T>, IIncludableQueryable<T, TIncludesProperty>>> includesNavigationPropertyPathFunc)
        {
            return RepositoryBase.GetFirstOrDefaultObject<TK, TIncludesProperty>(where, orderBy, orderingType, includesNavigationPropertyPathFunc);
        }

        public virtual async Task<T> GetObjectAsync<TK, TIncludesProperty>(Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, Expression<Func<DbSet<T>, IIncludableQueryable<T, TIncludesProperty>>> includesNavigationPropertyPathFunc)
        {
            return await RepositoryBase.GetFirstOrDefaultObjectAsync(where, orderBy, orderingType, includesNavigationPropertyPathFunc);
        }

        #endregion

        #region GetFullList

        public virtual PagedList<T> GetFullList<TK>(Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, params string[] includes)
        {
            return this.GetObjectList(0, 0, where, orderBy, orderingType, includes);
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public virtual async Task<PagedList<T>> GetFullListAsync<TK>(Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, params string[] includes)
        {
            return await this.GetObjectListAsync(0, 0, where, orderBy, orderingType, includes);
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderField">xxx desc, yyy asc</param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public virtual async Task<PagedList<T>> GetFullListAsync(Expression<Func<T, bool>> where, string orderField = null, params string[] includes)
        {
            return await RepositoryBase.GetObjectListAsync(where, orderField, 0, 0, includes);
        }

        public virtual PagedList<T> GetFullList<TK, TIncludesProperty>(Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, Expression<Func<DbSet<T>, IIncludableQueryable<T, TIncludesProperty>>> includesNavigationPropertyPathFunc)
        {
            return this.GetObjectList<TK, TIncludesProperty>(0, 0, where, orderBy, orderingType, includesNavigationPropertyPathFunc);
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TIncludesProperty"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderingType"></param>
        /// <param name="includesNavigationPropertyPathFunc"></param>
        /// <returns></returns>
        public virtual async Task<PagedList<T>> GetFullListAsync<TK, TIncludesProperty>(Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, Expression<Func<DbSet<T>, IIncludableQueryable<T, TIncludesProperty>>> includesNavigationPropertyPathFunc)
        {
            return await RepositoryBase.GetObjectListAsync(where, orderBy, orderingType, 0, 0, includesNavigationPropertyPathFunc);
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TIncludesProperty"></typeparam>
        /// <param name="where"></param>
        /// <param name="orderField">xxx desc, yyy asc</param>
        /// <param name="includesNavigationPropertyPathFunc"></param>
        /// <returns></returns>
        public virtual async Task<PagedList<T>> GetFullListAsync<TIncludesProperty>(Expression<Func<T, bool>> where, Expression<Func<DbSet<T>, IIncludableQueryable<T, TIncludesProperty>>> includesNavigationPropertyPathFunc, string orderField = null)
        {
            return await RepositoryBase.GetObjectListAsync(where, orderField, 0, 0, includesNavigationPropertyPathFunc);
        }

        #endregion

        #region GetObjectList（分页）

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <param name="where"></param>
        /// <param name="orderBy"></param>
        /// <param name="orderingType"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public virtual PagedList<T> GetObjectList<TK>(int pageIndex, int pageCount, Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, params string[] includes)
        {
            return RepositoryBase.GetObjectList(where, orderBy, orderingType, pageIndex, pageCount, includes);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageCount">每页数量</param>
        /// <param name="where">条件</param>
        /// <param name="orderBy">排序字段</param>
        /// <param name="orderingType">正序|倒叙</param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public virtual async Task<PagedList<T>> GetObjectListAsync<TK>(int pageIndex, int pageCount, Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, params string[] includes)
        {
            return await RepositoryBase.GetObjectListAsync(where, orderBy, orderingType, pageIndex, pageCount, includes);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageCount">每页数量</param>
        /// <param name="where">条件</param>
        /// <param name="orderBy">排序字段 eg.(xxx desc, bbb aec),默认升序</param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public virtual async Task<PagedList<T>> GetObjectListAsync(int pageIndex, int pageCount, Expression<Func<T, bool>> where, string orderBy, params string[] includes)
        {
            return await RepositoryBase.GetObjectListAsync(where, orderBy, pageIndex, pageCount, includes);
        }

        public virtual PagedList<T> GetObjectList<TK, TIncludesProperty>(int pageIndex, int pageCount, Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, Expression<Func<DbSet<T>, IIncludableQueryable<T, TIncludesProperty>>> includesNavigationPropertyPathFunc)
        {
            return RepositoryBase.GetObjectList<TK, TIncludesProperty>(where, orderBy, orderingType, pageIndex, pageCount, includesNavigationPropertyPathFunc);
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageCount">每页数量</param>
        /// <param name="where">条件</param>
        /// <param name="orderBy">排序字段 eg.(xxx desc, bbb aec),默认升序</param>
        /// <param name="includesNavigationPropertyPathFunc"></param>
        /// <returns></returns>
        public virtual async Task<PagedList<T>> GetObjectListAsync<TIncludesProperty>(int pageIndex, int pageCount, Expression<Func<T, bool>> where, string orderBy, Expression<Func<DbSet<T>, IIncludableQueryable<T, TIncludesProperty>>> includesNavigationPropertyPathFunc)
        {
            return await RepositoryBase.GetObjectListAsync<TIncludesProperty>(where, orderBy, pageIndex, pageCount, includesNavigationPropertyPathFunc);
        }


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageCount">每页数量</param>
        /// <param name="where">条件</param>
        /// <param name="orderBy">排序字段</param>
        /// <param name="orderingType">正序|倒叙</param>
        /// <param name="includesNavigationPropertyPathFunc"></param>
        /// <returns></returns>
        public virtual async Task<PagedList<T>> GetObjectListAsync<TK, TIncludesProperty>(int pageIndex, int pageCount, Expression<Func<T, bool>> where, Expression<Func<T, TK>> orderBy, OrderingType orderingType, Expression<Func<DbSet<T>, IIncludableQueryable<T, TIncludesProperty>>> includesNavigationPropertyPathFunc)
        {
            return await RepositoryBase.GetObjectListAsync<TK, TIncludesProperty>(where, orderBy, orderingType, pageIndex, pageCount, includesNavigationPropertyPathFunc);
        }


        #endregion

        #region GetCount

        public virtual int GetCount(Expression<Func<T, bool>> where, params string[] includes)
        {
            return RepositoryBase.ObjectCount(where, includes);
        }

        public virtual async Task<int> GetCountAsync(Expression<Func<T, bool>> where, params string[] includes)
        {
            return await RepositoryBase.ObjectCountAsync(where, includes);
        }

        public virtual int GetCount<TIncludesProperty>(Expression<Func<T, bool>> where, Expression<Func<DbSet<T>, IIncludableQueryable<T, TIncludesProperty>>> includesNavigationPropertyPathFunc)
        {
            return RepositoryBase.ObjectCount(where, includesNavigationPropertyPathFunc);
        }

        public virtual async Task<int> GetCountAsync<TIncludesProperty>(Expression<Func<T, bool>> where, Expression<Func<DbSet<T>, IIncludableQueryable<T, TIncludesProperty>>> includesNavigationPropertyPathFunc)
        {
            return await RepositoryBase.ObjectCountAsync(where, includesNavigationPropertyPathFunc);
        }

        #endregion

        #region GetSum

        public virtual decimal GetSum(Expression<Func<T, bool>> where, Expression<Func<T, decimal>> sum, params string[] includes)
        {
            return RepositoryBase.GetSum(where, sum, includes);
        }


        public virtual async Task<decimal> GetSumAsync(Expression<Func<T, bool>> where, Expression<Func<T, decimal>> sum, params string[] includes)
        {
            return await RepositoryBase.GetSumAsync(where, sum, includes);
        }

        public virtual async Task<decimal> GetSumAsync<TIncludesProperty>(Expression<Func<T, bool>> where, Expression<Func<T, decimal>> sum, Expression<Func<DbSet<T>, IIncludableQueryable<T, TIncludesProperty>>> includesNavigationPropertyPathFunc)
        {
            return await RepositoryBase.GetSumAsync(where, sum, includesNavigationPropertyPathFunc);
        }


        #endregion

        #region SaveObject & SaveChanges

        public virtual void SaveObject(T obj)
        {
            if (RepositoryBase.BaseDB.ManualDetectChangeObject)
            {
                TryDetectChange(obj);
            }

            RepositoryBase.Save(obj);

            AfterSaveObject?.Invoke(this.BaseData, obj);
        }

        public virtual async Task SaveObjectAsync(T obj)
        {
            if (RepositoryBase.BaseDB.ManualDetectChangeObject)
            {
                TryDetectChange(obj);
            }

            await RepositoryBase.SaveAsync(obj).ConfigureAwait(false);

            AfterSaveObject?.Invoke(this.BaseData, obj);
        }

        public virtual async Task SaveObjectListAsync(IEnumerable<T> objs)
        {
            await RepositoryBase.SaveObjectListAsync(objs);

            foreach (var item in objs)
            {
                AfterSaveObject?.Invoke(this.BaseData, item);
            }
        }

        public virtual void SaveChanges()
        {
            RepositoryBase.SaveChanges();

            AfterSaveChanges?.Invoke(this.BaseData);
        }

        public virtual async Task SaveChangesAsync()
        {
            await RepositoryBase.SaveChangesAsync().ConfigureAwait(false);

            AfterSaveChanges?.Invoke(this.BaseData);
        }


        #endregion

        #region Delete


        public virtual void DeleteObject(Expression<Func<T, bool>> predicate)
        {
            T obj = GetObject(predicate);

            DeleteObject(obj);

            AfterDeleteObject?.Invoke(this.BaseData, obj);
        }

        public virtual async Task DeleteObjectAsync(Expression<Func<T, bool>> predicate)
        {
            T obj = await GetObjectAsync(predicate);

            await DeleteObjectAsync(obj);

            AfterDeleteObject?.Invoke(this.BaseData, obj);
        }


        public virtual void DeleteObject(T obj)
        {
            RepositoryBase.Delete(obj);

            AfterDeleteObject?.Invoke(this.BaseData, obj);
        }

        public virtual async Task DeleteObjectAsync(T obj)
        {
            await RepositoryBase.DeleteAsync(obj, true);

            AfterDeleteObject?.Invoke(this.BaseData, obj);
        }

        public virtual void DeleteAll(IEnumerable<T> objects)
        {
            var list = objects.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                DeleteObject(list[i]);
            }
        }

        public virtual async Task DeleteAllAsync(Expression<Func<T, bool>> where, Action<T> deleteItemAction = null, bool softDelete = false)
        {
            var list = await GetFullListAsync(where);
            await RepositoryBase.DeleteAllAsync(list, deleteItemAction, softDelete);

            foreach (var obj in list)
            {
                AfterDeleteObject?.Invoke(this.BaseData, obj);
            }
        }

        public virtual async Task DeleteAllAsync(IEnumerable<T> objects, Action<T> deleteItemAction = null, bool softDelete = false)
        {
            await RepositoryBase.DeleteAllAsync(objects, deleteItemAction, softDelete);

            foreach (var obj in objects)
            {
                AfterDeleteObject?.Invoke(this.BaseData, obj);
            }
        }

        #endregion

        #region Transaction

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public async Task BeginTransactionAsync()
        {
            await RepositoryBase.BeginTransactionAsync();
        }

        /// <summary>
        /// 开启事务, 此方法回自动提交事务，失败则回滚
        /// </summary>
        /// <returns></returns>
        public async Task BeginTransactionAsync(Action action)
        {
            await RepositoryBase.BeginTransactionAsync();
            try
            {
                action();
                CommitTransaction();
            }
            catch (Exception)
            {
                RollbackTransaction();
                throw;
            }
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public void BeginTransaction()
        {
            RepositoryBase.BeginTransaction();
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public void BeginTransaction(Action body, Action<Exception> rollbackAction = null)
        {
            BeginTransaction();
            try
            {
                body();
                CommitTransaction();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                rollbackAction?.Invoke(ex);
                throw;
            }
        }


        /// <summary>
        /// 开启事务, 此方法回自动提交事务，失败则回滚
        /// </summary>
        /// <returns></returns>
        public async Task BeginTransactionAsync(Func<Task> body, Action<Exception> rollbackAction = null)
        {
            await BeginTransactionAsync();
            try
            {
                await body();
                CommitTransaction();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                rollbackAction?.Invoke(ex);
                throw ex;
            }
        }


        /// <summary>
        /// 开启事务, 此方法会自动提交事务，失败则回滚
        /// </summary>
        /// <param name="body"></param>
        /// <param name="rollbackAction">处理一个异常并抛出自定义的异常</param>
        /// <returns></returns>
        public async Task BeginTransactionAsync(Func<Task> body, Func<Exception, Exception> rollbackAction)
        {
            await BeginTransactionAsync();
            try
            {
                await body();
                CommitTransaction();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                throw rollbackAction?.Invoke(ex) ?? ex;
            }
        }


        /// <summary>
        /// 开启事务, 此方法会自动提交事务，失败则回滚
        /// </summary>
        /// <param name="body"></param>
        /// <param name="rollbackAction">处理一个异常并抛出自定义的异常</param>
        /// <returns></returns>
        public async Task BeginTransactionAsync(Func<Task> bodyAsync, Func<Exception, Task<Exception>> rollbackActionAsync)
        {
            await BeginTransactionAsync();
            try
            {
                await bodyAsync();
                CommitTransaction();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                throw await rollbackActionAsync?.Invoke(ex) ?? ex;
            }
        }


        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <returns></returns>
        public void RollbackTransaction()
        {
            RepositoryBase.RollbackTransaction();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <returns></returns>
        public void CommitTransaction()
        {
            RepositoryBase.CommitTransaction();
        }

        #endregion

        #region Mapping

        /// <summary>
        /// 使用 Mapper.Map&lt;TDto&gt;(entity) 快速返回
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TDto Mapping<TDto>(T entity)
        {
            return Mapper.Map<TDto>(entity);
        }

        /// <summary>
        /// 将 PageList 转为 DTO 对象
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="pagedList"></param>
        /// <returns></returns>
        public PagedList<TDto> Mapping<TDto>(PagedList<T> pagedList)
        {
            var dtoList = pagedList.Select(Mapper.Map<TDto>).ToList();
            return new PagedList<TDto>(dtoList, pagedList.PageIndex, pagedList.PageCount, pagedList.TotalCount, pagedList.SkipCount);
        }

        #endregion

        #region Tenant

        /// <summary>
        /// 强制设置租户信息
        /// </summary>
        /// <param name="requestTenantInfo"></param>
        /// <returns></returns>
        public bool SetTenantInfo(RequestTenantInfo requestTenantInfo)
        {
            if (this.BaseData.BaseDB.BaseDataContext is ISenparcEntitiesDbContext senparcDB)
            {
                senparcDB.TenantInfo = requestTenantInfo;
                return true;
            }
            return false;
        }


        #endregion
    }
}