using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.ID);
            builder.Property(x => x.ID).HasDefaultValueSql("NEWID()");

          
            builder
            .HasOne(s => s.Department)
            .WithMany(g => g.Employees)
            .HasForeignKey(s => s.DepartmentId);

            builder
            .HasOne(s => s.User)
            .WithOne(g => g.Employee)
            .HasForeignKey<Employee>(s => s.UserId);

           






        }
    }

}
