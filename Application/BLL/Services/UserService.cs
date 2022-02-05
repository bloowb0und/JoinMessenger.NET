using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using Core.Models.ServiceMethodsModels;
using DAL.Abstractions.Interfaces;
using DAL.Repository;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IEmailNotificationService _emailNotificationService;

        public UserService(IEmailNotificationService emailNotificationService,
            UnitOfWork unitOfWork)
        {
            _emailNotificationService = emailNotificationService;
            _unitOfWork = unitOfWork;
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

            if (_unitOfWork.UserRepository.Any(u => u.Email == user.Email)
                ||_unitOfWork.UserRepository.Any(u => u.Login == user.Login)) // check if email is unique
            {
                return false;
            }

            await _unitOfWork.UserRepository.Create(user);
            
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

            if (!_unitOfWork.UserRepository.Any())
            {
                return null;
            }

            User foundUser = null;
            if (isLogin)
            {
                foundUser = _unitOfWork.UserRepository.FindByCondition(u => u.Login == username).SingleOrDefault();
            }
            else
            {
                foundUser = _unitOfWork.UserRepository.FindByCondition(u => u.Email == username).SingleOrDefault();
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
            var foundUser = _unitOfWork.UserRepository.FindByCondition(u => u.Email == email).SingleOrDefault();

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
            if (_unitOfWork.UserRepository.FirstOrDefault(u => u.Id == user.Id) == null)
            {
                return false;
            }

            user.Name = string.IsNullOrWhiteSpace(newUserData.Name) ? user.Name : newUserData.Name;
            
            if (!_unitOfWork.UserRepository.Any(u => u.Login == newUserData.Login))
            {
                user.Login = string.IsNullOrWhiteSpace(newUserData.Login) ? user.Login : newUserData.Login;
            }
            
            user.Password = string.IsNullOrWhiteSpace(newUserData.Password) ? user.Password : newUserData.Password;

            await _unitOfWork.UserRepository.Update(user);
            
            return true;
        }
    }
}