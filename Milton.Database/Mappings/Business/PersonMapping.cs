using Milton.Database.Models.Business;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace Milton.Database.Mappings.Business
{
    public class PersonMapping : EntityTypeConfiguration<Person>
    {
        public PersonMapping()
        {
            //Map to table
            this.ToTable("Person");

            //Define primary key
            this.HasKey(a => a.PersonId);

            #region Foreign Keys
            //Roles
            this.HasMany(a => a.Roles)
                .WithMany(a => a.People)
                .Map(a =>
                {
                    a.MapLeftKey("PersonId");
                    a.MapRightKey("RoleId");
                    a.ToTable("RolePerson");
                });

            #endregion

            #region Column Specifications

            this.Property(a => a.CreatedOnUtc).IsRequired();
            this.Property(a => a.ModifiedOnUtc).IsRequired();
            this.Property(a => a.PersonGuid).IsRequired();

            this.Property(a => a.FirstName).HasMaxLength(32);
            this.Property(a => a.Surname).HasMaxLength(32);

            this.Property(a => a.Mobile).HasMaxLength(16);

            this.Property(a => a.Email).IsRequired().HasMaxLength(128);

            this.Property(a => a.PasswordSalt).IsOptional().HasMaxLength(255);
            this.Property(a => a.Password).IsRequired().HasMaxLength(255);

            this.Property(a => a.PersonStatusEnum).IsRequired().HasMaxLength(16);

            this.Ignore(a => a.PersonStatus);

            #endregion
        }
    }
}
