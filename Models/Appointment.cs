namespace smartclinic_web.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? AppointmentTime { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? DoctorNote { get; set; }

        // Navigation properties
        public User? Patient { get; set; }
        public User? Doctor { get; set; }
    }
}
