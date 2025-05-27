using Events.Domain.Entities;
using Events.Domain.Repositories;
using Events.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Infrastructure.Repositories
{
    public class CategoryRepository
        : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context)
            : base(context) { }
    }
}
