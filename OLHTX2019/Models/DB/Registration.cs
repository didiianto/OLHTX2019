//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OLHTX2019.Models.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class Registration
    {
        public System.Guid Id { get; set; }
        public string SerialNo { get; set; }
        public string Name { get; set; }
        public string IDType { get; set; }
        public string IDNumber { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Organisation { get; set; }
        public byte[] Image { get; set; }
        public string ImageType { get; set; }
        public string ImageName { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public Nullable<System.DateTime> DateSubmited { get; set; }
        public string Designation { get; set; }
        public string StepsAction { get; set; }
        public Nullable<bool> IsOpenForm { get; set; }
    }
}
