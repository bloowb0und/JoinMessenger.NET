using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using BLL.Helpers;
using Core.Models;
using Core.Models.ServiceMethodsModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork, IEmailNotificationService emailNotificationService)
        {
            _unitOfWork = unitOfWork;
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

            if (await _unitOfWork.UserRepository
                .Any(u => u.Email == user.Email 
                          || u.Login == user.Login)) // check if email and login is unique
            {
                return false;
            }

            user.Password = PasswordHelper.HashPassword(user.Password);
            
            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await _unitOfWork.UserRepository.CreateAsync(user);
                    await _unitOfWork.SaveAsync();
                    
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch 
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }

            return true;
        }

        public async Task<User> SignInAsync(string username, string password)
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

            if (!await _unitOfWork.UserRepository.Any())
            {
                return null;
            }

            User foundUser = null;
            if (isLogin)
            {
                foundUser = _unitOfWork.UserRepository.FirstOrDefault(u => u.Login == username);
            }
            else
            {
                foundUser = _unitOfWork.UserRepository.FirstOrDefault(u => u.Email == username);
            }

            if (foundUser == null
                || !PasswordHelper.VerifyHashedPassword(foundUser.Password, password))
            {
                return null;
            }

            return foundUser;
        }

        public async Task<bool> PasswordRecoveryAsync(string email)
        {
            var foundUser = _unitOfWork.UserRepository.FirstOrDefault(u => u.Email == email);

            if (foundUser == null)
            {
                return false;
            }

            var generatedPassword = PasswordHelper.GetRandomPassword();

            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.UserRepository.FirstOrDefault(u => u.Id == foundUser.Id).Password =
                        PasswordHelper.HashPassword(generatedPassword);
                    
                    _unitOfWork.UserRepository.Update(foundUser);
                    await _unitOfWork.SaveAsync();
                }
                catch 
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
                
                foundUser.Password = generatedPassword;

                if (!await _emailNotificationService.SendForgotPasswordAsync(foundUser))
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return false;
                }
                
                await _unitOfWork.CommitTransactionAsync();
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
            
            if (!await _unitOfWork.UserRepository.Any(u => u.Login == newUserData.Login))
            {
                user.Login = string.IsNullOrWhiteSpace(newUserData.Login) ? user.Login : newUserData.Login;
            }
            
            user.Password = string.IsNullOrWhiteSpace(newUserData.Password) ? user.Password : newUserData.Password;

            using (_unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    _unitOfWork.UserRepository.Update(user);
                    await _unitOfWork.SaveAsync();
                    
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch 
                {
                    await _unitOfWork.RollbackTransactionAsync();
                }
            }
            
            return true;
        }
    }
}