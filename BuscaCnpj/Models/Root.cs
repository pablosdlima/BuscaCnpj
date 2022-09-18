using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BuscaCnpj.Models
{
    public class Root
    {
        public string status { get; set; }
        public DateTime? ultima_atualizacao { get; set; }

        [Required(ErrorMessage = "CNPJ é obrigatório")]
        [RegularExpression(@"(^(\d{2}.\d{3}.\d{3}/\d{4}-\d{2})|(\d{14})$)", ErrorMessage = "Informe um CNPJ válido")] //cnpj
        public string cnpj { get; set; }
        public string tipo { get; set; }
        public string porte { get; set; }
        public string nome { get; set; }
        public string fantasia { get; set; }
        public string abertura { get; set; }
        public AtividadePrincipal atividade_principal { get; set; }
        public List<AtividadesSecundaria> atividades_secundarias { get; set; }
        public string natureza_juridica { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string cep { get; set; }
        public string bairro { get; set; }
        public string municipio { get; set; }
        public string uf { get; set; }
        public string email { get; set; }
        public string telefone { get; set; }
        public string efr { get; set; }
        public string situacao { get; set; }
        public string data_situacao { get; set; }
        public string motivo_situacao { get; set; }
        public string situacao_especial { get; set; }
        public string data_situacao_especial { get; set; }
        public string capital_social { get; set; }
        public List<Qsa> qsa { get; set; }
        public Billing billing { get; set; }

        public static implicit operator Root(Task<Root> v)
        {
            throw new NotImplementedException();
        }
    }
}