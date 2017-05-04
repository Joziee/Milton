using Milton.Database;
using Milton.Database.Models.Business;
using Milton.Database.Models.Security;
using Milton.Services.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Milton.Services.Business
{
    public class PersonService : IPersonService
    {
        #region Constants
        protected const string CACHE_KEY_PERSON_PATTERN = "Milton.Cache.Person";
        protected const string CACHE_KEY_PERSON_BY_ID = "Milton.Cache.Person.GetById({0})";
        protected const string CACHE_KEY_PERSON_BY_ORGANIZATION_ID = "Milton.Cache.Person.GetByOrganizationId({0})";
        protected const string CACHE_KEY_PERSON_BY_EMAIL = "Milton.Cache.Person.GetByEmail({0})";
        protected const string CACHE_KEY_PERSON_BY_GUID = "Milton.Cache.Person.GetByGuid({0})";
        protected const string CACHE_KEY_PERSON_ALL = "Milton.Cache.Person.GetAll({0},{1})";
        protected const string CACHE_KEY_PERSON_SALT = "Milton.Cache.Person.GetPersonSalt({0})";
        protected const string CACHE_KEY_PERSON_PASSWORD = "Milton.Cache.Person.GetPersonPassword({0})";
        protected const string CACHE_KEY_PERSON_ROLE = "Milton.Cache.Person.GetByRole({0},{1},{2})";
        protected const string CACHE_KEY_PERSON_ROLES = "Milton.Cache.Person.GetPersonRoles({0})";
        protected const string CACHE_KEY_PERSON_EXCLUDING_ROLES = "Milton.Cache.Person.GetByExcludingRoles({0},{1})";
        #endregion

        #region Fields
        protected ICacheManager _cacheManager;
        protected IDataRepository<Person> _personRepository;
        protected IDataRepository<Role> _roleRepo;
        #endregion

        #region Constructor
        public PersonService(ICacheManager cacheManager,
            IDataRepository<Person> personRepository,
            IDataRepository<Role> roleRepo
            )
        {
            _cacheManager = cacheManager;
            _personRepository = personRepository;
            _roleRepo = roleRepo;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public virtual Person Register(Person person)
        {
            var existingPerson = GetByEmail(person.Email);
            if (existingPerson != null)
            {
                throw new Exception("The email is already in use! Please use a different email or try logging in.");
            }

            //if (String.IsNullOrEmpty(person.PasswordSalt)) person.PasswordSalt = Storefront.Core.Crypto.Hash.GetSalt();
            //hash the password
            //person.Password = Storefront.Core.Crypto.Hash.GetHash(person.Password, person.PasswordSalt);

            InsertPerson(person);
            return person;
        }


        /// <summary>
        /// Gets the user profile for a person with the specified email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public virtual Person GetByEmail(string email)
        {
            String key = String.Format(CACHE_KEY_PERSON_BY_EMAIL, email);
            return _cacheManager.Get<Person>(key, 10, () =>
            {
                return _personRepository.Table.FirstOrDefault(p => p.Email == email);
            });
        }

        /// <summary>
        /// Gets the user profile for a person with the specified id
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public virtual Person GetById(Int32 personId)
        {
            String key = String.Format(CACHE_KEY_PERSON_BY_ID, personId);
            return _cacheManager.Get<Person>(key, 10, () =>
            {
                return _personRepository.Table.FirstOrDefault(p => p.PersonId == personId);
            });
        }

        /// <summary>
        /// Gets a list of Person objects with the specified Ids
        /// </summary>
        /// <param name="personIds">The Ids of the Persons to look for</param>
        /// <returns>A list of Person objects</returns>
        public virtual List<Person> GetByIds(Int32[] personIds)
        {
            List<Person> people = new List<Person>();

            foreach (var personId in personIds) people.Add(GetById(personId));

            return people;
        }

        /// <summary>
        /// Get the person with the specified guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public virtual Person GetByGuid(Guid guid)
        {
            String key = String.Format(CACHE_KEY_PERSON_BY_GUID, guid);
            return _cacheManager.Get<Person>(key, 10, () =>
            {
                return _personRepository.Table.Include(x=>x.Roles).FirstOrDefault(p => p.PersonGuid == guid);
            });
        }

        /// <summary>
        /// Returns a list of all Person objects
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>A list of Person objects</returns>
        public virtual List<Person> GetAll(Int32 page = 0, Int32 pageSize = Int32.MaxValue)
        {
            String key = String.Format(CACHE_KEY_PERSON_ALL, page, pageSize);
            return _cacheManager.Get<List<Person>>(key, 10, () =>
            {
                var result = _personRepository.Table;

                //Sorting
                result = result.OrderBy(p => p.Surname).OrderBy(p => p.FirstName);

                //Paging
                result = result.Skip(page * pageSize).Take(pageSize);

                return result.ToList();
            });
        }

        /// <summary>
        /// Gets a list of Person objects for the specified role
        /// </summary>
        /// <param name="systemRole">The system role to use as a filter</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>List of Person objects belonging to the specified role</returns>
        public virtual List<Person> GetBySystemRole(SystemRoles systemRole, Int32 page = 0, Int32 pageSize = Int32.MaxValue)
        {
            String sr = systemRole.ToString();
            String key = String.Format(CACHE_KEY_PERSON_ROLE, sr, page, pageSize);

            return _cacheManager.Get<List<Person>>(key, 10, () =>
            {

                var role = _roleRepo.Table.Where(r => r.IsSystemRole && r.SystemName == sr).FirstOrDefault();
                var result = _personRepository.Table.Where(p => p.Roles.Any(r => r.RoleId == role.RoleId));

                return result.ToList<Person>();
            });
        }

        public virtual List<Person> GetByExcludingRoles(Int32 page = 0, Int32 pageSize = Int32.MaxValue)
        {
            String key = String.Format(CACHE_KEY_PERSON_EXCLUDING_ROLES, page, pageSize);

            return _cacheManager.Get<List<Person>>(key, 10, () =>
            {
                var result = _personRepository.Table.Include(x => x.Roles).Where(p => p.Roles.Count == 0);

                return result.ToList();
            });
        }

        /// <summary>
        /// Gets a list of all Role objects linked to a Person
        /// </summary>
        /// <param name="personId">The id of the Person</param>
        /// <returns>A list of Role obejcts</returns>
        public virtual List<Role> GetPersonRoles(Int32 personId)
        {
            String key = String.Format(CACHE_KEY_PERSON_ROLES, personId);
            return _cacheManager.Get<List<Role>>(key, 10, () =>
            {
                var result = _personRepository.Table.FirstOrDefault(r => r.PersonId == personId).Roles;

                return result.ToList();
            });
        }

        /// <summary>
        /// Gets the password salt for a specific person
        /// </summary>
        /// <param name="personId">The id of the person</param>
        /// <returns>The person's password salt</returns>
        public virtual String GetPersonSalt(Int32 personId)
        {
            String key = String.Format(CACHE_KEY_PERSON_SALT, personId);
            return _cacheManager.Get<String>(key, 10, () =>
            {
                //Query
                return _personRepository.Table.Where(p => p.PersonId == personId).FirstOrDefault().PasswordSalt;
            });
        }

        /// <summary>
        /// Gets the password salt for a specific person
        /// </summary>
        /// <param name="person">The person</param>
        /// <returns>THe person's password salt</returns>
        public virtual String GetPersonSalt(Person person)
        {
            return GetPersonSalt(person.PersonId);
        }

        /// <summary>
        /// Gets the password for the specified person
        /// </summary>
        /// <param name="personId">The id of the person</param>
        /// <returns>The person's password</returns>
        public virtual String GetPersonPassword(Int32 personId)
        {
            String key = String.Format(CACHE_KEY_PERSON_PASSWORD, personId);
            return _cacheManager.Get<String>(key, 10, () =>
            {
                //Query
                return _personRepository.Table.Where(p => p.PersonId == personId).FirstOrDefault().Password;
            });
        }

        /// <summary>
        /// Gets the password for the specified person
        /// </summary>
        /// <param name="person">The person</param>
        /// <returns>The person's password</returns>
        public virtual String GetPersonPassword(Person person)
        {
            return GetPersonPassword(person.PersonId);
        }

        /// <summary>
        /// Insert a new person
        /// </summary>
        /// <param name="person">The person to insert into the repository</param>
        public virtual void InsertPerson(Person person)
        {
            _personRepository.Insert(person);
            _cacheManager.RemoveByPattern(CACHE_KEY_PERSON_PATTERN);
        }

        /// <summary>
        /// Updates a person
        /// </summary>
        /// <param name="person">The person to update</param>
        public virtual void UpdatePerson(Person person)
        {
            _personRepository.Update(person);
            _cacheManager.RemoveByPattern(CACHE_KEY_PERSON_PATTERN);
        }

        /// <summary>
        /// Deletes a person
        /// </summary>
        /// <param name="person">The person to delete</param>
        public virtual void DeletePerson(Person person)
        {
            UpdatePerson(person);
            _personRepository.Delete(person);

        }
    }
}