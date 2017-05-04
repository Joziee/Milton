using Milton.Database;
using Milton.Database.Models.Security;
using Milton.Services.Business;
using Milton.Services.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Milton.Database.Models.Business;
using System.Web;

namespace Milton.Services.Security
{
    public class SecurityService : ISecurityService
    {
        #region Constants
        protected const String PATTERN = "Milton.Cache.Security";

        protected const String KEY_GET_ROLE_BY_ID = "Milton.Cache.Security.GetRoleById({0})";
        protected const String KEY_GET_SYSTEM_ROLE = "Milton.Cache.Security.GetSystemRole({0})";
        protected const String KEY_GET_ROLES = "Milton.Cache.Security.GetRoles({0},{1},{2},{3})";
        protected const String KEY_GET_ROLES_BY_PERSON_ID = "Milton.Cache.Security.GetRolesByPersonId({0},{1},{2},{3})";
        protected const String KEY_IN_ROLE_BY_ID = "Milton.Cache.Security.IsInRoleById({0},{1})";
        protected const String KEY_IN_ROLE_BY_NAME = "Milton.Cache.Security.IsInRoleByName({0},{1})";
        protected const String KEY_IN_ROLES_BY_NAMES = "Milton.Cache.Security.IsInRolesByNames({0},{1})";
        #endregion

        #region Fields
        protected ICacheManager _cache;
        protected IDataRepository<Role> _roleRepo;
        protected IPersonService _personService;
        #endregion

        #region Constructor
        public SecurityService(
            ICacheManager cache,
            IDataRepository<Role> roleRepo,
            IPersonService personService)
        {
            _cache = cache;
            _roleRepo = roleRepo;
            _personService = personService;
        }
        #endregion

        #region ISecurityService

