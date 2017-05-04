using Milton.Database.Common;
using Milton.Database.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Business
{
    public class Person : BaseEntity
    {
        #region Constructor
        public Person()
        {
            this.PersonGuid = Guid.NewGuid();
            this.Roles = new HashSet<Role>();
            this.PersonStatus = PersonStatus.Enabled;
        }
        #endregion

        #region Value Properties
        public int PersonId { get; set; }
        public Guid PersonGuid { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }

        public string PasswordSalt { get; set; }
        public string Password { get; set; }
        internal string PersonStatusEnum { get; set; }
        #endregion

        #region Enumeration Properties
        public PersonStatus PersonStatus
        {
            get { return (PersonStatus)Enum.Parse(typeof(PersonStatus), this.PersonStatusEnum); }
            set { this.PersonStatusEnum = value.ToString(); }
        }
        #endregion

        #region Navigation Properties
        public ICollection<Role> Roles { get; set; }
        #endregion
    }
}
