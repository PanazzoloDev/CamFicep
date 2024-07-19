using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CamFicep.ViewModel
{
    static class Processador
    {
        public static string Processar(DataGrid Selecoes, Perfil P, Parametros parametros, string destino)
        {
            StringBuilder cmd_cnc = new StringBuilder();
            cmd_cnc.AppendLine("");
            cmd_cnc.AppendLine("[[PRF]]");
            cmd_cnc.AppendFormat("[PRF] CP:W P:{0} {1} {2} {3} {4} {5} {6} \n",P.Nome, P.Larg_Alma, P.Esp_Alma, P.Larg_AbaDireita,P.Esp_AbaDireita, P.PesoLinear, P.Raio);
            cmd_cnc.AppendLine("");
            cmd_cnc.AppendLine("[[MAT]]");
            cmd_cnc.AppendLine("[MAT] M:A572-50 CM0 WS7.860 ");
            cmd_cnc.AppendLine("");
            cmd_cnc.AppendLine("[[PCS]]");
            cmd_cnc.AppendLine("[HEAD]");
            cmd_cnc.AppendFormat("C:{0} D:{1} N:{2} POS:{3} \n", parametros.Pedido, parametros.Item, parametros.Programador,parametros.Ordem);
            cmd_cnc.AppendFormat("M:A572-50 CP:W P:{0} \n", P.Nome);
            cmd_cnc.AppendFormat("LP{0} \n", parametros.Comprimento);
            cmd_cnc.AppendFormat("QI{0} SCA101 \n", parametros.Quantidade);

            foreach (Furacao sel in Selecoes.Items)
            {
                double Y = sel.Y;
                int org = parametros.Orgs[sel.Aba + sel.Origem];

                string Apalpador = string.Empty;
                if (sel.Origem.Contains("M"))
                {
                    Y = Y * 2;
                    Apalpador = " M840 ";
                }
                else
                {
                    if (sel.Aba != "DB")
                    {
                        if (org == 1 || org == 11)
                        {
                            Apalpador = " M830 ";
                        }
                        else if (org == 0 || org == 10)
                        {
                            Apalpador = " ";
                        }
                    }
                    else
                    {
                        if (org == 1 || org == 11)
                        {
                            Apalpador = " ";
                        }
                        else if (org == 0 || org == 10)
                        {
                            Apalpador = " M830 ";
                        }
                    }
                }


                string Ferr;
                switch (sel.Diam.ToString())
                {
                    case "14":
                        Ferr = "HSS";
                        break;
                    case "18":
                        Ferr = "SR-18";
                        break;
                    default:
                        Ferr = "";
                        break;
                }


                if (org == 0)//SE O A ORIGEM FOR 0, NÃO INSERE O COMANDO 'ORG'
                {
                    cmd_cnc.AppendLine("[HOL] TS31 " + sel.Aba + sel.Diam + ".000 COD:" + Ferr + " X" + sel.X + " Y" + Y + Apalpador);
                }
                else//SE O A ORIGEM FOR != 0, INSERE O COMANDO 'ORG'
                {
                    cmd_cnc.AppendLine("[HOL] TS31 " + sel.Aba + sel.Diam + ".000 COD:" + Ferr + " ORG" + org + " X" + sel.X + " Y" + Y + Apalpador);
                }

            }
            string arquivo = string.Empty;
            if (parametros.Ordem != "000000")
            {
                arquivo = parametros.Ordem;
            }
            else
            {
                arquivo = parametros.Item;
            }
            System.IO.File.WriteAllText(destino + @"\" + arquivo + ".fnc", cmd_cnc.ToString());
            return cmd_cnc.ToString();
        } 
    }
}
