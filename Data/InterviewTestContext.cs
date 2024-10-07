using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using InterviewTest.Model;

namespace InterviewTest.Data
{
    public class InterviewTestContext : DbContext
    {
        public InterviewTestContext (DbContextOptions<InterviewTestContext> options)
            : base(options)
        {
        }

        public DbSet<InterviewTest.Model.Employee> Employee { get; set; } = default!;
    }
}
