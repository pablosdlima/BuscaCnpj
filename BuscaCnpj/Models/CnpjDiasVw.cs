using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BuscaCnpj.Models
{
    public class CnpjDiasVw
    {
        [DefaultValue(1)]
        [Required(ErrorMessage = "'Dias' é obrigatório")]
        public int Dias { get; set; }

        [Required(ErrorMessage = "CNPJ é obrigatório")]
        [RegularExpression(@"(^(\d{2}.\d{3}.\d{3}/\d{4}-\d{2})|(\d{14})$)", ErrorMessage = "Informe um CNPJ válido")]
        public string Cnpj { get; set; }
    }
}
