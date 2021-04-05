using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker.Models
{
    public class ExternalAPIs
    {
        private ExternalAPIsContext context;
        [Key]

        public int APIKeyId { get; set; }
        public string Service { get; set; }
        public string APIKey { get; set; }
        public string KeyOwner { get; set; }
        public int Quota { get; set; }
        public DateTime ActiveDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public string APIHost {get; set;}

    }
}
