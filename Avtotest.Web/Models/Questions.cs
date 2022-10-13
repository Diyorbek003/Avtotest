namespace Avtotest.Web.Models;

public class QuestionEntity
{

    public int id { get; set; }
    public string? Question { get; set; }

    public string? Description { get; set; }
    public List<Choices>? Choices { get; set; }

    public string? Image { get; set; }
}

