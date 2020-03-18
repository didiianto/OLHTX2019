using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OLHTX2019.Models.ViewModel
{
    public class AuditLogViewModel
    {
        public Guid Id { get; set; }
        public Guid RegId { get; set; }
        public string AdminId { get; set; }
        public string Action { get; set; }
        public string Module { get; set; }
        public string SubModule { get; set; }
        public string DataChage { get; set; }
        public DateTime DateAction { get; set; }
    }
}