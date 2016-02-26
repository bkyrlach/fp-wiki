using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

namespace fp_wiki.DataContext
{
    public class FpWikiDataContext : DbContext
    {
        private Guid _instanceIdentifier;
        private bool _isDisposed;

        public DbSet<MethodDescriptor> MethodDescriptors { get; set; }
        public DbSet<ParameterDescriptor> ParameterDescriptors { get; set; }

        public FpWikiDataContext()
            : base("Server=APNSQL-DEV;Database=fp_wiki;Trusted_Connection=Yes;")
        {
            _instanceIdentifier = Guid.NewGuid();
            SetCommandTimeout();
            Configuration.UseDatabaseNullSemantics = true;
        }

        public void SetCommandTimeout(int seconds = 300)
        {
            ((IObjectContextAdapter) this).ObjectContext.CommandTimeout = seconds;
        }

        public string GetIdentifier
        {
            get { return _instanceIdentifier.ToString(); }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                base.Dispose(disposing);

                _isDisposed = true;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<FpWikiDataContext>(null);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //Alert to AlertRecipient
            modelBuilder.Entity<MethodDescriptor>()
                .HasMany(md => md.Parameters)
                .WithRequired(p => p.Method);

            modelBuilder.Entity<MethodDescriptor>()
                .HasRequired(md => md.HelpContent)
                .WithRequiredPrincipal(hc => hc.Method);
        }
    }
    [Table("Method")]
    public class MethodDescriptor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public HelpContent HelpContent { get; set; }
        public ICollection<ParameterDescriptor> Parameters { get; set; } 
    }
    [Table("Parameter")]
    public class ParameterDescriptor
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public MethodDescriptor Method { get; set; }
        public int ParameterOrder { get; set; }
    }

    public class HelpContent
    {
        public int Id { get; set; }
        public MethodDescriptor Method { get; set; } 

    }
}