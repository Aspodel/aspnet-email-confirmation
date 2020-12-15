using EmailConfirmation.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailConfirmation.Api.Database
{
    public class EmailConfirmationContext : IdentityDbContext<User>
    {
        public EmailConfirmationContext(DbContextOptions<EmailConfirmationContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
