using System.Data;

namespace NurseSchedulingSystem.DTOs
{
    public class ShiftStatusDto
    {
        public DateTime Date {  get; set; }
        public List<ShiftDetail> ShiftDetails { get; set; } = new();
    }
    public class ShiftDetail
    {
        public string ShiftType {  get; set; } //A、E、N 班次類型
        public int Count {  get; set; } //目前排了多少人
        public int Required {  get; set; } //規定要幾個人
    }
}
