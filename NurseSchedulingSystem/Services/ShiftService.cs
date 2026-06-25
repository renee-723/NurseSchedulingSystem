using Microsoft.EntityFrameworkCore;
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
        public async Task<bool> IsConsecutiveDaysExceeded(int nurseId,DateTime targetDate)
        {
            //往日期往回推五天
            var startDate = targetDate.AddDays(-5);

            var count = await _context.ShiftSchedules
                .Where(s => s.NurseId == nurseId  //只撈該護理師的資料
                        && s.Date >= startDate
                        && s.Date < targetDate)   //只撈「目標日之前」的那 5 天資料（不包含目標日當天）
                .CountAsync();   //統計這五天內資料庫裡有幾筆班表
            return count >= 5;   //如果超過5(回傳true)代表已超過連續上班上限
        }
    }
}
