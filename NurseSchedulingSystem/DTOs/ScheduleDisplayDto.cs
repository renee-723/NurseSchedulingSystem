using System.Reflection.Metadata;

namespace NurseSchedulingSystem.DTOs
{
    //給前端以及員工看到的班表
    public class ScheduleDisplayDto
    {
        public string NurseName {  get; set; }
        public DateTime Date { get; set; }
        public String ShiftType {  get; set; }
    }
}
