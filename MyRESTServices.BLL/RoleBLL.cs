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
    public class RoleBLL : IRoleBLL
    {
        private readonly IRoleData _roleData;
        private readonly IMapper _mapper;

        public RoleBLL(IRoleData roleData, IMapper mapper)
        {
            _roleData = roleData;
            _mapper = mapper;
        }

        public Task<Task> AddRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public async Task<Task> AddUserToRole(string username, int roleId)
        {
            try
            {
                var addUserToRole = await _roleData.AddUserToRole(username, roleId);
                return addUserToRole;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<IEnumerable<RoleDTO>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RoleDTO>> GetAllRoles()
        {
            try
            {
                var roles = await _roleData.GetAllRoles();
                var rolesDto = _mapper.Map<IEnumerable<RoleDTO>>(roles);
                return rolesDto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }
    }
}


