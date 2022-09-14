using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BuscaCnpj.Models
{
    public class Lote
    {
        [Required]
        public string Cnpjs;
        public List<string> ListaLote;
    }
}
