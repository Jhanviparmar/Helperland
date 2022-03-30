using Helperland.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Helperland.Models
{
    public class SPSettings
    {
        public User Users { get; set; }
        public UserAddress UserAddresses { get; set; }
    }
}
