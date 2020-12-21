using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookWebHooks.Entities
{
    public class FacebookAdsFormData
    {
        public int Id { get; set; }
        public string PageId { get; set; }
        public string FormName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime CreateTime { get; set; }

    }
}
