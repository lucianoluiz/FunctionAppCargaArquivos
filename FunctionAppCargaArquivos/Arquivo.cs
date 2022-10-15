using Dapper.Contrib.Extensions;
using System;

namespace FunctionAppCargaArquivos
{
    [Table("dbo.Arquivos")]
    public class Arquivo
    {
        [Key]
        public int IdArquivo { get; set; }
        public string Nome { get; set; }
        public DateTime? DataCarga { get; set; }
    }
}