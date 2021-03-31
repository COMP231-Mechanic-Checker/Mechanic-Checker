using MechanicChecker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker.UnitTests
{
    public class LocalProductBL
    {
        private IEntityRepository<LocalProduct> _localRepository { get; set; }
        public LocalProductBL(IEntityRepository<LocalProduct> localrepos)
        {
            _localRepository = localrepos;
        }
        public List<LocalProduct> GetAllActiveProduts()
        {
            var result = new List<LocalProduct>();
            result = _localRepository.GetAllQueryAble().Where(s => s.IsVisible == false).ToList();
            return result;
        }
        public bool GetActiveProducts(LocalProduct product)
        {
            var isAdded = false;
            try
            {
                _localRepository.Insert(product);
                isAdded = true;
            }catch(Exception e)
            {
                isAdded = false;
            }
            return isAdded;
        }
    }
}
