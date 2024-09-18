using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewChatServer
{
    public class Clients
    {

        public Guid GuidID { get; set; } = Guid.NewGuid();
        public string Name { get; set; }

    }
}
