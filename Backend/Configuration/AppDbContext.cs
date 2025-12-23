using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPIBackend.Models;
using WebAPIBackend.Models.Audit;
using WebAPIBackend.Models.RequestData;
using WebAPIBackend.Models.ViewModels;

namespace WebAPIBackend.Configuration
{
    public class AppDbContext : IdentityDbContext
    {

        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<AddressType> AddressTypes { get; set; }



        public virtual DbSet<BuyerDetail> BuyerDetails { get; set; }

        public virtual DbSet<CompanyDetail> CompanyDetails { get; set; }

        public virtual DbSet<CompanyOwner> CompanyOwners { get; set; }

        public virtual DbSet<CompanyOwnerAddress> CompanyOwnerAddresses { get; set; }

        public virtual DbSet<EducationLevel> EducationLevels { get; set; }

        public virtual DbSet<FormsReference> FormsReferences { get; set; }

        public virtual DbSet<Gaurantee> Gaurantees { get; set; }

        public virtual DbSet<GuaranteeType> GuaranteeTypes { get; set; }

        public virtual DbSet<Guarantor> Guarantors { get; set; }

    

        public virtual DbSet<Haqulemtyaz> Haqulemtyazs { get; set; }

        public virtual DbSet<IdentityCardType> IdentityCardTypes { get; set; }

        public virtual DbSet<LicenseDetail> LicenseDetails { get; set; }

        public virtual DbSet<Location> Locations { get; set; }

        public virtual DbSet<LostdocumentsType> LostdocumentsTypes { get; set; }

        public virtual DbSet<PeriodicForm> PeriodicForms { get; set; }

        public virtual DbSet<PropertyAddress> PropertyAddresses { get; set; }

        public virtual DbSet<PropertyDetail> PropertyDetails { get; set; }

        public virtual DbSet<PropertyCancellation> PropertyCancellations { get; set; }

        public virtual DbSet<PropertyType> PropertyTypes { get; set; }

        public virtual DbSet<PunitType> PunitTypes { get; set; }

        public virtual DbSet<SellerDetail> SellerDetails { get; set; }

        public virtual DbSet<Setum> Seta { get; set; }

        public virtual DbSet<TransactionType> TransactionTypes { get; set; }

       // public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<VehiclesBuyerDetail> VehiclesBuyerDetails { get; set; }

        public virtual DbSet<VehiclesPropertyDetail> VehiclesPropertyDetails { get; set; }

        public virtual DbSet<VehiclesSellerDetail> VehiclesSellerDetails { get; set; }

        public virtual DbSet<VehiclesWitnessDetail> VehiclesWitnessDetails { get; set; }

        public virtual DbSet<Violation> Violations { get; set; }

        public virtual DbSet<ViolationType> ViolationTypes { get; set; }
        public virtual DbSet<Propertyaudit> Propertyaudits { get; set; }

        public virtual DbSet<Propertybuyeraudit> Propertybuyeraudits { get; set; }

        public virtual DbSet<Propertyselleraudit> Propertyselleraudits { get; set; }

        public virtual DbSet<WitnessDetail> WitnessDetails { get; set; }

        public virtual DbSet<Vehicleaudit> Vehicleaudits { get; set; }

        public virtual DbSet<Vehicleselleraudit> Vehicleselleraudits { get; set; }

        public virtual DbSet<Vehiclebuyeraudit> Vehiclebuyeraudit { get; set; }

        public virtual DbSet<Licenseaudit> Licenseaudits { get; set; }
        public virtual DbSet<Guarantorsaudit> Guarantorsaudits { get; set; }
        public virtual DbSet<Graunteeaudit> Graunteeaudits { get; set; }
        public virtual DbSet<Companyowneraudit> Companyowneraudits { get; set; }
        public virtual DbSet<Companydetailsaudit> Companydetailsaudits { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public DbSet<UserProfileWithCompany> UserProfileWithCompany { get; set; }

        public DbSet<GetPrintType> GetPrintType { get; set; }
        public DbSet<getVehiclePrintData> getVehiclePrintData { get; set; }
        public DbSet<LicenseView> LicenseView { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<LicenseView>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("LicenseView");

                entity.Property(e => e.PhoneNumber).HasMaxLength(13);
                entity.Property(e => e.Tin).HasColumnName("TIN");
            });
            modelBuilder.Entity<AddressType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("AddressType_pkey");

