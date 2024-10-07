using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests
{
    public class EncryptDataDto
    {
        public string Data { get; set; } = null!;
        public Guid TaskID { get; set; }
        public bool DataEncrypted { get; set; }
    }
}