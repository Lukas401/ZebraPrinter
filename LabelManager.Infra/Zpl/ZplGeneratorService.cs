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
    private const string TemplateZpl =

    "^XA\n" +
    "^PW640\n" +
    "^LL1920\n" +
    "^CI28\n" +
    "^FO20,30^GB600,3,3^FS\n" +// Linha separadora topo
    "^FO20,60^A0N,70,70^FDSEQ: {NUMERO_ETIQUETA}^FS\n" +// SEQ — destaque grande no topo
    "^FO20,150^GB600,3,3^FS\n" +// Linha separadora
    "^FO20,170^A0N,55,55^FDTRAT: {TRAT}^FS\n" +// TRAT
    "^FO20,240^A0N,55,55^FDREP: {REP}^FS\n" +// REP
    "^FO20,310^A0N,55,55^FDLinha: {LINHA}^FS\n" +// LINHA
    "^FO20,390^GB600,3,3^FS\n" +// Linha separadora rodapé
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

