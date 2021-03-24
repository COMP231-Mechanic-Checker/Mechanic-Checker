using MechanicChecker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker.UnitTests
{
    public class LocalProductDbContext:DbContext
    {
        public LocalProductDbContext(DbContextOptions options) : base(options)
        {

        }
        DbSet<LocalProduct> product { get; set; }
    }
}
