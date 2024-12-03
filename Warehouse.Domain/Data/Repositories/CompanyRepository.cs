using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Warehouse.Domain.Models.Entities;

namespace Warehouse.Domain.Data.Repositories;

public class CompanyRepository : BaseRepository
{
    public CompanyRepository(DomainDbContext context) : base(context)
    {
        }

    public async Task<Company?> GetById(Guid id)
    {
           return await Context.Companies
               .Include(c => c.Users)
               .Include(c => c.Address)
               .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<bool> CheckIfTaxIdentificationNumberIsInUse(string taxIdentificationNumber)
    {
            var companies = await Context.Companies
                .Where(r => r.TaxIdentificationNumber.Equals(taxIdentificationNumber)).ToListAsync();
            return companies.Any();
        }
}