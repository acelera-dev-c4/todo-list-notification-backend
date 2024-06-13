using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models;

public class Notifications
{
    public int? Id { get; set; }
    public int SubscriptionId { get; set; }
    public string Message { get; set; } = "";
    public bool Read { get; set; } = false;
}
