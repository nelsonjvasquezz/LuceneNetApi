using LuceneNetApi.Models;
using LuceneNetApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace LuceneNetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LuceneController : ControllerBase
    {
        private readonly ILuceneService _luceneService;
        private readonly ILogger<LuceneController> _logger;

        public LuceneController(ILuceneService luceneService, ILogger<LuceneController> logger)
        {
            _luceneService = luceneService;
            _logger = logger;
        }

        [HttpPost("create-index")]
        public ActionResult CreateIndex()
        {
            var json = System.IO.File.ReadAllText("documentsData.json");
            var documentos = JsonConvert.DeserializeObject<List<DocumentoGestionado>>(json);

            _logger.LogInformation("Creating index for {documentCount} documents", documentos.Count);

            _luceneService.CreateIndex(documentos);

            _logger.LogInformation("Index created successfully");

            return NoContent();
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<DocumentoGestionado>> Search([FromBody] SearchDto searchDto)
        {
            _logger.LogInformation("Searching for documents with query: {query} and limit: {documentLimit}", searchDto.Query, searchDto.DocumentLimit);

            var documentos = _luceneService.Search(searchDto.Query, searchDto.DocumentLimit, searchDto.AreaCodigo, searchDto.TipoDocumentoCodigo);

            _logger.LogInformation("Found {documentCount} documents", documentos.Count());

            return documentos.ToList();
        }
    }
}
