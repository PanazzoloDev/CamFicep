using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CamFicep.ViewModel
{
    static class oOracle
    {
        private static OracleConnection Conectar()
        {
            OracleConnection con = new OracleConnection(Properties.Resources.str);
            con.Open();
            return con;

        }
        private static void Desconectar(OracleConnection con)
        {
            if (con.State != System.Data.ConnectionState.Closed)
            {
                con.Close();
            }
        }
        public static DataTable Pesquisar_Ordem( string of)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("SELECT TOR.NUM_ORDEM, PLA.COD_ITEM, TOR.QTDE, PDV.NUM_PEDIDO FROM FOCCO3I.TORDENS TOR");
            str.AppendLine("INNER JOIN FOCCO3I.TITENS_PLANEJAMENTO PLA          ON PLA.ID = TOR.ITPL_ID");
            str.AppendLine("LEFT JOIN FOCCO3I.TORDENS_VINC_ITPDV VINC          ON VINC.ORDEM_ID = TOR.ID");
            str.AppendLine("LEFT JOIN FOCCO3I.TITENS_PDV ITPDV                 ON ITPDV.ID = VINC.ITPDV_ID");
            str.AppendLine("LEFT JOIN FOCCO3I.TPEDIDOS_VENDA PDV               ON PDV.ID = ITPDV.PDV_ID");
            str.AppendLine("WHERE TOR.NUM_ORDEM = " + of);
            DataTable dt = new DataTable();
            try
            {
                OracleConnection con = Conectar();
                OracleCommand cmd = new OracleCommand(str.ToString(), con);

                dt.Load(cmd.ExecuteReader());
                Desconectar(con);
            }
            catch
            {

            }
            return dt;
        }
    }
}
