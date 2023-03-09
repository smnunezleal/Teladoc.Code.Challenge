using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Teladoc.Code.Challenge.Models;

namespace Teladoc.Code.Challenge.Context
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
        : base(options)
        {
        }
        public DbSet<User> Users { get; set; }


        

    }
}
