using LuceneNetApi.Models;
using System.Collections.Generic;

namespace LuceneNetApi.Services
{
    public interface ICandidatoService : IIndexService
    {
        IEnumerable<Candidato> Search(string searchTerms, int documentLimit);
    }
}