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
    public class CandidatoService : ICandidatoService
    {
        private readonly string _indexPath;

        public LuceneVersion LuceneVersion => LuceneVersion.LUCENE_48;

        public CandidatoService(string indexPath)
        {
            _indexPath = indexPath;
        }

        public bool CanCreateIt(string indexName)
        {
            return indexName.Equals("Candidato", StringComparison.OrdinalIgnoreCase);
        }

        public void CreateIndex()
        {
            using var indexDirectory = FSDirectory.Open(_indexPath);
            using var analyzer = new StandardAnalyzer(LuceneVersion);
            var indexWriterConfig = new IndexWriterConfig(LuceneVersion, analyzer)
            {
                OpenMode = OpenMode.CREATE
            };

            using (var indexWriter = new IndexWriter(indexDirectory, indexWriterConfig))
            {
                var candidatos = GetCandidatos();

                foreach (var candidato in candidatos)
                {
                    var document = new Document
                    {
                        new StringField("Codigo", candidato.Codigo.ToString(), Field.Store.YES),
                        new TextField("DescripcionPersonal", candidato.DescripcionPersonal, Field.Store.YES),
                        new TextField("ExperienciaLaboral", candidato.ExperienciaLaboral, Field.Store.YES),
                        new TextField("Educacion", candidato.Educacion, Field.Store.YES),
                        new TextField("Habilidades", candidato.Habilidades, Field.Store.YES)
                    };

                    indexWriter.AddDocument(document);
                }

                indexWriter.Commit();
            }
        }

        public IEnumerable<Candidato> Search(string searchTerms, int documentLimit)
        {
            using var indexDirectory = FSDirectory.Open(_indexPath);
            using var indexReader = DirectoryReader.Open(indexDirectory);
            using var analyzer = new StandardAnalyzer(LuceneVersion);
            var indexSearcher = new IndexSearcher(indexReader);
            var queryParser = new MultiFieldQueryParser(LuceneVersion, ["DescripcionPersonal", "ExperienciaLaboral", "Educacion", "Habilidades"], analyzer);
            var query = queryParser.Parse(searchTerms);
            var topDocs = indexSearcher.Search(query, documentLimit);

            var candidatos = new List<Candidato>();
            foreach (var scoreDoc in topDocs.ScoreDocs)
            {
                var document = indexSearcher.Doc(scoreDoc.Doc);
                var candidato = new Candidato
                {
                    Codigo = int.Parse(document.Get("Codigo")),
                    DescripcionPersonal = document.Get("DescripcionPersonal"),
                    ExperienciaLaboral = document.Get("ExperienciaLaboral"),
                    Educacion = document.Get("Educacion"),
                    Habilidades = document.Get("Habilidades")
                };

                candidatos.Add(candidato);
            }

            return candidatos;
        }

        private static IEnumerable<Candidato> GetCandidatos() => [
                new Candidato
                {
                    Codigo = 1,
                    DescripcionPersonal = "Organizado y responsable",
                    ExperienciaLaboral = "Senior Developer en Acme Inc.",
                    Educacion = "Ingeniero en Sistemas",
                    Habilidades = "Capacidad de liderazgo"
                },
                new Candidato
                {
                    Codigo = 2,
                    DescripcionPersonal = "Creativo y proactivo",
                    ExperienciaLaboral = "Desarrollador en Beta Corp.",
                    Educacion = "Licenciado en Informática",
                    Habilidades = "Trabajo en equipo"
                },
                new Candidato
                {
                    Codigo = 3,
                    DescripcionPersonal = "Dinámico y entusiasta",
                    ExperienciaLaboral = "Programador en Gamma Ltd.",
                    Educacion = "Técnico en Informática",
                    Habilidades = "Comunicación efectiva"
                },
                new Candidato
                {
                    Codigo = 4,
                    DescripcionPersonal = "Analítico y detallista",
                    ExperienciaLaboral = "Analista de Sistemas en Delta SA",
                    Educacion = "Ingeniero en Informática",
                    Habilidades = "Resolución de problemas"
                }
            ];
    }
}