using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests
{
    public class EncryptRequestDto
    {
        public required string passPhrase { get; set; }
        public Guid encryptionJobId { get; set; }
    }
}