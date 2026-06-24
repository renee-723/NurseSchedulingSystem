namespace NurseSchedulingSystem.Entities
{
    public class Nurse
    {
        public int Id { get; set; }   //護理師ID
        public string Name { get; set; } 
        public string Role {  get; set; }  //職別(N1.N2)
        public bool isActive { get; set; } = true;  //預設為在職
    }
}
