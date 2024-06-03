namespace LuceneNetApi.Models
{
    public class SearchDto
    {
        public string Query { get; set; }
        public int DocumentLimit { get; set; } = 10;
        public int? AreaCodigo { get; set; }
        public int? TipoDocumentoCodigo { get; set; }
    }
}
