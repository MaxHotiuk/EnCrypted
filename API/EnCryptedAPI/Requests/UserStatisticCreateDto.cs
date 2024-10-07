using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnCryptedAPI.Requests;
public class UserStatisticCreateDto
{
    public int TotalTasksCompleted { get; set; }
    public int TotalTimeTracked { get; set; }
}