        #region Roles
        /// <summary>
        /// Create a new role
        /// </summary>
        /// <param name="role"></param>
        public virtual void InsertRole(Role role)
        {
            if (role == null) throw new ArgumentNullException("role");

            _roleRepo.Insert(role);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        /// <param name="role"></param>
        public virtual void UpdateRole(Role role)
        {
            if (role == null) throw new ArgumentNullException("role");

            _roleRepo.Update(role);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Permanently deletes a role
        /// </summary>
        /// <param name="role"></param>
        public virtual void DeleteRole(Role role)
        {
            if (role == null) throw new ArgumentNullException("role");

            _roleRepo.Delete(role);
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Get the role with the specified row id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public virtual Role GetRoleById(int roleId)
        {
            String key = String.Format(KEY_GET_ROLE_BY_ID, roleId);
            return _cache.Get<Role>(key, 10, () =>
            {
                //Query
                var result = _roleRepo.Table.Include(x => x.People).FirstOrDefault(x => x.RoleId == roleId);

                //Return
                return result;
            });
        }

        /// <summary>
        /// Get the roles with the specified ids
        /// </summary>
        /// <param name="roleIds">The role ids to find</param>
        /// <returns>A list of role objects</returns>
        public virtual List<Role> GetRolesByIds(Int32[] roleIds)
        {
            List<Role> roles = new List<Role>();

            foreach (Int32 roleId in roleIds)
                roles.Add(GetRoleById(roleId));

            return roles;
        }

        /// <summary>
        /// Get the role with the specified system name
        /// </summary>
        /// <param name="systemName"></param>
        /// <returns></returns>
        public virtual Role GetSystemRole(SystemRoles role)
        {
            String r = role.ToString();
            String key = String.Format(KEY_GET_SYSTEM_ROLE, r);
            return _cache.Get<Role>(key, 10, () =>
            {
                //Query
                var result = _roleRepo.Table.FirstOrDefault(c => c.SystemName == r);

                //Return
                return result;
            });
        }

        /// <summary>
        /// Get all roles that match the criteria
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="onlySystemRoles"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual List<Role> GetRoles(bool? enabled = true, Nullable<Boolean> onlySystemRoles = null, int page = 0, int pageSize = Int32.MaxValue)
        {
            if (page < 0) throw new ArgumentOutOfRangeException("page");
            if (pageSize <= 0) throw new ArgumentOutOfRangeException("pageSize");

            String key = String.Format(KEY_GET_ROLES, enabled, onlySystemRoles, page, pageSize);
            return _cache.Get<List<Role>>(key, 10, () =>
            {
                //Query
                var result = _roleRepo.Table;

                //Filter
                if (enabled.HasValue) result = result.Where(r => r.Enabled == enabled.Value);
                if (onlySystemRoles.HasValue) result = result.Where(r => r.IsSystemRole == onlySystemRoles.Value);

                //Sort
                result = result.OrderBy(r => r.Name);

                //Page
                result = result.Skip(page * pageSize)
                    .Take(pageSize);

                //Return
                return result.ToList();
            });
        }

        /// <summary>
        /// Get the roles for the specified person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="enabled"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual List<Role> GetRolesByPersonId(int personId, Nullable<Boolean> enabled = null, int page = 0, int pageSize = Int32.MaxValue)
        {
            if (page < 0) throw new ArgumentOutOfRangeException("page");
            if (pageSize <= 0) throw new ArgumentOutOfRangeException("pageSize");

            String key = String.Format(KEY_GET_ROLES_BY_PERSON_ID, personId, enabled, page, pageSize);
            return _cache.Get<List<Role>>(key, 10, () =>
            {
                //Query
                var result = _roleRepo.Table.Where(r => r.People.Any(m => m.PersonId == personId));

                //Filter
                if (enabled.HasValue) result = result.Where(r => r.Enabled == enabled.Value);

                var listResult = result.ToList();

                //Sort
                listResult = listResult.OrderBy(r => r.Name).ToList();

                //Page
                listResult = listResult.Skip(page * pageSize).Take(pageSize).ToList();

                //Return
                return listResult;
            });
        }

        /// <summary>
        /// Determines if the person has the specified role
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public virtual bool IsInRole(int personId, int roleId)
        {
            String key = String.Format(KEY_IN_ROLE_BY_ID, personId, roleId);
            return _cache.Get<Boolean>(key, 10, () =>
            {
                return _roleRepo.Table.Any(r => r.RoleId == roleId && r.People.Any(p => p.PersonId == personId));
            });
        }

        /// <summary>
        /// Determine if the person has the specified role
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="systemName"></param>
        /// <returns></returns>
        public virtual bool IsInRole(int personId, string name)
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name");

            String key = String.Format(KEY_IN_ROLE_BY_NAME, personId, name);
            return _cache.Get<Boolean>(key, 10, () =>
            {
                return _roleRepo.Table.Any(r => r.Name == name && r.People.Any(p => p.PersonId == personId));
            });
        }

        /// <summary>
        /// Determine if the person is ins one or more of the specified roles
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public virtual bool IsInRoles(int personId, string[] roles)
        {
            if (roles.Length == 0) throw new ArgumentException("roles");

            String key = String.Format(KEY_IN_ROLES_BY_NAMES, personId, String.Join(",", roles));
            return _cache.Get<Boolean>(key, 10, () =>
            {
                Boolean contains = _roleRepo.Table.Any(r => roles.Contains(r.Name) && r.People.Any(p => p.PersonId == personId));

                return contains;
            });
        }

        /// <summary>
        /// Add a person to the specified role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="personId"></param>
        public virtual void AddPersonToRole(int roleId, int personId)
        {
            //Get the role
            Role role = GetRoleById(roleId);
            if (role == null) throw new ArgumentException("roleId");

            //Get the person
            Person person = _personService.GetById(personId);
            if (person == null) throw new ArgumentException("personId");

            //Check if the person is already associated with the role
            if (_roleRepo.Table.Any(r => r.RoleId == roleId && r.People.Any(p => p.PersonId == personId))) return;

            //Add the person to the role
            role.People.Add(person);
            UpdateRole(role);

            //Clear the cache
            _cache.RemoveByPattern(PATTERN);
        }

        /// <summary>
        /// Remove a person from a role
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="personId"></param>
        public virtual void RemovePersonFromRole(int roleId, int personId)
        {
            //Get the role
            Role role = GetRoleById(roleId);
            if (role == null) throw new ArgumentException("roleId");

            //Get the person
            Person person = _personService.GetById(personId);
            if (person == null) throw new ArgumentException("personId");

            //Check if the person is already associated with the role
            if (!_roleRepo.Table.Any(r => r.RoleId == roleId && r.People.Any(p => p.PersonId == personId))) return;

            //Remove the person to the role
            role.People.Remove(person);
            UpdateRole(role);

            //Clear the cache
            _cache.RemoveByPattern(PATTERN);
        }
        #endregion
        #endregion

        #region Login
        /// <summary>
        /// Authenticates a user and optionally generates a new session id
        /// </summary>
        /// <param name="email">The email address of the user</param>
        /// <param name="password">The clear text passwords of the user</param>
        /// <param name="sessionId">The session id of the user</param>
        /// <param name="newSession">Determines if a new session id should be generated</param>
        /// <returns></returns>
        public bool Authenticate(string email, string password, out string sessionId, bool newSession = true)
        {
            //Check for arguments
            if (String.IsNullOrWhiteSpace(email)) throw new ArgumentNullException("email");
            if (String.IsNullOrWhiteSpace(password)) throw new ArgumentNullException("password");

            //Set default values
            sessionId = null;
            email = email.Trim();
            password = password.Trim();     //Don't allow whitespace characters in passwords

            //Get the person with matching email
            var person = _personService.GetByEmail(email);
            if (person == null) return false;

            //Calculate the password hash
            string hash = Hash.GetHash(password, person.PasswordSalt);
            if (hash != person.Password) return false;

            //Generate a new session id if neccesary
            if (newSession)
            {
                person.PersonGuid = Guid.NewGuid();
                _personService.UpdatePerson(person);
            }

            //Success
            sessionId = person.PersonGuid.ToString();
            return true;
        }

        /// <summary>
        /// Sets the auth cookie for the user.
        /// </summary>
        /// <remarks>
        /// Only works when there is a a HttpContext
        /// </remarks>
        /// <param name="sessionId"></param>
        public void SetAuthCookie(string sessionId, DateTime? expires = null)
        {
            //Ignore if there is no http context
            if (HttpContext.Current == null) return;

            //Get the http context request and response streams
            var request = HttpContext.Current.Request;
            var response = HttpContext.Current.Response;

            //Create the cookie
            HttpCookie authCookie = new HttpCookie("UserAuth", sessionId);
            if (expires.HasValue) authCookie.Expires = expires.Value;
            if (request.Cookies["UserAuth"] != null) response.Cookies.Set(authCookie);
            else response.Cookies.Add(authCookie);
        }

        /// <summary>
        /// Check if the auth cookie exists and is valid
        /// </summary>
        /// <remarks>
        /// Only works when there is a a HttpContext
        /// </remarks>
        /// <returns></returns>
        public bool CheckAuthCookie(out String sessionId)
        {
            //By default the sessionId is null
            sessionId = null;

            //Ignore if there is no http context
            if (HttpContext.Current == null) return false;

            //Get the http context request and response streams
            var request = HttpContext.Current.Request;

            //Check if a user is logged in (or return the login view)
            HttpCookie cookie = request.Cookies["UserAuth"];
            if (cookie == null) return false;

            //Check if the user login is valid
            Guid guid = Guid.Empty;
            if (!Guid.TryParse(cookie.Value, out guid)) return false;

            //Find the user with the session id
            var person = _personService.GetByGuid(guid);
            if (person == null) return false;

            //OK
            sessionId = cookie.Value;
            return true;
        }
        #endregion
    }
}
