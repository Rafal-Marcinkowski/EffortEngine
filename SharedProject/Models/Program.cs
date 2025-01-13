namespace SharedProject.Models;

public class Program
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal TotalWorkTime { get; set; }
    public List<Bug> Bugs { get; set; }
    public List<Feature> Features { get; set; }
}
