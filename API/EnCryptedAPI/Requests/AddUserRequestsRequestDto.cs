using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests
{
    public class AddUserRequestsRequestDto
    {
        public required string Text { get; set; }
        public required bool IsEncrypted { get; set; }
        public required int UserId { get; set; }
    }
}