using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureFileDelivery.Domain.Enums;

// Represents the type of file stored in the system
public enum FileType
{
    // PDF document (primary format for bank statements)
    Pdf = 1,

    // CSV file (for transaction exports)
    Csv = 2,

    // Excel file (for detailed reports)
    Excel = 3,

    // Other supported document types
    Other = 99
}
