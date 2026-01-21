using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureFileDelivery.Domain.Enums;

// Represents the current status of a download link
public enum LinkStatus
{
    // Link is active and can be used
    Active = 1,

    // Link has been used and accessed
    Used = 2,

    // Link has expired based on time limit
    Expired = 3,

    // Link has been manually revoked/disabled
    Revoked = 4
}