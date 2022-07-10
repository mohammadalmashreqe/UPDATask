using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntitiesConfigurations
{
    public class EmployeePositionConfiguration : IEntityTypeConfiguration<EmployeePosition>
    {
        public void Configure(EntityTypeBuilder<EmployeePosition> builder)
        {
         

            builder.HasKey(x => new { x.EmployeeID, x.PositionID });
            builder.HasOne(x => x.Employee).
                WithMany(p => p.UserPositions).
                HasForeignKey(bc => bc.EmployeeID);


          
            builder.HasOne(x => x.Position).
                WithMany(p => p.UserPositions).
                HasForeignKey(bc => bc.PositionID);
        }
    }     
    
}
