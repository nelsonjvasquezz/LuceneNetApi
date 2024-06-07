using LuceneNetApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace LuceneNetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndexController : ControllerBase
    {
        private readonly IEnumerable<IIndexService> _indexServices;
        private readonly ILogger<IndexController> _logger;

        public IndexController(IEnumerable<IIndexService> indexServices, ILogger<IndexController> logger)
        {
            _indexServices = indexServices;
            _logger = logger;
        }

        [HttpPost("create-index/{indexName}")]
        public ActionResult CreateIndex(string indexName)
        {
            _logger.LogInformation("Creating index {IndexName}", indexName);
            var indexService = _indexServices.FirstOrDefault(service => service.CanCreateIt(indexName));

            if (indexService == null)
            {
                _logger.LogWarning("Index {IndexName} not found", indexName);
                return NotFound();
            }

            indexService.CreateIndex();
            _logger.LogInformation("Index {IndexName} created successfully", indexName);

            return NoContent();
        }
    }
}
