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
    
    public partial class Setting
    {
        public System.Guid Id { get; set; }
        public string WebUrl { get; set; }
        public string RootStorageVirtual { get; set; }
        public string RootStoragePhysical { get; set; }
        public string RunningNo { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public string EmailSender { get; set; }
        public string EmailSenderName { get; set; }
        public string EmailCC { get; set; }
        public string EmailBcc { get; set; }
        public string EmailConfirmationSubject { get; set; }
        public string SerialFormat { get; set; }
        public string EmailHost { get; set; }
        public Nullable<int> EmailPort { get; set; }
        public string WebLocation { get; set; }
        public string EmailUsername { get; set; }
        public string EmailPassword { get; set; }
        public string EmailInviteSubject { get; set; }
    }
}
