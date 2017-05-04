using Milton.Database.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Security
{
    public interface ISecurityService
    {
        #region Roles
        void InsertRole(Role role);
        void UpdateRole(Role role);
        void DeleteRole(Role role);

        Role GetRoleById(Int32 roleId);
        List<Role> GetRolesByIds(Int32[] roleIds);
        Role GetSystemRole(SystemRoles role);

        List<Role> GetRoles(Boolean? enabled = true, Nullable<Boolean> onlySystemRoles = null, Int32 page = 0, Int32 pageSize = Int32.MaxValue);
        List<Role> GetRolesByPersonId(Int32 personId, Nullable<Boolean> enabled = null, Int32 page = 0, Int32 pageSize = Int32.MaxValue);

        Boolean IsInRole(Int32 personId, Int32 roleId);
        Boolean IsInRole(Int32 personId, String name);
        Boolean IsInRoles(Int32 personId, String[] roles);

        void AddPersonToRole(Int32 roleId, Int32 personId);
        void RemovePersonFromRole(Int32 roleId, Int32 personId);
        #endregion

        #region Login
        bool Authenticate(String email, string password, out string sessionId, bool newSession = false);
        void SetAuthCookie(String sessionId, DateTime? expires = null);
        bool CheckAuthCookie(out String sessionId);
        #endregion
    }
}
