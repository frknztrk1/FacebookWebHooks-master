using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacebookWebHooks.Entities
{
    [Table("FacebookAdsLogs")]
    public class FacebookAdsLog
    {
        public int Id { get; set; }
        public string LogText { get; set; }
        public DateTime LogDate { get; set; } = DateTime.Now;

    }
}
