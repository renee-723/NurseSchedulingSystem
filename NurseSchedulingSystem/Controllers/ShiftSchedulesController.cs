using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NurseSchedulingSystem.Data;
using NurseSchedulingSystem.DTOs;
using NurseSchedulingSystem.Entities;
using NurseSchedulingSystem.Services;

namespace NurseSchedulingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftSchedulesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ShiftService _shiftService;
        public ShiftSchedulesController(ShiftService shiftService, AppDbContext context)
        {
            _shiftService = shiftService;
            _context = context;
        }

        //新增一筆班表
        [HttpPost]
        public async Task<IActionResult> CreatSchedule(ShiftSchedule schedule)
        {
            //先檢查這位護理師是否存在
            var nurse = await _context.Nurses.FindAsync(schedule.NurseId);
            if (nurse == null)
            {
                return BadRequest("找不到護理師");
            }
            if ((schedule.ShiftType == "E" || schedule.ShiftType == "N") && nurse.Role != "N3")
            {
                return BadRequest($"班別{schedule.ShiftType}要求N3以上職級!");
            }
            if (await _shiftService.IsConsecutiveDaysExceeded(schedule.NurseId, schedule.Date))
            {
                return BadRequest("該護理師已連續上班5天，必須休息");
            }
            // 將資料庫中撈到的護理師資訊關聯給班表
            //schedule.Nurse = nurse;
            _context.ShiftSchedules.Add(schedule);
            await _context.SaveChangesAsync();
            return Ok(schedule);
        }
        [HttpPost("Batch")]
        public async Task<IActionResult> BatchCreate([FromBody]BatchScheduleRequest request)
        {
            // 使用 EF Core 的 Transaction 機制，確保「要嘛全部成功，要嘛一筆都不存」
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach(var item in request.Schedules)
                {
                    // 檢查邏輯：複製已經寫好的檢查規則
                    if (await _shiftService.IsConsecutiveDaysExceeded(item.NurseId, item.Date))
                    {
                        //如果違規，直接取消所有異動
                        await transaction.RollbackAsync();
                        return BadRequest($"排班失敗:護理師{item.NurseId}違反連續連續排班限制");
                    }
                    // 檢查邏輯：如果已經有班，就攔截
                    if (await _shiftService.IsNurseAlreadyScheduled(item.NurseId, item.Date))
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"排班失敗：護理師 {item.NurseId} 在 {item.Date.ToShortDateString()} 已經有班了！");
                    }

                    // 轉換為實體物件並存入
                    var schedule = new ShiftSchedule
                    {
                        NurseId = item.NurseId,
                        Date = item.Date,
                        ShiftType = item.ShiftType
                    };
                    _context.ShiftSchedules.Add(schedule);
                }
                // 全部檢查通過，才一次寫入資料庫
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok("批次排班成功");
            }
            catch (Exception ex)
            {
                // 如果發生未知錯誤，也要回復
                await transaction.RollbackAsync();
                return StatusCode(500, $"系統錯誤:{ex.Message}");
            }
        }


        //查詢所有班表
        [HttpGet]
        public async Task<IActionResult> GetAllSchedules()
        {
            var schedules = await _context.ShiftSchedules.Include(s => s.Nurse).ToListAsync();
            return Ok(schedules);
        }

        //查詢現在每個班總共有多少人上
        [HttpGet("Status/{date}")]
        public async Task<IActionResult> GetStatus([FromRoute]DateTime date)
        {
            //找出當天所有班表(用.Date確保指比對日期不比對時間)
            var dailySchedules = await _context.ShiftSchedules
                .Where(s =>s.Date.Date == date.Date)
                .ToListAsync();
            //定義你的編制標準
            var requirements = new Dictionary<string, int> { { "D", 5 }, { "E", 3 }, { "N", 2 } };
            //統計與回傳
            var status = new ShiftStatusDto
            {
                Date = date,
                ShiftDetails = requirements.Select(r => new ShiftDetail
                {
                    ShiftType = r.Key,
                    Required = r.Value,
                    Count = dailySchedules.Count(s => s.ShiftType == r.Key)
                }).ToList()
            };
            return Ok(status);
        }

        //給員工看的精簡班表
        [HttpGet("Display")]
        public async Task<IActionResult> GetEmployeeSchedule()
        {
            var schedules = await _context.ShiftSchedules  //針對ShiftSchedules這個表格進行查詢
                .Include(s=>s.Nurse)  //連動查詢
                .Select(s=>new ScheduleDisplayDto  
                {
                    NurseName=s.Nurse.Name,
                    Date = s.Date,
                    ShiftType = s.ShiftType,
                })
                .ToListAsync();
                return Ok(schedules);
        }

        //刪除班表
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _context.ShiftSchedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound("找不到這筆班表!");
            }
            _context.ShiftSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return Ok($"已成功刪除ID 為{id}的班表");
        }
    }
    
}
