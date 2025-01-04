public class PageinationCS()
{
    public int Page {get; set;}
    public int PageSize {get;set;}
    public int TotItems {get;set;}
    public List<ItemCS>? Data {get; set;}
}