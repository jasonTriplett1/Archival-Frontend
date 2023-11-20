using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMDataRepository.CustomModels
{
    [Table("DataArchivalStatus")]
    public class DataArchivalStatus
    {
        public string StatusName { get; set; }

        public int Count { get; set; }

    }
}
