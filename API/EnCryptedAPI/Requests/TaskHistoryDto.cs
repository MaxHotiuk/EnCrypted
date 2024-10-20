using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests;

public class TaskHistoryDto
{
    public Guid TaskID { get; set; }
    public required string TaskStatus { get; set; }
}