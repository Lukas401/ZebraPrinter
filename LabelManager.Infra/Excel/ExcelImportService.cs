using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabelManager.Domain.Interfaces;
using LabelManager.Domain.Entities;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;

namespace LabelManager.Infra.Excel;

public class ExcelImportService : IExcelImportService
{
    private const string ColNumeroEtiqueta = "seq";
    private const string ColRep = "rep";
    private const string ColTrat= "trat";
    private const string ColLinha = "linha";
    public IReadOnlyList<LabelData> Importar(string caminhoArquivo)
    {
        if (!File.Exists(caminhoArquivo)) throw new FileNotFoundException("Arquivo Excel Não encontrado.", caminhoArquivo);

        using var workbook = new XLWorkbook(caminhoArquivo);
        var planilha = workbook.Worksheets.First();
        var colunas = MapearColunas (planilha); //mapeia o nome da coluna index

        if (!colunas.ContainsKey(ColNumeroEtiqueta))
            throw new InvalidOperationException(
                "Coluna 'Seq' não encontrada na planilha");

        var resultado = new List<LabelData>();
        var ultimaLinha = planilha.LastRowUsed()?.RowNumber() ?? 1;

        for (int row = 2; row <= ultimaLinha; row++) //inicia em 2 pois 1 é cabeçalho
        {
            var seq = LerCelula(planilha, row, colunas, ColNumeroEtiqueta);
            if (string.IsNullOrWhiteSpace(seq)) continue; //ignora linha vazia e continua

            var label = new LabelData
            {
                Row = row,
                NumeroEtiqueta = seq,
                Seq = seq,
                Rep = LerCelula(planilha, row, colunas, ColRep),
                Trat = LerCelula(planilha, row, colunas, ColTrat),
                Linha = LerCelula(planilha, row, colunas, ColLinha),
                
            };

            Validar(label);
            resultado.Add(label);
        }   
        return resultado.AsReadOnly();
    }

    private static Dictionary<string, int> MapearColunas(IXLWorksheet ws)
    {
        var mapa = new Dictionary<string, int>();
        var headerRow = ws.FirstRowUsed();
        if (headerRow is null) return mapa;

        foreach (var cell in headerRow.CellsUsed())
        {
            var nome = cell.GetString().Trim().ToLower().Replace(" ", "").Replace("ã", "a").Replace("ç", "c");
            mapa[nome] = cell.Address.ColumnNumber;
        }
        return mapa;
    }
    private static string? LerCelula(IXLWorksheet ws, int row,
        Dictionary<string, int> colunas, string chave)
    {
        if (!colunas.TryGetValue(chave, out int col)) return null;
        var valor = ws.Cell(row, col).GetString().Trim();
        return string.IsNullOrEmpty(valor) ? null : valor;
    }

    private static void Validar(LabelData label)
    {
        if (string.IsNullOrWhiteSpace(label.NumeroEtiqueta))
        {
            label.IsValid = false;
            label.ErroValidacao = "Seq está vazio.";
        }
    }

}

   

