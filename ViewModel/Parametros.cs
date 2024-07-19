using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamFicep.ViewModel
{
    public class Parametros
    {
        public string Item { get; set; }
        public string Ordem { get; set; }
        public string Material { get; set; } = "S/M";
        public string Programador { get; set; }
        public string Quantidade { get; set; }
        public string Pedido { get; set; }
        public double Comprimento { get; set; }
        public Dictionary<string, int> Orgs;

        public Parametros(string it, string mt, string pr, string qt, double comp, string of, string ped)
        {
            this.Item = it;
            this.Ordem = of != ""? of:"000000";
            this.Material = mt;
            this.Programador = pr;
            this.Quantidade = qt;
            this.Pedido = ped != "" ? ped:"0000";
            this.Comprimento = comp;
            Gerar_ORGS();
        }
        public void Gerar_ORGS()
        {
            Orgs = new Dictionary<string, int>();
            Orgs.Add("DBIE", 1); Orgs.Add("DCIE", 0); Orgs.Add("DAIE", 0);
            Orgs.Add("DBME+", 3); Orgs.Add("DCME+", 2); Orgs.Add("DAME+", 2);
            Orgs.Add("DBME-", 2); Orgs.Add("DCME-", 3); Orgs.Add("DAME-", 3);
            Orgs.Add("DBSE", 0); Orgs.Add("DCSE", 1); Orgs.Add("DASE", 1);
            Orgs.Add("DBID", 11); Orgs.Add("DCID", 10); Orgs.Add("DAID", 10);
            Orgs.Add("DBMD+", 13); Orgs.Add("DCMD+", 12); Orgs.Add("DAMD+", 12);
            Orgs.Add("DBMD-", 12); Orgs.Add("DCMD-", 13); Orgs.Add("DAMD-", 13);
            Orgs.Add("DBSD", 10); Orgs.Add("DCSD", 11); Orgs.Add("DASD", 11);
        }
    }
}
