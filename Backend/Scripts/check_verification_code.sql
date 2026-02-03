-- Check if verification code exists and get related property data
-- Run this to debug verification code: PRO-2026-D3XU8P

-- Check if verification code exists
SELECT 
    dv.Id,
    dv.VerificationCode,
    dv.DocumentId,
    dv.DocumentType,
    dv.IsRevoked,
    dv.RevokedReason,
    dv.CreatedAt,
    dv.CreatedBy
FROM org.DocumentVerifications dv
WHERE dv.VerificationCode = 'PRO-2026-D3XU8P';

-- If verification exists, check the property details
SELECT 
    pd.Id,
    pd.PNumber,
    pd.Parea,
    pd.Price,
    pd.iscomplete,
    pd.CreatedAt,
    pd.CreatedBy,
    pd.CompanyId
FROM org.PropertyDetails pd
WHERE pd.Id = (
    SELECT DocumentId 
    FROM org.DocumentVerifications 
    WHERE VerificationCode = 'PRO-2026-D3XU8P' 
    AND DocumentType = 'PropertyDocument'
);

-- Check seller details for the property
SELECT 
    sd.Id,
    sd.FirstName,
    sd.FatherName,
    sd.GrandFather,
    sd.ElectronicNationalIdNumber,
    sd.PhoneNumber,
    sd.Photo,
    sd.PaddressProvinceId,
    sd.PaddressDistrictId,
    sd.PaddressVillage,
    sd.PropertyDetailsId
FROM org.SellerDetails sd
WHERE sd.PropertyDetailsId = (
    SELECT DocumentId 
    FROM org.DocumentVerifications 
    WHERE VerificationCode = 'PRO-2026-D3XU8P' 
    AND DocumentType = 'PropertyDocument'
);

-- Check buyer details for the property
SELECT 
    bd.Id,
    bd.FirstName,
    bd.FatherName,
    bd.GrandFather,
    bd.ElectronicNationalIdNumber,
    bd.PhoneNumber,
    bd.Photo,
    bd.PaddressProvinceId,
    bd.PaddressDistrictId,
    bd.PaddressVillage,
    bd.PropertyDetailsId
FROM org.BuyerDetails bd
WHERE bd.PropertyDetailsId = (
    SELECT DocumentId 
    FROM org.DocumentVerifications 
    WHERE VerificationCode = 'PRO-2026-D3XU8P' 
    AND DocumentType = 'PropertyDocument'
);

-- Check property address
SELECT 
    pa.Id,
    pa.ProvinceId,
    pa.DistrictId,
    pa.Village,
    pa.PropertyDetailsId
FROM org.PropertyAddresses pa
WHERE pa.PropertyDetailsId = (
    SELECT DocumentId 
    FROM org.DocumentVerifications 
    WHERE VerificationCode = 'PRO-2026-D3XU8P' 
    AND DocumentType = 'PropertyDocument'
);
