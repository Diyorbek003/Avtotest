namespace Avtotest.Web.Models;

public class Ticket
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public int FromIndex { get; set; }

    public int QuestionCount { get; set; }

    public int CorrectAnswerCount { get; set; }

    public bool IsTraining { get; set; }


    public Ticket()
    {

    }
    public Ticket( int userId, int fromIndex, int questionCount)
    {
        UserId = userId;
        FromIndex = fromIndex;
        QuestionCount = questionCount;
        CorrectAnswerCount = 0;
    }
}
