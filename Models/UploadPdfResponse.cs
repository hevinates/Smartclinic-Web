public class UploadPdfResponse
{
    public string? date { get; set; }
    public List<TestItem>? results { get; set; }
}

public class TestItem
{
    public string? name { get; set; }
    public string? result { get; set; }
    public string? range { get; set; }
    public bool isOutOfRange { get; set; }
}
