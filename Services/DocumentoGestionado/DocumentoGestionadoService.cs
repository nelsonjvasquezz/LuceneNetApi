using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using LuceneNetApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LuceneNetApi.Services
{
    public class DocumentoGestionadoService : IDocumentoGestionadoService
    {
        private readonly string _indexPath;

        public LuceneVersion LuceneVersion => LuceneVersion.LUCENE_48;

        public DocumentoGestionadoService(string indexPath)
        {
            _indexPath = indexPath;
        }

        public bool CanCreateIt(string indexName)
        {
            throw new System.NotImplementedException();
        }

        public void CreateIndex()
        {
            var documentos = GetDocumentosGestionados();
            using var directory = FSDirectory.Open(_indexPath);
            using var analyzer = new StandardAnalyzer(LuceneVersion);
            var config = new IndexWriterConfig(LuceneVersion, analyzer)
            {
                OpenMode = OpenMode.CREATE // Set the OpenMode to CREATE to delete existing index
            };
            using var writer = new IndexWriter(directory, config);

            // Delete existing documents
            writer.DeleteAll();

            foreach (var documento in documentos)
            {
                var doc = new Document
                {
                    new Int32Field("doc_codigo", documento.Codigo, Field.Store.YES),
                    new Int32Field("doc_codtdo", documento.TipoDocumentoCodigo, Field.Store.YES),
                    new Int32Field("doc_codare", documento.AreaCodigo, Field.Store.YES),
                    new TextField("doc_titulo", documento.Titulo, Field.Store.YES),
                    new TextField("doc_descripcion", documento.Descripcion, Field.Store.YES),
                    new TextField("doc_palabras_claves", documento.PalabrasClave, Field.Store.YES),
                };

                writer.AddDocument(doc);
            }

            writer.Commit();
            writer.Flush(triggerMerge: false, applyAllDeletes: false);
        }

        private IEnumerable<DocumentoGestionado> GetDocumentosGestionados()
        {
            var json = System.IO.File.ReadAllText("documentsData.json");
            var documentos = JsonConvert.DeserializeObject<List<DocumentoGestionado>>(json);

            return documentos;
        }

        public IEnumerable<DocumentoGestionado> Search(string searchTerms, int documentLimit, int? areaCodigo = null, int? tipoDocumentoCodigo = null)
        {
            using var directory = FSDirectory.Open(_indexPath);
            using var reader = DirectoryReader.Open(directory);
            using var analyzer = new StandardAnalyzer(LuceneVersion);
            var searcher = new IndexSearcher(reader);

            // Crear la consulta principal
            var mainQuery = new BooleanQuery();

            if (!string.IsNullOrWhiteSpace(searchTerms))
            {
                var titleQuery = new QueryParser(LuceneVersion, "doc_titulo", analyzer).Parse(searchTerms);
                var keywordsQuery = new QueryParser(LuceneVersion, "doc_palabras_claves", analyzer).Parse(searchTerms);

                mainQuery.Add(titleQuery, Occur.SHOULD);
                mainQuery.Add(keywordsQuery, Occur.SHOULD);
            }
            else
            {
                mainQuery.Add(new MatchAllDocsQuery(), Occur.MUST);
            }

            // Construir la consulta boolean
            var booleanQuery = new BooleanQuery
            {
                { mainQuery, Occur.MUST }
            };

            // Filtro para código de área
            if (areaCodigo.HasValue)
            {
                var areaFilter = NumericRangeQuery.NewInt32Range("doc_codare", areaCodigo.Value, areaCodigo.Value, true, true);
                booleanQuery.Add(areaFilter, Occur.MUST);
            }

            // Filtro para código de tipo de documento
            if (tipoDocumentoCodigo.HasValue)
            {
                var tipoDocumentoFilter = NumericRangeQuery.NewInt32Range("doc_codtdo", tipoDocumentoCodigo.Value, tipoDocumentoCodigo.Value, true, true);
                booleanQuery.Add(tipoDocumentoFilter, Occur.MUST);
            }

            // Ejecutar la consulta
            var topDocs = searcher.Search(booleanQuery, documentLimit).ScoreDocs;
            var results = new List<DocumentoGestionado>();

            foreach (var scoreDoc in topDocs)
            {
                var document = searcher.Doc(scoreDoc.Doc);
                results.Add(new DocumentoGestionado
                {
                    Codigo = int.Parse(document.Get("doc_codigo")),
                    TipoDocumentoCodigo = int.Parse(document.Get("doc_codtdo")),
                    AreaCodigo = int.Parse(document.Get("doc_codare")),
                    Titulo = document.Get("doc_titulo"),
                    Descripcion = document.Get("doc_descripcion"),
                    PalabrasClave = document.Get("doc_palabras_claves"),
                });
            }

            return results;
        }
    }
}
