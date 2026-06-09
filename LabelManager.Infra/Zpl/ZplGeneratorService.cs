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
    "^FO20,15^A0N,30,30^FDSEQ: {NUMERO_ETIQUETA}^FS\n" +
    "^FO20,55^A0N,25,25^FDTRAT: {TRAT}^FS\n" +
    "^FO20,85^A0N,25,25^FDREP: {REP}^FS\n" +
    "^FO20,115^A0N,25,25^FDLinha: {LINHA}^FS\n" +
    "^FO20,150^BY2^BCN,60,Y,N,N^FD{NUMERO_ETIQUETA}^FS\n" +
    "^XZ";

    public string GerarZpl(LabelData label)
    {
        return TemplateZpl
        .Replace("{NUMERO_ETIQUETA}", label.NumeroEtiqueta)
        .Replace("{TRAT}", label.Trat ?? string.Empty)
        .Replace("{REP}", label.Rep ?? string.Empty)
        .Replace("{LINHA}", label.Linha ?? string.Empty);

    }
    public string GerarZplLote(IEnumerable<LabelData> labels)
    {
        return string.Join(Environment.NewLine, labels.Select(GerarZpl));
    }

}

