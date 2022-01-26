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

        public User SignIn(string username, string password)
        {
            User foundUser;
            var isLogin = false;
            
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password)) // check if any values are null or empty
            {
                throw new ArgumentException("One or more values are null or empty.");
            }
            
            try
            {
                var checkedEmail = new MailAddress(username).Address; // check if email string is in a email format
            }
            catch (FormatException e)
            {
                isLogin = true;
            }

            if (isLogin)
            {
                foundUser = _userRepository.FindByCondition(u => u.Login == username).FirstOrDefault() ?? throw new ArgumentException("User with a given data was not found.");;
            }
            else
            {
                foundUser = _userRepository.FindByCondition(u => u.Email == username).FirstOrDefault() ?? throw new ArgumentException("User with a given data was not found.");
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

        public void ChangeUserData(User user, int dataIdx, string oldValue, string newValue)
        {
            var currentUser = _userRepository.FindByCondition(u => u.Id == user.Id).FirstOrDefault() ??
                              throw new ArgumentException("No user found.");

            if (string.IsNullOrWhiteSpace(oldValue) || string.IsNullOrWhiteSpace(newValue))
            {
                throw new ArgumentException("One or more values are null or empty.");
            }

            switch (dataIdx)
            {
                default:
                    throw new ArgumentException("Wrong data index entered.");

                case 0: // password idx
                    if (currentUser.Password != oldValue)
                    {
                        throw new ArgumentException("Wrong old password entered.");
                    }

                    currentUser.Password = newValue;
                    break;

                case 1: // login idx
                    if (currentUser.Login != oldValue)
                    {
                        throw new ArgumentException("Wrong old login entered.");
                    }

                    currentUser.Login = newValue;
                    break;

                case 2: // name idx
                    if (currentUser.Name != oldValue)
                    {
                        throw new ArgumentException("Wrong old name entered.");
                    }

                    currentUser.Name = newValue;
                    break;
            }

            _userRepository.DeleteAsync(user);
            _userRepository.CreateAsync(currentUser);
        }
    }
}