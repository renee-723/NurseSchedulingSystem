namespace NurseSchedulingSystem.DTOs
{
    public class BatchScheduleRequest
    {
        public int CreatedByUserId {  get; set; } //紀錄是誰在進行操作，方便任何追蹤
        public List<ScheduleItem> Schedules { get; set; } = new List<ScheduleItem>();  //核心:排班清單
    }

    public class ScheduleItem
    {
        public int NurseId { get; set; }
        public DateTime Date { get; set; }
        public string ShiftType { get; set; }
    }
}
