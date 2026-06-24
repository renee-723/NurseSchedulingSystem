namespace NurseSchedulingSystem.Entities
{
    public class ShiftSchedule
    {
        public int Id { get; set; }  //班表的id
        public int NurseId {  get; set; }  //護理師ID
        public DateTime Date { get; set; }  //排班日期
        public string ShiftType {  get; set; }  //班別(早/小夜/大夜)
        public virtual Nurse? Nurse { get; set; }   //給誰的班表
    }
}
