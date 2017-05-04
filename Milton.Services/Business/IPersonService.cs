using Milton.Database.Models.Business;
using Milton.Database.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Services.Business
{
    public interface IPersonService
    {
        Person GetByEmail(String email);
        Person GetByGuid(Guid guid);
        Person GetById(Int32 personId);
        List<Person> GetByIds(Int32[] personIds);
        List<Person> GetAll(Int32 page = 0, Int32 pageSize = Int32.MaxValue);
        List<Person> GetBySystemRole(SystemRoles systemRole, Int32 page = 0, Int32 pageSize = Int32.MaxValue);
        List<Role> GetPersonRoles(Int32 personId);
        String GetPersonSalt(Int32 personId);
        String GetPersonSalt(Person person);

        String GetPersonPassword(Int32 personId);
        String GetPersonPassword(Person person);

        void InsertPerson(Person person);
        void UpdatePerson(Person person);
        void DeletePerson(Person person);

        Person Register(Person model);
    }
}
