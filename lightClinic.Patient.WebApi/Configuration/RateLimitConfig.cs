namespace lightClinic.Patient.WebApi.Configuration;

public class RateLimitConfig
{
    public RateLimitPolicy Global { get; set; } = new();
    public RateLimitPolicy Authenticated { get; set; } = new();
    public RateLimitPolicy Anonymous { get; set; } = new();
    public RateLimitPolicy Auth { get; set; } = new();
}

public class RateLimitPolicy
{
    public int PermitLimit { get; set; } = 100;
    public int WindowSeconds { get; set; } = 60;
    public int QueueLimit { get; set; } = 10;
    public int SegmentsPerWindow { get; set; } = 1;
}
