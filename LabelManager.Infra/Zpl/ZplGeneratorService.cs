using LabelManager.Domain.Interfaces;
using LabelManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenLabel.Templating;

namespace LabelManager.Infra.Zpl;

public class ZplGeneratorService : IZplGeneratorService
{
    // Template ZPL base da etiqueta (100mm x 50mm, 203 DPI)
    // ^XA = início  |  ^XZ = fim
    // ^FO = posição (x,y)  |  ^FD = dado do campo  |  ^FS = fim do campo
    // ^BY = configuração do barcode  |  ^BC = Code128
    private const string TemplateZpl =
    "^XA\n" +
    "^FO20,15^A0N,28,28^FD{NUMERO_ETIQUETA}^FS\n" +
    "^FO20,50^BY2^BCN,60,Y,N,N^FD{CODIGO_BARRAS}^FS\n" +
    "^FO20,130^A0N,20,20^FDProduto: {PRODUTO}^FS\n" +
    "^FO20,155^A0N,20,20^FDLote: {LOTE}^FS\n" +
    "^FO20,180^A0N,20,20^FDValidade: {DATA_VALIDADE}^FS\n" +
    "^XZ";

    public string GerarZpl(LabelData label)
    {
        // Formata a data removendo hora desnecessária
        var dataFormatada = string.Empty;
        if (!string.IsNullOrEmpty(label.DataValidade) &&
            DateTime.TryParse(label.DataValidade, out var data))
            dataFormatada = data.ToString("dd/MM/yyyy");
        else
            dataFormatada = label.DataValidade ?? string.Empty;

        // Substituição manual — mais confiável que o TemplateHandler do OpenLabel
        return TemplateZpl
            .Replace("{NUMERO_ETIQUETA}", label.NumeroEtiqueta)
            .Replace("{CODIGO_BARRAS}", label.CodigoBarras ?? label.NumeroEtiqueta)
            .Replace("{PRODUTO}", label.Produto ?? string.Empty)
            .Replace("{LOTE}", label.Lote ?? string.Empty)
            .Replace("{DATA_VALIDADE}", dataFormatada);
    }
    public string GerarZplLote(IEnumerable<LabelData> labels)
    {
        return string.Join(Environment.NewLine, labels.Select(GerarZpl));
    }

}

