using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        public async Task<bool> Register(User? user)
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

            await _userRepository.CreateAsync(user);
            
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

            if (!_userRepository.Any())
            {
                return null;
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

        public async Task<bool> PasswordRecovery(string email)
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

            if (!await _emailNotificationService.SendForgotPassword(foundUser))
            {
                return false;
            }
            
            return true;
        }

        public bool ChangeUserData(User? user, UserDataTypes userDataType, string oldValue, string newValue)
        {
            var currentUser = _userRepository.FindByCondition(u => u.Id == user.Id).FirstOrDefault();

            if (currentUser == null)
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
                    if (currentUser.Password != oldValue)
                    {
                        return false;
                    }

                    currentUser.Password = newValue;
                    break;

                case UserDataTypes.Login: // login idx
                    if (currentUser.Login != oldValue)
                    {
                        return false;
                    }

                    currentUser.Login = newValue;
                    break;

                case UserDataTypes.Name: // name idx
                    if (currentUser.Name != oldValue)
                    {
                        return false;
                    }

                    currentUser.Name = newValue;
                    break;
                
                default:
                    return false;
            }

            _userRepository.DeleteAsync(user);
            _userRepository.CreateAsync(currentUser);
            
            return true;
        }
    }
}