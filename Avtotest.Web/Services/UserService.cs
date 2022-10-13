using Avtotest.Web.Models;
using Avtotest.Web.Repositories;

namespace Avtotest.Web.Services;

public class UserService
{
    private CookiesService _cookiesService;

    private UsersRepository _usersRepository;

	public UserService()
	{
		_cookiesService = new CookiesService();
        _usersRepository = new UsersRepository();   
	}

	public User? GetUserFromCookie(HttpContext context)
	{
        var userPhone = _cookiesService.GetUserPhoneFromCookie(context);
        if (userPhone != null)
        {
            var user = _usersRepository.GetUserByPhoneNumber(userPhone);
            if (user.Phone == userPhone)
            {
                return user;
            }
           
        } return null;
        
    }
}
