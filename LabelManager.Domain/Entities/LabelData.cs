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
        public string NumeroEtiqueta { get; set; } = string.Empty;
        public string? Produto { get; set; }
        public string? Descricao { get; set; }
        public string? Lote {  get; set; }
        public string? DataValidade { get; set; }
        public string? CodigoBarras { get; set; }
        public bool IsValid { get; set; } = true;
        public string? ErroValidacao { get; set; }

    }
}
