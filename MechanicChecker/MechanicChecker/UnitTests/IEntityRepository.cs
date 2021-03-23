using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker.UnitTests
{
    public interface IEntityRepository<T> where T:class,new()
    {
        void Insert(T entity);
        IQueryable<T> GetAllQueryAble();
    }
}
