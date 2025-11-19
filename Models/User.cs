namespace smartclinic_web.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }

        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? BloodGroup { get; set; }
        public string? DoctorName { get; set; }
        public string? DoctorHospital { get; set; }
    }
}
