using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Emolumentos.Apresentacao;
using Emolumentos.Dominio;

namespace Emolumentos.Desktop
{
    /// <summary>View passiva da tela de cálculo: nenhuma regra aqui, só controles.</summary>
    public partial class CalculoForm : Form, ICalculoView
    {
        // Preencher um combo dispara o evento de seleção; enquanto a tela é montada, ele não é do usuário.
        private bool _preenchendo;

        public CalculoForm()
        {
            InitializeComponent();
        }

        public event EventHandler SelecaoAlterada;

        public event EventHandler CalcularSolicitado;

        public Uf UfSelecionada
        {
            get { return Selecionado<Uf>(cboUf); }
        }

        public TipoAto AtoSelecionado
        {
            get { return Selecionado<TipoAto>(cboAto); }
        }

        public ReducaoLegal ReducaoSelecionada
        {
            get { return Selecionado<ReducaoLegal>(cboReducao); }
        }

        public string QuantidadeDigitada
        {
            get { return txtQuantidade.Text.Trim(); }
        }

        public string ValorDeclaradoDigitado
        {
            get { return txtValor.Text.Trim(); }
        }

        public void ExibirUfs(IReadOnlyList<Uf> ufs)
        {
            Preencher(cboUf, ufs, Rotulos.De);
        }

        public void ExibirAtos(IReadOnlyList<TipoAto> atos)
        {
            Preencher(cboAto, atos, Rotulos.De);
        }

        public void ExibirReducoes(IReadOnlyList<ReducaoLegal> reducoes)
        {
            Preencher(cboReducao, reducoes, Rotulos.De);
            cboReducao.Enabled = reducoes.Count > 1;
        }

        public void HabilitarValorDeclarado(bool habilitado)
        {
            txtValor.Enabled = habilitado;
            if (!habilitado)
            {
                txtValor.Clear();
            }
        }

        public void ExibirResultado(ResultadoExibido resultado)
        {
            lblTitulo.Text = resultado.Titulo;
            lblFundamento.Text = resultado.Fundamento;
            lblTotal.Text = "Total: " + resultado.Total;

            lvwParcelas.BeginUpdate();
            lvwParcelas.Items.Clear();
            foreach (LinhaParcela parcela in resultado.Parcelas)
            {
                lvwParcelas.Items.Add(new ListViewItem(new[] { parcela.Nome, parcela.ValorUnitario, parcela.Valor }));
            }

            lvwParcelas.EndUpdate();
            lblStatus.Text = string.Empty;
        }

        public void ExibirErro(string mensagem)
        {
            MessageBox.Show(this, mensagem, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void ExibirHistorico(IReadOnlyList<LinhaHistorico> linhas)
        {
            lvwHistorico.BeginUpdate();
            lvwHistorico.Items.Clear();
            foreach (LinhaHistorico linha in linhas)
            {
                lvwHistorico.Items.Add(new ListViewItem(new[]
                {
                    linha.Data, linha.Uf, linha.Ato, linha.Quantidade, linha.Total
                }));
            }

            lvwHistorico.EndUpdate();
        }

        public void ExibirAviso(string mensagem)
        {
            lblStatus.Text = mensagem;
        }

        private void AoAlterarSelecao(object sender, EventArgs e)
        {
            if (!_preenchendo)
            {
                SelecaoAlterada?.Invoke(this, EventArgs.Empty);
            }
        }

        private void AoClicarCalcular(object sender, EventArgs e)
        {
            CalcularSolicitado?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Troca os itens do combo, mantendo a escolha atual quando ela continua disponível.</summary>
        private void Preencher<T>(ComboBox combo, IReadOnlyList<T> valores, Func<T, string> rotulo)
        {
            _preenchendo = true;
            try
            {
                object anterior = combo.SelectedItem;
                combo.Items.Clear();
                combo.Items.AddRange(valores.Select(v => (object)new Opcao<T>(v, rotulo(v))).ToArray());
                combo.SelectedItem = anterior;
                if (combo.SelectedIndex < 0 && combo.Items.Count > 0)
                {
                    combo.SelectedIndex = 0;
                }
            }
            finally
            {
                _preenchendo = false;
            }
        }

        private static T Selecionado<T>(ComboBox combo)
        {
            var opcao = combo.SelectedItem as Opcao<T>;
            return opcao == null ? default(T) : opcao.Valor;
        }

        /// <summary>Item de combo: guarda o valor do domínio e mostra o rótulo.</summary>
        private sealed class Opcao<T>
        {
            private readonly string _rotulo;

            public Opcao(T valor, string rotulo)
            {
                Valor = valor;
                _rotulo = rotulo;
            }

            public T Valor { get; }

            public override string ToString()
            {
                return _rotulo;
            }

            public override bool Equals(object obj)
            {
                var outra = obj as Opcao<T>;
                return outra != null && EqualityComparer<T>.Default.Equals(Valor, outra.Valor);
            }

            public override int GetHashCode()
            {
                return EqualityComparer<T>.Default.GetHashCode(Valor);
            }
        }
    }
}
