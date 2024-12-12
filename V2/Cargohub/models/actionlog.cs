public class ActionLogCS
{
    public int id { get; set; }
    public int supplier_id { get; set; }
    public string? action { get; set; }
    public DateTime timestamp { get; set; }
    public string? performed_by { get; set; }
}