using LuceneNetApi.Models;
using System.Collections.Generic;

namespace LuceneNetApi.Services
{
    public interface IExpedienteService : IIndexService
    {
        IEnumerable<Expediente> Search(string searchTerms, int documentLimit);
    }
}