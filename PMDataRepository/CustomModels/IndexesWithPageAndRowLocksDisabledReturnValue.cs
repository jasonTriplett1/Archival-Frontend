using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMDataRepository.CustomModels
{
    [Table("IndexesWithPageAndRowLocksDisabledReturnValues")]
    public class IndexesWithPageAndRowLocksDisabledReturnValue
    {
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public bool? allow_page_locks { get; set; }
        public bool? allow_row_locks { get; set; }
        public string FixCommand { get; set; }
        public const string FromSql = @"SELECT t.name AS TableName
	,i.name AS IndexName
	,i.allow_page_locks
	,i.allow_row_locks
	,'ALTER INDEX [' + I.name + '] ON [' + SCHEMA_NAME(T.SCHEMA_ID) + '].[' + T.name + '] REBUILD WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON)' AS FixCommand
FROM PlayerManagement.sys.indexes i
JOIN PlayerManagement.sys.tables t ON t.object_id = i.object_id
WHERE (
		allow_row_locks <> 1
		OR allow_page_locks <> 1
		)
	AND t.is_ms_shipped = 0
ORDER BY TableName
	,IndexName
";

    }

}
