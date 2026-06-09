using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using LabelManager.Application.Services;
using LabelManager.Domain.Entities;
using LabelManager.Infra.Excel;
using LabelManager.Infra.Zpl;
using LabelManager.Infra.Excel;
using LabelManager.Infra.Zpl;
using Serilog;

namespace LabelManager.UI;

public partial class frmMain : Form
{
    private readonly LabelGenerationService _service;
    private readonly PrintService _printService;
    private IReadOnlyList<LabelData> _etiquetas = [];

    public frmMain()
    {
        InitializeComponent();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("logs/labelmanager.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        _service = new LabelGenerationService(
            new ExcelImportService(),
            new ZplGeneratorService());

        _printService = new PrintService();
    }

    private void btnSelecionar_Click(object sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog
        {
            Title = "Selecionar planilha de etiquetas",
            Filter = "Excel|*.xlsx;*.xls"
        };

        if (dlg.ShowDialog() != DialogResult.OK) return;

        txtArquivo.Text = dlg.FileName;
        ImportarPlanilha(dlg.FileName);
    }

    private void ImportarPlanilha(string caminho)
    {
        try
        {
            Cursor = Cursors.WaitCursor;
            _etiquetas = _service.ImportarPlanilha(caminho);

            gridControl.DataSource = _etiquetas.ToList();

            lblStatus.Text = $"{_etiquetas.Count} etiqueta(s) importada(s). " +
                             $"{_etiquetas.Count(e => !e.IsValid)} com erro(s).";

            btnGerarZpl.Enabled = _etiquetas.Any(e => e.IsValid);

            Log.Information("Planilha importada: {Arquivo} — {Total} etiquetas",
                caminho, _etiquetas.Count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao importar planilha");
            XtraMessageBox.Show($"Erro ao importar:\n{ex.Message}",
                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void btnGerarZpl_Click(object sender, EventArgs e)
    {
        try
        {
            var zpl = _service.GerarZplLote(_etiquetas);
            txtZpl.Text = zpl;
            btnImprimir.Enabled = true;
            lblStatus.Text = "ZPL gerado! Clique em 'Enviar p/ Impressora' para visualizar.";

            Log.Information("ZPL gerado para {Total} etiquetas",
                _etiquetas.Count(x => x.IsValid));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao gerar ZPL");
            XtraMessageBox.Show($"Erro ao gerar ZPL:\n{ex.Message}",
                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnImprimir_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtZpl.Text)) return;

        try
        {
            btnImprimir.Enabled = false;
            lblStatus.Text = "Enviando para impressora virtual...";

            await _printService.EnviarZplAsync(txtZpl.Text);

            lblStatus.Text = "✔ ZPL enviado! Verifique o Virtual-ZPL-Printer.";
            Log.Information("ZPL enviado para impressora virtual");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Erro ao enviar ZPL");
            XtraMessageBox.Show(
                $"Não foi possível conectar ao Virtual-ZPL-Printer.\n" +
                $"Verifique se o programa está aberto na porta 9100.\n\n{ex.Message}",
                "Erro de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            btnImprimir.Enabled = true;
        }
    }

    private void gridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
    {
        if (sender is not GridView view) return;
        if (view.GetFocusedRow() is not LabelData label) return;

        txtZpl.Text = _service.GerarZplUnico(label);
        btnImprimir.Enabled = true;
    }
}