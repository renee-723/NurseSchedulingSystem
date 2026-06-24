using NurseSchedulingSystem.Data;

namespace NurseSchedulingSystem.Services
{
    public class ShiftService
    {
        private readonly AppDbContext _context;
        public ShiftService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> IsShiftValid(Entities.ShiftSchedule schedule)
        {
            // 先檢查一下護理師存不存在
            var nurse = await _context.Nurses.FindAsync(schedule.NurseId);
            return nurse != null;
        }
    }
}
