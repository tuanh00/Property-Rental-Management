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

    public partial class appointment
    {
        public int appointmentId { get; set; }
        public int managerId { get; set; }
        public int tenantId { get; set; }

        [Display(Name = "Appointment Date")]
        public System.DateTime appointmentDate { get; set; }
    
        public virtual manager manager { get; set; }
        public virtual tenant tenant { get; set; }
    }
}
