using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data
{
    public class DataDbContext : IdentityDbContext<IdentityUser>
    {
        public DataDbContext(DbContextOptions<DataDbContext> options)
            : base(options)
        {
        }
    }
}