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
        private readonly IGenericRepository<User?> _userRepository;
        private readonly IEmailNotificationService _emailNotificationService;

        public UserService(IGenericRepository<User?> userRepository, IEmailNotificationService emailNotificationService)
        {
            _userRepository = userRepository;
            _emailNotificationService = emailNotificationService;
        }

        public bool Register(User? user)
        {
            if (user == null)
            {
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(user.Name) 
                || string.IsNullOrWhiteSpace(user.Email) 
                || string.IsNullOrWhiteSpace(user.Login) 
                || string.IsNullOrWhiteSpace(user.Password)) // check if any values are null or empty
            {
                return false;
            }

            try
            {
                var checkedEmail = new MailAddress(user.Email).Address; // check if email string is in a email format
            }
            catch (FormatException e)
            {
                return false;
            }

            // Password must contain numbers, lowercase or uppercase letters, include special symbols, at least 8 characters, at most 24 characters.
            var passwordRegex = new Regex(@"(?=.*[0-9])(?=.*[a-zA-Z])(?=([\x21-\x7e]+)[^a-zA-Z0-9]).{8,24}",
                RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            if (!passwordRegex.IsMatch(user.Password)) // check if password is strong
            {
                return false;
            }

            if (_userRepository.Any(u => u.Email == user.Email)) // check if email is unique
            {
                return false;
            }

            if (_userRepository.Any(u => u.Login == user.Login)) // check if login is unique
            {
                return false;
            }

            _userRepository.CreateAsync(user);
            
            return true;
        }

        public User? SignIn(string username, string password)
        {
            var isLogin = false;
            
            if (string.IsNullOrWhiteSpace(username) 
                || string.IsNullOrWhiteSpace(password)) // check if any values are null or empty
            {
                return null;
            }
            
            try
            {
                var checkedEmail = new MailAddress(username).Address; // check if email string is in a email format
            }
            catch (FormatException e)
            {
                isLogin = true;
            }

            User? foundUser = null;
            if (isLogin)
            {
                foundUser = _userRepository.FindByCondition(u => u.Login == username).FirstOrDefault();
            }
            else
            {
                foundUser = _userRepository.FindByCondition(u => u.Email == username).FirstOrDefault();
            }

            if (foundUser == null)
            {
                return null;
            }

            if (foundUser.Password != password)
            {
                return null;
            }

            return foundUser;
        }

        public bool PasswordRecovery(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var foundUser = _userRepository.FindByCondition(u => u.Email == email).FirstOrDefault();

            if (foundUser == null)
            {
                return false;
            }

            _emailNotificationService.SendForgotPassword(foundUser);
            
            return true;
        }

        public bool ChangeUserData(User user, UserDataTypes userDataType, string oldValue, string newValue)
        {
            if (_userRepository.FindByCondition(u => u.Id == user.Id).FirstOrDefault() == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(oldValue) || string.IsNullOrWhiteSpace(newValue))
            {
                return false;
            }

            switch (userDataType)
            {
                case UserDataTypes.Password: // password idx
                    if (user.Password != oldValue)
                    {
                        return false;
                    }

                    user.Password = newValue;
                    break;

                case UserDataTypes.Login: // login idx
                    if (user.Login != oldValue)
                    {
                        return false;
                    }

                    user.Login = newValue;
                    break;

                case UserDataTypes.Name: // name idx
                    if (user.Name != oldValue)
                    {
                        return false;
                    }

                    user.Name = newValue;
                    break;
                
                default:
                    return false;
            }

            _userRepository.UpdateAsync(user);
            
            return true;
        }
    }
}