using Milton.Database.Models.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Security
{
    public class Role : BaseEntity
    {
        #region Constructor
        public Role()
        {
            this.Enabled = true;
            this.People = new HashSet<Person>();
        }
        #endregion

        #region Value Properties
        public Int32 RoleId { get; set; }
        public String Name { get; set; }
        public String SystemName { get; set; }
        public Boolean IsSystemRole { get; set; }
        public Boolean Enabled { get; set; }
        #endregion

        #region Navigation Properties
        public ICollection<Person> People { get; set; }
        #endregion
    }
}
