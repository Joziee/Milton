using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models.Configuration
{
    public class Setting : BaseEntity
    {
        public Setting()
        {
            //Set default values
            this.CreatedOnUtc = DateTime.Now;
            this.ModifiedOnUtc = DateTime.Now;
        }

        public System.Int32 SettingId { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public System.DateTime ModifiedOnUtc { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
