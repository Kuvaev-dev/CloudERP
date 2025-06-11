using DatabaseAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Context;

public partial class CloudDBEntities : DbContext
{
    public CloudDBEntities()
    {
    }

    public CloudDBEntities(DbContextOptions<CloudDBEntities> options)
        : base(options)
    {
    }

    public virtual DbSet<tblAccountActivity> tblAccountActivity { get; set; }

    public virtual DbSet<tblAccountControl> tblAccountControl { get; set; }

    public virtual DbSet<tblAccountHead> tblAccountHead { get; set; }

    public virtual DbSet<tblAccountSetting> tblAccountSetting { get; set; }

    public virtual DbSet<tblAccountSubControl> tblAccountSubControl { get; set; }

    public virtual DbSet<tblBranch> tblBranch { get; set; }

    public virtual DbSet<tblBranchType> tblBranchType { get; set; }

    public virtual DbSet<tblCategory> tblCategory { get; set; }

    public virtual DbSet<tblCompany> tblCompany { get; set; }

    public virtual DbSet<tblCustomer> tblCustomer { get; set; }

    public virtual DbSet<tblCustomerInvoice> tblCustomerInvoice { get; set; }

    public virtual DbSet<tblCustomerInvoiceDetail> tblCustomerInvoiceDetail { get; set; }

    public virtual DbSet<tblCustomerPayment> tblCustomerPayment { get; set; }

    public virtual DbSet<tblCustomerReturnInvoice> tblCustomerReturnInvoice { get; set; }

    public virtual DbSet<tblCustomerReturnInvoiceDetail> tblCustomerReturnInvoiceDetail { get; set; }

    public virtual DbSet<tblCustomerReturnPayment> tblCustomerReturnPayment { get; set; }

    public virtual DbSet<tblEmployee> tblEmployee { get; set; }

    public virtual DbSet<tblFinancialYear> tblFinancialYear { get; set; }

    public virtual DbSet<tblPayroll> tblPayroll { get; set; }

    public virtual DbSet<tblPurchaseCart> tblPurchaseCart { get; set; }

    public virtual DbSet<tblPurchaseCartDetail> tblPurchaseCartDetail { get; set; }

    public virtual DbSet<tblSaleCartDetail> tblSaleCartDetail { get; set; }

    public virtual DbSet<tblStock> tblStock { get; set; }

    public virtual DbSet<tblSupplier> tblSupplier { get; set; }

    public virtual DbSet<tblSupplierInvoice> tblSupplierInvoice { get; set; }

    public virtual DbSet<tblSupplierInvoiceDetail> tblSupplierInvoiceDetail { get; set; }

    public virtual DbSet<tblSupplierPayment> tblSupplierPayment { get; set; }

    public virtual DbSet<tblSupplierReturnInvoice> tblSupplierReturnInvoice { get; set; }

    public virtual DbSet<tblSupplierReturnInvoiceDetail> tblSupplierReturnInvoiceDetail { get; set; }

    public virtual DbSet<tblSupplierReturnPayment> tblSupplierReturnPayment { get; set; }

    public virtual DbSet<tblSupportTicket> tblSupportTicket { get; set; }

    public virtual DbSet<tblTask> tblTask { get; set; }

    public virtual DbSet<tblTransaction> tblTransaction { get; set; }

    public virtual DbSet<tblUser> tblUser { get; set; }

