using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models;

public class Subscriptions
{
    public int Id { get; set; }
    public int SubTaskIdSubscriber { get; set; }
    public int MainTaskIdTopic { get; set; }

}
