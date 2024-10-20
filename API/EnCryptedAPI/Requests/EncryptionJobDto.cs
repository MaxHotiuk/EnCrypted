using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests;

public class EncryptionJobDto
{
    public Guid JobID { get; set; }
    public required string JobStatus { get; set; }
}