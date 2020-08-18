using System;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace NomadMVC.Models
{
    public class CustomerModel {
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public string PrimaryContactId { get; set; }
        public string Name { get; set; }
        public string PriceLevelCode { get; set; }
        public bool IsTaxable { get; set; }
        public string TaxGroupCode { get; set; }
        public string TaxCodeNumber { get; set; }
        public string CustomerTypeCode { get; set; }
        public string PaymentTermCode { get; set; }
        public string AuthorizationCode { get; set; }
        public string TermAddressId { get; set; }
        public string PrimaryAddressId { get; set; }
        public float AvailableCredit { get; set; }
        public float CreditLimit { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public string TimeStamp { get; set; }
    }

    public class EliasModel {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string ParentName { get; set; }
        public IFormFile Image { get; set; }
        public byte[] ImageArray { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class EliasTypeModel {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
