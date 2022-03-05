using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using BLL.Helpers;
using Core.Models;
using Core.Models.ServiceMethodsModels;
using DAL.Abstractions.Interfaces;
using FluentResults;

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

        private Result ValidateUser(User user)
        {
            var result = Result.Ok();
            if (!UserHelper.IsValidEmail(user.Email))
            {
                result = Result.Fail("Wrong email format"); // check if email string is in a email format
            }
            
            // Password must contain numbers, lowercase or uppercase letters, include special symbols, at least 8 characters, at most 24 characters.
            var passwordRegex = new Regex(@"(?=.*[0-9])(?=.*[a-zA-Z])(?=([\x21-\x7e]+)[^a-zA-Z0-9]).{8,24}",
                RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            
            if (!passwordRegex.IsMatch(user.Password)) // check if password is strong
            {
                result.WithError("Wrong password format");
            }
            
            return result;
        }

        public async Task<Result> RegisterAsync(User user)
        {
            Result methodResult = null;
            
            if ((methodResult = this.ValidateUser(user)).IsFailed)
            {
                return methodResult.WithErrors(methodResult.Errors);
            }

            if (await _unitOfWork.UserRepository
                    .Any(u => u.Email == user.Email 
                              || u.Login == user.Login)) // check if email and login is unique
            {
                return Result.Fail("User already exists.");
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

            return Result.Ok();
        }

        public async Task<Result<User>> SignInAsync(string username, string password)
        {
            var isLogin = UserHelper.IsValidEmail(username);

            var foundUser = isLogin
                ? _unitOfWork.UserRepository.FirstOrDefault(u => u.Login == username)
                : _unitOfWork.UserRepository.FirstOrDefault(u => u.Email == username);

            if (foundUser == null
                || !PasswordHelper.VerifyHashedPassword(foundUser.Password, password))
            {
                return Result.Fail("User was not found.");
            }

            return Result.Ok(foundUser);
        }

        public async Task<Result> PasswordRecoveryAsync(string email)
        {
            var foundUser = _unitOfWork.UserRepository.FirstOrDefault(u => u.Email == email);

            if (foundUser == null)
            {
                return Result.Fail("User was not found.");
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

                if (!(await _emailNotificationService.SendForgotPasswordAsync(foundUser)).IsFailed)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Result.Fail("Error while sending email.");
                }
                
                await _unitOfWork.CommitTransactionAsync();
            }

            return Result.Ok();
        }

        public async Task<Result> ChangeUserDataAsync(User user, UserServiceChangeUserData newUserData)
        {
            if (_unitOfWork.UserRepository.FirstOrDefault(u => u.Id == user.Id) == null)
            {
                return Result.Fail("User was not found.");
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
            
            return Result.Ok();
        }
    }
}