namespace ltracker.Data
{
    using Entities;
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.RegularExpressions;

    public class LearningContext : DbContext
    {   
        // El contexto se ha configurado para usar una cadena de conexión 'LearningContext' del archivo 
        // de configuración de la aplicación (App.config o Web.config). De forma predeterminada, 
        // esta cadena de conexión tiene como destino la base de datos 'ltracker.Data.LearningContext' de la instancia LocalDb. 
        // 
        // Si desea tener como destino una base de datos y/o un proveedor de base de datos diferente, 
        // modifique la cadena de conexión 'LearningContext'  en el archivo de configuración de la aplicación.
        public LearningContext()
            : base("name=LearningContext")
        {
        }

        private string GetTableName(Type type)
        {
            var result = Regex.Replace(type.Name, ".[A-Z]", m => m.Value[0] + "_" + m.Value[1]);

            return result.ToLower();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Por default en postgresql el schema es public, NO dbo como sql server.
            modelBuilder.HasDefaultSchema("public");
            //nombres de propiedades en minúscula como pide postgresql
            modelBuilder.Properties().Configure(p => p.HasColumnName(p.ClrPropertyInfo.Name.ToLower()));
            //nombre de tablas en minúscula como lo pide postgresql
            modelBuilder.Types().Configure(c => c.ToTable(GetTableName(c.ClrType)));
            //Precisión por defecto de decimales a menos que se especifique otro
            modelBuilder.Properties<decimal>().Configure(config => config.HasPrecision(10, 2));
            //Todos los id son claves primarias
            modelBuilder.Properties().Where(x => x.Name.ToLower().Equals("id")).Configure(config => config.IsKey());
            
            //Se pueden crear EntityTypeConfigurations, en este caso es pequeño y se pusieron en el context

            var individual = modelBuilder.Entity<Individual>();
            individual.ToTable("individuals");
            individual.HasKey(x => x.Id);
            individual.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            individual.Property(x => x.Email).HasMaxLength(500).IsRequired();
            individual.Property(x => x.Name).HasMaxLength(500).IsRequired();

            var topic = modelBuilder.Entity<Topic>();
            topic.ToTable("topics");
            topic.HasKey(x => x.Id);
            topic.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            topic.Property(x => x.Name).HasMaxLength(500).IsRequired();
            topic.Property(x => x.Description).HasMaxLength(500).IsRequired();
            
            topic.HasMany(x => x.Courses);

            var course = modelBuilder.Entity<Course>();
            course.ToTable("courses");
            course.HasKey(x => x.Id);
            course.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            course.Property(x => x.Name).HasMaxLength(500).IsRequired();
            course.Property(x => x.Description).HasMaxLength(500).IsOptional();
            course.Property(x => x.DurationAVG).IsOptional();
            
            course.HasMany(x => x.Topics).WithMany().Map(rel => {
                rel.MapLeftKey("courseid");
                rel.MapRightKey("topicid");
                rel.ToTable("courses_in_topics");
            });

            var assignedCourse = modelBuilder.Entity<AssignedCourse>();
            assignedCourse.ToTable("assignedcourses");
            assignedCourse.HasKey(x => x.Id);
            assignedCourse.Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            assignedCourse.Property(x => x.isCompleted).IsRequired();
            assignedCourse.Property(x => x.StartDate).IsOptional();
            assignedCourse.Property(x => x.FinishDate).IsOptional();
            assignedCourse.Property(x => x.TotalHours).IsOptional();
            assignedCourse.Property(x => x.AssignmentDate).IsRequired();
            assignedCourse.HasRequired(x => x.Invidivual).WithMany().HasForeignKey(x => x.IndividualId);
            assignedCourse.HasRequired(x => x.Course).WithMany().HasForeignKey(x => x.CourseId);

            base.OnModelCreating(modelBuilder);
        }

        // Agregue un DbSet para cada tipo de entidad que desee incluir en el modelo. Para obtener más información 
        // sobre cómo configurar y usar un modelo Code First, vea http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Individual> Individuals { get; set; }
        public virtual DbSet<AssignedCourse> AssignedCourses { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Topic> Topics { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}