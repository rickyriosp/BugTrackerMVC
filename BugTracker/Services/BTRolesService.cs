﻿using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTRolesService : IBTRolesService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public BTRolesService(ApplicationDbContext context, UserManager<BTUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AddUserToRoleAsync(BTUser user, string roleName)
        {
            bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;

            return result;
        }
        
        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            try
            {
                List<IdentityRole> result = new();
                result = await _context.Roles.ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Retrieving All Roles. ---> {ex.Message}");
                throw;
            }
        }

        public async Task<string> GetRoleNameByIdAsync(string roleId)
        {
            IdentityRole role = _context.Roles.Find(roleId);
            string result = await _roleManager.GetRoleNameAsync(role);
            
            return result;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(BTUser user)
        {
            IEnumerable<string> result = await _userManager.GetRolesAsync(user);

            return result;
        }

        public async Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            List<BTUser> users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
            List<BTUser> result = users.Where(u => u.CompanyId == companyId).ToList();

            return result;
        }

        public async Task<List<BTUser>> GetUsersNotInRoleAsync(string roleName, int companyId)
        {
            List<string> usersIds = (await _userManager.GetUsersInRoleAsync(roleName)).Select(u => u.Id).ToList();
            List<BTUser> roleUsers = _context.Users.Where(u => !usersIds.Contains(u.Id)).ToList();
            
            List<BTUser> result = roleUsers.Where(u => u.CompanyId == companyId).ToList();

            return result;
        }

        public async Task<bool> RemoveUserFromRoleAsync(BTUser user, string roleName)
        {
            bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;

            return result;
        }

        public async Task<bool> RemoveUserFromRolesAsync(BTUser user, IEnumerable<string> roles)
        {
            bool result = (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;

            return result;
        }
        
        public async Task<bool> IsUserInRoleAsync(BTUser user, string roleName)
        {
            bool result = await _userManager.IsInRoleAsync(user, roleName);

            return result;
        }

    }
}
