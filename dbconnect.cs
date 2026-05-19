using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace GTSv2_FINPROJ
{
    public static class playdata
    {
        public static string name { get; set; }
        public static string repu { get; set; } = "";
        public static int silver { get; set; } = 2000;
        public static int compvoy { get; set; } = 0;
        public static int boletas { get; set; } = 6;
        public static int currentid { get; set; }
        public static int curkilos { get; set; } = 1000;
        public static int accountid { get; set; } 
        public static string email { get; set; }  
        public static string role { get; set; }
        public static string lastplayed { get; set; } = "";
    }
    public class unitdata
    {
        public string Category;
        public string Name;
        public string Info;
    }
    public enum mode
    {
        buy = 1,
        sell = 2,
        viewcargo = 3
    }
    public enum kmode
    { 
        ask = 1,
        quiz = 2
    }
    public class dbconnect
    {
        private static string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Kiarra\OneDrive\Documents\GTS_database.accdb";
        public static (int accountID, string role) login(string email, string password)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string q = "SELECT accountID, role FROM accounts WHERE email = @email AND password = @pass";
                    using (OleDbCommand cmd = new OleDbCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@pass", password);
                        using (OleDbDataReader r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                                return ((int)r["accountID"], r["role"].ToString());
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Login error: " + ex.Message); }
            }
            return (-1, "");
        }

        public static int register(string email, string password)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();

                    string check = "SELECT COUNT(*) FROM accounts WHERE email = @email";
                    using (OleDbCommand cmd = new OleDbCommand(check, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        int count = (int)cmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("An account with that email already exists.");
                            return -1;
                        }
                    }

                    string ins = "INSERT INTO accounts (email, [password], [role]) VALUES (@email, @pass, 'player')";
                    using (OleDbCommand cmd = new OleDbCommand(ins, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@pass", password);
                        cmd.ExecuteNonQuery();
                    }

                    string getid = "SELECT @@IDENTITY";
                    using (OleDbCommand cmd = new OleDbCommand(getid, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                catch (Exception ex) { MessageBox.Show("Register error: " + ex.Message); }
            }
            return -1;
        }

        public static bool hasplayprogress(int accountid)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string q = "SELECT COUNT(*) FROM playprogress WHERE accountID = @aid";
                    using (OleDbCommand cmd = new OleDbCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@aid", accountid);
                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
                catch { }
            }
            return false;
        }

        public static bool loadprogress(int accountid)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string q = "SELECT playerID, playername, silver, reputation, compvoy, kilos, boletas, lastplayed FROM playprogress WHERE accountID = @aid";
                    using (OleDbCommand cmd = new OleDbCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@aid", accountid);
                        using (OleDbDataReader r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                playdata.currentid = (int)r["playerID"];
                                playdata.name = r["playername"].ToString();
                                playdata.silver = (int)r["silver"];
                                playdata.compvoy = (int)r["compvoy"];
                                playdata.repu = getreputation(playdata.compvoy);
                                playdata.curkilos = (int)r["kilos"];
                                if (r["boletas"] != DBNull.Value)
                                {
                                    playdata.boletas = (int)r["boletas"];
                                }
                                else
                                {
                                    playdata.boletas = 6;
                                }
                                if (r["lastplayed"] != DBNull.Value)
                                {
                                    DateTime ld = Convert.ToDateTime(r["lastplayed"]);

                                    playdata.lastplayed = ld.ToString("MMM dd, yyyy  hh:mm tt");
                                }
                                else
                                {
                                    playdata.lastplayed = "Never";
                                }
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Load error: " + ex.Message); }
            }
            return false;
        }

        public static string[] getfacts(int pid)
        {
            string[] facts = { "", "", "0", "0" };

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();

                    string qgen = "SELECT TOP 1 genID, genfacts FROM genfacts ORDER BY RND(-1000 * TimeValue(Now()) * genID)";
                    OleDbCommand cmdgen = new OleDbCommand(qgen, conn);
                    using (OleDbDataReader resgen = cmdgen.ExecuteReader())
                    {
                        if (resgen.Read())
                        {
                            facts[0] = resgen["genfacts"].ToString();
                            facts[2] = resgen["genID"].ToString();
                        }
                    }

                    string qport = "SELECT TOP 1 factID, portfacts FROM portfacts WHERE portID = @p1 ORDER BY RND(-1000 * TimeValue(Now()) * factID)";
                    using (OleDbCommand cmdport = new OleDbCommand(qport, conn))
                    {
                        cmdport.Parameters.AddWithValue("@p1", pid); 
                        using (OleDbDataReader resport = cmdport.ExecuteReader())
                        {
                            if (resport.Read())
                            {
                                facts[1] = resport["portfacts"].ToString();
                                facts[3] = resport["factID"].ToString(); 
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Database Error: " + ex.Message);
                }
            }
            return facts;
        }

        public static List<string> getpfacts(int pid)
        {
            List<string> facts = new List<string>();

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string qport = "SELECT portfacts, factID FROM portfacts WHERE portID = @p1";
                    using (OleDbCommand cmdport = new OleDbCommand(qport, conn))
                    {
                        cmdport.Parameters.AddWithValue("@p1", pid);
                        using (OleDbDataReader resport = cmdport.ExecuteReader())
                        {
                            while (resport.Read())
                            {
                                facts.Add(resport["portfacts"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Database Error: " + ex.Message);
                }
            }

            return facts;
        }

        public static List<unitdata> getunits()
        {
            List<unitdata> list = new List<unitdata>();

            string query = "SELECT uc.[type] AS categoryName, ug.[unit], ug.[info] " +
                           "FROM unitguide AS ug " +
                           "INNER JOIN unitcategory AS uc ON ug.[unitID] = uc.[categoryID] " +
                           "ORDER BY ug.[unitID] ASC, ug.[ID] ASC";

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read() == true)
                            {
                                unitdata fact = new unitdata();

                                if (reader["categoryName"] != DBNull.Value)
                                {
                                    fact.Category = reader["categoryName"].ToString();
                                }
                                else
                                {
                                    fact.Category = "";
                                }

                                if (reader["unit"] != DBNull.Value)
                                {
                                    fact.Name = reader["unit"].ToString();
                                }
                                else
                                {
                                    fact.Name = "";
                                }

                                if (reader["info"] != DBNull.Value)
                                {
                                    fact.Info = reader["info"].ToString();
                                }
                                else
                                {
                                    fact.Info = "";
                                }

                                list.Add(fact);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Database Error " + ex.Message);
                }
            }

            return list;
        }
        public static bool saveprog(string name, int silver, int repu, int voyages, int accid)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();

                    string checkSql = "SELECT COUNT(*) FROM playprogress WHERE accountID = ?";
                    int count = 0;
                    using (OleDbCommand checkCmd = new OleDbCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("?", accid);
                        count = (int)checkCmd.ExecuteScalar();
                    }

                    string sql;
                    if (count == 0)
                    {
                        sql = "INSERT INTO playprogress (playername, silver, reputation, compvoy, kilos, accountID, boletas, lastplayed) " +
                              "VALUES (?, ?, ?, ?, 1000, ?, 6, Now())";

                        using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("?", name);    
                            cmd.Parameters.AddWithValue("?", silver);  
                            cmd.Parameters.AddWithValue("?", repu);   
                            cmd.Parameters.AddWithValue("?", voyages);
                            cmd.Parameters.AddWithValue("?", accid);    
                            int rowsAffected = cmd.ExecuteNonQuery();
                            return rowsAffected > 0;
                        }
                    }
                    else
                    {
                        sql = "UPDATE playprogress SET playername = ?, silver = ?, reputation = ?, compvoy = ?, lastplayed = Now() WHERE accountID = ?";

                        using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("?", name);
                            cmd.Parameters.AddWithValue("?", silver);
                            cmd.Parameters.AddWithValue("?", repu);
                            cmd.Parameters.AddWithValue("?", voyages);
                            cmd.Parameters.AddWithValue("?", accid);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            return rowsAffected > 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Save Error: " + ex.Message);
                    return false;
                }
            }
        }
        public static void saveknowjourn(int pid, int fid, int gid)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();

                    if (fid > 0)
                    {
                        string checkp = "SELECT COUNT(*) FROM knowjourn WHERE playerID = " + pid + " AND factID = " + fid;
                        OleDbCommand cmdcheckp = new OleDbCommand(checkp, conn);
                        int countp = (int)cmdcheckp.ExecuteScalar();

                        if (countp == 0)
                        {
                            string inport = "INSERT INTO knowjourn (playerID, factID) VALUES (" + pid + "," + fid + ")";
                            OleDbCommand cmdInPort = new OleDbCommand(inport, conn);
                            cmdInPort.ExecuteNonQuery();
                        }
                    }

                    if (gid > 0)
                    {
                        string checkg = "SELECT COUNT(*) FROM knowjourn WHERE playerID = " + pid + " AND genID = " + gid;
                        OleDbCommand cmdcheckg = new OleDbCommand(checkg, conn);
                        int countg = (int)cmdcheckg.ExecuteScalar();

                        if (countg == 0)
                        {
                            string ingen = "INSERT INTO knowjourn (playerID, genID) VALUES (" + pid + "," + gid + ")";
                            OleDbCommand cmdInGen = new OleDbCommand(ingen, conn);
                            cmdInGen.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Save Error: " + ex.Message);
                }
            }
        }
        public static List<string> getunlockedfacts(int pid, int specificPortID, bool gen)
        {
            List<string> ret = new List<string>();
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query;
                    if (gen)
                    {
                        query = "SELECT g.genfacts FROM genfacts g " +
                                "INNER JOIN knowjourn k ON g.genID = k.genID " +
                                "WHERE k.playerID = " + pid;
                    }
                    else
                    {

                        query = "SELECT pf.portfacts FROM portfacts pf " +
                                "INNER JOIN knowjourn kj ON pf.factID = kj.factID " +
                                "WHERE kj.playerID = " + pid + " AND pf.portID = " + specificPortID;
                    }

                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ret.Add(reader[0].ToString());
                        }
                    }
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show("Journal Error: " + ex.Message); 
                }
            }
            return ret;
        }

        public static List<(int factID, string fact)>getquizfacts(int portid, int count)
        {
            var facts = new List<(int, string)>();

            using(OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string q = "SELECT TOP " + count + " factID, portfacts FROM portfacts WHERE portID = @pid ORDER BY RND(-1000 * TimeValue(Now()) * factID)";
                    using (OleDbCommand cmd = new OleDbCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", portid);
                        using (OleDbDataReader r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                facts.Add(((int)r["factID"], r["portfacts"].ToString()));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Quizfacts Error: " + ex.Message);
                }
            }
            return facts;
        }
        public static List<string> getwrongfacts(int excludePortid, int count)
        {
            var facts = new List<string>();
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string q = "SELECT TOP " + count + " portfacts FROM portfacts WHERE portID <> @pid ORDER BY RND(-1000*TimeValue(Now())*factID)";
                    using (OleDbCommand cmd = new OleDbCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", excludePortid);
                        using (OleDbDataReader r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                facts.Add(r["portfacts"].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show("Wrong facts error: " + ex.Message); 
                }
            }
            return facts;
        }
        public static List<string> getportnames()
        {
            var names = new List<string>();
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string q = "SELECT portname FROM portdata";
                    using (OleDbCommand cmd = new OleDbCommand(q, conn))
                    using (OleDbDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            names.Add(r["portname"].ToString());
                        }
                    }
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show("Port names error: " + ex.Message);
                }
            }
            return names;
        }
        public static void saverepu(int playerid, int portid, int score, string title)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string check = "SELECT COUNT(*) FROM quiztbl WHERE playerID = @pid AND portID = @port";
                    using (OleDbCommand cmd = new OleDbCommand(check, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", playerid);
                        cmd.Parameters.AddWithValue("@port", portid);
                        int exists = (int)cmd.ExecuteScalar();

                        if (exists > 0)
                        {
                            string upd = "UPDATE quiztbl SET repuscore = @score, reputitle = @title WHERE playerID = @pid AND portID = @port";
                            using (OleDbCommand ucmd = new OleDbCommand(upd, conn))
                            {
                                ucmd.Parameters.AddWithValue("@score", score);
                                ucmd.Parameters.AddWithValue("@title", title);
                                ucmd.Parameters.AddWithValue("@pid", playerid);
                                ucmd.Parameters.AddWithValue("@port", portid);
                                ucmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string ins = "INSERT INTO quiztbl (playerID, portID, repuscore, reputitle) VALUES (@pid, @port, @score, @title)";
                            using (OleDbCommand icmd = new OleDbCommand(ins, conn))
                            {
                                icmd.Parameters.AddWithValue("@pid", playerid);
                                icmd.Parameters.AddWithValue("@port", portid);
                                icmd.Parameters.AddWithValue("@score", score);
                                icmd.Parameters.AddWithValue("@title", title);
                                icmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show("Save repu error: " + ex.Message); 
                }
            }
        }
        public static (int score, string title) getrepu(int playerid, int portid)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string q = "SELECT repuscore, reputitle FROM quiztbl WHERE playerID = @pid AND portID = @port";
                    using (OleDbCommand cmd = new OleDbCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", playerid);
                        cmd.Parameters.AddWithValue("@port", portid);
                        using (OleDbDataReader r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                                return ((int)r["repuscore"], r["reputitle"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("get repu error: " + ex.Message);
                }
            }
            return (0, "Zopenco");
        }
        public static string getreputation(int compvoy)
        {
            if (compvoy >= 6) { return "Consignatario"; }
            if (compvoy >= 4) { return "Mercader Registrado"; }
            if (compvoy >= 2) { return "Oficial de Almacén"; }
            return "Aprendiz de Gremio";
        }
        public static bool portcompvoy(int pid)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

                    string query = "UPDATE playprogress SET compvoy = compvoy + 1, boletas = IIF(boletas > 0, boletas - 1, 0) WHERE playerID = @id";
                    OleDbCommand cmd = new OleDbCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", playdata.currentid);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        string updrep = "UPDATE playprogress SET reputation = @rep WHERE playerID = @id";
                        using (OleDbCommand rcmd = new OleDbCommand(updrep, conn))
                        {
                            int newcompvoy = playdata.compvoy + 1;
                            string newrepu = getreputation(newcompvoy);

                            rcmd.Parameters.AddWithValue("@rep", newrepu);
                            rcmd.Parameters.AddWithValue("@id", playdata.currentid);
                            rcmd.ExecuteNonQuery();
                        }

                        loadprogress(playdata.accountid);
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Voyage Update Error: " + ex.Message);
                return false;
            }
        }

        internal void itemclick(object sender, EventArgs e, Label nameLbl, Label priceLbl, Label unitLbl, Label weightLbl, Label infoLbl)
        {
            hovereff iconclick = (hovereff)sender;
            int selid = iconclick.dataid;

            if (selid == 0) return;

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    try
                    {
                        string query = "SELECT itemname, basevalue, [Bulk Unit], Kilos, info FROM itemlist WHERE itemID = @id";
                        OleDbCommand cmd = new OleDbCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", selid);

                        conn.Open();
                        OleDbDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            nameLbl.Text = reader["itemname"].ToString();
                            priceLbl.Text = reader["basevalue"].ToString() + " Real de a Ocho";
                            unitLbl.Text = reader["Bulk Unit"].ToString();
                            weightLbl.Text = reader["Kilos"].ToString() + " kilos";
                            infoLbl.Text = reader["info"].ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Database error: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error: " + ex.Message);
            }
        }
        public static float getrepumod(int playerid, int portid, bool buy)
        {
            var (score, title) = getrepu(playerid, portid);

            switch (title)
            {
                case "Perito":
                    if(buy == true)
                    {
                        return 0.80f;
                    }
                    else
                    {
                        return 1.20f;
                    }
                    break;
                case "Mercachifle":
                    if (buy == true)
                    {
                        return 0.85f;
                    }
                    else
                    {
                        return 1.15f;
                    }
                    break;
                default:
                    if(buy == true)
                    {
                        return 1.20f;
                    }
                    else
                    {
                        return 0.80f;
                    }
                    break;
            }

        }
        public static bool BuyItem(int itemid, int qty, int itemw, int iprice, int pID)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    int totweight = qty * itemw;
                    float mod = getrepumod(playdata.currentid, pID, true);
                    int totcost = (int)(qty * iprice * mod);

                    if (playdata.silver < totcost)
                    {
                        MessageBox.Show("Not enough silver! You need " + totcost + " Reales.");
                        return false;
                    }

                    if (playdata.curkilos - totweight < 50)
                    {
                        MessageBox.Show("Not enough cargo space! You must keep at least 50 kg free.");
                        return false;
                    }

                    string updatestats = "UPDATE playprogress SET silver = silver - @cost, kilos = kilos - @w WHERE playerID = @id";
                    using (OleDbCommand cmd = new OleDbCommand(updatestats, conn))
                    {
                        cmd.Parameters.AddWithValue("@cost", totcost);
                        cmd.Parameters.AddWithValue("@w", totweight);
                        cmd.Parameters.AddWithValue("@id", playdata.currentid);
                        cmd.ExecuteNonQuery();
                    }

                    string checkinv = "SELECT quantity FROM inventory WHERE playerID = @pid AND itemID = @iid";
                    OleDbCommand ccmd = new OleDbCommand(checkinv, conn);
                    ccmd.Parameters.AddWithValue("@pid", playdata.currentid);
                    ccmd.Parameters.AddWithValue("@iid", itemid);
                    object res = ccmd.ExecuteScalar();

                    if (res == null)
                    {
                        string addinv = "INSERT INTO inventory (playerID, itemID, quantity) VALUES (@pid, @iid, @qty)";
                        OleDbCommand ain = new OleDbCommand(addinv, conn);
                        ain.Parameters.AddWithValue("@pid", playdata.currentid);
                        ain.Parameters.AddWithValue("@iid", itemid);
                        ain.Parameters.AddWithValue("@qty", qty);
                        ain.ExecuteNonQuery();
                    }
                    else
                    {
                        string uppinv = "UPDATE inventory SET quantity = quantity + @qty WHERE playerID = @pid AND itemID = @iid";
                        OleDbCommand uinv = new OleDbCommand(uppinv, conn);
                        uinv.Parameters.AddWithValue("@pid", playdata.currentid);
                        uinv.Parameters.AddWithValue("@iid", itemid);
                        uinv.Parameters.AddWithValue("@qty", qty);
                        uinv.ExecuteNonQuery();
                    }

                    playdata.silver -= totcost;
                    playdata.curkilos -= totweight;
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
                return false;
            }
        }

        public static DataTable getnventory(int pid)
        {
            DataTable dt = new DataTable();
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                string query = "SELECT i.itemID, il.itemname, i.quantity, il.basevalue " +
                               "FROM inventory i INNER JOIN itemlist il ON i.itemID = il.itemID " +
                               "WHERE i.playerID = " + pid + " AND i.quantity > 0";
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                conn.Open();
                adapter.Fill(dt);
            }
            return dt;
        }
        public static bool sellitem(int pid, int itemid, int qty, int iweight, int iprice)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();

                    int currentqty = 0;
                    string check = "SELECT quantity FROM inventory WHERE playerID = " + playdata.currentid + " AND itemID = " + itemid;
                    using (OleDbCommand cmd = new OleDbCommand(check, conn))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            currentqty = Convert.ToInt32(result);
                        }
                    }

                    if (qty >= currentqty)
                    {
                        string deleteQuery = "DELETE FROM inventory WHERE playerID = " + playdata.currentid + " AND itemID = " + itemid;
                        new OleDbCommand(deleteQuery, conn).ExecuteNonQuery();
                    }
                    else
                    {
                        string updateInv = "UPDATE inventory SET quantity = quantity - " + qty +
                                           " WHERE playerID = " + playdata.currentid + " AND itemID = " + itemid;
                        new OleDbCommand(updateInv, conn).ExecuteNonQuery();
                    }

                    int gained = qty * iprice;
                    float mod = getrepumod(playdata.currentid, pid, false);
                    gained = (int)(qty * iprice * mod);

                    int wfree = qty * iweight;

                    string updateStats = "UPDATE playprogress SET silver = silver + " + gained +
                                         ", kilos = kilos + " + wfree +
                                         " WHERE playerID = " + playdata.currentid;
                    new OleDbCommand(updateStats, conn).ExecuteNonQuery();

                    playdata.silver += gained;
                    playdata.curkilos += wfree;

                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Transaction failed: " + ex.Message);
                    return false;
                }
            }
        }
        public static bool deleteall(int accountid)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();

                    int playerid = -1;
                    string getplayer = "SELECT playerID FROM playprogress WHERE accountID = @aid";
                    using (OleDbCommand cmd = new OleDbCommand(getplayer, conn))
                    {
                        cmd.Parameters.AddWithValue("@aid", accountid);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                            playerid = Convert.ToInt32(result);
                    }

                    if (playerid > 0)
                    {
                        new OleDbCommand("DELETE FROM inventory WHERE playerID = " + playerid, conn).ExecuteNonQuery();

                        new OleDbCommand("DELETE FROM knowjourn WHERE playerID = " + playerid, conn).ExecuteNonQuery();

                        new OleDbCommand("DELETE FROM quiztbl WHERE playerID = " + playerid, conn).ExecuteNonQuery();

                        new OleDbCommand("DELETE FROM playprogress WHERE playerID = " + playerid, conn).ExecuteNonQuery();
                    }

                    string delaccount = "DELETE FROM accounts WHERE accountID = @aid";
                    using (OleDbCommand cmd = new OleDbCommand(delaccount, conn))
                    {
                        cmd.Parameters.AddWithValue("@aid", accountid);
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Delete account error: " + ex.Message);
                    return false;
                }
            }
        }
        public static void updatelastplayed(int playerid)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string q = "UPDATE playprogress SET lastplayed = Now() WHERE playerID = @id";
                    using (OleDbCommand cmd = new OleDbCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", playerid);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Last played update error: " + ex.Message); }
            }
        }

        public static DataTable getallplayers()
        {
            DataTable dt = new DataTable();
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string q = @"SELECT p.playerID, p.playername, a.email, p.silver, 
                        p.reputation, p.compvoy, p.kilos, p.boletas, p.lastplayed,
                        q.portID, q.repuscore, q.reputitle
                        FROM (playprogress p 
                        INNER JOIN accounts a ON p.accountID = a.accountID)
                        LEFT JOIN quiztbl q ON p.playerID = q.playerID
                        WHERE a.role = 'player'
                        ORDER BY p.playername, q.portID";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(q, conn);
                    adapter.Fill(dt);
                }
                catch (Exception ex) { MessageBox.Show("Analytics error: " + ex.Message); }
            }
            return dt;
        }

        public static DataTable getplayerquiz(int playerid)
        {
            DataTable dt = new DataTable();
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string q = @"SELECT q.portID, d.portname, q.repuscore, q.reputitle
                        FROM quiztbl q
                        INNER JOIN portdata d ON q.portID = d.portID
                        WHERE q.playerID = @pid
                        ORDER BY q.portID";
                    using (OleDbCommand cmd = new OleDbCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", playerid);
                        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                }
                catch (Exception ex) { MessageBox.Show("Player quiz error: " + ex.Message); }
            }
            return dt;
        }

        public static bool savefeedback(int playerid, int rating, string feedback)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string q = "INSERT INTO feedback (playerID, rating, feedback, datesubmitted) VALUES (@pid, @rat, @fb, Now())";
                    using (OleDbCommand cmd = new OleDbCommand(q, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", playerid);
                        cmd.Parameters.AddWithValue("@rat", rating);
                        cmd.Parameters.AddWithValue("@fb", feedback);
                        cmd.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (Exception ex) { MessageBox.Show("Feedback error: " + ex.Message); return false; }
            }
        }

        public static DataTable getallfeedback()
        {
            DataTable dt = new DataTable();
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string q = @"SELECT p.playername, a.email, f.rating, f.feedback, f.datesubmitted
                        FROM (feedback f 
                        INNER JOIN playprogress p ON f.playerID = p.playerID)
                        INNER JOIN accounts a ON p.accountID = a.accountID
                        ORDER BY f.datesubmitted DESC";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(q, conn);
                    adapter.Fill(dt);
                }
                catch (Exception ex) { MessageBox.Show("Get feedback error: " + ex.Message); }
            }
            return dt;
        }
    }
}