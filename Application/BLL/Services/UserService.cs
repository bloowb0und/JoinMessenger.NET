using System.Net.Mail;
using System.Text.RegularExpressions;
using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _userRepository;

    public UserService(IGenericRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public void Register(User user)
    {
        var createdUser = new User();

        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email) ||
            string.IsNullOrWhiteSpace(user.Login) || string.IsNullOrWhiteSpace(user.Password))
        {
            Console.WriteLine("One or more values are null or empty.");
            throw new ArgumentException();
        }

        try
        {
            var checkedEmail = new MailAddress(user.Email).Address; // check if email is formatted
        }
        catch (FormatException e)
        {
            Console.WriteLine("Invalid email format " + e.Message);
            throw;
        }

        // Must contain numbers, lowercase or uppercase letters, include special symbols, at least 8 characters, at most 24 characters.
        var passwordRegex = new Regex(@"(?=.*[0-9])(?=.*[a-zA-Z])(?=([\x21-\x7e]+)[^a-zA-Z0-9]).{8,24}",
            RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

        if (!passwordRegex.IsMatch(user.Password)) // check if password is strong
        {
            Console.WriteLine("Password is not strong enough.");
            Console.WriteLine("It must contain numbers, lowercase or uppercase letters, include special symbols, at least 8 characters, at most 24 characters.");
            throw new FormatException();
        }

        if (!Equals(_userRepository.FindByCondition(u => u.Email == user.Email), Enumerable.Empty<User>())) // check if email is unique
        {
            Console.WriteLine("Email must be unique.");
            throw new ArgumentException();
        }
        if (!Equals(_userRepository.FindByCondition(u => u.Login == user.Login), Enumerable.Empty<User>())) // check if login is unique
        {
            Console.WriteLine("Email must be unique.");
            throw new ArgumentException();
        }

        _userRepository.CreateAsync(createdUser);
    }
}