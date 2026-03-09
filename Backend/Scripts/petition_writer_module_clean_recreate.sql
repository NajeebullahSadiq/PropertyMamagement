    -- =====================================================
    -- Petition Writer License Module Clean Recreation Script
    -- =====================================================
    -- Purpose: Drop and recreate all petition writer license module tables
    -- Schema: org (organization)
    -- Date: 2026-03-09
    -- 
    -- WARNING: THIS SCRIPT WILL DELETE ALL PETITION WRITER LICENSE DATA!
    -- =====================================================

    -- =====================================================
    -- STEP 1: DROP ALL EXISTING TABLES
    -- =====================================================

    DO $$ 
    BEGIN
        RAISE NOTICE 'Starting petition writer license module table cleanup...';
    END $$;

    -- Drop tables in correct order (children first, then parents)
    DROP TABLE IF EXISTS org."PetitionWriterRelocations" CASCADE;
    DROP TABLE IF EXISTS org."PetitionWriterLicenses" CASCADE;

    DO $$ 
    BEGIN
        RAISE NOTICE '✓ All petition writer license module tables dropped successfully';
    END $$;

    -- =====================================================
    -- STEP 2: ENSURE SCHEMAS EXIST
    -- =====================================================

    CREATE SCHEMA IF NOT EXISTS org;
    CREATE SCHEMA IF NOT EXISTS look;

    -- =====================================================
    -- STEP 3: CREATE TRANSACTION TABLES (org schema)
    -- =====================================================

    -- 1. PetitionWriterLicenses (Main petition writer license table)
    CREATE TABLE org."PetitionWriterLicenses" (
        "Id" SERIAL PRIMARY KEY,
        
        -- License Information
        "LicenseNumber" VARCHAR(50) NOT NULL,
        "ProvinceId" INTEGER,
        
        -- Tab 1: مشخصات عریضه‌نویس (Petition Writer Information)
        "ApplicantName" VARCHAR(200) NOT NULL,
        "ApplicantFatherName" VARCHAR(200),
        "ApplicantGrandFatherName" VARCHAR(200),
        "MobileNumber" VARCHAR(20),
        "Competency" VARCHAR(50),
        
        -- Identity Card Information
        "ElectronicNationalIdNumber" VARCHAR(50) NOT NULL,
        
        -- Permanent Address (سکونت اصلی)
        "PermanentProvinceId" INTEGER,
        "PermanentDistrictId" INTEGER,
        "PermanentVillage" VARCHAR(500),
        
        -- Current Address (سکونت فعلی)
        "CurrentProvinceId" INTEGER,
        "CurrentDistrictId" INTEGER,
        "CurrentVillage" VARCHAR(500),
        
        -- Activity Location (ساحه فعالیت)
        "DetailedAddress" VARCHAR(1000),
        "ActivityLocation" VARCHAR(500),
        "ActivityNahia" VARCHAR(200),
        
        -- Picture Path
        "PicturePath" VARCHAR(500),
        
        -- Tab 2: ثبت مالیه و مشخصات جواز (Financial Info & License Details)
        "BankReceiptNumber" VARCHAR(100),
        "BankReceiptDate" DATE,
        "District" VARCHAR(200),
        "LicenseType" VARCHAR(50),
        "LicensePrice" DECIMAL(18, 2),
        "LicenseCost" DECIMAL(18, 2),
        "LicenseIssueDate" DATE,
        "LicenseExpiryDate" DATE,
        
        -- Tab 3: لغو / انصراف (Cancellation/Withdrawal)
        "LicenseStatus" INTEGER DEFAULT 1,
        "CancellationDate" DATE,
        
        -- Soft delete
        "Status" BOOLEAN DEFAULT true,
        
        -- Audit Fields
        "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
        "CreatedBy" VARCHAR(50),
        "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE,
        "UpdatedBy" VARCHAR(50),
        
        -- Foreign Keys
        CONSTRAINT "FK_PetitionWriterLicenses_Province" 
            FOREIGN KEY ("ProvinceId") 
            REFERENCES look."Location"("ID") 
            ON DELETE SET NULL,
        CONSTRAINT "FK_PetitionWriterLicenses_PermanentProvince" 
            FOREIGN KEY ("PermanentProvinceId") 
            REFERENCES look."Location"("ID") 
            ON DELETE SET NULL,
        CONSTRAINT "FK_PetitionWriterLicenses_PermanentDistrict" 
            FOREIGN KEY ("PermanentDistrictId") 
            REFERENCES look."Location"("ID") 
            ON DELETE SET NULL,
        CONSTRAINT "FK_PetitionWriterLicenses_CurrentProvince" 
            FOREIGN KEY ("CurrentProvinceId") 
            REFERENCES look."Location"("ID") 
            ON DELETE SET NULL,
        CONSTRAINT "FK_PetitionWriterLicenses_CurrentDistrict" 
            FOREIGN KEY ("CurrentDistrictId") 
            REFERENCES look."Location"("ID") 
            ON DELETE SET NULL,
            
        -- Constraints
        CONSTRAINT "CHK_PetitionWriterLicenses_LicenseStatus" 
            CHECK ("LicenseStatus" IN (1, 2, 3)),
        CONSTRAINT "UQ_PetitionWriterLicenses_LicenseNumber" 
            UNIQUE ("LicenseNumber")
    );

    -- 2. PetitionWriterRelocations (Relocation History)
    CREATE TABLE org."PetitionWriterRelocations" (
        "Id" SERIAL PRIMARY KEY,
        "PetitionWriterLicenseId" INTEGER NOT NULL,
        "NewActivityLocation" VARCHAR(500) NOT NULL,
        "RelocationDate" DATE,
        "Remarks" VARCHAR(1000),
        
        -- Audit Fields
        "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
        "CreatedBy" VARCHAR(50),
        
        -- Foreign Keys
        CONSTRAINT "FK_PetitionWriterRelocations_License" 
            FOREIGN KEY ("PetitionWriterLicenseId") 
            REFERENCES org."PetitionWriterLicenses"("Id") 
            ON DELETE CASCADE
    );

    DO $$ 
    BEGIN
        RAISE NOTICE '✓ All 2 petition writer license module tables created successfully';
    END $$;

    -- =====================================================
    -- STEP 4: CREATE INDEXES FOR PERFORMANCE
    -- =====================================================

    -- PetitionWriterLicenses indexes
    CREATE INDEX "IX_PetitionWriterLicenses_ProvinceId" 
        ON org."PetitionWriterLicenses"("ProvinceId");
    CREATE INDEX "IX_PetitionWriterLicenses_LicenseNumber" 
        ON org."PetitionWriterLicenses"("LicenseNumber");
    CREATE INDEX "IX_PetitionWriterLicenses_ApplicantName" 
        ON org."PetitionWriterLicenses"("ApplicantName");
    CREATE INDEX "IX_PetitionWriterLicenses_ElectronicNationalIdNumber" 
        ON org."PetitionWriterLicenses"("ElectronicNationalIdNumber");
    CREATE INDEX "IX_PetitionWriterLicenses_LicenseType" 
        ON org."PetitionWriterLicenses"("LicenseType");
    CREATE INDEX "IX_PetitionWriterLicenses_LicenseStatus" 
        ON org."PetitionWriterLicenses"("LicenseStatus");
    CREATE INDEX "IX_PetitionWriterLicenses_LicenseIssueDate" 
        ON org."PetitionWriterLicenses"("LicenseIssueDate");
    CREATE INDEX "IX_PetitionWriterLicenses_LicenseExpiryDate" 
        ON org."PetitionWriterLicenses"("LicenseExpiryDate");
    CREATE INDEX "IX_PetitionWriterLicenses_Status" 
        ON org."PetitionWriterLicenses"("Status");
    CREATE INDEX "IX_PetitionWriterLicenses_CreatedAt" 
        ON org."PetitionWriterLicenses"("CreatedAt");

    -- PetitionWriterRelocations indexes
    CREATE INDEX "IX_PetitionWriterRelocations_LicenseId" 
        ON org."PetitionWriterRelocations"("PetitionWriterLicenseId");
    CREATE INDEX "IX_PetitionWriterRelocations_RelocationDate" 
        ON org."PetitionWriterRelocations"("RelocationDate");
    CREATE INDEX "IX_PetitionWriterRelocations_CreatedAt" 
        ON org."PetitionWriterRelocations"("CreatedAt");

    DO $$ 
    BEGIN
        RAISE NOTICE '✓ All performance indexes created successfully';
    END $$;

    -- =====================================================
    -- STEP 5: ADD COLUMN COMMENTS FOR DOCUMENTATION
    -- =====================================================

    -- PetitionWriterLicenses comments
    COMMENT ON TABLE org."PetitionWriterLicenses" IS 'ثبت جواز عریضه‌نویسان - Petition Writer License Registration';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."LicenseNumber" IS 'نمبر جواز - License Number (Format: PROVINCE_CODE-SEQUENTIAL_NUMBER)';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."ProvinceId" IS 'ولایت - Province for license numbering';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."ApplicantName" IS 'نام متقاضی - Applicant Name';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."ApplicantFatherName" IS 'نام پدر - Father Name';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."ApplicantGrandFatherName" IS 'نام پدر کلان - Grandfather Name';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."MobileNumber" IS 'شماره تماس - Mobile Number';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."Competency" IS 'درجه اهلیت - Competency Level (اعلی, اوسط, ادنی)';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."ElectronicNationalIdNumber" IS 'الیکټرونیکی تذکره - Electronic National ID';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."PermanentProvinceId" IS 'ولایت سکونت اصلی - Permanent Province';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."PermanentDistrictId" IS 'ولسوالی سکونت اصلی - Permanent District';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."PermanentVillage" IS 'قریه سکونت اصلی - Permanent Village';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."CurrentProvinceId" IS 'ولایت سکونت فعلی - Current Province';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."CurrentDistrictId" IS 'ولسوالی سکونت فعلی - Current District';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."CurrentVillage" IS 'قریه سکونت فعلی - Current Village';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."DetailedAddress" IS 'ادرس دقیق محل فعالیت - Detailed Activity Address';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."ActivityLocation" IS 'محل فعالیت عریضه‌نویس - Activity Location';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."ActivityNahia" IS 'ناحیه محل فعالیت - Activity Nahia';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."PicturePath" IS 'عکس - Picture Path';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."BankReceiptNumber" IS 'نمبر رسید بانکی - Bank Receipt Number';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."BankReceiptDate" IS 'تاریخ رسید بانکی - Bank Receipt Date';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."District" IS 'ناحیه - District';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."LicenseType" IS 'نوعیت جواز - License Type (جدید, تجدید, مثنی)';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."LicensePrice" IS 'قیمت جواز - License Price';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."LicenseCost" IS 'قیمت - License Cost (based on type)';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."LicenseIssueDate" IS 'تاریخ صدور - Issue Date';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."LicenseExpiryDate" IS 'تاریخ ختم - Expiry Date';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."LicenseStatus" IS 'وضعیت جواز - License Status (1=Active, 2=Cancelled, 3=Withdrawn)';
    COMMENT ON COLUMN org."PetitionWriterLicenses"."CancellationDate" IS 'تاریخ لغو - Cancellation Date';

    -- PetitionWriterRelocations comments
    COMMENT ON TABLE org."PetitionWriterRelocations" IS 'نقل مکان - Relocation History';
    COMMENT ON COLUMN org."PetitionWriterRelocations"."PetitionWriterLicenseId" IS 'شناسه جواز - License ID';
    COMMENT ON COLUMN org."PetitionWriterRelocations"."NewActivityLocation" IS 'محل فعالیت جدید - New Activity Location';
    COMMENT ON COLUMN org."PetitionWriterRelocations"."RelocationDate" IS 'تاریخ نقل مکان - Relocation Date';
    COMMENT ON COLUMN org."PetitionWriterRelocations"."Remarks" IS 'ملاحظات - Remarks';

    DO $$ 
    BEGIN
        RAISE NOTICE '✓ All column comments added successfully';
    END $$;

    -- =====================================================
    -- STEP 6: VERIFICATION QUERIES
    -- =====================================================

    DO $$ 
    DECLARE
        transaction_table_count INTEGER;
        index_count INTEGER;
        constraint_count INTEGER;
    BEGIN
        -- Count transaction tables
        SELECT COUNT(*) INTO transaction_table_count
        FROM information_schema.tables 
        WHERE table_schema = 'org' 
        AND table_name IN ('PetitionWriterLicenses', 'PetitionWriterRelocations');
        
        -- Count indexes
        SELECT COUNT(*) INTO index_count
        FROM pg_indexes 
        WHERE schemaname = 'org'
        AND tablename LIKE 'PetitionWriter%';
        
        -- Count constraints
        SELECT COUNT(*) INTO constraint_count
        FROM information_schema.table_constraints
        WHERE table_schema = 'org'
        AND table_name IN ('PetitionWriterLicenses', 'PetitionWriterRelocations');
        
        RAISE NOTICE '';
        RAISE NOTICE '========================================';
        RAISE NOTICE 'Petition Writer License Module Recreation Complete!';
        RAISE NOTICE '========================================';
        RAISE NOTICE '';
        RAISE NOTICE 'Tables Created:';
        RAISE NOTICE '  - org.PetitionWriterLicenses';
        RAISE NOTICE '  - org.PetitionWriterRelocations';
        RAISE NOTICE '';
        RAISE NOTICE 'Latest Features Included:';
        RAISE NOTICE '  ✓ Province-based access control (ProvinceId)';
        RAISE NOTICE '  ✓ Province-specific license numbering';
        RAISE NOTICE '  ✓ Electronic National ID support';
        RAISE NOTICE '  ✓ Competency level tracking (اعلی, اوسط, ادنی)';
        RAISE NOTICE '  ✓ License types (جدید, تجدید, مثنی)';
        RAISE NOTICE '  ✓ License cost auto-calculation';
        RAISE NOTICE '  ✓ Mobile number field';
        RAISE NOTICE '  ✓ Activity location and Nahia fields';
        RAISE NOTICE '  ✓ Permanent and current address tracking';
        RAISE NOTICE '  ✓ License status tracking (Active, Cancelled, Withdrawn)';
        RAISE NOTICE '  ✓ Relocation history tracking';
        RAISE NOTICE '  ✓ Picture/photo support';
        RAISE NOTICE '  ✓ Unique license number constraint';
        RAISE NOTICE '  ✓ Comprehensive audit fields';
        RAISE NOTICE '  ✓ Performance indexes (% total)', index_count;
        RAISE NOTICE '';
        RAISE NOTICE 'Verification:';
        RAISE NOTICE '  - Transaction tables: % / 2', transaction_table_count;
        RAISE NOTICE '  - Indexes created: %', index_count;
        RAISE NOTICE '  - Constraints: %', constraint_count;
        RAISE NOTICE '';
        RAISE NOTICE 'Next Steps:';
        RAISE NOTICE '  1. Verify table structure above';
        RAISE NOTICE '  2. Test petition writer license registration in frontend';
        RAISE NOTICE '  3. Verify all fields save correctly';
        RAISE NOTICE '  4. Test relocation functionality';
        RAISE NOTICE '  5. Verify license number generation';
        RAISE NOTICE '';
    END $$;