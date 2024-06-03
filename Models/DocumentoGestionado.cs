namespace LuceneNetApi.Models
{
    public class DocumentoGestionado
    {
        ///<summary>
        /// Codigo del documento
        ///</summary>
        public int Codigo { get; set; } // doc_codigo (Primary key)

        ///<summary>
        /// Codigo del tipo de documento
        ///</summary>
        public int TipoDocumentoCodigo { get; set; } // doc_codtdo

        ///<summary>
        /// Codigo del Area del documento
        ///</summary>
        public int AreaCodigo { get; set; } // doc_codare

        ///<summary>
        /// Titulo del documento
        ///</summary>
        public string Titulo { get; set; } // doc_titulo (length: 1000)

        ///<summary>
        /// Descripcion del documento
        ///</summary>
        public string Descripcion { get; set; } // doc_descripcion (length: 4000)

        ///<summary>
        /// Lista de palabras clave separadas por coma (keywords)
        ///</summary>
        public string PalabrasClave { get; set; } // doc_palabras_claves (length: 1000)

        ///<summary>
        /// Indica cómo se interpreta el detalle de permisos asociados al documento
        ///</summary>
        public string ModoAsociacionPermisosDb { get; set; } = "TodosExcluyendo"; // doc_asoc_permisos (length: 20)
    }
}
