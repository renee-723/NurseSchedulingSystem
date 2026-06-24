using Microsoft.EntityFrameworkCore; 
using NurseSchedulingSystem.Entities; 

namespace NurseSchedulingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // 這裡就像是「檔案櫃」，宣告我們有哪些表格要管理
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<ShiftSchedule> ShiftSchedules { get; set; }
    }
}
