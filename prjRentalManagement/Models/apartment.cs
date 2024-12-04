//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace prjRentalManagement.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class apartment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public apartment()
        {
            this.eventOwners = new HashSet<eventOwner>();
        }
    
        public int apartmentId { get; set; }

        [Required(ErrorMessage = "Apartment No. is required.")]
        [Range(1, 9999, ErrorMessage = "Apartment No. must be a number between 1 and 9999.")]
        [Display(Name = "Apartment No.")]
        public int apartmentNo { get; set; }

        [Required(ErrorMessage = "Number of rooms is required.")]
        [Range(1, 10, ErrorMessage = "The number of rooms must be between 1 and 10.")]
        [Display(Name = "Number of rooms")]
        public int nbRooms { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be a positive value.")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid currency format.")]
        [Display(Name = "Price")]
        public decimal price { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(10, ErrorMessage = "Status cannot exceed 10 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Status must contain only letters and spaces.")]
        [Display(Name = "Apartment Status")]
        public string status { get; set; }

        [Display(Name = "Building ID")]
        public int buildingId { get; set; }

        [Display(Name = "Tenant ID")]
        public Nullable<int> tenantId { get; set; }
    
        public virtual building building { get; set; }
        public virtual tenant tenant { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<eventOwner> eventOwners { get; set; }
    }
}
