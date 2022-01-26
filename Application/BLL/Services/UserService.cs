using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IEmailNotificationService _emailNotificationService;

        public UserService(IGenericRepository<User> userRepository, IEmailNotificationService emailNotificationService)
        {
            _userRepository = userRepository;
            _emailNotificationService = emailNotificationService;
        }

        public void Register(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.Login) ||
                string.IsNullOrWhiteSpace(user.Password)) // check if any values are null or empty
            {
                throw new ArgumentException("One or more values are null or empty.");
            }

            try
            {
                var checkedEmail = new MailAddress(user.Email).Address; // check if email string is in a email format
            }
            catch (FormatException e)
            {
                throw new FormatException("Invalid email format " + e);
            }

            // Password must contain numbers, lowercase or uppercase letters, include special symbols, at least 8 characters, at most 24 characters.
            var passwordRegex = new Regex(@"(?=.*[0-9])(?=.*[a-zA-Z])(?=([\x21-\x7e]+)[^a-zA-Z0-9]).{8,24}",
                RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            if (!passwordRegex.IsMatch(user.Password)) // check if password is strong
            {
                throw new FormatException("Password is not strong enough.");
            }

            if (!Equals(_userRepository.FindByCondition(u => u.Email == user.Email),
                    Enumerable.Empty<User>())) // check if email is unique
            {
                throw new ArgumentException("Email must be unique.");
            }

            if (!Equals(_userRepository.FindByCondition(u => u.Login == user.Login),
                    Enumerable.Empty<User>())) // check if login is unique
            {
                throw new ArgumentException("Login must be unique.");
            }

            _userRepository.CreateAsync(user);
        }

        public User SignIn(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password)) // check if any values are null or empty
            {
                throw new ArgumentException("One or more values are null or empty.");
            }

            var foundUser = _userRepository.FindByCondition(u => u.Login == login).FirstOrDefault();

            if (foundUser == null)
            {
                throw new ArgumentException("User with a given login was not found.");
            }

            if (foundUser.Password != password)
            {
                throw new ArgumentException("Wrong password.");
            }

            return foundUser;
        }

        public void PasswordRecovery(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email string can't be empty.");
            }

            var foundUser = _userRepository.FindByCondition(u => u.Email == email).FirstOrDefault();

            if (foundUser == null)
            {
                throw new ArgumentException("User with a given email was not found.");
            }

            _emailNotificationService.SendForgotPassword(foundUser);
        }
    }
}