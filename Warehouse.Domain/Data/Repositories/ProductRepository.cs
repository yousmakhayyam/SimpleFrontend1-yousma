using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Models.Entities;

namespace Warehouse.Domain.Data.Repositories;

public class ProductRepository : BaseRepository
{
    public ProductRepository(DomainDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetById(Guid id)
    {
        return Context.Products.Include(p => p.Images).FirstOrDefault(r => r.Id == id);
    }
}