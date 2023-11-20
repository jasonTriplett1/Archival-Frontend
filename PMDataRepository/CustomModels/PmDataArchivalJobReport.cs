using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMDataRepository.CustomModels
{
    [Table("PmDataArchivalJobReport")]
    public class PmDataArchivalJobReport
    {
        public string name { get; set; }

        public byte enabled { get; set; }

        public DateTime? start_execution_date { get; set; }

        public DateTime? stop_execution_date { get; set; }

        public DateTime? next_scheduled_run_date { get; set; }

    }
}
