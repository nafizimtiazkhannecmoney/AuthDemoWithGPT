using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Authdemo.Entities;
using Authdemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Authdemo.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository userRepository, PasswordHasher<User> passwordHasher, IJwtService jwtService, IEmailService emailService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            if (user == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if(result != PasswordVerificationResult.Success)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }

            // Generate JWT + Refresh Token
            var token = _jwtService.GenerateToken(user);
            //var refreshToken = GenerateRefreshToken();
            var refreshToken = GenerateSecureToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                ExpiryDate = GenerateRefreshTokenExpiry(),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false,
                UserId = user.Id
            };
            // Save refresh token to DB
            await _userRepository.AddRefreshTokenAsync(refreshTokenEntity);

            // Read the token to get the expiration time
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            return new LoginResponse
            {
                Success = true,
                Message = "Login Successful",
                Username = user.Username,
                Role = user.Role,
                Token = token,
                ExpiresAt = jwt.ValidTo.ToLocalTime(),
                ExpiresAtString = jwt.ValidTo.ToLocalTime().ToString("dd MMM yyyy, hh:mm:ss tt"),
                RefreshToken = refreshTokenEntity.Token
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {

            if (request.Password != request.ConfirmPassword)
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "Passwords do not match."
                };
            }

            if (await _userRepository.UsernameExistsAsync(request.Username))
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "User already exists."
                };
            }

            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                return new RegisterResponse
                {
                    Success = false,
                    Message = "Email already exists."
                };
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Role = "User",
                IsActive = true,
                Department = "Finance"
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await _userRepository.AddUserAsync(user);

            return new RegisterResponse
            {
                Success = true,
                Message = "Registration successful."
            };
        }

        

        //public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
        //{
        //    var refreshTokenEntity = await _userRepository.GetRefreshTokenAsync(request.RefreshToken);
        //    if (refreshTokenEntity == null || refreshTokenEntity.IsRevoked || refreshTokenEntity.ExpiryDate < DateTime.UtcNow)
        //    {
        //        return new LoginResponse
        //        {
        //            Success = false,
        //            Message = "Invalid or expired refresh token."
        //        };
        //    }


        //    //var user = await _userRepository.GetByIdAsync(refreshTokenEntity.UserId);
        //    var user = refreshTokenEntity.User;
        //    if (user == null)
        //    {
        //        return new LoginResponse
        //        {
        //            Success = false,
        //            Message = "User not found."
        //        };
        //    }
        //    // Generate new JWT + Refresh Token
        //    var token = _jwtService.GenerateToken(user);
        //    var newRefreshToken = GenerateRefreshToken();
        //    // Revoke the old refresh token
        //    refreshTokenEntity.IsRevoked = true;
        //    await _userRepository.UpdateRefreshTokenAsync(refreshTokenEntity);
        //    // Save the new refresh token to DB
        //    var newRefreshTokenEntity = new RefreshToken
        //    {
        //        Token = newRefreshToken,
        //        ExpiryDate = GenerateRefreshTokenExpiry(),
        //        CreatedAt = DateTime.UtcNow,
        //        IsRevoked = false,
        //        UserId = user.Id
        //    };
        //    await _userRepository.AddRefreshTokenAsync(newRefreshTokenEntity);
        //    // Read the token to get the expiration time
        //    var handler = new JwtSecurityTokenHandler();
        //    var jwt = handler.ReadJwtToken(token);
        //    return new LoginResponse
        //    {
        //        Success = true,
        //        Message = "Token refreshed successfully.",
        //        Username = user.Username,
        //        Role = user.Role,
        //        Token = token,
        //        ExpiresAt = jwt.ValidTo.ToLocalTime(),
        //        ExpiresAtString = jwt.ValidTo.ToLocalTime().ToString("dd MMM yyyy, hh:mm tt"),
        //        RefreshToken = newRefreshTokenEntity.Token
        //    };
        //}

        public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var refreshTokenEntity = await _userRepository.GetRefreshTokenAsync(request.RefreshToken);
            if (refreshTokenEntity == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Refresh token not found."
                };
            }
            if (refreshTokenEntity.IsRevoked)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Refresh token revoked."
                };
            }
            if (refreshTokenEntity.ExpiryDate < DateTime.UtcNow)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Refresh token expired."
                };
            }


            //var user = await _userRepository.GetByIdAsync(refreshTokenEntity.UserId);
            var user = refreshTokenEntity.User;
            if (user == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "User not found."
                };
            }
            // Generate new JWT + Refresh Token
            var token = _jwtService.GenerateToken(user);

            // Revoke the old refresh token
            refreshTokenEntity.IsRevoked = true;
            await _userRepository.UpdateRefreshTokenAsync(refreshTokenEntity);

            // Generate Refresh Token
            //var newRefreshToken = GenerateRefreshToken();
            var newRefreshToken = GenerateSecureToken();

            
            // Save the new refresh token to DB
            var newRefreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                ExpiryDate = GenerateRefreshTokenExpiry(),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false,
                UserId = user.Id
            };
            await _userRepository.AddRefreshTokenAsync(newRefreshTokenEntity);
            // Read the token to get the expiration time
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return new LoginResponse
            {
                Success = true,
                Message = "Token refreshed successfully.",
                Username = user.Username,
                Role = user.Role,
                Token = token,
                ExpiresAt = jwt.ValidTo.ToLocalTime(),
                ExpiresAtString = jwt.ValidTo.ToLocalTime().ToString("dd MMM yyyy, hh:mm tt"),
                //RefreshToken = newRefreshTokenEntity.Token,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            //throw new NotImplementedException();
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                return true;
            }

            // token generate
            //var token = GeneratePasswordResetToken();
            var token = GenerateSecureToken();

            var resetToken = new PasswordResetToken
            {
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false,
                UserId = user.Id
            };

            await _userRepository.AddPasswordResetTokenAsync(resetToken);

            await _emailService.SendPasswordResetEmailAsync(
            user.Email,
            resetToken.Token);

            return true;
        }


        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            //throw new NotImplementedException();
            if (request.NewPassword != request.ConfirmPassword)
            {
                return false;
            }

            var resetToken = await _userRepository.GetPasswordResetTokenAsync(request.Token);

            if (resetToken == null)
            {
                return false;
            }

            if (resetToken.IsUsed)
            {
                return false;
            }
            if (resetToken.ExpiryDate < DateTime.UtcNow)
            {
                return false;
            }

            var user = resetToken.User;
            
            if(user == null)
            {
                return false;
            }


            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            await _userRepository.UpdateUserAsync(user);

            resetToken.IsUsed = true;
            await _userRepository.UpdatePasswordResetTokenAsync(resetToken);

            return true;
        }


        // Token Generations
        private static string GeneratePasswordResetToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes);
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(randomBytes);
        }
        private static DateTime GenerateRefreshTokenExpiry()
        {
            return DateTime.UtcNow.AddDays(30);
        }

        private static string GenerateSecureToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if(user == null)
            {
                return false;
            }

            var passwordResult = _passwordHasher.VerifyHashedPassword(user,user.PasswordHash,request.CurrentPassword);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return false;
            }

            if(request.NewPassword != request.ConfirmPassword)
            {
                return false;
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            await _userRepository.UpdateUserAsync(user);

            return true;
        }

        public async Task<bool> LogoutAsync(int userId, LogoutRequest request)
        {
            var refreshToken = await _userRepository.GetRefreshTokenAsync(request.RefreshToken);

            if(refreshToken == null)
            {
                return false;
            }

            if(refreshToken.UserId != userId)
            {
                return false;
            }

            if (refreshToken.IsRevoked)
            {
                return false;
            }

            refreshToken.IsRevoked = true;
            await _userRepository.UpdateRefreshTokenAsync(refreshToken);

            return true;
        }
    }
}
