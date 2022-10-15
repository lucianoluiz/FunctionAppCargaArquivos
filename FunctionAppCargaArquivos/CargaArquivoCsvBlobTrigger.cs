using Dapper.Contrib.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace FunctionAppCargaArquivos
{
    public class CargaArquivoCsvBlobTrigger
    {
        [FunctionName("CargaArquivoCsvBlobTrigger")]
        public async Task Run([BlobTrigger("arquivocsv-processamento/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"Arquivo: {name}");

            if (!name.ToLower().Contains(".csv"))
            {
                log.LogError($"O arquivo {name} não será processado já que possui uma extensão inválida!");
                return;
            }

            if (myBlob.Length > 0)
            {
                using var reader = new StreamReader(myBlob);
                using var conexaoSql = new SqlConnection(Environment.GetEnvironmentVariable("DefaultConnection"));

                Arquivo arquivo = new Arquivo
                {
                    Nome = name,
                    DataCarga = DateTime.Now
                };

                await conexaoSql.InsertAsync(arquivo);
                log.LogInformation($"Id gerado para o arquivo: {arquivo.IdArquivo}");

                int numLinha = 1;
                string linha = await reader.ReadLineAsync();

                while (linha != null)
                {
                    await conexaoSql.InsertAsync(new LinhaArquivo
                    {
                        IdArquivo = arquivo.IdArquivo,
                        NumLinha = numLinha,
                        Conteudo = linha
                    });
                    log.LogInformation($"Linha {numLinha}: {linha}");

                    numLinha++;
                    linha = await reader.ReadLineAsync();
                }

                log.LogInformation($"Concluído o processamento do arquivo {name}");
            }
        }
    }
}
