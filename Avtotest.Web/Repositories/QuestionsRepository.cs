using Avtotest.Web.Models;
using Microsoft.Data.Sqlite;

namespace Avtotest.Web.Repositories;

public class QuestionsRepository
{
    private string _connectionString = "Data source=avtotest.db";// shu nomdagi db faylni ochyapmiz

    private SqliteConnection _connection; // sqlitedan connection olyapmiz

    
    public QuestionsRepository() // kop abyekt olmaslik uchun uni ichiga abyet olibketyapmiz
    {
        _connection = new SqliteConnection(_connectionString);
       
        
    }
    public int GetQuestionsCount() // savollarni sonini aniqlash uchun funksiya
    {
        _connection.Open();//connaction ochilyapti
        var cmd = _connection.CreateCommand();// yangi tablitsa ochilyapti
        cmd.CommandText = "select count(*) from questions";//uni textiga nimani chiqarishligi buyrugi berilyapti

        var data = cmd.ExecuteReader();// qaytarib oqiberadi

        while (data.Read())// hamma savollarni olishi uchun uni whileni ichiga olyapmiz
        {
            var count = data.GetInt32(0);// son bolgani uchun data.int bolyapti 0 bolsa uni nechanchi orindaturgan tablitsasini aniqlaydi
            _connection.Close();// harbir connection ochilgandan keyin uni albatta yopish kk
            data.Close();
            return count;// savollar soni 
        }
        _connection.Close();//yopilyapti
        return 0;
    }

    public QuestionEntity GetQuestionById(int id) // aynan user tanlagan savolni olish uchun questionni idisini tekshiryapmiz
    {
        _connection.Open();// connaction ochilyapti
        var question = new QuestionEntity();// savollar listidan abyet olinyapti
        var cmd = _connection.CreateCommand();// tablitsa ochilyapti
        cmd.CommandText = "select * from questions where id = @id;";// u qabul qiladigan qiymatlari elon qilinyapti
        cmd.Parameters.AddWithValue("id", id);// qiymati korsatilyapti
        cmd.Prepare();// qoshilyapti
        var data = cmd.ExecuteReader(); // qaytib oqiberishi uchun
        while (data.Read()) // hamma questionlarni oqishi uchun whileni ichiga olinyapti
        {
            question.id = data.GetInt32(0);//qiymat
            question.Question = data.GetString(1);// qiymat
            question.Description = data.GetString(2);// qiymat
            question.Image = data.IsDBNull(3) ? null://yoqbolishi mumkin qiymat
            data.GetString(3);//bolmasa
        }

        question.Choices = GetQuestionsChoices(id); // tanlangan savolni variantlariga aynan osha savolni varyatlari ekani uchun osha savolni varyanti berilyapti
        _connection.Close();// connaction yopilyapti
        return question;// qiymat jonatilyapti
    }

    public List<QuestionEntity> GetQuestionsRange(int from, int count) // fromdan boshlab countgacha oladi savollarni
    {
        _connection.Open(); // connection ochilyapti
        var question = new List<QuestionEntity>();  // savollardan abyekt olinyapti
        for (int i = from; i < from + count; i++)
        {
            question.Add(GetQuestionById(i)); // fromdan boshlab get questionga qoshib ketadi 20 ta
        }
        _connection.Close();// connection yopilyapti
        return question;// qiymat qaytarilyapti
    }

    private List<Choices> GetQuestionsChoices(int questionId)// varyantlarni listidan abyet olinyapti
    {
        _connection.Open();// ochilyapti
        var choices = new List<Choices>();// abyekt olinyapti
        var cmd = _connection.CreateCommand();// commant qilish uchun create qilinyapti
        cmd.CommandText = "select * from choices where questionId = @questionId;";// questionIdni choicelarini yani varyantlarini ber diyapti 

        cmd.Parameters.AddWithValue("questionId", questionId);// qiymati korsatilyapti
        cmd.Prepare();// qiymat qoshilyapti
        var data = cmd.ExecuteReader();// qaytib oqiberyapti
        while (data.Read())// hamma qiymatlarni oqish uchun whileni ichiga olinyapti
        {
           var choice = new Choices();  
            choice.Id = data.GetInt32(0);   
            choice.Ansvers = data.GetBoolean(2);
            choice.Text = data.GetString(1);

            choices.Add(choice);// choicesga qoshilyapti qiymatlar
           
        }
         _connection.Close();// yopilyapti
        return choices;// qiymat qaytyapti
    }
}

