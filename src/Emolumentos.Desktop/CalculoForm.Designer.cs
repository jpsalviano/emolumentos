namespace Emolumentos.Desktop
{
    partial class CalculoForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblUf = new System.Windows.Forms.Label();
            this.cboUf = new System.Windows.Forms.ComboBox();
            this.lblAto = new System.Windows.Forms.Label();
            this.cboAto = new System.Windows.Forms.ComboBox();
            this.lblQuantidade = new System.Windows.Forms.Label();
            this.txtQuantidade = new System.Windows.Forms.TextBox();
            this.lblValor = new System.Windows.Forms.Label();
            this.txtValor = new System.Windows.Forms.TextBox();
            this.lblReducao = new System.Windows.Forms.Label();
            this.cboReducao = new System.Windows.Forms.ComboBox();
            this.btnCalcular = new System.Windows.Forms.Button();
            this.grpResultado = new System.Windows.Forms.GroupBox();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lvwParcelas = new System.Windows.Forms.ListView();
            this.colParcela = new System.Windows.Forms.ColumnHeader();
            this.colUnitario = new System.Windows.Forms.ColumnHeader();
            this.colValor = new System.Windows.Forms.ColumnHeader();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblFundamento = new System.Windows.Forms.Label();
            this.grpHistorico = new System.Windows.Forms.GroupBox();
            this.lvwHistorico = new System.Windows.Forms.ListView();
            this.colData = new System.Windows.Forms.ColumnHeader();
            this.colHistoricoUf = new System.Windows.Forms.ColumnHeader();
            this.colHistoricoAto = new System.Windows.Forms.ColumnHeader();
            this.colHistoricoQuantidade = new System.Windows.Forms.ColumnHeader();
            this.colHistoricoTotal = new System.Windows.Forms.ColumnHeader();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.grpResultado.SuspendLayout();
            this.grpHistorico.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // lblUf
            //
            this.lblUf.AutoSize = true;
            this.lblUf.Location = new System.Drawing.Point(16, 19);
            this.lblUf.Name = "lblUf";
            this.lblUf.Text = "UF:";
            //
            // cboUf
            //
            this.cboUf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboUf.Location = new System.Drawing.Point(130, 16);
            this.cboUf.Name = "cboUf";
            this.cboUf.Size = new System.Drawing.Size(180, 21);
            this.cboUf.TabIndex = 0;
            this.cboUf.SelectedIndexChanged += new System.EventHandler(this.AoAlterarSelecao);
            //
            // lblAto
            //
            this.lblAto.AutoSize = true;
            this.lblAto.Location = new System.Drawing.Point(16, 49);
            this.lblAto.Name = "lblAto";
            this.lblAto.Text = "Ato:";
            //
            // cboAto
            //
            this.cboAto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAto.Location = new System.Drawing.Point(130, 46);
            this.cboAto.Name = "cboAto";
            this.cboAto.Size = new System.Drawing.Size(280, 21);
            this.cboAto.TabIndex = 1;
            this.cboAto.SelectedIndexChanged += new System.EventHandler(this.AoAlterarSelecao);
            //
            // lblQuantidade
            //
            this.lblQuantidade.AutoSize = true;
            this.lblQuantidade.Location = new System.Drawing.Point(16, 79);
            this.lblQuantidade.Name = "lblQuantidade";
            this.lblQuantidade.Text = "Quantidade:";
            //
            // txtQuantidade
            //
            this.txtQuantidade.Location = new System.Drawing.Point(130, 76);
            this.txtQuantidade.Name = "txtQuantidade";
            this.txtQuantidade.Size = new System.Drawing.Size(80, 20);
            this.txtQuantidade.TabIndex = 2;
            this.txtQuantidade.Text = "1";
            //
            // lblValor
            //
            this.lblValor.AutoSize = true;
            this.lblValor.Location = new System.Drawing.Point(16, 109);
            this.lblValor.Name = "lblValor";
            this.lblValor.Text = "Valor declarado (R$):";
            //
            // txtValor
            //
            this.txtValor.Location = new System.Drawing.Point(130, 106);
            this.txtValor.Name = "txtValor";
            this.txtValor.Size = new System.Drawing.Size(140, 20);
            this.txtValor.TabIndex = 3;
            //
            // lblReducao
            //
            this.lblReducao.AutoSize = true;
            this.lblReducao.Location = new System.Drawing.Point(16, 139);
            this.lblReducao.Name = "lblReducao";
            this.lblReducao.Text = "Redução legal:";
            //
            // cboReducao
            //
            this.cboReducao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReducao.Location = new System.Drawing.Point(130, 136);
            this.cboReducao.Name = "cboReducao";
            this.cboReducao.Size = new System.Drawing.Size(400, 21);
            this.cboReducao.TabIndex = 4;
            //
            // btnCalcular
            //
            this.btnCalcular.Location = new System.Drawing.Point(560, 134);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(108, 25);
            this.btnCalcular.TabIndex = 5;
            this.btnCalcular.Text = "Calcular";
            this.btnCalcular.UseVisualStyleBackColor = true;
            this.btnCalcular.Click += new System.EventHandler(this.AoClicarCalcular);
            //
            // grpResultado
            //
            this.grpResultado.Controls.Add(this.lblTitulo);
            this.grpResultado.Controls.Add(this.lvwParcelas);
            this.grpResultado.Controls.Add(this.lblTotal);
            this.grpResultado.Controls.Add(this.lblFundamento);
            this.grpResultado.Location = new System.Drawing.Point(16, 172);
            this.grpResultado.Name = "grpResultado";
            this.grpResultado.Size = new System.Drawing.Size(652, 232);
            this.grpResultado.TabStop = false;
            this.grpResultado.Text = "Composição do valor";
            //
            // lblTitulo
            //
            this.lblTitulo.AutoEllipsis = true;
            this.lblTitulo.Location = new System.Drawing.Point(12, 22);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(628, 16);
            //
            // lvwParcelas
            //
            this.lvwParcelas.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colParcela,
                this.colUnitario,
                this.colValor});
            this.lvwParcelas.FullRowSelect = true;
            this.lvwParcelas.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvwParcelas.Location = new System.Drawing.Point(12, 44);
            this.lvwParcelas.Name = "lvwParcelas";
            this.lvwParcelas.Size = new System.Drawing.Size(628, 126);
            this.lvwParcelas.TabIndex = 6;
            this.lvwParcelas.View = System.Windows.Forms.View.Details;
            //
            // colParcela
            //
            this.colParcela.Text = "Parcela";
            this.colParcela.Width = 340;
            //
            // colUnitario
            //
            this.colUnitario.Text = "Por unidade";
            this.colUnitario.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colUnitario.Width = 130;
            //
            // colValor
            //
            this.colValor.Text = "Valor";
            this.colValor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colValor.Width = 130;
            //
            // lblTotal
            //
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(12, 178);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(628, 22);
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblFundamento
            //
            this.lblFundamento.AutoEllipsis = true;
            this.lblFundamento.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblFundamento.Location = new System.Drawing.Point(12, 206);
            this.lblFundamento.Name = "lblFundamento";
            this.lblFundamento.Size = new System.Drawing.Size(628, 16);
            //
            // grpHistorico
            //
            this.grpHistorico.Controls.Add(this.lvwHistorico);
            this.grpHistorico.Location = new System.Drawing.Point(16, 412);
            this.grpHistorico.Name = "grpHistorico";
            this.grpHistorico.Size = new System.Drawing.Size(652, 176);
            this.grpHistorico.TabStop = false;
            this.grpHistorico.Text = "Últimos cálculos";
            //
            // lvwHistorico
            //
            this.lvwHistorico.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colData,
                this.colHistoricoUf,
                this.colHistoricoAto,
                this.colHistoricoQuantidade,
                this.colHistoricoTotal});
            this.lvwHistorico.FullRowSelect = true;
            this.lvwHistorico.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvwHistorico.Location = new System.Drawing.Point(12, 22);
            this.lvwHistorico.Name = "lvwHistorico";
            this.lvwHistorico.Size = new System.Drawing.Size(628, 142);
            this.lvwHistorico.TabIndex = 7;
            this.lvwHistorico.View = System.Windows.Forms.View.Details;
            //
            // colData
            //
            this.colData.Text = "Data";
            this.colData.Width = 120;
            //
            // colHistoricoUf
            //
            this.colHistoricoUf.Text = "UF";
            this.colHistoricoUf.Width = 40;
            //
            // colHistoricoAto
            //
            this.colHistoricoAto.Text = "Ato";
            this.colHistoricoAto.Width = 250;
            //
            // colHistoricoQuantidade
            //
            this.colHistoricoQuantidade.Text = "Qtd.";
            this.colHistoricoQuantidade.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colHistoricoQuantidade.Width = 60;
            //
            // colHistoricoTotal
            //
            this.colHistoricoTotal.Text = "Total";
            this.colHistoricoTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colHistoricoTotal.Width = 130;
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 598);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(684, 22);
            this.statusStrip.SizingGrip = false;
            //
            // lblStatus
            //
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Spring = true;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // CalculoForm
            //
            this.AcceptButton = this.btnCalcular;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 620);
            this.Controls.Add(this.lblUf);
            this.Controls.Add(this.cboUf);
            this.Controls.Add(this.lblAto);
            this.Controls.Add(this.cboAto);
            this.Controls.Add(this.lblQuantidade);
            this.Controls.Add(this.txtQuantidade);
            this.Controls.Add(this.lblValor);
            this.Controls.Add(this.txtValor);
            this.Controls.Add(this.lblReducao);
            this.Controls.Add(this.cboReducao);
            this.Controls.Add(this.btnCalcular);
            this.Controls.Add(this.grpResultado);
            this.Controls.Add(this.grpHistorico);
            this.Controls.Add(this.statusStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CalculoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Emolumentos";
            this.grpResultado.ResumeLayout(false);
            this.grpHistorico.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblUf;
        private System.Windows.Forms.ComboBox cboUf;
        private System.Windows.Forms.Label lblAto;
        private System.Windows.Forms.ComboBox cboAto;
        private System.Windows.Forms.Label lblQuantidade;
        private System.Windows.Forms.TextBox txtQuantidade;
        private System.Windows.Forms.Label lblValor;
        private System.Windows.Forms.TextBox txtValor;
        private System.Windows.Forms.Label lblReducao;
        private System.Windows.Forms.ComboBox cboReducao;
        private System.Windows.Forms.Button btnCalcular;
        private System.Windows.Forms.GroupBox grpResultado;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.ListView lvwParcelas;
        private System.Windows.Forms.ColumnHeader colParcela;
        private System.Windows.Forms.ColumnHeader colUnitario;
        private System.Windows.Forms.ColumnHeader colValor;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblFundamento;
        private System.Windows.Forms.GroupBox grpHistorico;
        private System.Windows.Forms.ListView lvwHistorico;
        private System.Windows.Forms.ColumnHeader colData;
        private System.Windows.Forms.ColumnHeader colHistoricoUf;
        private System.Windows.Forms.ColumnHeader colHistoricoAto;
        private System.Windows.Forms.ColumnHeader colHistoricoQuantidade;
        private System.Windows.Forms.ColumnHeader colHistoricoTotal;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}
