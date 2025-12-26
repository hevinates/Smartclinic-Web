namespace smartclinic_web.Models
{
    public class TestResult
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public User? Patient { get; set; }

        public DateTime TestDate { get; set; }
        public string? TestName { get; set; }
        public string? Value { get; set; }
        public string? ReferenceRange { get; set; }
        public bool IsOutOfRange { get; set; }
    }
}
