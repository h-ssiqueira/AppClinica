﻿using AutoMapper;
using Contracts.DTO.User;
using Contracts.Entities;
using Contracts.Interfaces.Repositories;
using Contracts.Interfaces.Services;
using Contracts.RequestHandle;
using Contracts.TransactionObjects.Login;
using Contracts.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Business.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _Mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;


        public UserService(IMapper Mapper, IConfiguration configuration, IUserRepository userRepository)
        {
            _Mapper = Mapper;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<RequestResult<LoginResponseDto>> Register(UserDto registerRequest)
        {
            try
            {
                var userExists = await _userRepository.CheckIfUserExistsByEmail(registerRequest.Email);

                if (userExists)
                    return new RequestResult<LoginResponseDto>(null, true, RequestAnswer.UserDuplicateCreateError.GetDescription());

                var model = _Mapper.Map<User>(registerRequest);
                model.Active = true;

                var response = await _userRepository.Register(model);

                if (response.Id == 0)
                    return new RequestResult<LoginResponseDto>(null, true, RequestAnswer.UserCreateError.GetDescription());

                var loginDto = new LoginRequestDto
                {
                    Email = response.Email,
                    Password = response.Password
                };

                var login = await Login(loginDto);

                return new RequestResult<LoginResponseDto>(login.Result);
            }
            catch (Exception ex)
            {
                return new RequestResult<LoginResponseDto>(null, true, ex.Message);
            }
        }
        public async Task<RequestResult<LoginResponseDto>> Login(LoginRequestDto loginRequest)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAndPassword(loginRequest.Email, loginRequest.Password);

                if (user == null)
                    return new RequestResult<LoginResponseDto>(null, true, RequestAnswer.UserCredError.GetDescription());

                var loginResponse = new LoginResponseDto
                {
                    UserId = user.Id,
                    UserName = user.Name,
                    UserEmail = user.Email,
                    Token = TokenService.GenerateToken(user, _configuration["Settings:JwtSecret"])
                };

                var result = new RequestResult<LoginResponseDto>(loginResponse, false);

                return result;
            }
            catch (Exception ex)
            {
                var msg = ex.Message.Contains("See the inner exception for details") ? ex.InnerException.Message : ex.Message;
                return new RequestResult<LoginResponseDto>(null, true, msg);
            }
        }
        public async Task<RequestResult<UserDto>> GetUserById(int id)
        {
            try
            {
                var model = await _userRepository.GetUserById(id);

                if (model == null)
                    return new RequestResult<UserDto>(null, true, RequestAnswer.UserNotFound.GetDescription());

                var dto = _Mapper.Map<UserDto>(model);
                var result = new RequestResult<UserDto>(dto);

                return result;
            }
            catch (Exception ex)
            {
                return new RequestResult<UserDto>(null, true, ex.Message);
            }
        }
        public async Task<RequestResult<UserDto>> GetUserByEmail(string email)
        {
            try
            {
                var model = await _userRepository.GetUserByEmail(email);

                if (model == null)
                    return new RequestResult<UserDto>(null, true, RequestAnswer.UserNotFound.GetDescription());

                var dto = _Mapper.Map<UserDto>(model);
                var result = new RequestResult<UserDto>(dto);

                return result;
            }
            catch (Exception ex)
            {
                return new RequestResult<UserDto>(null, true, ex.Message);
            }
        }
        public async Task<RequestResult<RequestAnswer>> UpdateUser(UserDto user)
        {
            try
            {
                var userCheck = await _userRepository.CheckIfUserExistsById(user.Id);

                if (!userCheck)
                    return new RequestResult<RequestAnswer>(RequestAnswer.UserNotFound);

                var model = _Mapper.Map<User>(user);
                await _userRepository.UpdateUser(model);

                return new RequestResult<RequestAnswer>(RequestAnswer.UserUpdateSuccess);
            }
            catch (Exception)
            {
                return new RequestResult<RequestAnswer>(RequestAnswer.UserUpdateError, true);
            }
        }
        public async Task<RequestResult<RequestAnswer>> DeleteUser(int id)
        {
            try
            {
                await _userRepository.DeleteUser(id);

                return new RequestResult<RequestAnswer>(RequestAnswer.UserDeleteSuccess);
            }
            catch (Exception)
            {
                return new RequestResult<RequestAnswer>(RequestAnswer.UserDeleteError, true);
            }
        }
    }
}
