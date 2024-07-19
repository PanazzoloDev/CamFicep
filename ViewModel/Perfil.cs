using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamFicep.ViewModel
{
    public class Perfil
    {
        public string Nome { get; set; }
        public string Larg_Alma { get; set; }
        public string Esp_Alma { get; set; }
        public string Larg_AbaDireita { get; set; }
        public string Esp_AbaDireita { get; set; }
        public string PesoLinear { get; set; }
        public string Raio { get; set; }

        public Perfil(string nome, string la, string ea, string ld, string ed, string pl, string r)
        {
            this.Nome = nome;
            this.Larg_Alma = "SA"+la;
            this.Esp_Alma = "TA"+ea;
            this.Larg_AbaDireita = "SB"+ld;
            this.Esp_AbaDireita = "TB"+ed;
            this.PesoLinear = "WL"+pl;
            this.Raio = "R"+r;
        }
    }
}
