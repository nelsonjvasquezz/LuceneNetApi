using LuceneNetApi.Models;
using System.Collections.Generic;

namespace LuceneNetApi.Services
{
    public interface IDocumentoGestionadoService : IIndexService
    {
        IEnumerable<DocumentoGestionado> Search(string searchTerms, int documentLimit, int? areaCodigo = null, int? tipoDocumentoCodigo = null);
    }
}
