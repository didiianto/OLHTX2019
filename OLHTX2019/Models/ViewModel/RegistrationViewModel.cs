using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OLHTX2019.Models.ViewModel
{
    public class RegistrationViewModel
    {
        public Guid Id { get; set; }
        public string IdStringEncypted { get; set; }
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
        public string StepAction { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateSubmited { get; set; }
        public string DateSubmitedString { get; set; }
        public string Designation { get; set; }
        public bool IsOpenForm { get; set; }
        public string OpenForm { get; set; }
        public string AdminRole { get; set; }
    }
    public class RegistrationListViewModel
    {
        public List<RegistrationViewModel> RegistrationViewModelList { get; set; }
    }
}