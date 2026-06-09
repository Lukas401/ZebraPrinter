using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelManager.Domain.Entities
{
    public class LabelData
    {
        public int Row { get; set; }

        //campos da planilha
        public string NumeroEtiqueta { get; set; } = string.Empty;
        public string? Seq { get; set; }
        public string? Rep { get; set; }
        public string? Trat {  get; set; }
        public string? Linha { get; set; }
        public string? CodigoBarras { get; set; }
        

        //validação
        public bool IsValid { get; set; } = true;
        public string? ErroValidacao { get; set; }

    }
}
