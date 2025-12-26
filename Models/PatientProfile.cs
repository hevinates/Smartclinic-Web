namespace smartclinic_web.Models
{
    public class PatientProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? Age { get; set; }
        public string? BloodGroup { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public int? DoctorId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public User? Doctor { get; set; }
    }
}
