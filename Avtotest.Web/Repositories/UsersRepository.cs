using Avtotest.Web.Models;
using Microsoft.Data.Sqlite;
using System.Xml.Linq;

namespace Avtotest.Web.Repositories;

public class UsersRepository
{
    private const string ConnectionString = "Data source=users.db"; // userlarni malumotlarini saqlash uchun db fayl
    private SqliteConnection _connection;
    private SqliteCommand _command;


    public UsersRepository()
    {
        OpenConnection();// connection ochyapti
        CreateUsersTable();// userlarga tablitsa yaratyapti
    }

    public void OpenConnection()// connection ochyapti
    {
        _connection = new SqliteConnection(ConnectionString);
        _connection.Open();
        _command = _connection.CreateCommand();
    }

    public void CreateUsersTable()
    {
        //tablitsaga qabul qilishi kk bolgan qiymatlari berilyapti
        _command.CommandText = "create table if not exists users(id integer primary key autoincrement, name TEXT, phone TEXT, password TEXT, image TEXT)";
        _command.ExecuteNonQuery();// qiymatlarni qoshyapti
    }

    public void InsertUser(User user)
    {
        // userni namesini phonesini pagvordini qabul qilib insert qiliyapti
        _command.CommandText = $"INSERT INTO users(name, phone, password) values(@name, @phone, @password)";
        _command.Parameters.AddWithValue("@name", user.Name);
        _command.Parameters.AddWithValue("@phone", user.Phone);
        _command.Parameters.AddWithValue("@password", user.Password);
        //_command.Parameters.AddWithValue("@image", user.Image);
        _command.Prepare();

        _command.ExecuteNonQuery();
    }

    public List<User> GetUsers() // userlarni olish uchun uni listga qoshyapti
    {
        var users = new List<User>(); // abyekt olinyapti

        _command.CommandText = "SELECT * FROM users";// userlarni hammasini sorayapti
        var data = _command.ExecuteReader();// qaytib oqiyapti

        while (data.Read())// hamma userlarni argumentlarini  oqish uchun whileni ichiga olinyapti
        {
            var user = new User();// abyekt

            user.Index = data.GetInt32(0);// qiymat
            user.Name = data.GetString(1);// qiymat 
            user.Phone = data.GetString(2);// qiymat
            //user.Image = data.GetString(4);// qiymat

            users.Add(user);// user listga olinyapti
        }

        return users;// qiymat qaytyapti
    }

    public User GetUserByIndex(int index)
    {
        var user = new User();

        _command.CommandText = $"SELECT * FROM users WHERE id = {index}";
        var data = _command.ExecuteReader();
        while (data.Read())
        {
            user.Index = data.GetInt32(0);
            user.Name = data.GetString(1);
            user.Phone = data.GetString(2);
           // user.Image = data.GetString(4);
        }

        return user;
    }
    public User GetUserByPhoneNumber(string phoneNumber)
    {
        var user = new User();

        _command.CommandText = $"SELECT * FROM users WHERE phone = '{phoneNumber}'";
        var data = _command.ExecuteReader();
        while (data.Read())
        {
            user.Index = data.GetInt32(0);
            user.Name = data.GetString(1);
            user.Phone = data.GetString(2);
            user.Password = data.GetString(3);
           // user.Image = data.GetString(4);
        }

        return user;
    }

    public void DeleteUser(int index)
    {
        _command.CommandText = $"DELETE FROM users WHERE id = {index}";
        _command.ExecuteNonQuery();
    }

    public void UpdateUser(User user)
    {
        _command.CommandText = "UPDATE users SET name = @name, phone = @phone, password = @password, image = @image WHERE id = @userId";
        _command.Parameters.AddWithValue("@name", user.Name);
        _command.Parameters.AddWithValue("@phone", user.Phone);
        _command.Parameters.AddWithValue("@password", user.Password);
       // _command.Parameters.AddWithValue("@image", user.Image);
        _command.Parameters.AddWithValue("@userId", user.Index);
        _command.Prepare();

        _command.ExecuteNonQuery();
    }
}
