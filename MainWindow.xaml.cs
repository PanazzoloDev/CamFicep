using CamFicep.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CamFicep
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string Origem;
        bool Definindo_Origem;
        InventorApp Inventor;
        Brush Cor;

        public MainWindow()
        {
            InitializeComponent();
        }
        public void btn_processar_Click(object sender, RoutedEventArgs e)
        {
            string msg = Verif_Entradas_Obrigatorias();
            if (msg.Contains("ok") != true)
            {
                MessageBox.Show(msg, "Parâmetro não informado.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(txt_ordem.Text))
            {
                if(MessageBox.Show("O parâmetro 'Ordem' não foi informado. \n Deseja continuar?","Parâmetro não informado.",MessageBoxButton.YesNo,MessageBoxImage.Information) == MessageBoxResult.No)
                {
                    return;
                }
            }
            if (string.IsNullOrEmpty(txt_pedido.Text))
            {
                if (MessageBox.Show("O parâmetro 'Pedido' não foi informado. \n Deseja continuar?", "Parâmetro não informado.", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No)
                {
                    return;
                }
            }
            Parametros par = new Parametros(txt_item.Text,txt_material.Text,txt_programador.Text, txt_qtde.Text,double.Parse(lbl_comp_total.Text), txt_ordem.Text, txt_pedido.Text);
            Task.Delay(1500);
            txt_saida.FontSize = 14;
            txt_saida.HorizontalContentAlignment = HorizontalAlignment.Left;
            txt_saida.FontWeight = FontWeights.Normal;
            txt_saida.Foreground = Brushes.White;
            txt_saida.FontStyle = FontStyles.Normal;
            txt_saida.Text = Processador.Processar(dg_selecionados, (Perfil)cb_perfis.SelectedItem, par, txt_destino.Text);
            MessageBox.Show("Pós-Processado com sucesso.", "Concluído.", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void dg_selecionados_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                Furacao antigo = (Furacao)e.RemovedCells[0].Item;
                antigo.Rep_Design.Stroke = Brushes.Black;

                Furacao novo = (Furacao)e.AddedCells[0].Item;
                novo.Rep_Design.Stroke = Brushes.Yellow;
            }
            catch
            {

            }
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.Source.GetType() == typeof(Ellipse) && Definindo_Origem == false)
            {
                Ellipse rep = (Ellipse)e.Source;
                int i = dg_selecionados.Items.IndexOf(rep.Tag);
                dg_selecionados.SelectedIndex = i;
            }else if(e.Source.GetType() == typeof(Ellipse) && Definindo_Origem == true)
            {
                Ellipse rep = (Ellipse)e.Source;
                Furacao furo = (Furacao)rep.Tag;

                furo.Alterar_Origem(furo, Origem);
                furo.Rep_Design.Fill = Cor;

                dg_selecionados.ItemsSource = null;
                dg_selecionados.ItemsSource = InventorApp.Atualizar_Lista();

                int i = dg_selecionados.Items.IndexOf(furo);
                dg_selecionados.SelectedIndex = i;
            }
        }
        private void btn_open_Click(object sender, RoutedEventArgs e)
        {
            string caminho = "";
            bool encontrado = false;
            if (string.IsNullOrEmpty(txt_item.Text) == false && string.IsNullOrWhiteSpace(txt_item.Text) == false)
            {
                lbl_status.Text = "Procurando...";
                if (System.IO.File.Exists(Properties.Settings.Default.txtPath))
                {
                    string[] txt = System.IO.File.ReadAllLines(Properties.Settings.Default.txtPath);
                    foreach (string linha in txt)
                    {
                        if (linha.Contains(@"\" + txt_item.Text + ".idw"))
                        {
                            if (System.IO.File.Exists(linha)) caminho = linha;
                            encontrado = true;
                            break;
                        }
                    }
                }
            } 

            if (!encontrado)
            {
                Microsoft.Win32.OpenFileDialog op = new Microsoft.Win32.OpenFileDialog()
                {
                    FileName = string.Empty,
                    Multiselect = false,
                    Filter = "Drawing Files (*.idw)|*.idw"
                };
                op.ShowDialog();
                if (System.IO.File.Exists(op.FileName))
                {
                    caminho = op.FileName;
                }
                else
                {
                    lbl_status.Text = "Arquivo não selecionado/inválido.";
                    return;
                }           
            }
            lbl_status.Text = "Abrindo...";
            //Inventor.Resetar(this);
            Inventor = new InventorApp();
            string msg = (Inventor.Importar_Params(this, caminho));
            dg_selecionados.ItemsSource = InventorApp.Atualizar_Lista();
            MessageBox.Show(msg);
        }
        private void btn_se_Click(object sender, RoutedEventArgs e)
        {
            Origem = "SE";
            Definindo_Origem = true;
            Cor = Brushes.LimeGreen;
            lbl_status.Text = "Selecione as furações ou pressione ESC.";
        }
        private void btn_sd_Click(object sender, RoutedEventArgs e)
        {
            Origem = "SD";
            Definindo_Origem = true;
            Cor = Brushes.Red;
            lbl_status.Text = "Selecione as furações ou pressione ESC.";
        }
        private void btn_me_Click(object sender, RoutedEventArgs e)
        {
            Origem = "ME";
            Definindo_Origem = true;
            Cor = Brushes.Cyan;
            lbl_status.Text = "Selecione as furações ou pressione ESC.";
        }
        private void btn_md_Click(object sender, RoutedEventArgs e)
        {
            Origem = "MD";
            Definindo_Origem = true;
            Cor = Brushes.BlueViolet;
            lbl_status.Text = "Selecione as furações ou pressione ESC.";
        }
        private void btn_ie_Click(object sender, RoutedEventArgs e)
        {
            Origem = "IE";
            Definindo_Origem = true;
            Cor = Brushes.Orange;
            lbl_status.Text = "Selecione as furações ou pressione ESC.";
        }
        private void btn_id_Click(object sender, RoutedEventArgs e)
        {
            Origem = "ID";
            Definindo_Origem = true;
            Cor = Brushes.Blue;
            lbl_status.Text = "Selecione as furações ou pressione ESC.";
        }
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Escape && Definindo_Origem == true)
            {
                Origem = string.Empty;
                Definindo_Origem = false;
                Cor = Brushes.White;
                lbl_status.Text = "Concluído";
            }
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Perfil> perf = InventorApp.ObterPerfis();
            if (perf.Count == 0)
            {
                lbl_status.Text = "'Perfis.csv' não encontrado.";
                return;
            }
            txt_programador.Text = Environment.MachineName;
            cb_perfis.ItemsSource = perf;

            txt_caminhos.Text = Properties.Settings.Default.txtPath;
        }
        private string Verif_Entradas_Obrigatorias()
        {
            if (InventorApp.Importado == false) return "É necessário informar e abrir um documento.";
            if (string.IsNullOrEmpty(txt_item.Text)) return "O parâmetro 'Ident. Item' é obrigatório.";
            if (string.IsNullOrEmpty(txt_qtde.Text)) return "O parâmetro 'Quantidade' é obrigatório.";
            if (string.IsNullOrEmpty(txt_programador.Text)) return "O parâmetro 'Programador' é obrigatório.";
            return "ok";
        }
        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            Inventor.Resetar(this);

        }
        private void txt_ordem_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = oOracle.Pesquisar_Ordem(txt_ordem.Text);
                txt_item.Text = dt.Rows[0][1].ToString();
                txt_qtde.Text = string.Format("{0:0.00}",double.Parse(dt.Rows[0][2].ToString()));
                txt_pedido.Text = dt.Rows[0][3].ToString();

            }
            catch
            {

            }
        }
        private void btn_SelectFilePath_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                folderDialog.Description = "Selecione a pasta de destino dos arquivos";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    txt_destino.Text = selectedPath;
                }
            }
        }
        private void btn_SelectCaminhosTxt_Click(object sender, RoutedEventArgs e)
        {
            using (var fileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                fileDialog.Filter = "Text Files (*.txt)|*.txt";
                fileDialog.Title = "Selecione o arquivo dos caminhos .txt";
                fileDialog.Multiselect = false;

                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedPath = fileDialog.FileName;
                    txt_caminhos.Text = selectedPath;
                    Properties.Settings.Default.txtPath = selectedPath;
                    Properties.Settings.Default.Save();
                }
            }

        }
    }
}
