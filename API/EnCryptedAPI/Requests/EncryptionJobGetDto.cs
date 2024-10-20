using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests;

public class EncryptionJobGetDto
{
    public Guid JobID { get; set; }
    public required string EncryptedData { get; set; }
}