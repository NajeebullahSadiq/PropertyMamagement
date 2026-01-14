using System;
using System.Collections.Generic;

namespace WebAPIBackend.Models;

public partial class Location
{
    public int Id { get; set; }

    public string Dari { get; set; } = null!;

    public int IsActive { get; set; }

    public string? Code { get; set; }

    public string? Path { get; set; }

    public string? PathDari { get; set; }

    public int? ParentId { get; set; }

    public int? TypeId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<BuyerDetail> BuyerDetailPaddressDistricts { get; } = new List<BuyerDetail>();

    public virtual ICollection<BuyerDetail> BuyerDetailPaddressProvinces { get; } = new List<BuyerDetail>();

    public virtual ICollection<BuyerDetail> BuyerDetailTaddressDistricts { get; } = new List<BuyerDetail>();

    public virtual ICollection<BuyerDetail> BuyerDetailTaddressProvinces { get; } = new List<BuyerDetail>();

    public virtual ICollection<CompanyOwnerAddress> CompanyOwnerAddressDistricts { get; } = new List<CompanyOwnerAddress>();

    public virtual ICollection<CompanyOwnerAddress> CompanyOwnerAddressProvinces { get; } = new List<CompanyOwnerAddress>();

    public virtual ICollection<Guarantor> GuarantorPaddressDistricts { get; } = new List<Guarantor>();

    public virtual ICollection<Guarantor> GuarantorPaddressProvinces { get; } = new List<Guarantor>();

    public virtual ICollection<Guarantor> GuarantorTaddressDistricts { get; } = new List<Guarantor>();

    public virtual ICollection<Guarantor> GuarantorTaddressProvinces { get; } = new List<Guarantor>();

    public virtual ICollection<SellerDetail> SellerDetailPaddressDistricts { get; } = new List<SellerDetail>();

    public virtual ICollection<SellerDetail> SellerDetailPaddressProvinces { get; } = new List<SellerDetail>();

    public virtual ICollection<SellerDetail> SellerDetailTaddressDistricts { get; } = new List<SellerDetail>();

    public virtual ICollection<SellerDetail> SellerDetailTaddressProvinces { get; } = new List<SellerDetail>();

    public virtual ICollection<VehiclesBuyerDetail> VehiclesBuyerDetailPaddressDistricts { get; } = new List<VehiclesBuyerDetail>();

    public virtual ICollection<VehiclesBuyerDetail> VehiclesBuyerDetailPaddressProvinces { get; } = new List<VehiclesBuyerDetail>();

    public virtual ICollection<VehiclesBuyerDetail> VehiclesBuyerDetailTaddressDistricts { get; } = new List<VehiclesBuyerDetail>();

    public virtual ICollection<VehiclesBuyerDetail> VehiclesBuyerDetailTaddressProvinces { get; } = new List<VehiclesBuyerDetail>();

    public virtual ICollection<VehiclesSellerDetail> VehiclesSellerDetailPaddressDistricts { get; } = new List<VehiclesSellerDetail>();

    public virtual ICollection<VehiclesSellerDetail> VehiclesSellerDetailPaddressProvinces { get; } = new List<VehiclesSellerDetail>();

    public virtual ICollection<VehiclesSellerDetail> VehiclesSellerDetailTaddressDistricts { get; } = new List<VehiclesSellerDetail>();

    public virtual ICollection<VehiclesSellerDetail> VehiclesSellerDetailTaddressProvinces { get; } = new List<VehiclesSellerDetail>();
}
