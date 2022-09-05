namespace BuscaCnpj.Business.Utils
{
    public static class Utilidades
    {
        public static string FormataCnpj(string cnpj)
        {
            cnpj = cnpj.Replace(".", "");
            cnpj = cnpj.Replace("/", "");
            cnpj = cnpj.Replace("-", "");
            return cnpj;
        }
    }
}