    public virtual DbSet<tblUserType> tblUserType { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:clouderpdbserver.database.windows.net,1433;Initial Catalog=clouderpdb;Persist Security Info=False;User ID=clouderpdbserver;Password=Qwerty_009_008;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<tblAccountActivity>(entity =>
        {
            entity.HasKey(e => e.AccountActivityID);

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<tblAccountControl>(entity =>
        {
            entity.HasKey(e => e.AccountControlID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblAccountControl_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblAccountControl_tblCompany");

            entity.HasIndex(e => e.UserID, "IX_FK_tblAccountControl_tblUser");

            entity.Property(e => e.AccountControlName).HasMaxLength(50);

            entity.HasOne(d => d.AccountHead).WithMany(p => p.tblAccountControl)
                .HasForeignKey(d => d.AccountHeadID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAccountControl_tblAccountHead");

            entity.HasOne(d => d.Branch).WithMany(p => p.tblAccountControl)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAccountControl_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblAccountControl)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAccountControl_tblCompany");

            entity.HasOne(d => d.User).WithMany(p => p.tblAccountControl)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAccountControl_tblUser");
        });

        modelBuilder.Entity<tblAccountHead>(entity =>
        {
            entity.HasKey(e => e.AccountHeadID);

            entity.HasIndex(e => e.UserID, "IX_FK_tblAccountHead_tblUser");

            entity.Property(e => e.AccountHeadName).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.tblAccountHead)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAccountHead_tblUser");
        });

        modelBuilder.Entity<tblAccountSetting>(entity =>
        {
            entity.HasKey(e => e.AccountSettingID);

            entity.HasIndex(e => e.AccountActivityID, "IX_FK_tblAccountSetting_tblAccountActivity");

            entity.HasIndex(e => e.AccountControlID, "IX_FK_tblAccountSetting_tblAccountControl");

            entity.HasIndex(e => e.AccountHeadID, "IX_FK_tblAccountSetting_tblAccountHead");

            entity.HasIndex(e => e.AccountSubControlID, "IX_FK_tblAccountSetting_tblAccountSubControl");

            entity.HasIndex(e => e.BranchID, "IX_FK_tblAccountSetting_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblAccountSetting_tblCompany");

            entity.HasOne(d => d.AccountActivity).WithMany(p => p.tblAccountSetting)
                .HasForeignKey(d => d.AccountActivityID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAccountSetting_tblAccountActivity");

            entity.HasOne(d => d.AccountControl).WithMany(p => p.tblAccountSetting)
                .HasForeignKey(d => d.AccountControlID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAccountSetting_tblAccountControl");

            entity.HasOne(d => d.AccountHead).WithMany(p => p.tblAccountSetting)
                .HasForeignKey(d => d.AccountHeadID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAccountSetting_tblAccountHead");

            entity.HasOne(d => d.AccountSubControl).WithMany(p => p.tblAccountSetting)
                .HasForeignKey(d => d.AccountSubControlID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAccountSetting_tblAccountSubControl");

            entity.HasOne(d => d.Branch).WithMany(p => p.tblAccountSetting)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAccountSetting_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblAccountSetting)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAccountSetting_tblCompany");

            entity.HasOne(d => d.User).WithMany(p => p.tblAccountSetting)
                .HasForeignKey(d => d.UserID)
                .HasConstraintName("FK_tblAccountSetting_tblUser");
        });

        modelBuilder.Entity<tblAccountSubControl>(entity =>
        {
            entity.HasKey(e => e.AccountSubControlID);

            entity.HasIndex(e => e.AccountControlID, "IX_FK_tblAccountSubControl_tblAccountControl");

            entity.HasIndex(e => e.AccountHeadID, "IX_FK_tblAccountSubControl_tblAccountHead");

            entity.HasIndex(e => e.BranchID, "IX_FK_tblAccountSubControl_tblBranch");

            entity.HasIndex(e => e.UserID, "IX_FK_tblAccountSubControl_tblUser");

            entity.Property(e => e.AccountSubControlName).HasMaxLength(50);

            entity.HasOne(d => d.AccountControl).WithMany(p => p.tblAccountSubControl)
                .HasForeignKey(d => d.AccountControlID)
                .HasConstraintName("FK_tblAccountSubControl_tblAccountControl");

            entity.HasOne(d => d.AccountHead).WithMany(p => p.tblAccountSubControl)
                .HasForeignKey(d => d.AccountHeadID)
                .HasConstraintName("FK_tblAccountSubControl_tblAccountHead");

            entity.HasOne(d => d.Branch).WithMany(p => p.tblAccountSubControl)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAccountSubControl_tblBranch");

            entity.HasOne(d => d.User).WithMany(p => p.tblAccountSubControl)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblAccountSubControl_tblUser");
        });

        modelBuilder.Entity<tblBranch>(entity =>
        {
            entity.HasKey(e => e.BranchID);

            entity.HasIndex(e => e.BranchTypeID, "IX_FK_tblBranch_tblBranchType");

            entity.Property(e => e.BranchAddress).HasMaxLength(300);
            entity.Property(e => e.BranchContact).HasMaxLength(50);
            entity.Property(e => e.BranchName).HasMaxLength(50);

            entity.HasOne(d => d.BranchType).WithMany(p => p.tblBranch)
                .HasForeignKey(d => d.BranchTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblBranch_tblBranchType");
        });

        modelBuilder.Entity<tblBranchType>(entity =>
        {
            entity.HasKey(e => e.BranchTypeID);

            entity.Property(e => e.BranchType).HasMaxLength(50);
        });

        modelBuilder.Entity<tblCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblCategory_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblCategory_tblCompany");

            entity.HasIndex(e => e.UserID, "IX_FK_tblCategory_tblUser");

            entity.Property(e => e.CategoryName).HasMaxLength(50);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblCategory)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCategory_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblCategory)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCategory_tblCompany");

            entity.HasOne(d => d.User).WithMany(p => p.tblCategory)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCategory_tblUser");
        });

        modelBuilder.Entity<tblCompany>(entity =>
        {
            entity.HasKey(e => e.CompanyID);

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Logo).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<tblCustomer>(entity =>
        {
            entity.HasKey(e => e.CustomerID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblCustomer_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblCustomer_tblCompany");

            entity.HasIndex(e => e.UserID, "IX_FK_tblCustomer_tblUser");

            entity.Property(e => e.CustomerAddress).HasMaxLength(300);
            entity.Property(e => e.CustomerContact).HasMaxLength(150);
            entity.Property(e => e.Customername).HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(300);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblCustomer)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomer_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblCustomer)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomer_tblCompany");

            entity.HasOne(d => d.User).WithMany(p => p.tblCustomer)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomer_tblUser");
        });

        modelBuilder.Entity<tblCustomerInvoice>(entity =>
        {
            entity.HasKey(e => e.CustomerInvoiceID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblCustomerInvoice_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblCustomerInvoice_tblCompany");

            entity.HasIndex(e => e.CustomerID, "IX_FK_tblCustomerInvoice_tblCustomer");

            entity.HasIndex(e => e.UserID, "IX_FK_tblCustomerInvoice_tblUser");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNo).HasMaxLength(150);
            entity.Property(e => e.Title).HasMaxLength(150);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblCustomerInvoice)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerInvoice_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblCustomerInvoice)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerInvoice_tblCompany");

            entity.HasOne(d => d.Customer).WithMany(p => p.tblCustomerInvoice)
                .HasForeignKey(d => d.CustomerID)
                .HasConstraintName("FK_tblCustomerInvoice_tblCustomer");

            entity.HasOne(d => d.User).WithMany(p => p.tblCustomerInvoice)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerInvoice_tblUser");
        });

        modelBuilder.Entity<tblCustomerInvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.CustomerInvoiceDetailID);

