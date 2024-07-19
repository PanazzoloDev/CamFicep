using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CamFicep.ViewModel
{
    public class Furacao
    {
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public double X_Original = 0;
        public double Y_Original = 0;
        public double Diam { get; set; } = 0;
        public string Aba { get; set; }
        public string Origem { get; set; }
        public Face Parent;

        public Ellipse Rep_Design;
        public DrawingCurve FuroView;

        public Furacao(Face aba, DrawingCurve linha, string nome_aba,Canvas Design, string origem = "N/A")
        {
            this.Aba = nome_aba;
            this.Origem = origem;
            this.Parent = aba;
            this.FuroView = linha;

            GeometryIntent limx = Parent.Limites[0];
            GeometryIntent limy = Parent.Limites[2];

            double[] parametros = Obter_Parametros(linha, limx, limy);
            if (parametros != null)
            {
                this.X = parametros[1];
                this.X_Original = parametros[1];
                this.Y = parametros[0];
                this.Y_Original = parametros[0];
                this.Diam = parametros[2];
            }
            double esc_v = Design.ActualHeight / aba.Altura;
            double esc_h = Design.ActualWidth / aba.Comprimento;
            double esc = (esc_v + esc_h) / 2;

            this.Rep_Design = new Ellipse()
            {
                Fill = Brushes.Purple,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Width = this.Diam * esc,
                Height = this.Diam * esc,
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = this
            };
            Design.Children.Add(Rep_Design);
            System.Windows.Controls.Canvas.SetLeft(Rep_Design, (this.X * esc_h) - (this.Diam * esc / 2));
            System.Windows.Controls.Canvas.SetTop(Rep_Design, (this.Y * esc_v) - (this.Diam * esc / 2));
        }
        public bool Alterar_Origem(Furacao fr,string origem = "ME")
        {
            double[] parametros;
            switch (origem)
            {
                case "SE":
                    parametros = Obter_Parametros(fr.FuroView, fr.Parent.Limites[2], fr.Parent.Limites[0]);
                    if (parametros == null) return false;
                    this.X = parametros[0];
                    this.Y = parametros[1];
                    this.Origem = "SE";
                    return true;

                case "SD":
                    parametros = Obter_Parametros(fr.FuroView, fr.Parent.Limites[3], fr.Parent.Limites[0]);
                    if (parametros == null) return false;
                    this.X = parametros[0];
                    this.Y = parametros[1];
                    this.Origem = "SD";
                    return true;

                case "ME":
                    parametros = Obter_Parametros(fr.FuroView, fr.Parent.Limites[2], fr.Parent.LinhaCentro);
                    if (parametros == null) return false;
                    this.X = parametros[0];
                    this.Y = parametros[1];
                    if(this.Y_Original >= (fr.Parent.Altura / 2))
                    {
                        this.Origem = "ME-"; 
                    }
                    else
                    {
                        this.Origem = "ME+";
                    }
                    return true;
                case "MD":
                    parametros = Obter_Parametros(fr.FuroView, fr.Parent.Limites[3], fr.Parent.LinhaCentro);
                    if (parametros == null) return false;
                    this.X = parametros[0];
                    this.Y = parametros[1];
                    if (this.Y_Original >= (fr.Parent.Altura / 2))
                    {
                        this.Origem = "MD-";
                    }
                    else
                    {
                        this.Origem = "MD+";
                    }
                    return true;

                case "IE":
                    parametros = Obter_Parametros(fr.FuroView, fr.Parent.Limites[2], fr.Parent.Limites[1]);
                    if (parametros == null) return false;
                    this.X = parametros[0];
                    this.Y = parametros[1];
                    this.Origem = "IE";
                    return true;

                case "ID":
                    parametros = Obter_Parametros(fr.FuroView, fr.Parent.Limites[3], fr.Parent.Limites[1]);
                    if (parametros == null) return false;
                    this.X = parametros[0];
                    this.Y = parametros[1];
                    this.Origem = "ID";
                    return true;

                default:
                    return false;
            }
        }
        private double[] Obter_Parametros(DrawingCurve linha, GeometryIntent Limite_X, GeometryIntent Limite_Y)
        {
           
            string strx, stry, diam;
            LinearGeneralDimension dim;
            DiameterGeneralDimension dimg;
            GeometryIntent furo = Parent.Parent.ActiveSheet.CreateGeometryIntent(linha);
            try
            {
                dim = Parent.Parent.ActiveSheet.DrawingDimensions.GeneralDimensions.AddLinear(linha.CenterPoint, furo, Limite_X);
                strx = dim.Text.Text; dim.Delete();

                dim = Parent.Parent.ActiveSheet.DrawingDimensions.GeneralDimensions.AddLinear(linha.CenterPoint, Limite_Y, furo);
                stry = dim.Text.Text; dim.Delete();

                dimg = Parent.Parent.ActiveSheet.DrawingDimensions.GeneralDimensions.AddDiameter(linha.CenterPoint, furo);
                diam = dimg.Text.Text; dimg.Delete();
                //return new double[3] { double.Parse(strx.Replace('.', ',')), double.Parse(stry.Replace('.', ',')), double.Parse(diam.Trim('n').Replace('.', ',')) };
                return new double[3] { double.Parse(strx.Replace(',','.')), double.Parse(stry.Replace(',', '.')), double.Parse(diam.Trim('n').Replace(',', '.')) };
            }
            catch
            {
                return null;
            }


        }
    }
}