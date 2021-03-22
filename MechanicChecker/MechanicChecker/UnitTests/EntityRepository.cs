using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker.UnitTests
{
    public class EntityRepository<T> : IEntityRepository<T> where T : class, new()
    {
        private LocalProductDbContext _dbcontext;
        private DbSet<T> _dbset;

        public EntityRepository(LocalProductDbContext dbcontext)
        {
            _dbcontext = dbcontext;
            _dbset = _dbcontext.Set<T>();
        }
        public IQueryable<T> GetAllQueryAble()
        {
            return _dbset;
        }

        public void Insert(T entity)
        {
            _dbset.Add(entity);
            _dbcontext.SaveChanges();
        }
    }

}
