using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EntitiesConfigurations
{
    public class DepartmentsConfiguration : IEntityTypeConfiguration<Departments>
    {
        public void Configure(EntityTypeBuilder<Departments> builder)
        {
            builder.HasKey(e => e.ID);
            builder.Property(x => x.ID).HasDefaultValueSql("NEWID()");
        }
    } 
    
}
