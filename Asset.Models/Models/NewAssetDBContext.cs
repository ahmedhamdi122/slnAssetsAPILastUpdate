using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Asset.API.Models
{
    public partial class NewAssetDBContext : DbContext
    {
        public NewAssetDBContext()
        {
        }

        public NewAssetDBContext(DbContextOptions<NewAssetDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicationType> ApplicationTypes { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<AssetDetail> AssetDetails { get; set; }
        public virtual DbSet<AssetDetailAttachment> AssetDetailAttachments { get; set; }
        public virtual DbSet<AssetMovement> AssetMovements { get; set; }
        public virtual DbSet<AssetOwner> AssetOwners { get; set; }
        public virtual DbSet<AssetPeriority> AssetPeriorities { get; set; }
        public virtual DbSet<AssetScrap> AssetScraps { get; set; }
        public virtual DbSet<AssetStatus> AssetStatuses { get; set; }
        public virtual DbSet<AssetStatusTransaction> AssetStatusTransactions { get; set; }
        public virtual DbSet<AssetStockTaking> AssetStockTakings { get; set; }
        public virtual DbSet<AssetWorkOrderTask> AssetWorkOrderTasks { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Building> Buildings { get; set; }
        public virtual DbSet<Calibration> Calibrations { get; set; }
        public virtual DbSet<CalibrationAttachment> CalibrationAttachments { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryType> CategoryTypes { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Classification> Classifications { get; set; }
        public virtual DbSet<CommetieeMember> CommetieeMembers { get; set; }
        public virtual DbSet<ContractAttachment> ContractAttachments { get; set; }
        public virtual DbSet<ContractDetail> ContractDetails { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Ecri> Ecris { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Engineer> Engineers { get; set; }
        public virtual DbSet<ExternalAssetMovement> ExternalAssetMovements { get; set; }
        public virtual DbSet<ExternalAssetMovementAttachment> ExternalAssetMovementAttachments { get; set; }
        public virtual DbSet<ExternalFix> ExternalFixes { get; set; }
        public virtual DbSet<ExternalFixFile> ExternalFixFiles { get; set; }
        public virtual DbSet<Floor> Floors { get; set; }
        public virtual DbSet<Governorate> Governorates { get; set; }
        public virtual DbSet<Hospital> Hospitals { get; set; }
        public virtual DbSet<HospitalApplication> HospitalApplications { get; set; }
        public virtual DbSet<HospitalApplicationAttachment> HospitalApplicationAttachments { get; set; }
        public virtual DbSet<HospitalDepartment> HospitalDepartments { get; set; }
        public virtual DbSet<HospitalEngineer> HospitalEngineers { get; set; }
        public virtual DbSet<HospitalExecludeReason> HospitalExecludeReasons { get; set; }
        public virtual DbSet<HospitalHoldReason> HospitalHoldReasons { get; set; }
        public virtual DbSet<HospitalReasonTransaction> HospitalReasonTransactions { get; set; }
        public virtual DbSet<HospitalSupplierStatus> HospitalSupplierStatuses { get; set; }
        public virtual DbSet<ManufacturerPmasset> ManufacturerPmassets { get; set; }
        public virtual DbSet<MasterAsset> MasterAssets { get; set; }
        public virtual DbSet<MasterAssetAttachment> MasterAssetAttachments { get; set; }
        public virtual DbSet<MasterAssetComponent> MasterAssetComponents { get; set; }
        public virtual DbSet<MasterContract> MasterContracts { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<ModulePermission> ModulePermissions { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<Origin> Origins { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Phase> Phases { get; set; }
        public virtual DbSet<PmassetTask> PmassetTasks { get; set; }
        public virtual DbSet<PmassetTaskSchedule> PmassetTaskSchedules { get; set; }
        public virtual DbSet<PmassetTime> PmassetTimes { get; set; }
        public virtual DbSet<Pmtime> Pmtimes { get; set; }
        public virtual DbSet<Problem> Problems { get; set; }
        public virtual DbSet<Request> Requests { get; set; }
        public virtual DbSet<RequestDocument> RequestDocuments { get; set; }
        public virtual DbSet<RequestMode> RequestModes { get; set; }
        public virtual DbSet<RequestPeriority> RequestPeriorities { get; set; }
        public virtual DbSet<RequestPhase> RequestPhases { get; set; }
        public virtual DbSet<RequestStatus> RequestStatuses { get; set; }
        public virtual DbSet<RequestTracking> RequestTrackings { get; set; }
        public virtual DbSet<RequestType> RequestTypes { get; set; }
        public virtual DbSet<RoleCategory> RoleCategories { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Scrap> Scraps { get; set; }
        public virtual DbSet<ScrapAttachment> ScrapAttachments { get; set; }
        public virtual DbSet<ScrapReason> ScrapReasons { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<StockTakingHospital> StockTakingHospitals { get; set; }
        public virtual DbSet<StockTakingSchedule> StockTakingSchedules { get; set; }
        public virtual DbSet<SubCategory> SubCategories { get; set; }
        public virtual DbSet<SubOrganization> SubOrganizations { get; set; }
        public virtual DbSet<SubProblem> SubProblems { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<SupplierAttachment> SupplierAttachments { get; set; }
        public virtual DbSet<SupplierExeclude> SupplierExecludes { get; set; }
        public virtual DbSet<SupplierExecludeAsset> SupplierExecludeAssets { get; set; }
        public virtual DbSet<SupplierExecludeAttachment> SupplierExecludeAttachments { get; set; }
        public virtual DbSet<SupplierExecludeReason> SupplierExecludeReasons { get; set; }
        public virtual DbSet<SupplierHoldReason> SupplierHoldReasons { get; set; }
        public virtual DbSet<Visit> Visits { get; set; }
        public virtual DbSet<VisitAttachment> VisitAttachments { get; set; }
        public virtual DbSet<VisitType> VisitTypes { get; set; }
        public virtual DbSet<WnpmassetTime> WnpmassetTimes { get; set; }
        public virtual DbSet<WnpmassetTimeAttachment> WnpmassetTimeAttachments { get; set; }
        public virtual DbSet<WorkOrder> WorkOrders { get; set; }
        public virtual DbSet<WorkOrderAssign> WorkOrderAssigns { get; set; }
        public virtual DbSet<WorkOrderAttachment> WorkOrderAttachments { get; set; }
        public virtual DbSet<WorkOrderPeriority> WorkOrderPeriorities { get; set; }
        public virtual DbSet<WorkOrderStatus> WorkOrderStatuses { get; set; }
        public virtual DbSet<WorkOrderTask> WorkOrderTasks { get; set; }
        public virtual DbSet<WorkOrderTracking> WorkOrderTrackings { get; set; }
        public virtual DbSet<WorkOrderType> WorkOrderTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=NewAssetDB;Integrated Security=true;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;MultipleActiveResultSets=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<ApplicationType>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Discriminator).IsRequired();

                entity.Property(e => e.DisplayName).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.RoleId).HasMaxLength(450);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AssetDetail>(entity =>
            {
                entity.Property(e => e.Barcode).HasMaxLength(20);

                entity.Property(e => e.Code).HasMaxLength(15);

                entity.Property(e => e.CostCenter).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(450);

                entity.Property(e => e.DepreciationRate).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FixCost).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Ponumber)
                    .HasMaxLength(50)
                    .HasColumnName("PONumber");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.QrData).HasMaxLength(500);

                entity.Property(e => e.QrFilePath).HasMaxLength(255);

                entity.Property(e => e.SerialNumber).HasMaxLength(20);

                entity.Property(e => e.WarrantyEnd).HasColumnType("date");

                entity.Property(e => e.WarrantyExpires).HasMaxLength(50);

                entity.Property(e => e.WarrantyStart).HasColumnType("date");

                entity.HasOne(d => d.Building)
                    .WithMany(p => p.AssetDetails)
                    .HasForeignKey(d => d.BuildingId)
                    .HasConstraintName("FK_AssetDetails_Buildings");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.AssetDetails)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_AssetDetails_Departments");

                entity.HasOne(d => d.Floor)
                    .WithMany(p => p.AssetDetails)
                    .HasForeignKey(d => d.FloorId)
                    .HasConstraintName("FK_AssetDetails_Floors");

                entity.HasOne(d => d.MasterAsset)
                    .WithMany(p => p.AssetDetails)
                    .HasForeignKey(d => d.MasterAssetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AssetDetails_MasterAssets");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.AssetDetails)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK_AssetDetails_Rooms");
            });

            modelBuilder.Entity<AssetDetailAttachment>(entity =>
            {
                entity.Property(e => e.FileName).HasMaxLength(200);

                entity.Property(e => e.Title).HasMaxLength(100);
            });

            modelBuilder.Entity<AssetMovement>(entity =>
            {
                entity.Property(e => e.MoveDesc).HasMaxLength(500);

                entity.HasOne(d => d.Building)
                    .WithMany(p => p.AssetMovements)
                    .HasForeignKey(d => d.BuildingId)
                    .HasConstraintName("FK_AssetMovements_Buildings");

                entity.HasOne(d => d.Floor)
                    .WithMany(p => p.AssetMovements)
                    .HasForeignKey(d => d.FloorId)
                    .HasConstraintName("FK_AssetMovements_Floors");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.AssetMovements)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK_AssetMovements_Rooms");
            });

            modelBuilder.Entity<AssetScrap>(entity =>
            {
                entity.HasOne(d => d.Scrap)
                    .WithMany(p => p.AssetScraps)
                    .HasForeignKey(d => d.ScrapId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_AssetScrap_Scrap");

                entity.HasOne(d => d.ScrapReason)
                    .WithMany(p => p.AssetScraps)
                    .HasForeignKey(d => d.ScrapReasonId)
                    .HasConstraintName("FK_AssetScrap_ScrapReasons");
            });

            modelBuilder.Entity<AssetStatus>(entity =>
            {
                entity.ToTable("AssetStatus");

                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<AssetStatusTransaction>(entity =>
            {
                entity.HasOne(d => d.AssetStatus)
                    .WithMany(p => p.AssetStatusTransactions)
                    .HasForeignKey(d => d.AssetStatusId)
                    .HasConstraintName("FK_AssetStatusTransactions_AssetStatus");
            });

            modelBuilder.Entity<AssetStockTaking>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.CaptureDate).HasColumnType("datetime");

                entity.Property(e => e.Latitude).HasColumnType("decimal(18, 8)");

                entity.Property(e => e.Longtitude).HasColumnType("decimal(18, 8)");

                entity.Property(e => e.StschedulesId).HasColumnName("STSchedulesId");

                entity.Property(e => e.UserId).HasMaxLength(450);
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<Building>(entity =>
            {
                entity.Property(e => e.Brief).HasMaxLength(500);

                entity.Property(e => e.BriefAr).HasMaxLength(500);

                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<Calibration>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.DeviceId).HasColumnName("DeviceID");

                entity.Property(e => e.EngineerId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Device)
                    .WithMany(p => p.Calibrations)
                    .HasForeignKey(d => d.DeviceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Calibrations_AssetDetails");

                entity.HasOne(d => d.Engineer)
                    .WithMany(p => p.Calibrations)
                    .HasForeignKey(d => d.EngineerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Calibrations_AspNetUsers");
            });

            modelBuilder.Entity<CalibrationAttachment>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CalibrationId).HasColumnName("CalibrationID");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Calibration)
                    .WithMany(p => p.CalibrationAttachments)
                    .HasForeignKey(d => d.CalibrationId)
                    .HasConstraintName("FK_CalibrationAttachments_Calibrations");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NameAr).HasMaxLength(100);
            });

            modelBuilder.Entity<CategoryType>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NameAr).HasMaxLength(100);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Latitude).HasColumnType("decimal(18, 8)");

                entity.Property(e => e.Longtitude).HasColumnType("decimal(18, 8)");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);

                entity.HasOne(d => d.Governorate)
                    .WithMany(p => p.Cities)
                    .HasForeignKey(d => d.GovernorateId)
                    .HasConstraintName("FK_Cities_Governorates");
            });

            modelBuilder.Entity<Classification>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<CommetieeMember>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Email)
                    .HasMaxLength(320)
                    .HasColumnName("EMail");

                entity.Property(e => e.Website).HasMaxLength(2083);
            });

            modelBuilder.Entity<ContractAttachment>(entity =>
            {
                entity.Property(e => e.DocumentName).HasMaxLength(100);

                entity.Property(e => e.FileName).HasMaxLength(25);

                entity.HasOne(d => d.MasterContract)
                    .WithMany(p => p.ContractAttachments)
                    .HasForeignKey(d => d.MasterContractId)
                    .HasConstraintName("FK_ContractAttachments_MasterContracts");
            });

            modelBuilder.Entity<ContractDetail>(entity =>
            {
                entity.HasOne(d => d.MasterContract)
                    .WithMany(p => p.ContractDetails)
                    .HasForeignKey(d => d.MasterContractId)
                    .HasConstraintName("FK_ContractDetails_MasterContracts");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<Ecri>(entity =>
            {
                entity.ToTable("ECRIS");

                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NameAr).HasMaxLength(100);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.CardId).HasMaxLength(14);

                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Email).HasMaxLength(320);

                entity.Property(e => e.EmpImg).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(15);

                entity.Property(e => e.WhatsApp).HasMaxLength(15);
            });

            modelBuilder.Entity<Engineer>(entity =>
            {
                entity.Property(e => e.CardId).HasMaxLength(14);

                entity.Property(e => e.Code).HasMaxLength(15);

                entity.Property(e => e.Email).HasMaxLength(320);

                entity.Property(e => e.EngImg).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(15);

                entity.Property(e => e.WhatsApp).HasMaxLength(15);
            });

            modelBuilder.Entity<ExternalAssetMovement>(entity =>
            {
                entity.Property(e => e.HospitalName).HasMaxLength(500);

                entity.Property(e => e.MovementDate).HasColumnType("datetime");

                entity.Property(e => e.Notes).HasMaxLength(500);
            });

            modelBuilder.Entity<ExternalAssetMovementAttachment>(entity =>
            {
                entity.Property(e => e.FileName).HasMaxLength(25);

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<ExternalFix>(entity =>
            {
                entity.Property(e => e.ComingDate).HasColumnType("datetime");

                entity.Property(e => e.ComingNotes).HasMaxLength(50);

                entity.Property(e => e.ExpectedDate).HasColumnType("datetime");

                entity.Property(e => e.Notes).HasMaxLength(50);

                entity.Property(e => e.OutDate).HasColumnType("datetime");

                entity.Property(e => e.OutNumber).HasMaxLength(50);
            });

            modelBuilder.Entity<ExternalFixFile>(entity =>
            {
                entity.Property(e => e.FileName).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<Floor>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);

                entity.HasOne(d => d.Building)
                    .WithMany(p => p.Floors)
                    .HasForeignKey(d => d.BuildingId)
                    .HasConstraintName("FK_Floors_Buildings");
            });

            modelBuilder.Entity<Governorate>(entity =>
            {
                entity.Property(e => e.Area).HasColumnType("decimal(18, 5)");

                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Latitude).HasColumnType("decimal(18, 8)");

                entity.Property(e => e.Logo).HasMaxLength(50);

                entity.Property(e => e.Longtitude).HasColumnType("decimal(18, 8)");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);

                entity.Property(e => e.Population).HasColumnType("decimal(18, 3)");
            });

            modelBuilder.Entity<Hospital>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.ContractEnd).HasColumnType("date");

                entity.Property(e => e.ContractName).HasMaxLength(50);

                entity.Property(e => e.ContractStart).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(320);

                entity.Property(e => e.ManagerName).HasMaxLength(50);

                entity.Property(e => e.ManagerNameAr).HasMaxLength(50);

                entity.Property(e => e.Mobile).HasMaxLength(20);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Hospitals)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("FK_Hospitals_Cities");

                entity.HasOne(d => d.Governorate)
                    .WithMany(p => p.Hospitals)
                    .HasForeignKey(d => d.GovernorateId)
                    .HasConstraintName("FK_Hospitals_Governorates");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Hospitals)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK_Hospitals_Organizations");

                entity.HasOne(d => d.SubOrganization)
                    .WithMany(p => p.Hospitals)
                    .HasForeignKey(d => d.SubOrganizationId)
                    .HasConstraintName("FK_Hospitals_SubOrganizations");
            });

            modelBuilder.Entity<HospitalApplication>(entity =>
            {
                entity.Property(e => e.ActionDate).HasColumnType("datetime");

                entity.Property(e => e.AppDate).HasColumnType("datetime");

                entity.Property(e => e.AppNumber).HasMaxLength(50);

                entity.Property(e => e.Comment).HasMaxLength(500);

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.AppType)
                    .WithMany(p => p.HospitalApplications)
                    .HasForeignKey(d => d.AppTypeId)
                    .HasConstraintName("FK_HospitalApplications_ApplicationTypes");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.HospitalApplications)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_HospitalApplications_HospitalSupplierStatuses");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.HospitalApplications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_HospitalExecludeAssets_AspNetUsers");
            });

            modelBuilder.Entity<HospitalApplicationAttachment>(entity =>
            {
                entity.Property(e => e.FileName).HasMaxLength(25);

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasOne(d => d.HospitalReasonTransaction)
                    .WithMany(p => p.HospitalApplicationAttachments)
                    .HasForeignKey(d => d.HospitalReasonTransactionId)
                    .HasConstraintName("FK_HospitalApplicationAttachments_HospitalReasonTransactions");
            });

            modelBuilder.Entity<HospitalDepartment>(entity =>
            {
                entity.HasOne(d => d.Department)
                    .WithMany(p => p.HospitalDepartments)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_HospitalDepartments_Departments");
            });

            modelBuilder.Entity<HospitalEngineer>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Hosp)
                    .WithMany(p => p.HospitalEngineers)
                    .HasForeignKey(d => d.HospId)
                    .HasConstraintName("FK_HospitalEngineers_Hospitals");
            });

            modelBuilder.Entity<HospitalExecludeReason>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<HospitalHoldReason>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<HospitalReasonTransaction>(entity =>
            {
                entity.HasOne(d => d.HospitalApplication)
                    .WithMany(p => p.HospitalReasonTransactions)
                    .HasForeignKey(d => d.HospitalApplicationId)
                    .HasConstraintName("FK_HospitalReasonTrasactions_HospitalApplications");
            });

            modelBuilder.Entity<HospitalSupplierStatus>(entity =>
            {
                entity.Property(e => e.Color).HasMaxLength(50);

                entity.Property(e => e.Icon).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<ManufacturerPmasset>(entity =>
            {
                entity.ToTable("ManufacturerPMAssets");

                entity.Property(e => e.Comment).HasMaxLength(3000);

                entity.Property(e => e.DoneDate).HasColumnType("datetime");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.IsDone).HasDefaultValueSql("((0))");

                entity.Property(e => e.Pmdate)
                    .HasColumnType("datetime")
                    .HasColumnName("PMDate");

                entity.Property(e => e.SysDoneDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<MasterAsset>(entity =>
            {
                entity.Property(e => e.Ampair).HasMaxLength(10);

                entity.Property(e => e.AssetImg).HasMaxLength(50);

                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Ecriid).HasColumnName("ECRIId");

                entity.Property(e => e.ElectricRequirement).HasMaxLength(10);

                entity.Property(e => e.Frequency).HasMaxLength(10);

                entity.Property(e => e.ModelNumber).HasMaxLength(20);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NameAr).HasMaxLength(100);

                entity.Property(e => e.Pmbgcolor)
                    .HasMaxLength(10)
                    .HasColumnName("PMBGColor");

                entity.Property(e => e.Pmcolor)
                    .HasMaxLength(10)
                    .HasColumnName("PMColor");

                entity.Property(e => e.PmtimeId).HasColumnName("PMTimeId");

                entity.Property(e => e.Power).HasMaxLength(10);

                entity.Property(e => e.VersionNumber).HasMaxLength(20);

                entity.Property(e => e.Voltage).HasMaxLength(10);

                entity.HasOne(d => d.Ecri)
                    .WithMany(p => p.MasterAssets)
                    .HasForeignKey(d => d.Ecriid)
                    .HasConstraintName("FK_MasterAssets_ECRIS");

                entity.HasOne(d => d.Origin)
                    .WithMany(p => p.MasterAssets)
                    .HasForeignKey(d => d.OriginId)
                    .HasConstraintName("FK_MasterAssets_Origins");

                entity.HasOne(d => d.SubCategory)
                    .WithMany(p => p.MasterAssets)
                    .HasForeignKey(d => d.SubCategoryId)
                    .HasConstraintName("FK_MasterAssets_SubCategories");
            });

            modelBuilder.Entity<MasterAssetAttachment>(entity =>
            {
                entity.Property(e => e.FileName).HasMaxLength(25);

                entity.Property(e => e.Title).HasMaxLength(100);
            });

            modelBuilder.Entity<MasterAssetComponent>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NameAr).HasMaxLength(100);

                entity.Property(e => e.PartNo).HasMaxLength(20);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<MasterContract>(entity =>
            {
                entity.Property(e => e.ContractDate).HasColumnType("date");

                entity.Property(e => e.Cost).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.From).HasColumnType("date");

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.Serial).HasMaxLength(50);

                entity.Property(e => e.Subject).HasMaxLength(100);

                entity.Property(e => e.To).HasColumnType("date");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<ModulePermission>(entity =>
            {
                entity.HasKey(e => new { e.ModuleId, e.PermissionId });

                entity.ToTable("ModulePermission");

                entity.Property(e => e.ModuleId).HasColumnName("ModuleID");

                entity.Property(e => e.PermissionId).HasColumnName("PermissionID");

                entity.HasOne(d => d.Module)
                    .WithMany(p => p.ModulePermissions)
                    .HasForeignKey(d => d.ModuleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModulePermission_Modules");

                entity.HasOne(d => d.Permission)
                    .WithMany(p => p.ModulePermissions)
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModulePermission_Permissions");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.DirectorName).HasMaxLength(50);

                entity.Property(e => e.DirectorNameAr).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(320);

                entity.Property(e => e.Mobile).HasMaxLength(20);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<Origin>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("name")
                    .IsFixedLength(true);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("value")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Phase>(entity =>
            {
                entity.ToTable("Phase");
            });

            modelBuilder.Entity<PmassetTask>(entity =>
            {
                entity.ToTable("PMAssetTasks");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<PmassetTaskSchedule>(entity =>
            {
                entity.ToTable("PMAssetTaskSchedules");

                entity.Property(e => e.PmassetTaskId).HasColumnName("PMAssetTaskId");

                entity.Property(e => e.PmassetTimeId).HasColumnName("PMAssetTimeId");

                entity.HasOne(d => d.PmassetTask)
                    .WithMany(p => p.PmassetTaskSchedules)
                    .HasForeignKey(d => d.PmassetTaskId)
                    .HasConstraintName("FK_PMAssetTaskSchedules_PMAssetTasks");

                entity.HasOne(d => d.PmassetTime)
                    .WithMany(p => p.PmassetTaskSchedules)
                    .HasForeignKey(d => d.PmassetTimeId)
                    .HasConstraintName("FK_PMAssetTaskSchedules_PMAssetTimes");
            });

            modelBuilder.Entity<PmassetTime>(entity =>
            {
                entity.ToTable("PMAssetTimes");

                entity.Property(e => e.Pmdate)
                    .HasColumnType("date")
                    .HasColumnName("PMDate");
            });

            modelBuilder.Entity<Pmtime>(entity =>
            {
                entity.ToTable("PMTimes");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.ToTable("Request");

                entity.Property(e => e.CreatedById).HasMaxLength(450);

                entity.Property(e => e.IsOpened).HasDefaultValueSql("((0))");

                entity.Property(e => e.RequestCode).HasMaxLength(15);

                entity.Property(e => e.Subject).HasMaxLength(100);

                entity.HasOne(d => d.CreatedBy)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.CreatedById);

                entity.HasOne(d => d.RequestMode)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.RequestModeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.RequestPeriority)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.RequestPeriorityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.RequestType)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.RequestTypeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.SubProblem)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.SubProblemId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RequestDocument>(entity =>
            {
                entity.ToTable("RequestDocument");

                entity.Property(e => e.DocumentName).HasMaxLength(100);

                entity.Property(e => e.FileName).HasMaxLength(25);

                entity.HasOne(d => d.RequestTracking)
                    .WithMany(p => p.RequestDocuments)
                    .HasForeignKey(d => d.RequestTrackingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RequestMode>(entity =>
            {
                entity.ToTable("RequestMode");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<RequestPeriority>(entity =>
            {
                entity.ToTable("RequestPeriority");

                entity.Property(e => e.Color).HasMaxLength(10);

                entity.Property(e => e.Icon).HasMaxLength(30);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<RequestPhase>(entity =>
            {
                entity.ToTable("RequestPhase");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.RequestPhases)
                    .HasForeignKey(d => d.EmployeeId);

                entity.HasOne(d => d.Phase)
                    .WithMany(p => p.RequestPhases)
                    .HasForeignKey(d => d.PhaseId);

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.RequestPhases)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<RequestStatus>(entity =>
            {
                entity.ToTable("RequestStatus");

                entity.Property(e => e.Color).HasMaxLength(50);

                entity.Property(e => e.Icon).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<RequestTracking>(entity =>
            {
                entity.ToTable("RequestTracking");

                entity.Property(e => e.CreatedById).HasMaxLength(450);

                entity.Property(e => e.IsOpened).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.CreatedBy)
                    .WithMany(p => p.RequestTrackings)
                    .HasForeignKey(d => d.CreatedById);

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.RequestTrackings)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.RequestStatus)
                    .WithMany(p => p.RequestTrackings)
                    .HasForeignKey(d => d.RequestStatusId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RoleCategory>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("RolePermission");

                entity.Property(e => e.PermissionId).HasColumnName("PermissionID");

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(450)
                    .HasColumnName("RoleID");

                entity.HasOne(d => d.Permission)
                    .WithMany()
                    .HasForeignKey(d => d.PermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePermission_Permissions");

                entity.HasOne(d => d.Role)
                    .WithMany()
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RolePermission_AspNetRoles");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);

                entity.HasOne(d => d.Floor)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.FloorId)
                    .HasConstraintName("FK_Rooms_Floors");
            });

            modelBuilder.Entity<Scrap>(entity =>
            {
                entity.Property(e => e.ScrapDate).HasColumnType("datetime");

                entity.Property(e => e.ScrapNo).HasMaxLength(50);

                entity.Property(e => e.SysDate).HasColumnType("datetime");

                entity.HasOne(d => d.AssetDetail)
                    .WithMany(p => p.Scraps)
                    .HasForeignKey(d => d.AssetDetailId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Scrap_AssetDetails");
            });

            modelBuilder.Entity<ScrapAttachment>(entity =>
            {
                entity.Property(e => e.FileName).HasMaxLength(25);

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasOne(d => d.Scrap)
                    .WithMany(p => p.ScrapAttachments)
                    .HasForeignKey(d => d.ScrapId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ScrapAttachments_Scrap");
            });

            modelBuilder.Entity<ScrapReason>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.Property(e => e.KeyName).HasMaxLength(100);

                entity.Property(e => e.KeyValue).HasMaxLength(100);

                entity.Property(e => e.KeyValueAr).HasMaxLength(100);
            });

            modelBuilder.Entity<StockTakingHospital>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.StschedulesId).HasColumnName("STSchedulesId");
            });

            modelBuilder.Entity<StockTakingSchedule>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.Stcode)
                    .HasMaxLength(50)
                    .HasColumnName("STCode");

                entity.Property(e => e.UserId).HasMaxLength(450);
            });

            modelBuilder.Entity<SubCategory>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<SubOrganization>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.DirectorName).HasMaxLength(50);

                entity.Property(e => e.DirectorNameAr).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(320);

                entity.Property(e => e.Mobile).HasMaxLength(20);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<SubProblem>(entity =>
            {
                entity.HasOne(d => d.Problem)
                    .WithMany(p => p.SubProblems)
                    .HasForeignKey(d => d.ProblemId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.ContactPerson).HasMaxLength(200);

                entity.Property(e => e.Email)
                    .HasMaxLength(320)
                    .HasColumnName("EMail");

                entity.Property(e => e.Fax).HasMaxLength(15);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Notes).HasMaxLength(500);

                entity.Property(e => e.Website).HasMaxLength(2083);
            });

            modelBuilder.Entity<SupplierAttachment>(entity =>
            {
                entity.Property(e => e.FileName).HasMaxLength(25);

                entity.Property(e => e.Title).HasMaxLength(100);
            });

            modelBuilder.Entity<SupplierExeclude>(entity =>
            {
                entity.HasOne(d => d.Reason)
                    .WithMany(p => p.SupplierExecludes)
                    .HasForeignKey(d => d.ReasonId)
                    .HasConstraintName("FK_SupplierExecludes_SupplierExecludeReasons");

                entity.HasOne(d => d.SupplierExecludeAsset)
                    .WithMany(p => p.SupplierExecludes)
                    .HasForeignKey(d => d.SupplierExecludeAssetId)
                    .HasConstraintName("FK_SupplierExecludes_SupplierExecludeAssets");
            });

            modelBuilder.Entity<SupplierExecludeAsset>(entity =>
            {
                entity.Property(e => e.ActionDate).HasColumnType("datetime");

                entity.Property(e => e.Comment).HasMaxLength(500);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.ExNumber).HasMaxLength(50);

                entity.Property(e => e.ExecludeDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.HasOne(d => d.AppType)
                    .WithMany(p => p.SupplierExecludeAssets)
                    .HasForeignKey(d => d.AppTypeId)
                    .HasConstraintName("FK_SupplierExecludeAssets_ApplicationTypes");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.SupplierExecludeAssets)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_SupplierExecludeAssets_HospitalSupplierStatuses");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SupplierExecludeAssets)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_SupplierExecludeAssets_AspNetUsers");
            });

            modelBuilder.Entity<SupplierExecludeAttachment>(entity =>
            {
                entity.Property(e => e.FileName).HasMaxLength(25);

                entity.Property(e => e.Title).HasMaxLength(100);
            });

            modelBuilder.Entity<SupplierExecludeReason>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<SupplierHoldReason>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.Name).HasMaxLength(150);

                entity.Property(e => e.NameAr).HasMaxLength(150);
            });

            modelBuilder.Entity<Visit>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.Latitude).HasColumnType("decimal(18, 8)");

                entity.Property(e => e.Longtitude).HasColumnType("decimal(18, 8)");

                entity.HasOne(d => d.Hospital)
                    .WithMany(p => p.Visits)
                    .HasForeignKey(d => d.HospitalId)
                    .HasConstraintName("FK_Visits_Hospitals");

                entity.HasOne(d => d.VisitType)
                    .WithMany(p => p.Visits)
                    .HasForeignKey(d => d.VisitTypeId)
                    .HasConstraintName("FK_Visits_VisitTypes");
            });

            modelBuilder.Entity<VisitAttachment>(entity =>
            {
                entity.Property(e => e.FileName).HasMaxLength(25);

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasOne(d => d.Visit)
                    .WithMany(p => p.VisitAttachments)
                    .HasForeignKey(d => d.VisitId)
                    .HasConstraintName("FK_VisitAttachments_Visits");
            });

            modelBuilder.Entity<VisitType>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(15);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<WnpmassetTime>(entity =>
            {
                entity.ToTable("WNPMAssetTimes");

                entity.Property(e => e.Comment).HasMaxLength(3000);

                entity.Property(e => e.DoneDate).HasColumnType("datetime");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.IsDone).HasDefaultValueSql("((0))");

                entity.Property(e => e.Pmdate)
                    .HasColumnType("datetime")
                    .HasColumnName("PMDate");

                entity.Property(e => e.SysDoneDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<WnpmassetTimeAttachment>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("WNPMAssetTimeAttachments");

                entity.Property(e => e.FileName).HasMaxLength(200);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.Property(e => e.WnpmassetTimeId).HasColumnName("WNPMAssetTimeId");
            });

            modelBuilder.Entity<WorkOrder>(entity =>
            {
                entity.Property(e => e.CreatedById).HasMaxLength(450);

                entity.HasOne(d => d.CreatedBy)
                    .WithMany(p => p.WorkOrders)
                    .HasForeignKey(d => d.CreatedById);

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.WorkOrders)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.WorkOrderPeriority)
                    .WithMany(p => p.WorkOrders)
                    .HasForeignKey(d => d.WorkOrderPeriorityId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.WorkOrderType)
                    .WithMany(p => p.WorkOrders)
                    .HasForeignKey(d => d.WorkOrderTypeId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WorkOrderAssign>(entity =>
            {
                entity.Property(e => e.CreatedBy).HasMaxLength(450);

                entity.Property(e => e.CreatedDate).HasColumnType("date");

                entity.Property(e => e.UserId).HasMaxLength(450);

                entity.Property(e => e.Wotid).HasColumnName("WOTId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.WorkOrderAssigns)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_WorkOrderAssigns_AspNetUsers");
            });

            modelBuilder.Entity<WorkOrderAttachment>(entity =>
            {
                entity.Property(e => e.DocumentName).HasMaxLength(100);

                entity.Property(e => e.FileName).HasMaxLength(25);

                entity.HasOne(d => d.WorkOrderTracking)
                    .WithMany(p => p.WorkOrderAttachments)
                    .HasForeignKey(d => d.WorkOrderTrackingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WorkOrderStatus>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Color).HasMaxLength(10);

                entity.Property(e => e.Icon).HasMaxLength(30);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.NameAr).HasMaxLength(50);
            });

            modelBuilder.Entity<WorkOrderTask>(entity =>
            {
                entity.HasOne(d => d.AssetWorkOrderTask)
                    .WithMany(p => p.WorkOrderTasks)
                    .HasForeignKey(d => d.AssetWorkOrderTaskId);

                entity.HasOne(d => d.WorkOrder)
                    .WithMany(p => p.WorkOrderTasks)
                    .HasForeignKey(d => d.WorkOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WorkOrderTracking>(entity =>
            {
                entity.Property(e => e.AssignedTo).HasMaxLength(450);

                entity.Property(e => e.CreatedById).HasMaxLength(450);

                entity.Property(e => e.PlannedEndDate).HasColumnType("date");

                entity.Property(e => e.PlannedStartDate).HasColumnType("date");

                entity.Property(e => e.WorkOrderId).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.CreatedBy)
                    .WithMany(p => p.WorkOrderTrackings)
                    .HasForeignKey(d => d.CreatedById);

                entity.HasOne(d => d.WorkOrder)
                    .WithMany(p => p.WorkOrderTrackings)
                    .HasForeignKey(d => d.WorkOrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.WorkOrderStatus)
                    .WithMany(p => p.WorkOrderTrackings)
                    .HasForeignKey(d => d.WorkOrderStatusId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
