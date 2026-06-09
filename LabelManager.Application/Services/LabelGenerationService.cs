using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabelManager.Domain.Entities;
using LabelManager.Domain.Interfaces;

namespace LabelManager.Application.Services;

    public class LabelGenerationService
    {
    private readonly IExcelImportService _excelService;
    private readonly IZplGeneratorService _zplService;

    public LabelGenerationService(
        IExcelImportService excelService,
        IZplGeneratorService zplService)
    {
        _excelService = excelService;
        _zplService = zplService;
    }

    public IReadOnlyList <LabelData> ImportarPlanilha(string caminhoArquivo) => _excelService.Importar(caminhoArquivo); //importa o excel e retorna as etiquetas para o front

    public string GerarZplLote(IEnumerable<LabelData> labels) //receb a lista e retorna o ZPL do lote 
    {
        var validas = labels.Where(l => l.IsValid).ToList();

        if (!validas.Any())
            throw new InvalidOperationException("Nenhuma etiqueta válida para gerar ZPL");
        return _zplService.GerarZplLote(validas);
    }

    public string GerarZplUnico (LabelData label) => _zplService.GerarZpl(label); //Gera zpl de uma única etiqueta

    }

