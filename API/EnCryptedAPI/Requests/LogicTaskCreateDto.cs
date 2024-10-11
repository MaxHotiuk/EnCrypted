using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests;

public class LogicTaskCreateDto
{
    public Guid TaskID { get; set; }
    public required string AllTextData { get; set; }
    public required bool IsEncrypted { get; set; }
    public required string PassPhrase { get; set; }
}