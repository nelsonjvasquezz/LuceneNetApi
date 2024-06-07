using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using LuceneNetApi.Models;
using System;
using System.Collections.Generic;

namespace LuceneNetApi.Services
{
    public class ExpedienteService : IExpedienteService
    {
        public LuceneVersion LuceneVersion => LuceneVersion.LUCENE_48;
        private readonly string _indexPath;

        public ExpedienteService(string indexPath)
        {
            _indexPath = indexPath;
        }

        public string IndexPath => throw new NotImplementedException();

        public bool CanCreateIt(string indexName)
        {
            return indexName.Equals("Expedientes", StringComparison.OrdinalIgnoreCase);
        }

        public void CreateIndex()
        {
            var expedientes = GetExpedientes();
            using var directory = FSDirectory.Open(_indexPath);
            using var analyzer = new StandardAnalyzer(LuceneVersion);
            var config = new IndexWriterConfig(LuceneVersion, analyzer)
            {
                OpenMode = OpenMode.CREATE
            };
            using var writer = new IndexWriter(directory, config);

            foreach (var expediente in expedientes)
            {
                var doc = new Document
                {
                    new Int32Field("Codigo", expediente.Codigo, Field.Store.YES),
                    new TextField("Nombres", expediente.Nombres, Field.Store.YES),
                    new TextField("Apellidos", expediente.Apellidos, Field.Store.YES),
                    new TextField("Sexo", expediente.Sexo, Field.Store.YES),
                    new TextField("Profesion", expediente.Profesion, Field.Store.YES)
                };

                writer.AddDocument(doc);
            }

            writer.Commit();
        }

        public IEnumerable<Expediente> Search(string searchTerms, int documentLimit)
        {
            using var directory = FSDirectory.Open(_indexPath);
            using var analyzer = new StandardAnalyzer(LuceneVersion);
            using var reader = DirectoryReader.Open(directory);
            var searcher = new IndexSearcher(reader);

            var parser = new QueryParser(LuceneVersion, "Nombres", analyzer);
            var query = parser.Parse(searchTerms);

            var hits = searcher.Search(query, documentLimit).ScoreDocs;

            var results = new List<Expediente>();

            foreach (var hit in hits)
            {
                var doc = searcher.Doc(hit.Doc);
                var expediente = new Expediente
                {
                    Codigo = doc.GetField("Codigo").GetInt32Value().Value,
                    Nombres = doc.GetField("Nombres").GetStringValue(),
                    Apellidos = doc.GetField("Apellidos").GetStringValue(),
                    Sexo = doc.GetField("Sexo").GetStringValue(),
                    Profesion = doc.GetField("Profesion").GetStringValue()
                };

                results.Add(expediente);
            }

            return results;
        }

        private IEnumerable<Expediente> GetExpedientes() => [
                new Expediente(){
                    Codigo = 1,
                    Nombres = "Expediente 1",
                    Apellidos = "Descripcion del expediente 1",
                    Sexo = "Masculino",
                    Profesion = "Profesion 1"
                },
                new Expediente(){
                    Codigo = 2,
                    Nombres = "Expediente 2",
                    Apellidos = "Descripcion del expediente 2",
                    Sexo = "Femenino",
                    Profesion = "Profesion 2"
                },
                new Expediente(){
                    Codigo = 3,
                    Nombres = "Expediente 3",
                    Apellidos = "Descripcion del expediente 3",
                    Sexo = "Masculino",
                    Profesion = "Profesion 3"
                },
                new Expediente(){
                    Codigo = 4,
                    Nombres = "Expediente 4",
                    Apellidos = "Descripcion del expediente 4",
                    Sexo = "Femenino",
                    Profesion = "Profesion 4"
                },
            ];
    }
}
