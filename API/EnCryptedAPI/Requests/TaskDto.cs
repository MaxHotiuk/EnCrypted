using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests;
public class TaskDto
{
    public required string Title { get; set; }
    public required string Description { get; set; }
}