            entity.HasIndex(e => e.CustomerInvoiceID, "IX_FK_tblCustomerInvoiceDetail_tblCustomerInvoice");

            entity.HasIndex(e => e.ProductID, "IX_FK_tblCustomerInvoiceDetail_tblStock");

            entity.HasOne(d => d.CustomerInvoice).WithMany(p => p.tblCustomerInvoiceDetail)
                .HasForeignKey(d => d.CustomerInvoiceID)
                .HasConstraintName("FK_tblCustomerInvoiceDetail_tblCustomerInvoice");

            entity.HasOne(d => d.Product).WithMany(p => p.tblCustomerInvoiceDetail)
                .HasForeignKey(d => d.ProductID)
                .HasConstraintName("FK_tblCustomerInvoiceDetail_tblStock");
        });

        modelBuilder.Entity<tblCustomerPayment>(entity =>
        {
            entity.HasKey(e => e.CustomerPaymentID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblCustomerPayment_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblCustomerPayment_tblCompany");

            entity.HasIndex(e => e.CustomerInvoiceID, "IX_FK_tblCustomerPayment_tblCustomerInvoice");

            entity.HasIndex(e => e.UserID, "IX_FK_tblCustomerPayment_tblUser");

            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNo).HasMaxLength(150);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblCustomerPayment)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerPayment_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblCustomerPayment)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerPayment_tblCompany");

            entity.HasOne(d => d.CustomerInvoice).WithMany(p => p.tblCustomerPayment)
                .HasForeignKey(d => d.CustomerInvoiceID)
                .HasConstraintName("FK_tblCustomerPayment_tblCustomerInvoice");

            entity.HasOne(d => d.User).WithMany(p => p.tblCustomerPayment)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerPayment_tblUser");
        });

        modelBuilder.Entity<tblCustomerReturnInvoice>(entity =>
        {
            entity.HasKey(e => e.CustomerReturnInvoiceID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblCustomerReturnInvoice_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblCustomerReturnInvoice_tblCompany");

            entity.HasIndex(e => e.CustomerID, "IX_FK_tblCustomerReturnInvoice_tblCustomer");

            entity.HasIndex(e => e.CustomerInvoiceID, "IX_FK_tblCustomerReturnInvoice_tblCustomerInvoice");

            entity.HasIndex(e => e.UserID, "IX_FK_tblCustomerReturnInvoice_tblUser");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNo).HasMaxLength(150);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblCustomerReturnInvoice)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnInvoice_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblCustomerReturnInvoice)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnInvoice_tblCompany");

            entity.HasOne(d => d.Customer).WithMany(p => p.tblCustomerReturnInvoice)
                .HasForeignKey(d => d.CustomerID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnInvoice_tblCustomer");

            entity.HasOne(d => d.CustomerInvoice).WithMany(p => p.tblCustomerReturnInvoice)
                .HasForeignKey(d => d.CustomerInvoiceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnInvoice_tblCustomerInvoice");

            entity.HasOne(d => d.User).WithMany(p => p.tblCustomerReturnInvoice)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnInvoice_tblUser");
        });

        modelBuilder.Entity<tblCustomerReturnInvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.CustomerReturnInvoiceDetailID);

            entity.HasIndex(e => e.CustomerInvoiceID, "IX_FK_tblCustomerReturnInvoiceDetail_tblCustomerInvoice");

            entity.HasIndex(e => e.CustomerInvoiceDetailID, "IX_FK_tblCustomerReturnInvoiceDetail_tblCustomerInvoiceDetail");

            entity.HasIndex(e => e.CustomerReturnInvoiceID, "IX_FK_tblCustomerReturnInvoiceDetail_tblCustomerReturnInvoice");

            entity.HasIndex(e => e.ProductID, "IX_FK_tblCustomerReturnInvoiceDetail_tblStock");

            entity.HasOne(d => d.CustomerInvoiceDetail).WithMany(p => p.tblCustomerReturnInvoiceDetail)
                .HasForeignKey(d => d.CustomerInvoiceDetailID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnInvoiceDetail_tblCustomerInvoiceDetail");

            entity.HasOne(d => d.CustomerInvoice).WithMany(p => p.tblCustomerReturnInvoiceDetail)
                .HasForeignKey(d => d.CustomerInvoiceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnInvoiceDetail_tblCustomerInvoice");

            entity.HasOne(d => d.CustomerReturnInvoice).WithMany(p => p.tblCustomerReturnInvoiceDetail)
                .HasForeignKey(d => d.CustomerReturnInvoiceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnInvoiceDetail_tblCustomerReturnInvoice");

            entity.HasOne(d => d.Product).WithMany(p => p.tblCustomerReturnInvoiceDetail)
                .HasForeignKey(d => d.ProductID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnInvoiceDetail_tblStock");
        });

        modelBuilder.Entity<tblCustomerReturnPayment>(entity =>
        {
            entity.HasKey(e => e.CustomerReturnPaymentID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblCustomerReturnPayment_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblCustomerReturnPayment_tblCompany");

            entity.HasIndex(e => e.CustomerID, "IX_FK_tblCustomerReturnPayment_tblCustomer");

            entity.HasIndex(e => e.CustomerInvoiceID, "IX_FK_tblCustomerReturnPayment_tblCustomerInvoice");

            entity.HasIndex(e => e.CustomerReturnInvoiceID, "IX_FK_tblCustomerReturnPayment_tblCustomerReturnInvoice");

            entity.HasIndex(e => e.UserID, "IX_FK_tblCustomerReturnPayment_tblUser");

            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNo).HasMaxLength(150);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblCustomerReturnPayment)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnPayment_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblCustomerReturnPayment)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnPayment_tblCompany");

            entity.HasOne(d => d.Customer).WithMany(p => p.tblCustomerReturnPayment)
                .HasForeignKey(d => d.CustomerID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnPayment_tblCustomer");

            entity.HasOne(d => d.CustomerInvoice).WithMany(p => p.tblCustomerReturnPayment)
                .HasForeignKey(d => d.CustomerInvoiceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnPayment_tblCustomerInvoice");

            entity.HasOne(d => d.CustomerReturnInvoice).WithMany(p => p.tblCustomerReturnPayment)
                .HasForeignKey(d => d.CustomerReturnInvoiceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnPayment_tblCustomerReturnInvoice");

            entity.HasOne(d => d.User).WithMany(p => p.tblCustomerReturnPayment)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblCustomerReturnPayment_tblUser");
        });

        modelBuilder.Entity<tblEmployee>(entity =>
        {
            entity.HasKey(e => e.EmployeeID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblEmployee_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblEmployee_tblCompany");

            entity.Property(e => e.Address).HasMaxLength(300);
            entity.Property(e => e.ContactNo).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.Designation).HasMaxLength(150);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Photo).HasMaxLength(150);
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");
            entity.Property(e => e.TIN).HasMaxLength(50);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblEmployee)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblEmployee_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblEmployee)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblEmployee_tblCompany");
        });

        modelBuilder.Entity<tblFinancialYear>(entity =>
        {
            entity.HasKey(e => e.FinancialYearID);

            entity.HasIndex(e => e.UserID, "IX_FK_tblFinancialYear_tblUser");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.FinancialYear).HasMaxLength(150);
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.tblFinancialYear)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblFinancialYear_tblUser");
        });

        modelBuilder.Entity<tblPayroll>(entity =>
        {
            entity.HasKey(e => e.PayrollID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblPayroll_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblPayroll_tblCompany");

            entity.HasIndex(e => e.EmployeeID, "IX_FK_tblPayroll_tblEmployee");

            entity.HasIndex(e => e.UserID, "IX_FK_tblPayroll_tblUser");

            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PayrollInvoiceNo).HasMaxLength(150);
            entity.Property(e => e.SalaryMonth).HasMaxLength(50);
            entity.Property(e => e.SalaryYear).HasMaxLength(50);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblPayroll)
                .HasForeignKey(d => d.BranchID)
                .HasConstraintName("FK_tblPayroll_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblPayroll)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPayroll_tblCompany");

            entity.HasOne(d => d.Employee).WithMany(p => p.tblPayroll)
                .HasForeignKey(d => d.EmployeeID)
                .HasConstraintName("FK_tblPayroll_tblEmployee");

            entity.HasOne(d => d.User).WithMany(p => p.tblPayroll)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPayroll_tblUser");
        });

        modelBuilder.Entity<tblPurchaseCart>(entity =>
        {
            entity.HasKey(e => e.PurchaseCartID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblPurchaseCartTable_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblPurchaseCartTable_tblCompany");

            entity.HasIndex(e => e.SupplierID, "IX_FK_tblPurchaseCartTable_tblSupplier");

            entity.HasIndex(e => e.UserID, "IX_FK_tblPurchaseCart_tblUser");

            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");

            entity.HasOne(d => d.Branch).WithMany(p => p.tblPurchaseCart)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPurchaseCartTable_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblPurchaseCart)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPurchaseCartTable_tblCompany");

            entity.HasOne(d => d.Supplier).WithMany(p => p.tblPurchaseCart)
                .HasForeignKey(d => d.SupplierID)
                .HasConstraintName("FK_tblPurchaseCartTable_tblSupplier");

            entity.HasOne(d => d.User).WithMany(p => p.tblPurchaseCart)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPurchaseCart_tblUser");
        });

        modelBuilder.Entity<tblPurchaseCartDetail>(entity =>
        {
            entity.HasKey(e => e.PurchaseCartDetailID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblPurchaseCartDetail_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblPurchaseCartDetail_tblCompany");

            entity.HasIndex(e => e.ProductID, "IX_FK_tblPurchaseCartDetail_tblStock");

            entity.HasIndex(e => e.UserID, "IX_FK_tblPurchaseCartDetail_tblUser");

            entity.HasOne(d => d.Branch).WithMany(p => p.tblPurchaseCartDetail)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPurchaseCartDetail_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblPurchaseCartDetail)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPurchaseCartDetail_tblCompany");

            entity.HasOne(d => d.Product).WithMany(p => p.tblPurchaseCartDetail)
                .HasForeignKey(d => d.ProductID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPurchaseCartDetail_tblStock");

            entity.HasOne(d => d.User).WithMany(p => p.tblPurchaseCartDetail)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPurchaseCartDetail_tblUser");
        });

        modelBuilder.Entity<tblSaleCartDetail>(entity =>
        {
            entity.HasKey(e => e.SaleCartDetailID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblSaleCartDetail_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblSaleCartDetail_tblCompany");

            entity.HasIndex(e => e.ProductID, "IX_FK_tblSaleCartDetail_tblStock");

            entity.HasIndex(e => e.UserID, "IX_FK_tblSaleCartDetail_tblUser");

            entity.HasOne(d => d.Branch).WithMany(p => p.tblSaleCartDetail)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSaleCartDetail_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblSaleCartDetail)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSaleCartDetail_tblCompany");

            entity.HasOne(d => d.Product).WithMany(p => p.tblSaleCartDetail)
                .HasForeignKey(d => d.ProductID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSaleCartDetail_tblStock");

            entity.HasOne(d => d.User).WithMany(p => p.tblSaleCartDetail)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSaleCartDetail_tblUser");
        });

        modelBuilder.Entity<tblStock>(entity =>
        {
            entity.HasKey(e => e.ProductID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblStock_tblBranch");

            entity.HasIndex(e => e.CategoryID, "IX_FK_tblStock_tblCategory");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblStock_tblCompany");

            entity.HasIndex(e => e.UserID, "IX_FK_tblStock_tblUser");

            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.Manufacture).HasColumnType("datetime");
            entity.Property(e => e.ProductName).HasMaxLength(80);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblStock)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblStock_tblBranch");

            entity.HasOne(d => d.Category).WithMany(p => p.tblStock)
                .HasForeignKey(d => d.CategoryID)
                .HasConstraintName("FK_tblStock_tblCategory");

            entity.HasOne(d => d.Company).WithMany(p => p.tblStock)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblStock_tblCompany");

            entity.HasOne(d => d.User).WithMany(p => p.tblStock)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblStock_tblUser");
        });

        modelBuilder.Entity<tblSupplier>(entity =>
        {
            entity.HasKey(e => e.SupplierID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblSupplier_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblSupplier_tblCompany");

            entity.HasIndex(e => e.UserID, "IX_FK_tblSupplier_tblUser");

            entity.Property(e => e.Discription).HasMaxLength(300);
            entity.Property(e => e.SupplierAddress).HasMaxLength(150);
            entity.Property(e => e.SupplierConatctNo).HasMaxLength(20);
            entity.Property(e => e.SupplierEmail).HasMaxLength(150);
            entity.Property(e => e.SupplierName).HasMaxLength(150);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblSupplier)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplier_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblSupplier)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplier_tblCompany");

            entity.HasOne(d => d.User).WithMany(p => p.tblSupplier)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplier_tblUser");
        });

        modelBuilder.Entity<tblSupplierInvoice>(entity =>
        {
            entity.HasKey(e => e.SupplierInvoiceID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblSupplierInvoiceTable_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblSupplierInvoiceTable_tblCompany");

            entity.HasIndex(e => e.SupplierID, "IX_FK_tblSupplierInvoiceTable_tblSupplier");

            entity.HasIndex(e => e.UserID, "IX_FK_tblSupplierInvoice_tblUser");

            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNo).HasMaxLength(150);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblSupplierInvoice)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierInvoiceTable_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblSupplierInvoice)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierInvoiceTable_tblCompany");

            entity.HasOne(d => d.Supplier).WithMany(p => p.tblSupplierInvoice)
                .HasForeignKey(d => d.SupplierID)
                .HasConstraintName("FK_tblSupplierInvoiceTable_tblSupplier");

            entity.HasOne(d => d.User).WithMany(p => p.tblSupplierInvoice)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierInvoice_tblUser");
        });

        modelBuilder.Entity<tblSupplierInvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.SupplierInvoiceDetailID);

            entity.HasIndex(e => e.ProductID, "IX_FK_tblSupplierInvoiceDetail_tblStock");

            entity.HasIndex(e => e.SupplierInvoiceID, "IX_FK_tblSupplierInvoiceDetail_tblSupplierInvoice");

            entity.HasOne(d => d.Product).WithMany(p => p.tblSupplierInvoiceDetail)
                .HasForeignKey(d => d.ProductID)
                .HasConstraintName("FK_tblSupplierInvoiceDetail_tblStock");

            entity.HasOne(d => d.SupplierInvoice).WithMany(p => p.tblSupplierInvoiceDetail)
                .HasForeignKey(d => d.SupplierInvoiceID)
                .HasConstraintName("FK_tblSupplierInvoiceDetail_tblSupplierInvoice");
        });

        modelBuilder.Entity<tblSupplierPayment>(entity =>
        {
            entity.HasKey(e => e.SupplierPaymentID);

            entity.HasIndex(e => e.SupplierID, "IX_FK_tblSupplierPayment_tblSupplier");

            entity.HasIndex(e => e.UserID, "IX_FK_tblSupplierPayment_tblUser");

            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNo).HasMaxLength(150);

            entity.HasOne(d => d.Supplier).WithMany(p => p.tblSupplierPayment)
                .HasForeignKey(d => d.SupplierID)
                .HasConstraintName("FK_tblSupplierPayment_tblSupplier");

            entity.HasOne(d => d.User).WithMany(p => p.tblSupplierPayment)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierPayment_tblUser");
        });

        modelBuilder.Entity<tblSupplierReturnInvoice>(entity =>
        {
            entity.HasKey(e => e.SupplierReturnInvoiceID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblSupplierReturnInvoice_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblSupplierReturnInvoice_tblCompany");

            entity.HasIndex(e => e.SupplierID, "IX_FK_tblSupplierReturnInvoice_tblSupplier");

            entity.HasIndex(e => e.SupplierInvoiceID, "IX_FK_tblSupplierReturnInvoice_tblSupplierInvoice");

            entity.HasIndex(e => e.UserID, "IX_FK_tblSupplierReturnInvoice_tblUser");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNo).HasMaxLength(100);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblSupplierReturnInvoice)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnInvoice_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblSupplierReturnInvoice)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnInvoice_tblCompany");

            entity.HasOne(d => d.Supplier).WithMany(p => p.tblSupplierReturnInvoice)
                .HasForeignKey(d => d.SupplierID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnInvoice_tblSupplier");

            entity.HasOne(d => d.SupplierInvoice).WithMany(p => p.tblSupplierReturnInvoice)
                .HasForeignKey(d => d.SupplierInvoiceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnInvoice_tblSupplierInvoice");

            entity.HasOne(d => d.User).WithMany(p => p.tblSupplierReturnInvoice)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnInvoice_tblUser");
        });

        modelBuilder.Entity<tblSupplierReturnInvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.SupplierReturnInvoiceDetailID);

            entity.HasIndex(e => e.ProductID, "IX_FK_tblSupplierReturnInvoiceDetail_tblStock");

            entity.HasIndex(e => e.SupplierInvoiceID, "IX_FK_tblSupplierReturnInvoiceDetail_tblSupplierInvoice");

            entity.HasIndex(e => e.SupplierInvoiceDetailID, "IX_FK_tblSupplierReturnInvoiceDetail_tblSupplierInvoiceDetail");

            entity.HasIndex(e => e.SupplierReturnInvoiceID, "IX_FK_tblSupplierReturnInvoiceDetail_tblSupplierReturnInvoice");

            entity.HasOne(d => d.Product).WithMany(p => p.tblSupplierReturnInvoiceDetail)
                .HasForeignKey(d => d.ProductID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnInvoiceDetail_tblStock");

            entity.HasOne(d => d.SupplierInvoiceDetail).WithMany(p => p.tblSupplierReturnInvoiceDetail)
                .HasForeignKey(d => d.SupplierInvoiceDetailID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnInvoiceDetail_tblSupplierInvoiceDetail");

            entity.HasOne(d => d.SupplierInvoice).WithMany(p => p.tblSupplierReturnInvoiceDetail)
                .HasForeignKey(d => d.SupplierInvoiceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnInvoiceDetail_tblSupplierInvoice");

            entity.HasOne(d => d.SupplierReturnInvoice).WithMany(p => p.tblSupplierReturnInvoiceDetail)
                .HasForeignKey(d => d.SupplierReturnInvoiceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnInvoiceDetail_tblSupplierReturnInvoice");
        });

        modelBuilder.Entity<tblSupplierReturnPayment>(entity =>
        {
            entity.HasKey(e => e.SupplierReturnPaymentID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblSupplierReturnPayment_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblSupplierReturnPayment_tblCompany");

            entity.HasIndex(e => e.SupplierID, "IX_FK_tblSupplierReturnPayment_tblSupplier");

            entity.HasIndex(e => e.SupplierInvoiceID, "IX_FK_tblSupplierReturnPayment_tblSupplierInvoice");

            entity.HasIndex(e => e.SupplierReturnInvoiceID, "IX_FK_tblSupplierReturnPayment_tblSupplierReturnInvoice");

            entity.HasIndex(e => e.UserID, "IX_FK_tblSupplierReturnPayment_tblUser");

            entity.Property(e => e.InvoiceDate).HasColumnType("datetime");
            entity.Property(e => e.InvoiceNo).HasMaxLength(150);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblSupplierReturnPayment)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnPayment_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblSupplierReturnPayment)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnPayment_tblCompany");

            entity.HasOne(d => d.Supplier).WithMany(p => p.tblSupplierReturnPayment)
                .HasForeignKey(d => d.SupplierID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnPayment_tblSupplier");

            entity.HasOne(d => d.SupplierInvoice).WithMany(p => p.tblSupplierReturnPayment)
                .HasForeignKey(d => d.SupplierInvoiceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnPayment_tblSupplierInvoice");

            entity.HasOne(d => d.SupplierReturnInvoice).WithMany(p => p.tblSupplierReturnPayment)
                .HasForeignKey(d => d.SupplierReturnInvoiceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnPayment_tblSupplierReturnInvoice");

            entity.HasOne(d => d.User).WithMany(p => p.tblSupplierReturnPayment)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupplierReturnPayment_tblUser");
        });

        modelBuilder.Entity<tblSupportTicket>(entity =>
        {
            entity.HasKey(e => e.TicketID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblSupportTicket_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblSupportTicket_tblCompany");

            entity.HasIndex(e => e.UserID, "IX_FK_tblSupportTicket_tblUser");

            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Message).HasMaxLength(400);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RespondedBy).HasMaxLength(100);
            entity.Property(e => e.ResponseDate).HasColumnType("datetime");
            entity.Property(e => e.Subject).HasMaxLength(100);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblSupportTicket)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupportTicket_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblSupportTicket)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupportTicket_tblCompany");

            entity.HasOne(d => d.User).WithMany(p => p.tblSupportTicket)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSupportTicket_tblUser");
        });

        modelBuilder.Entity<tblTask>(entity =>
        {
            entity.HasKey(e => e.TaskID);

            entity.HasIndex(e => e.BranchID, "IX_FK_tblTask_tblBranch");

            entity.HasIndex(e => e.CompanyID, "IX_FK_tblTask_tblCompany");

            entity.HasIndex(e => e.UserID, "IX_FK_tblTask_tblUser");

            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.ReminderDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Branch).WithMany(p => p.tblTask)
                .HasForeignKey(d => d.BranchID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblTask_tblBranch");

            entity.HasOne(d => d.Company).WithMany(p => p.tblTask)
                .HasForeignKey(d => d.CompanyID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblTask_tblCompany");

            entity.HasOne(d => d.User).WithMany(p => p.tblTask)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblTask_tblUser");
        });

        modelBuilder.Entity<tblTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionID);

            entity.HasIndex(e => e.AccountControlID, "IX_FK_tblTransaction_tblAccountControl");

            entity.HasIndex(e => e.AccountHeadID, "IX_FK_tblTransaction_tblAccountHead");

            entity.HasIndex(e => e.FinancialYearID, "IX_FK_tblTransaction_tblFinancialYear");

            entity.HasIndex(e => e.UserID, "IX_FK_tblTransaction_tblUser");

            entity.HasIndex(e => e.AccountHeadID, "IX_FK_tblTransection_tblAccountHead");

            entity.HasIndex(e => e.AccountSubControlID, "IX_FK_tblTransection_tblAccountSubControl");

            entity.HasIndex(e => e.FinancialYearID, "IX_FK_tblTransection_tblFinancialYear");

            entity.Property(e => e.InvoiceNo).HasMaxLength(150);
            entity.Property(e => e.TransectionDate).HasColumnType("datetime");
            entity.Property(e => e.TransectionTitle).HasMaxLength(150);

            entity.HasOne(d => d.AccountControl).WithMany(p => p.tblTransaction)
                .HasForeignKey(d => d.AccountControlID)
                .HasConstraintName("FK_tblTransaction_tblAccountControl");

            entity.HasOne(d => d.AccountHead).WithMany(p => p.tblTransaction)
                .HasForeignKey(d => d.AccountHeadID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblTransaction_tblAccountHead");

            entity.HasOne(d => d.AccountSubControl).WithMany(p => p.tblTransaction)
                .HasForeignKey(d => d.AccountSubControlID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblTransection_tblAccountSubControl");

            entity.HasOne(d => d.FinancialYear).WithMany(p => p.tblTransaction)
                .HasForeignKey(d => d.FinancialYearID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblTransaction_tblFinancialYear");

            entity.HasOne(d => d.User).WithMany(p => p.tblTransaction)
                .HasForeignKey(d => d.UserID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblTransaction_tblUser");
        });

        modelBuilder.Entity<tblUser>(entity =>
        {
            entity.HasKey(e => e.UserID);

            entity.HasIndex(e => e.UserTypeID, "IX_FK_tblUser_tblUserType");

            entity.Property(e => e.ContactNo).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.LastPasswordResetRequest).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(150);
            entity.Property(e => e.ResetPasswordCode).HasMaxLength(150);
            entity.Property(e => e.ResetPasswordExpiration).HasColumnType("datetime");
            entity.Property(e => e.Salt).HasMaxLength(200);
            entity.Property(e => e.UserName).HasMaxLength(150);

            entity.HasOne(d => d.UserType).WithMany(p => p.tblUser)
                .HasForeignKey(d => d.UserTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblUser_tblUserType");
        });

        modelBuilder.Entity<tblUserType>(entity =>
        {
            entity.HasKey(e => e.UserTypeID);

            entity.Property(e => e.UserType).HasMaxLength(150);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
