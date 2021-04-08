using Com.Atomatus.Bootstarter.Model;
using System;
using System.Collections.Generic;

namespace Com.Atomatus.Bootstarter.Services
{
    public interface IServiceCrud<TEntity, ID> : IService<TEntity, ID>
        where TEntity : IModel<ID>
    {
        #region [C]reate
        TEntity Insert(TEntity entity);
        #endregion

        #region [R]ead
        bool Exists(Guid uuid);
        
        bool Exists(TEntity e);

        TEntity Get(ID id);
        
        TEntity GetByUuid(Guid uuid);

        List<TEntity> PagingIndex(int index, int count);

        List<TEntity> Paging(int page = 0, int limit = -1);

        List<TEntity> List();
        #endregion

        #region [U]pdate
        TEntity Update(TEntity entity);
        #endregion

        #region [D]elete
        int Delete(IEnumerable<Guid> uuid);

        int Delete(params Guid[] uuid);

        bool Delete(Guid uuid);

        bool Delete(IEnumerable<TEntity> entity);

        bool Delete(params TEntity[] entity);
        #endregion
    }
}
