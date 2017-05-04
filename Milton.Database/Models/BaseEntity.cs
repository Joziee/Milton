using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milton.Database.Models
{
	public abstract class BaseEntity
	{
		public BaseEntity()
		{
			this.CreatedOnUtc = DateTime.Now;
			this.ModifiedOnUtc = DateTime.Now;
			this.ModifiedByUserId = 0;
		}

		public DateTime CreatedOnUtc { get; set; }
		public DateTime ModifiedOnUtc { get; set; }
		public Int32 ModifiedByUserId { get; set; }
	}
}
