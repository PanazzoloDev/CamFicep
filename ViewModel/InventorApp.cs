using System;
using Inventor;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using CamFicep.ViewModel;

namespace CamFicep.ViewModel
{
    public class InventorApp
    {
        private Inventor.Application oInvApp;
        private DrawingDocument dDoc;
        private DrawingView DB, DC, DA;
        private static List<Face> Abas = new List<Face>();
        public static bool Importado = false;
        public static List<Perfil> Perfis;

        public InventorApp()
        {
            try
            {
                oInvApp = Marshal.GetActiveObject("Inventor.Application") as Inventor.Application;
            }
            catch
            {

            }       
        } 
        public string Importar_Params(MainWindow frm, string caminho)
        {
            Inventor.Color cor = oInvApp.TransientObjects.CreateColor(0, 255, 255);
            dDoc = (DrawingDocument)oInvApp.Documents.Open(caminho, true);
            //dDoc = oInvApp.ActiveDocument as DrawingDocument;
            if (Classificar_Views(dDoc, out DB, out DC, out DA) == false)
            {
                return "Problemas com a importação.";
            }
            frm.pgb_processos.Value = 0;
            Face fcDB = new Face("DB", DB, dDoc, frm.CanvasDB, cor);
            if(fcDB.Altura == 0) return "Problemas com a view DB";
            Face fcDC = new Face("DC", DC, dDoc, frm.CanvasDC, cor);
            if (fcDC.Altura == 0) return "Problemas com a view DC";
            Face fcDA = new Face("DA", DA, dDoc, frm.CanvasDA, cor);
            if (fcDA.Altura == 0) return "Problemas com a view DA";
            frm.pgb_processos.Value = 100;

            frm.lbl_comp_total.Text = fcDB.Comprimento.ToString();
            frm.lbl_larg.Content = fcDB.Altura.ToString();
            frm.lbl_altura.Content = fcDC.Altura.ToString();
           
            Abas = new List <Face> { fcDB, fcDC, fcDA };

            Perfil Sug_Perfil = Perfis.Find(x => x.Larg_Alma.Contains(fcDC.Altura.ToString()) && x.Larg_AbaDireita.Contains(fcDB.Altura.ToString()));
            if (Sug_Perfil != null)
            {
                int i = frm.cb_perfis.Items.IndexOf(Sug_Perfil);
                frm.cb_perfis.SelectedIndex = i;

            }else
            {
                frm.lbl_status.Text = "Perfil não identificado, selecione-o.";
            }
            Importado = true;
            return "Importado com sucesso!";
        }
        public static List<Furacao> Atualizar_Lista()
        {
            List<Furacao> Selecoes = new List<Furacao>();
            List<Face> fcs = Abas;
            foreach (Face fc in Abas)
            {
                Selecoes.AddRange(fc.Furacoes);
            }
            return Selecoes;
        }
        private bool Classificar_Views(DrawingDocument dDoc, out DrawingView DB, out DrawingView DC, out DrawingView DA)
        {
            List<DrawingView> Views = new List<DrawingView>(); 
            for(int i = 1; i <= dDoc.ActiveSheet.DrawingViews.Count; i ++)
            {
                DrawingView view = dDoc.ActiveSheet.DrawingViews[i];
                if (view.Aligned == true && Views.Contains(view) == false || Views.Count(x => Math.Round(x.Position.X,2) == Math.Round(view.Position.X,2)) == 1 && Views.Contains(view) == false)
                { 
                    Views.Add(view);
                    i = 0;
                }
            }

            if(Views.Count != 3)
            {
                DB = null;
                DC = null;
                DA = null;
                return false;
            }
            try
            {
                var grupo = Views.GroupBy(x => Math.Round(x.Height, 3)).ToList();
                if (grupo[0].Count() > grupo[1].Count())
                {
                    DC = grupo[1].First();
                    Views.Remove(DC);
                }
                else
                {
                    DC = grupo[0].First();
                    Views.Remove(DC);
                }
                if (DC.Position.Y < Views[0].Position.Y)
                {
                    DB = Views[0];
                    DA = Views[1];
                }
                else
                {
                    DA = Views[0];
                    DB = Views[1];
                }
                return true;
            }
            catch
            {
                var grupo = Views.OrderBy(x => x.Position.Y).ToList();
                DB = grupo[2];
                DC = grupo[1];
                DA = grupo[0];

                return true;
            }

        }
        public static List<Perfil> ObterPerfis()
        {
            try
            {
                Perfis = new List<Perfil>();
                string[] linhas = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\Perfis.csv");
                for (int i = 1; i < linhas.Length; i++)
                {
                    string[] param = linhas[i].Split(';');
                    Perfis.Add(new Perfil(param[0], param[1], param[2], param[3], param[4], param[5], param[6]));
                }
                return Perfis;
            }
            catch
            {
                return Perfis;
            }
        }
        public void Resetar(MainWindow frm)
        {
            Importado = false;
            foreach (Face FC in Abas)
            {
                FC.Furacoes.Clear();            
            }
            Abas.Clear();
            //try { dDoc.Close(true); } catch { };
            oInvApp = null;
            frm.CanvasDA.Children.Clear();
            frm.CanvasDB.Children.Clear();
            frm.CanvasDC.Children.Clear();

            frm.txt_item.Text = string.Empty;
//            frm.txt_material.Text = string.Empty;
            frm.txt_ordem.Text = string.Empty;
            frm.txt_pedido.Text = string.Empty;
            frm.txt_qtde.Text = string.Empty;
            frm.txt_saida.Text = "AGUARDANDO PROCESSAMENTO.";
            frm.txt_saida.FontStyle = FontStyles.Italic;
            frm.txt_saida.FontWeight = FontWeights.Bold;
            frm.txt_saida.HorizontalContentAlignment = HorizontalAlignment.Center;
            frm.txt_saida.VerticalContentAlignment = VerticalAlignment.Center;
            frm.txt_saida.FontSize = 24;

            frm.lbl_altura.Content = "Altura";
            frm.lbl_comp_total.Text = "Comprimento Total";
            frm.lbl_larg.Content = "Largura";

            frm.dg_selecionados.ItemsSource = null;
        }
    }
}
