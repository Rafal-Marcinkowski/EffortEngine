namespace SharedProject.Models;

public class Program : BindableBase
{

    public int Id { get; set; }
    public string Name { get; set; }

    private decimal totalWorkTime;
    public decimal TotalWorkTime
    {
        get => totalWorkTime;
        set => SetProperty(ref totalWorkTime, value);
    }

    public List<Bug> Bugs { get; set; }
    public List<Feature> Features { get; set; }
    public List<Module> Modules { get; set; }
}
