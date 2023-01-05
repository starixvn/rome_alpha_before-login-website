namespace Fanex.Alpha.Before.Configurations;

public class LimitedCountry
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTime AffectedDate { get; set; }

    public bool Active { get; set; }

    public bool IsTesting { get; set; }

    public string TestingIp { get; set; }
}