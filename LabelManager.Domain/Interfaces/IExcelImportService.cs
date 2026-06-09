using LabelManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelManager.Domain.Interfaces;

    public interface IExcelImportService
    {
     IReadOnlyList<LabelData> Importar(string caminhoArquivo);
    }

