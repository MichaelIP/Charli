using McpNetwork.Charli.Server.Helpers;
using McpNetwork.Charli.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace McpNetwork.Charli.Managers.DatabaseManager.DataModel
{
    internal partial class CharliEntities : DbContext
    {

        private readonly static object dbCreationLocker = new object();

        public string databasePath { get; set; }

        public virtual DbSet<AccessControl> AccessControls { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Plugin> Plugins { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSecurityCode> UserSecurityCodes { get; set; }

        public CharliEntities(string dbPath)
        {
            this.databasePath = dbPath;
            lock (dbCreationLocker)
            {
                if (!File.Exists(this.databasePath))
                {
                    this.Database.EnsureCreated();
                    this.SeedInitialData();
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(String.Format("Filename={0}", this.databasePath));
        }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{


        //    modelBuilder.Entity<Device>(entity =>
        //    {
        //        entity.Property(e => e.CreationDate).HasDefaultValueSql("(getdate())");

        //        entity.Property(e => e.UpdateDate).HasDefaultValueSql("(getdate())");

        //        //entity.HasOne(d => d.User)
        //        //    .WithMany(p => p.Device)
        //        //    .HasForeignKey(d => d.UserId)
        //        //    .OnDelete(DeleteBehavior.ClientSetNull)
        //        //    .HasConstraintName("FK_Device_User");
        //    });

        //    modelBuilder.Entity<Schedule>(entity =>
        //    {
        //        entity.Property(e => e.Active).HasDefaultValueSql("((1))");
        //    });

        //    modelBuilder.Entity<User>(entity =>
        //    {
        //        entity.Property(e => e.Right1).HasDefaultValueSql("((1))");
        //    });

        //    modelBuilder.Entity<UserSecurityCode>(entity =>
        //    {
        //        entity.HasIndex(e => e.Token)
        //            .HasDatabaseName("IX_UserSecurityCode");

        //        entity.HasOne(d => d.User)
        //            .WithMany(p => p.UserSecurityCode)
        //            .HasForeignKey(d => d.UserId)
        //            .OnDelete(DeleteBehavior.ClientSetNull)
        //            .HasConstraintName("FK_UserSecurityCode_Users");
        //    });
        //}

        internal void SeedInitialData()
        {

            if (this.Users.Any())
            {
                return;
            }

            var encryptedPassword = SecurityHelper.HashPassword("admin", out string securityCode);

            this.Users.Add(
                new User
                {
                    UserName = "admin",
                    FirstName = "Administrator",
                    LastName = "Administrator",
                    Password = encryptedPassword,
                    SecurityCode = securityCode,
                    Active = true,
                    Right1 = 0xff,
                    Right2 = 0xff,
                    Right3 = 0xff,
                    Right4 = 0xff,
                    Right5 = 0xff,
                    CreationDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                }
            );

            this.AccessControls.AddRange(
                    new AccessControl
                    {
                        Name = "Authenticated",
                        SetNb = 0,
                        BitNb = 0,
                        CreationDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    },
                    new AccessControl
                    {
                        Name = "Administrator",
                        SetNb = 0,
                        BitNb = 1,
                        CreationDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    },
                    new AccessControl
                    {
                        Name = "PowerManager",
                        SetNb = 0,
                        BitNb = 2,
                        CreationDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    }
                );

            this.SaveChanges();

        }

    }
}
