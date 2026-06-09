using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace LabelManager.UI;

partial class frmMain
{
    private System.ComponentModel.IContainer components = null;

    // Controles
    private PanelControl pnlTopo;
    private PanelControl pnlCentro;
    private PanelControl pnlRodape;
    private SplitContainerControl splitMain;

    private SimpleButton btnSelecionar;
    private SimpleButton btnGerarZpl;
    private SimpleButton btnImprimir;
    private TextEdit txtArquivo;
    private LabelControl lblArquivo;
    private LabelControl lblStatus;

    private GridControl gridControl;
    private GridView gridView;

    private MemoEdit txtZpl;
    private LabelControl lblZpl;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        // ── Instâncias ──────────────────────────────────────────────────
        pnlTopo = new PanelControl();
        pnlCentro = new PanelControl();
        pnlRodape = new PanelControl();
        splitMain = new SplitContainerControl();

        btnSelecionar = new SimpleButton();
        btnGerarZpl = new SimpleButton();
        btnImprimir = new SimpleButton();
        txtArquivo = new TextEdit();
        lblArquivo = new LabelControl();
        lblStatus = new LabelControl();

        gridControl = new GridControl();
        gridView = new GridView();
        txtZpl = new MemoEdit();
        lblZpl = new LabelControl();

        // ── Form ────────────────────────────────────────────────────────
        Text = "LabelManager — Gerador de Etiquetas Zebra";
        Size = new Size(1200, 720);
        MinimumSize = new Size(900, 600);
        StartPosition = FormStartPosition.CenterScreen;

        // ── Painel Topo ─────────────────────────────────────────────────
        pnlTopo.Dock = DockStyle.Top;
        pnlTopo.Height = 60;
        pnlTopo.Padding = new Padding(8);

        lblArquivo.Text = "Arquivo:";
        lblArquivo.Location = new Point(10, 20);

        txtArquivo.Location = new Point(65, 16);
        txtArquivo.Size = new Size(550, 28);
        txtArquivo.Properties.ReadOnly = true;

        btnSelecionar.Text = "Selecionar Excel";
        btnSelecionar.Location = new Point(625, 15);
        btnSelecionar.Size = new Size(130, 30);
        btnSelecionar.Click += btnSelecionar_Click;

        btnGerarZpl.Text = "Gerar ZPL";
        btnGerarZpl.Location = new Point(765, 15);
        btnGerarZpl.Size = new Size(110, 30);
        btnGerarZpl.Enabled = false;
        btnGerarZpl.Click += btnGerarZpl_Click;

        btnImprimir.Text = "Enviar p/ Impressora";
        btnImprimir.Location = new Point(885, 15);
        btnImprimir.Size = new Size(150, 30);
        btnImprimir.Enabled = false;
        

        pnlTopo.Controls.AddRange([lblArquivo, txtArquivo,
            btnSelecionar, btnGerarZpl, btnImprimir]);

        // ── SplitContainer (Grid | ZPL) ─────────────────────────────────
        splitMain.Dock = DockStyle.Fill;
        splitMain.SplitterPosition = 650;

        // Grid
        gridControl.Dock = DockStyle.Fill;
        gridControl.MainView = gridView;
        gridView.OptionsBehavior.Editable = false;
        gridView.OptionsView.ShowGroupPanel = false;
        gridView.FocusedRowChanged += gridView_FocusedRowChanged;

        // Painel ZPL
        lblZpl.Text = "Preview ZPL:";
        lblZpl.Dock = DockStyle.Top;
        lblZpl.Height = 22;

        txtZpl.Dock = DockStyle.Fill;
        txtZpl.Properties.ReadOnly = false;
        txtZpl.Font = new Font("Consolas", 9f);

        splitMain.Panel1.Controls.Add(gridControl);
        splitMain.Panel2.Controls.AddRange([txtZpl, lblZpl]);

        // ── Painel Centro ───────────────────────────────────────────────
        pnlCentro.Dock = DockStyle.Fill;
        pnlCentro.Controls.Add(splitMain);

        // ── Painel Rodapé ───────────────────────────────────────────────
        pnlRodape.Dock = DockStyle.Bottom;
        pnlRodape.Height = 30;

        lblStatus.Text = "Pronto. Selecione uma planilha Excel para começar.";
        lblStatus.Location = new Point(10, 8);
        lblStatus.AutoSizeMode = LabelAutoSizeMode.None;
        lblStatus.Size = new Size(900, 18);

        pnlRodape.Controls.Add(lblStatus);

        // ── Montar Form ─────────────────────────────────────────────────
        Controls.AddRange([pnlTopo, pnlCentro, pnlRodape]);
    }
}