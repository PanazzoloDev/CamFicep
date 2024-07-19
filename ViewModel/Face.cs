using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CamFicep.ViewModel
{
    public class Face
    {
        public string Nome { get; set; }
        public double Comprimento { get; set; }
        public double Altura { get; set; }
        public double Escala_Vertical { get; set; }
        public double Escala_Horizontal { get; set; }

        public DrawingView Aba;
        public List<GeometryIntent> Limites = new List<GeometryIntent>();
        public List<Furacao> Furacoes = new List<Furacao>();
        public DrawingDocument Parent;
        public GeometryIntent LinhaCentro;
        public Canvas Design;

        public Face(string name, DrawingView view, DrawingDocument doc, Canvas design,Color cor)
        {
            this.Aba = view;
            this.Nome = name;
            this.Parent = doc;
            this.Design = design;
            if (view.ViewStyle == DrawingViewStyleEnum.kHiddenLineDrawingViewStyle || view.ViewStyle == DrawingViewStyleEnum.kFromBaseDrawingViewStyle)
                view.ViewStyle = DrawingViewStyleEnum.kHiddenLineRemovedDrawingViewStyle;
            this.Limites = Selecionar_Limites(view, cor);
            double alt, comp;
            Dimensoes(this.Limites[0], this.Limites[1], this.Limites[2], this.Limites[3],out comp,out alt);
            if (alt == 0) return;
            this.Altura = alt != 0? alt: 0;
            if (comp == 0) return;
            this.Comprimento = comp != 0 ? comp : 0;
            Furacoes = Criar_Furacoes(this.Aba, this.Nome, this.Design);
        }
        private List<GeometryIntent> Selecionar_Limites(DrawingView ABA, Color cor,double altSheet= 3000, double largSheet=3000)
        {
            double up = 0, down = altSheet, left = largSheet, right = 0;
            DrawingCurve UpLimit = null, DownLimit = null, LeftLimit = null, RightLimit = null;

            foreach (DrawingCurve linha in ABA.DrawingCurves)
            {
                if (linha.CurveType != CurveTypeEnum.kLineSegmentCurve) continue;
                var vetor = linha.Segments[1].Geometry.Direction;
                if ((vetor.X != -1 && vetor.Y != -1) && (vetor.X != 1 && vetor.Y != 1)) continue;
                 
                    //Limite Superior
                if (linha.MidPoint.Y > up)
                {
                    up = linha.MidPoint.Y;
                    UpLimit = linha;
                }
                //Limite da Direita
                if (linha.MidPoint.X > right)
                {
                    right = linha.MidPoint.X;
                    RightLimit = linha;
                }
                //Limite Inferior
                if (linha.MidPoint.Y < down)
                {
                    down = linha.MidPoint.Y;
                    DownLimit = linha;
                }
                //Limite da Esquerda
                if (linha.MidPoint.X < left)
                {
                    left = linha.MidPoint.X;
                    LeftLimit = linha;
                }
            }
            UpLimit.Color = cor;
            DownLimit.Color = cor;
            LeftLimit.Color = cor;
            RightLimit.Color = cor;

            GeometryIntent tp = Parent.ActiveSheet.CreateGeometryIntent(UpLimit);
            GeometryIntent bt = Parent.ActiveSheet.CreateGeometryIntent(DownLimit);
            LinhaCentro = Parent.ActiveSheet.CreateGeometryIntent(Parent.ActiveSheet.Centerlines.AddBisector(tp, bt));

            List<GeometryIntent> limites = new List<GeometryIntent>();
            GeometryIntent cima = Parent.ActiveSheet.CreateGeometryIntent(UpLimit);
            GeometryIntent baixo = Parent.ActiveSheet.CreateGeometryIntent(DownLimit);
            GeometryIntent esquerda = Parent.ActiveSheet.CreateGeometryIntent(LeftLimit);
            GeometryIntent direita = Parent.ActiveSheet.CreateGeometryIntent(RightLimit);
            if (limites.Contains(cima) == false) limites.Add(cima);
            if (limites.Contains(baixo) == false) limites.Add(baixo);
            if (limites.Contains(esquerda) == false) limites.Add(esquerda);
            if (limites.Contains(direita) == false) limites.Add(direita);


            if (limites.Count < 4) return null;
            return limites;
        }
        private List<Furacao> Criar_Furacoes(DrawingView aba, string nome_aba, Canvas design)
        {
            List<Furacao> frs = new List<Furacao>();
            foreach (DrawingCurve linha in aba.DrawingCurves)
            {
                if (linha.CurveType != CurveTypeEnum.kCircleCurve) continue;
                if (linha.Segments[1].Visible == false) continue;
                Furacao furo = new Furacao(this, linha,nome_aba, design);
                if(furo.X != 0 || furo.Y != 0) frs.Add(furo);
            }
            return frs;
        }
        private void Dimensoes(GeometryIntent lf, GeometryIntent rg, GeometryIntent tp, GeometryIntent bt, out double alt, out double comp)
        {
            LinearGeneralDimension dim;
            comp = 0;
            alt = 0;
            try
            {
                dim = Parent.ActiveSheet.DrawingDimensions.GeneralDimensions.AddLinear(lf.Geometry.MidPoint, lf, rg,DimensionTypeEnum.kHorizontalDimensionType);
                comp = double.Parse(dim.Text.Text.Replace(',', '.')); dim.Delete();
                //comp = double.Parse(dim.Text.Text.Replace('.', ',')); dim.Delete();

                dim = Parent.ActiveSheet.DrawingDimensions.GeneralDimensions.AddLinear(tp.Geometry.MidPoint, tp, bt,DimensionTypeEnum.kVerticalDimensionType);
                alt = double.Parse(dim.Text.Text.Replace(',', '.')); dim.Delete();
                //alt = double.Parse(dim.Text.Text.Replace('.', ',')); dim.Delete();

                Escala_Vertical = Design.ActualHeight / comp;
                Escala_Horizontal = Design.ActualWidth / alt;
            }
            catch
            {

            }

        }
    }
}