                entity.ToTable("AddressType", "look");
            });

            modelBuilder.Entity<BuyerDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("BuyerDetails_pkey");

                entity.ToTable("BuyerDetails", "tr");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(14);
                entity.Property(e => e.Photo).HasColumnName("photo");
                entity.HasOne(d => d.PaddressDistrict).WithMany(p => p.BuyerDetailPaddressDistricts)
                    .HasForeignKey(d => d.PaddressDistrictId)
                    .HasConstraintName("BuyerDetails_PaddressDistrictId_fkey");

                entity.HasOne(d => d.PaddressProvince).WithMany(p => p.BuyerDetailPaddressProvinces)
                    .HasForeignKey(d => d.PaddressProvinceId)
                    .HasConstraintName("BuyerDetails_PaddressProvinceId_fkey");

                entity.HasOne(d => d.PropertyDetails).WithMany(p => p.BuyerDetails)
                    .HasForeignKey(d => d.PropertyDetailsId)
                    .HasConstraintName("BuyerDetails_PropertyDetailsId_fkey");

                entity.HasOne(d => d.TaddressDistrict).WithMany(p => p.BuyerDetailTaddressDistricts)
                    .HasForeignKey(d => d.TaddressDistrictId)
                    .HasConstraintName("BuyerDetails_TaddressDistrictId_fkey");

                entity.HasOne(d => d.TaddressProvince).WithMany(p => p.BuyerDetailTaddressProvinces)
                    .HasForeignKey(d => d.TaddressProvinceId)
                    .HasConstraintName("BuyerDetails_TaddressProvinceId_fkey");
            });

            modelBuilder.Entity<CompanyDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("CompanyDetails_pkey");

                entity.ToTable("CompanyDetails", "org");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.PetitionNumber).HasMaxLength(12);
                entity.Property(e => e.PhoneNumber).HasMaxLength(13);
                entity.Property(e => e.Tin).HasColumnName("TIN");
            });

            modelBuilder.Entity<CompanyOwner>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("CompanyOwner_pkey");

                entity.ToTable("CompanyOwner", "org");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.HasOne(d => d.Company).WithMany(p => p.CompanyOwners)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("CompanyOwner_CompanyId_fkey");

                entity.HasOne(d => d.EducationLevel).WithMany(p => p.CompanyOwners)
                    .HasForeignKey(d => d.EducationLevelId)
                    .HasConstraintName("CompanyOwner_EducationLevelId_fkey");

                entity.HasOne(d => d.IdentityCardType).WithMany(p => p.CompanyOwners)
                    .HasForeignKey(d => d.IdentityCardTypeId)
                    .HasConstraintName("CompanyOwner_IdentityCardTypeId_fkey");
            });

            modelBuilder.Entity<CompanyOwnerAddress>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("CompanyOwnerAddress_pkey");

                entity.ToTable("CompanyOwnerAddress", "org");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.HasOne(d => d.AddressType).WithMany(p => p.CompanyOwnerAddresses)
                    .HasForeignKey(d => d.AddressTypeId)
                    .HasConstraintName("CompanyOwnerAddress_AddressTypeId_fkey");

                entity.HasOne(d => d.CompanyOwner).WithMany(p => p.CompanyOwnerAddresses)
                    .HasForeignKey(d => d.CompanyOwnerId)
                    .HasConstraintName("CompanyOwnerAddress_CompanyOwnerId_fkey");

                entity.HasOne(d => d.District).WithMany(p => p.CompanyOwnerAddressDistricts)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("CompanyOwnerAddress_DistrictId_fkey");

                entity.HasOne(d => d.Province).WithMany(p => p.CompanyOwnerAddressProvinces)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("CompanyOwnerAddress_ProvinceId_fkey");
            });

            modelBuilder.Entity<EducationLevel>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_educationlevel");

                entity.ToTable("EducationLevel", "look");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("nextval('look.educationlevel_id_seq'::regclass)")
                    .HasColumnName("ID");
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.Parentid).HasColumnName("parentid");
                entity.Property(e => e.Sorter).HasMaxLength(50);
            });

            modelBuilder.Entity<FormsReference>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("FormsReference_pkey");

                entity.ToTable("FormsReference", "look");
            });

            modelBuilder.Entity<Gaurantee>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("Gaurantee_pkey");

                entity.ToTable("Gaurantee", "org");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.HasOne(d => d.Company).WithMany(p => p.Gaurantees)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("Gaurantee_CompanyId_fkey");

                entity.HasOne(d => d.GuaranteeType).WithMany(p => p.Gaurantees)
                    .HasForeignKey(d => d.GuaranteeTypeId)
                    .HasConstraintName("Gaurantee_GuaranteeTypeId_fkey");
            });

            modelBuilder.Entity<GuaranteeType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("GuaranteeType_pkey");

                entity.ToTable("GuaranteeType", "look");
            });

            modelBuilder.Entity<Guarantor>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("Guarantors_pkey");

                entity.ToTable("Guarantors", "org");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(13);

                entity.HasOne(d => d.Company).WithMany(p => p.Guarantors)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("Guarantors_CompanyId_fkey");

                entity.HasOne(d => d.IdentityCardType).WithMany(p => p.Guarantors)
                    .HasForeignKey(d => d.IdentityCardTypeId)
                    .HasConstraintName("Guarantors_IdentityCardTypeId_fkey");

                entity.HasOne(d => d.PaddressDistrict).WithMany(p => p.GuarantorPaddressDistricts)
                    .HasForeignKey(d => d.PaddressDistrictId)
                    .HasConstraintName("Guarantors_PaddressDistrictId_fkey");

                entity.HasOne(d => d.PaddressProvince).WithMany(p => p.GuarantorPaddressProvinces)
                    .HasForeignKey(d => d.PaddressProvinceId)
                    .HasConstraintName("Guarantors_PaddressProvinceId_fkey");

                entity.HasOne(d => d.TaddressDistrict).WithMany(p => p.GuarantorTaddressDistricts)
                    .HasForeignKey(d => d.TaddressDistrictId)
                    .HasConstraintName("Guarantors_TaddressDistrictId_fkey");

                entity.HasOne(d => d.TaddressProvince).WithMany(p => p.GuarantorTaddressProvinces)
                    .HasForeignKey(d => d.TaddressProvinceId)
                    .HasConstraintName("Guarantors_TaddressProvinceId_fkey");
            });

            modelBuilder.Entity<Haqulemtyaz>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("Haqulemtyaz_pkey");

                entity.ToTable("Haqulemtyaz", "org");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.HasOne(d => d.Company).WithMany(p => p.Haqulemtyazs)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("Haqulemtyaz_CompanyId_fkey");
            });

            modelBuilder.Entity<IdentityCardType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("IdentityCardType_pkey");

                entity.ToTable("IdentityCardType", "look");
            });

            modelBuilder.Entity<LicenseDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("LicenseDetails_pkey");

                entity.ToTable("LicenseDetails", "org");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.HasOne(d => d.Area).WithMany(p => p.LicenseDetails)
                    .HasForeignKey(d => d.AreaId)
                    .HasConstraintName("LicenseDetails_AreaId_fkey");

                entity.HasOne(d => d.Company).WithMany(p => p.LicenseDetails)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("LicenseDetails_CompanyId_fkey");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("pk_location");

                entity.ToTable("Location", "look");

                entity.Property(e => e.Id)
                    .HasDefaultValueSql("nextval('look.location_id_seq'::regclass)")
                    .HasColumnName("ID");
                entity.Property(e => e.Code)
                    .HasMaxLength(3)
                    .IsFixedLength();
                entity.Property(e => e.Dari).HasMaxLength(255);
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.ParentId).HasColumnName("ParentID");
                entity.Property(e => e.Path).HasMaxLength(255);
                entity.Property(e => e.PathDari)
                    .HasMaxLength(255)
                    .HasColumnName("Path_Dari");
                entity.Property(e => e.TypeId).HasColumnName("TypeID");
            });

            modelBuilder.Entity<LostdocumentsType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("LostdocumentsType_pkey");

                entity.ToTable("LostdocumentsType", "look");
            });

            modelBuilder.Entity<PeriodicForm>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PeriodicForm_pkey");

                entity.ToTable("PeriodicForm", "org");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.HasOne(d => d.Company).WithMany(p => p.PeriodicForms)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("PeriodicForm_CompanyId_fkey");

                entity.HasOne(d => d.Reference).WithMany(p => p.PeriodicForms)
                    .HasForeignKey(d => d.ReferenceId)
                    .HasConstraintName("PeriodicForm_ReferenceId_fkey");
            });

            modelBuilder.Entity<PropertyAddress>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PropertyAddress_pkey");

                entity.ToTable("PropertyAddress", "tr");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.HasOne(d => d.PropertyDetails).WithMany(p => p.PropertyAddresses)
                    .HasForeignKey(d => d.PropertyDetailsId)
                    .HasConstraintName("PropertyAddress_PropertyDetailsId_fkey");
            });

            modelBuilder.Entity<PropertyCancellation>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PropertyCancellations_pkey");

                entity.ToTable("PropertyCancellations", "tr");

                entity.Property(e => e.CancellationDate).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CancelledBy).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(d => d.PropertyDetails).WithMany()
                    .HasForeignKey(d => d.PropertyDetailsId)
                    .HasConstraintName("PropertyCancellations_PropertyDetailsId_fkey");
            });

            modelBuilder.Entity<PropertyDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PropertyDetails_pkey");

                entity.ToTable("PropertyDetails", "tr");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.DocumentType).HasColumnName("DocumentType");
                entity.Property(e => e.IssuanceNumber).HasColumnName("IssuanceNumber");
                entity.Property(e => e.IssuanceDate).HasColumnType("timestamp with time zone");
                entity.Property(e => e.SerialNumber).HasColumnName("SerialNumber");
                entity.Property(e => e.TransactionDate).HasColumnType("timestamp with time zone");
                entity.Property(e => e.East).HasColumnName("east");
                entity.Property(e => e.iscomplete)
                    .HasDefaultValueSql("false")
                    .HasColumnName("iscomplete");
                entity.Property(e => e.iseditable)
                    .HasDefaultValueSql("false")
                    .HasColumnName("iseditable");
                entity.Property(e => e.North).HasColumnName("north");
                entity.Property(e => e.Parea).HasColumnName("PArea");
                entity.Property(e => e.PunitTypeId).HasColumnName("PUnitTypeId");
                entity.Property(e => e.South).HasColumnName("south");
                entity.Property(e => e.West).HasColumnName("west");

                entity.HasOne(d => d.PropertyType).WithMany(p => p.PropertyDetails)
                    .HasForeignKey(d => d.PropertyTypeId)
                    .HasConstraintName("PropertyDetails_PropertyTypeId_fkey");

                entity.HasOne(d => d.PunitType).WithMany(p => p.PropertyDetails)
                    .HasForeignKey(d => d.PunitTypeId)
                    .HasConstraintName("PropertyDetails_PUnitTypeId_fkey");

                entity.HasOne(d => d.TransactionType).WithMany(p => p.PropertyDetails)
                    .HasForeignKey(d => d.TransactionTypeId)
                    .HasConstraintName("PropertyDetails_TransactionTypeId_fkey");
            });

            modelBuilder.Entity<PropertyType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PropertyType_pkey");

                entity.ToTable("PropertyType", "look");
            });

            modelBuilder.Entity<PunitType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PUnitType_pkey");

                entity.ToTable("PUnitType", "look");
            });

            modelBuilder.Entity<SellerDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("SellerDetails_pkey");

                entity.ToTable("SellerDetails", "tr");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(14);
                entity.Property(e => e.Photo).HasColumnName("photo");
                entity.HasOne(d => d.PaddressDistrict).WithMany(p => p.SellerDetailPaddressDistricts)
                    .HasForeignKey(d => d.PaddressDistrictId)
                    .HasConstraintName("SellerDetails_PaddressDistrictId_fkey");

                entity.HasOne(d => d.PaddressProvince).WithMany(p => p.SellerDetailPaddressProvinces)
                    .HasForeignKey(d => d.PaddressProvinceId)
                    .HasConstraintName("SellerDetails_PaddressProvinceId_fkey");

                entity.HasOne(d => d.PropertyDetails).WithMany(p => p.SellerDetails)
                    .HasForeignKey(d => d.PropertyDetailsId)
                    .HasConstraintName("SellerDetails_PropertyDetailsId_fkey");

                entity.HasOne(d => d.TaddressDistrict).WithMany(p => p.SellerDetailTaddressDistricts)
                    .HasForeignKey(d => d.TaddressDistrictId)
                    .HasConstraintName("SellerDetails_TaddressDistrictId_fkey");

                entity.HasOne(d => d.TaddressProvince).WithMany(p => p.SellerDetailTaddressProvinces)
                    .HasForeignKey(d => d.TaddressProvinceId)
                    .HasConstraintName("SellerDetails_TaddressProvinceId_fkey");
            });

            modelBuilder.Entity<Setum>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("Seta_pkey");

                entity.ToTable("Seta", "org");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
            });

            modelBuilder.Entity<TransactionType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("TransactionType_pkey");

                entity.ToTable("TransactionType", "look");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<VehiclesBuyerDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("VehiclesBuyerDetails_pkey");

                entity.ToTable("VehiclesBuyerDetails", "tr");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(14);
                entity.Property(e => e.Photo).HasColumnName("photo");
                entity.HasOne(d => d.PaddressDistrict).WithMany(p => p.VehiclesBuyerDetailPaddressDistricts)
                    .HasForeignKey(d => d.PaddressDistrictId)
                    .HasConstraintName("VehiclesBuyerDetails_PaddressDistrictId_fkey");

                entity.HasOne(d => d.PaddressProvince).WithMany(p => p.VehiclesBuyerDetailPaddressProvinces)
                    .HasForeignKey(d => d.PaddressProvinceId)
                    .HasConstraintName("VehiclesBuyerDetails_PaddressProvinceId_fkey");

                entity.HasOne(d => d.PropertyDetails).WithMany(p => p.VehiclesBuyerDetails)
                    .HasForeignKey(d => d.PropertyDetailsId)
                    .HasConstraintName("VehiclesBuyerDetails_PropertyDetailsId_fkey");

                entity.HasOne(d => d.TaddressDistrict).WithMany(p => p.VehiclesBuyerDetailTaddressDistricts)
                    .HasForeignKey(d => d.TaddressDistrictId)
                    .HasConstraintName("VehiclesBuyerDetails_TaddressDistrictId_fkey");

                entity.HasOne(d => d.TaddressProvince).WithMany(p => p.VehiclesBuyerDetailTaddressProvinces)
                    .HasForeignKey(d => d.TaddressProvinceId)
                    .HasConstraintName("VehiclesBuyerDetails_TaddressProvinceId_fkey");
            });

            modelBuilder.Entity<VehiclesPropertyDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("VehiclesPropertyDetails_pkey");

                entity.ToTable("VehiclesPropertyDetails", "tr");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.HasOne(d => d.PropertyType).WithMany(p => p.VehiclesPropertyDetails)
                    .HasForeignKey(d => d.PropertyTypeId)
                    .HasConstraintName("VehiclesPropertyDetails_PropertyTypeId_fkey");

                entity.HasOne(d => d.TransactionType).WithMany(p => p.VehiclesPropertyDetails)
                    .HasForeignKey(d => d.TransactionTypeId)
                    .HasConstraintName("VehiclesPropertyDetails_TransactionTypeId_fkey");
            });

            modelBuilder.Entity<VehiclesSellerDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("VehiclesSellerDetails_pkey");

                entity.ToTable("VehiclesSellerDetails", "tr");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(14);
                entity.Property(e => e.Photo).HasColumnName("photo");
                entity.HasOne(d => d.PaddressDistrict).WithMany(p => p.VehiclesSellerDetailPaddressDistricts)
                    .HasForeignKey(d => d.PaddressDistrictId)
                    .HasConstraintName("VehiclesSellerDetails_PaddressDistrictId_fkey");

                entity.HasOne(d => d.PaddressProvince).WithMany(p => p.VehiclesSellerDetailPaddressProvinces)
                    .HasForeignKey(d => d.PaddressProvinceId)
                    .HasConstraintName("VehiclesSellerDetails_PaddressProvinceId_fkey");

                entity.HasOne(d => d.PropertyDetails).WithMany(p => p.VehiclesSellerDetails)
                    .HasForeignKey(d => d.PropertyDetailsId)
                    .HasConstraintName("VehiclesSellerDetails_PropertyDetailsId_fkey");

                entity.HasOne(d => d.TaddressDistrict).WithMany(p => p.VehiclesSellerDetailTaddressDistricts)
                    .HasForeignKey(d => d.TaddressDistrictId)
                    .HasConstraintName("VehiclesSellerDetails_TaddressDistrictId_fkey");

                entity.HasOne(d => d.TaddressProvince).WithMany(p => p.VehiclesSellerDetailTaddressProvinces)
                    .HasForeignKey(d => d.TaddressProvinceId)
                    .HasConstraintName("VehiclesSellerDetails_TaddressProvinceId_fkey");
            });

            modelBuilder.Entity<VehiclesWitnessDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("VehiclesWitnessDetails_pkey");

                entity.ToTable("VehiclesWitnessDetails", "tr");

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('tr.\"WitnessDetails_Id_seq\"'::regclass)");
                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(14);

                entity.HasOne(d => d.PropertyDetails).WithMany(p => p.VehiclesWitnessDetails)
                    .HasForeignKey(d => d.PropertyDetailsId)
                    .HasConstraintName("VehiclesWitnessDetails_PropertyDetailsId_fkey");
            });

            modelBuilder.Entity<Violation>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("Violation_pkey");

                entity.ToTable("Violation", "org");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.HasOne(d => d.ViolationType).WithMany(p => p.Violations)
                    .HasForeignKey(d => d.ViolationTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Violation_ViolationTypeId_fkey");
            });

            modelBuilder.Entity<ViolationType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("ViolationType_pkey");

                entity.ToTable("ViolationType", "look");
            });

            modelBuilder.Entity<WitnessDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("WitnessDetails_pkey");

                entity.ToTable("WitnessDetails", "tr");

                entity.Property(e => e.CreatedAt).HasColumnType("timestamp without time zone");
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.PhoneNumber).HasMaxLength(14);

                entity.HasOne(d => d.PropertyDetails).WithMany(p => p.WitnessDetails)
                    .HasForeignKey(d => d.PropertyDetailsId)
                    .HasConstraintName("WitnessDetails_PropertyDetailsId_fkey");
            });
            modelBuilder.Entity<Propertyaudit>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("propertyaudit_pkey");

                entity.ToTable("propertyaudit", "log");

                entity.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                entity.HasOne(d => d.Property).WithMany(p => p.Propertyaudits)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("propertyaudit_PropertyId_fkey");
            });

            modelBuilder.Entity<Propertybuyeraudit>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("propertybuyeraudit_pkey");

                entity.ToTable("propertybuyeraudit", "log");

                entity.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                entity.HasOne(d => d.Buyer).WithMany(p => p.Propertybuyeraudits)
                    .HasForeignKey(d => d.BuyerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("propertybuyeraudit_BuyerId_fkey");
            });

            modelBuilder.Entity<Propertyselleraudit>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("propertyselleraudit_pkey");

                entity.ToTable("propertyselleraudit", "log");

                entity.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                entity.HasOne(d => d.Seller).WithMany(p => p.Propertyselleraudits)
                    .HasForeignKey(d => d.SellerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("propertyselleraudit_SellerId_fkey");
            });
            modelBuilder.Entity<Vehicleaudit>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("vehicleaudit_pkey");

                entity.ToTable("vehicleaudit", "log");

                entity.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                entity.HasOne(d => d.Vehicle).WithMany(p => p.Vehicleaudits)
                    .HasForeignKey(d => d.VehicleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("vehicleaudit_VehicleId_fkey");
            });
            modelBuilder.Entity<Vehiclebuyeraudit>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("vehiclebuyeraudit_pkey");

                entity.ToTable("vehiclebuyeraudit", "log");

                entity.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                entity.HasOne(d => d.VehicleBuyer).WithMany(p => p.Vehiclebuyeraudits)
                    .HasForeignKey(d => d.VehicleBuyerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("vehiclebuyeraudit_VehicleBuyerId_fkey");
            });
            modelBuilder.Entity<Propertybuyeraudit>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("propertybuyeraudit_pkey");

                entity.ToTable("propertybuyeraudit", "log");

                entity.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                entity.HasOne(d => d.Buyer).WithMany(p => p.Propertybuyeraudits)
                    .HasForeignKey(d => d.BuyerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("propertybuyeraudit_BuyerId_fkey");
            });

            modelBuilder.Entity<Vehicleselleraudit>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("vehicleselleraudit_pkey");

                entity.ToTable("vehicleselleraudit", "log");

                entity.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                entity.HasOne(d => d.VehicleSeller).WithMany(p => p.Vehicleselleraudits)
                    .HasForeignKey(d => d.VehicleSellerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("vehicleselleraudit_VehicleSellerId_fkey");
            });
            modelBuilder.Entity<Licenseaudit>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("licenseaudit_pkey");

                entity.ToTable("licenseaudit", "log");

                entity.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                entity.HasOne(d => d.License).WithMany(p => p.Licenseaudits)
                    .HasForeignKey(d => d.LicenseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("licenseaudit_LicenseId_fkey");
            });

            modelBuilder.Entity<Guarantorsaudit>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("guarantorsaudit_pkey");

                entity.ToTable("guarantorsaudit", "log");

                entity.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                entity.HasOne(d => d.Guarantors).WithMany(p => p.Guarantorsaudits)
                    .HasForeignKey(d => d.GuarantorsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("guarantorsaudit_GuarantorsId_fkey");
            });
            modelBuilder.Entity<Graunteeaudit>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("graunteeaudit_pkey");

                entity.ToTable("graunteeaudit", "log");

                entity.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                entity.HasOne(d => d.Gaurantee).WithMany(p => p.Graunteeaudits)
                    .HasForeignKey(d => d.GauranteeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("graunteeaudit_GauranteeId_fkey");
            });

            modelBuilder.Entity<Companyowneraudit>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("companyowneraudit_pkey");

                entity.ToTable("companyowneraudit", "log");

                entity.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                entity.HasOne(d => d.Owner).WithMany(p => p.Companyowneraudits)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("companyowneraudit_OwnerId_fkey");
            });
            modelBuilder.Entity<Companydetailsaudit>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("companydetailsaudit_pkey");

                entity.ToTable("companydetailsaudit", "log");

                entity.Property(e => e.UpdatedAt).HasColumnType("timestamp without time zone");

                entity.HasOne(d => d.Company).WithMany(p => p.Companydetailsaudits)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("companydetailsaudit_CompanyId_fkey");
            });
            modelBuilder.Entity<Area>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("Area_pkey");

                entity.ToTable("Area", "look");
            });

        }


    }
}
