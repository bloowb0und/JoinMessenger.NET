using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using Core.Models.ServiceMethodsModels;
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

        public async Task<bool> RegisterAsync(User user)
        {
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

            if (_userRepository.Any(u => u.Email == user.Email)
                || _userRepository.Any(u => u.Login == user.Login)) // check if email is unique
            {
                return false;
            }

            await _userRepository.CreateAsync(user);
            
            return true;
        }

        public User SignIn(string username, string password)
        {
            var isLogin = false;

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

            User foundUser = null;
            if (isLogin)
            {
                foundUser = _userRepository.FindByCondition(u => u.Login == username).SingleOrDefault();
            }
            else
            {
                foundUser = _userRepository.FindByCondition(u => u.Email == username).SingleOrDefault();
            }

            if (foundUser == null
                || foundUser.Password != password)
            {
                return null;
            }

            return foundUser;
        }

        public async Task<bool> PasswordRecoveryAsync(string email)
        {
            var foundUser = _userRepository.FindByCondition(u => u.Email == email).SingleOrDefault();

            if (foundUser == null)
            {
                return false;
            }

            if (!await _emailNotificationService.SendForgotPasswordAsync(foundUser))
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ChangeUserDataAsync(User user, UserServiceChangeUserData newUserData)
        {
            if (_userRepository.FirstOrDefault(u => u.Id == user.Id) == null)
            {
                return false;
            }

            user.Name = string.IsNullOrWhiteSpace(newUserData.Name) ? user.Name : newUserData.Name;
            
            if (!_userRepository.Any(u => u.Login == newUserData.Login))
            {
                user.Login = string.IsNullOrWhiteSpace(newUserData.Login) ? user.Login : newUserData.Login;
            }
            
            user.Password = string.IsNullOrWhiteSpace(newUserData.Password) ? user.Password : newUserData.Password;

            await _userRepository.UpdateAsync(user);
            
            return true;
        }
    }
}