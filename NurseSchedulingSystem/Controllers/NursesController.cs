using Microsoft.AspNetCore.Mvc;
using NurseSchedulingSystem.Data;
using Microsoft.EntityFrameworkCore;
using NurseSchedulingSystem.Entities;

namespace NurseSchedulingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NursesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public NursesController(AppDbContext context)
        {
            _context = context;
        }

        //查詢護理師
        [HttpGet]
        public async Task<IActionResult> GetAllNurse()
        {
            var nurses = await _context.Nurses.ToListAsync();
            return Ok(nurses);
        } 

        //新增護理師
        [HttpPost]
        public async Task<IActionResult> CreateNurse(Nurse nurse)
        {
            _context.Nurses.Add(nurse); //把新護理師加入檔案庫
            await _context.SaveChangesAsync(); //存檔
            return Ok(nurse); //回傳剛剛新增的資料，確認成功
        }

        //更新護理師
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNurseStatus (int id, [FromBody]bool isActive)
        {
            var nurse = await _context.Nurses.FindAsync(id);
            if (nurse == null)
            {
                return NotFound($"找不到ID為{id}的護理師");
            }

            //更新狀態
            nurse.isActive = isActive;
            await _context.SaveChangesAsync();
            return Ok(nurse);
        }

        //刪除已離職護理師
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNurse(int id)
        {
            var nurse = await _context.Nurses.FindAsync(id);
            if (nurse == null) return NotFound("找不到這位護理師");

            //檢查是否有相關聯的班表
            var hasSchedules = await _context.ShiftSchedules.AnyAsync(s => s.NurseId == id);
            if (hasSchedules)
            {
                return BadRequest("該護理師已有排班紀錄，無法刪除，請先刪除其所有班表");
            }
            //沒有班表才可刪除
            _context.Nurses.Remove(nurse);
            await _context.SaveChangesAsync();
            return Ok("護理師資料已全數刪除");
        }
    }
}
