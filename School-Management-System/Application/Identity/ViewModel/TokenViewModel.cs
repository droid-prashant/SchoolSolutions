using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Identity.ViewModel
{
    public class TokenViewModel
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string Error { get; set; }
        public int StatusCode { get; set; }
        public bool Succeded { get; set; }
    }
}
