using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MyRESTServices.BLL.DTOs;
using MyRESTServices.BLL.Interfaces;
using MyRESTServices.Data.Interfaces;
using MyRESTServices.Domain.Models;

namespace MyRESTServices.BLL
{
    public class UserBLL : IUserBLL
    {
        private readonly IUserData _userData;
        private readonly IMapper _mapper;

        public UserBLL(IUserData userData, IMapper mapper)
        {
            _userData = userData;
            _mapper = mapper;
        }

        public async Task<Task> ChangePassword(string username, string newPassword)
        {
            await _userData.ChangePassword(username, newPassword);
            return Task.CompletedTask;
          
        }

        public Task<Task> Delete(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserDTO>> GetAll()
        {
            try
            {
                var users = await _userData.GetAll();
                var usersDto = _mapper.Map<IEnumerable<UserDTO>>(users);
                return usersDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<UserDTO>> GetAllWithRoles()
        {
            try
            {
                var users = await _userData.GetAllWithRoles();
                var usersDto = _mapper.Map<IEnumerable<UserDTO>>(users);
                return usersDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserDTO> GetByUsername(string username)
        {

            try
            {
                var user = await _userData.GetByUsername(username);
                var userDto = _mapper.Map<UserDTO>(user);
                return userDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           }

        public async Task<UserDTO> GetUserWithRoles(string username)
        {
            try
            {
                var user = await _userData.GetUserWithRoles(username);
                var userDto = _mapper.Map<UserDTO>(user);
                return userDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Task> Insert(UserCreateDTO entity)
        {
            try
            {
                var user =  _mapper.Map<User>(entity);
                await _userData.Insert(user);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserDTO> Login(LoginDTO loginDTO)
        {
            try
            {
                var user = await _userData.Login(loginDTO.Username, loginDTO.Password);
                var userDto = _mapper.Map<UserDTO>(user);
                return userDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
