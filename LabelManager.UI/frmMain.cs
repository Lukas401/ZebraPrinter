using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using LabelManager.Application.Services;
using LabelManager.Domain.Entities;
using LabelManager.Infra.Excel;
using LabelManager.Infra.Zpl;
using Serilog;
using System.Drawing.Text;

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
            ConfigurarGrid();

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
    private void ConfigurarGrid()
    {
        var view = gridControl.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
        if (view == null) return;

        view.Columns.Clear();
        view.OptionsBehavior.Editable = false;
        view.OptionsView.ShowGroupPanel = false;

        view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn
        { FieldName = "Seq", Caption = "SEQ", VisibleIndex = 0, Width = 60 });
        view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn
        { FieldName = "Rep", Caption = "REP", VisibleIndex = 1, Width = 80 });
        view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn
        { FieldName = "Trat", Caption = "TRAT", VisibleIndex = 2, Width = 80 });
        view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn
        { FieldName = "Linha", Caption = "LINHA", VisibleIndex = 3, Width = 70 });
        view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn
        { FieldName = "IsValid", Caption = "Válido", VisibleIndex = 4, Width = 60 });
        view.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn
        { FieldName = "ErroValidacao", Caption = "Erro", VisibleIndex = 5, Width = 150 });

        view.RowCellStyle += (s, e) =>
        {
            if (view.GetRow(e.RowHandle) is LabelData label && !label.IsValid)
            {
                e.Appearance.BackColor = System.Drawing.Color.LightCoral;
                e.Appearance.ForeColor = System.Drawing.Color.DarkRed;
            }
        };

    }

    private async void btnGerarZpl_Click(object sender, EventArgs e)
    {
        var validas = _etiquetas.Where(l => l.IsValid).ToList();

        if (!validas.Any())
        {
            XtraMessageBox.Show("Nenhuma etiqueta encontrada", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var confirmacao = XtraMessageBox.Show($"Serão geradas e enviadas {validas.Count} etiqueta(s) para a impressora.\nDeseja continuar?",
        "Confirmar Impressão", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (confirmacao != DialogResult.Yes) return;
        btnGerarZpl.Enabled = false;
        btnSelecionar.Enabled = false;

        int enviadas = 0;
        int erros = 0;
        var zplGerado = new System.Text.StringBuilder();

        try
        {
            foreach (var label in validas)
            {
                try
                {
                    // Gera o ZPL individual
                    var zpl = _service.GerarZplUnico(label);
                    zplGerado.AppendLine(zpl);

                    // Envia imediatamente para a impressora
                    await _printService.EnviarZplAsync(zpl);

                    enviadas++;
                    lblStatus.Text = $"Enviando... {enviadas}/{validas.Count} — " +
                                     $"SEQ: {label.Seq} | TRAT: {label.Trat} | REP: {label.Rep}";

                    // Força atualização da UI a cada etiqueta
                    System.Windows.Forms.Application.DoEvents();

                    // pausa para não sobrecarregar a impressora
                    await Task.Delay(800);
                }
                catch (Exception ex)
                {
                    erros++;
                    Log.Error(ex, "Erro ao enviar etiqueta SEQ {Seq}", label.Seq);
                }
            }
            // Exibe o ZPL completo no preview
            txtZpl.Text = zplGerado.ToString();

            lblStatus.Text = $"✔ Concluído! {enviadas} etiqueta(s) enviada(s). {erros} erro(s).";

            Log.Information("Lote concluído: {Enviadas} enviadas, {Erros} erros", enviadas, erros);

            XtraMessageBox.Show(
                $"Impressão concluída!\n\n✔ Enviadas: {enviadas}\n✖ Erros: {erros}",
                "Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        finally
        {
            btnGerarZpl.Enabled = true;
            btnSelecionar.Enabled = true;
        }
    }

    private void gridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
    {
        if (sender is not GridView view) return;
        if (view.GetFocusedRow() is not LabelData label) return;

        txtZpl.Text = _service.GerarZplUnico(label);
        //btnImprimir.Enabled = true;
    }
}