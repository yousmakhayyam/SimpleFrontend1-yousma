using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Data.Repositories;

public class BaseRepository
{
    public BaseRepository(DomainDbContext context)
    {
            Context = context;
        }

    public DomainDbContext Context { get; set; }
}