//          GUI app to simulate DIVVY bike operations
//          Jaskaran Singh      jsingh10        670193440
//          U. of Illinois, Chicago
//          CS480, Summer 2016
//          HW3
//


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace _480HW3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        bool listState=true;            //stops list clicks when not needed
        int state=0;                    //indicates what is in the list box
     
        //lists customers by ID, name, email, and joindate
        private void ShowCustomers()
        {
            listBox1.Items.Clear();
            string filename, connectinfo, sql;
            SqlConnection db;
            SqlCommand comm = new SqlCommand();

            filename = "DIVVY.mdf";

            connectinfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;
                AttachDBFilename=|DataDirectory|\{0};Integrated Security=True;",
                filename);
            db = new SqlConnection(connectinfo);
            db.Open();

            comm.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            DataSet ds = new DataSet();


            sql = String.Format(@"
                    SELECT CustomerID, FirstName, LastName, Email, DateJoined
                    FROM    Customers
            ");

            comm.CommandText = sql;
            adapter.Fill(ds);

            db.Close();
            listBox1.Items.Add(string.Format("{0,10}:{1,40}{2,40}{3,40}{4,40}", "CustomerID", "First Name", "Last Name", "Email", "DateJoined"));

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {

                string msg = String.Format(@"{0,10}:{1,40}{2,40}{3,40}{4,40}",
                    Convert.ToInt32(row["CustomerID"]),
                    Convert.ToString(row["FirstName"]),
                    Convert.ToString(row["LastName"]),
                    Convert.ToString(row["Email"]),
                    Convert.ToString(row["DateJoined"])
                    );

                this.listBox1.Items.Add(msg);
            }
            listState = true;
            state = 0;
        }

        //Lists Stations by Address, latitude, logitude
        private void ShowStations()
        {
            listBox1.Items.Clear();
            if (state == 2 || state == 5)
            {
                listBox1.Items.Add("Choose Station:");
                listBox1.Items.Add("");
            }
            else { state = 1; }
            string filename, connectinfo, sql;
            SqlConnection db;
            SqlCommand comm = new SqlCommand();

            filename = "DIVVY.mdf";

            connectinfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;
                AttachDBFilename=|DataDirectory|\{0};Integrated Security=True;",
                filename);
            db = new SqlConnection(connectinfo);
            db.Open();

            comm.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            DataSet ds = new DataSet();


            sql = String.Format(@"
                    SELECT StationID, CrossStreet1, CrossStreet2, Latitude, Longitude
                    FROM    Stations
            ");

            comm.CommandText = sql;
            adapter.Fill(ds);

            db.Close();
            listBox1.Items.Add(string.Format("{0,10}{1,40}{2,40}{3,40}{4,40}", "StationID", "Street1", "Street2", "Latitude", "Longitude"));

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {

                string msg = String.Format(@"{0,10}:{1,40}{2,40}{3,40}{4,40}",
                    Convert.ToInt32(row["StationID"]),
                    Convert.ToString(row["CrossStreet1"]),
                    Convert.ToString(row["CrossStreet2"]),
                    Convert.ToDouble(row["Latitude"]),
                    Convert.ToDouble(row["Longitude"])
                    );

                this.listBox1.Items.Add(msg);
            }
            listState = true;
        }

        //first button shows stations
        private void button1_Click(object sender, EventArgs e)
        {
            ShowStations();
        }

        //shows info about a particular station including all bikes docked.
        private void Display_StationInf(int station)
        {
            listBox1.Items.Clear();
            string filename, connectinfo, sql;
            SqlConnection db;
            SqlCommand comm = new SqlCommand();

            filename = "DIVVY.mdf";

            connectinfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;
                AttachDBFilename=|DataDirectory|\{0};Integrated Security=True;",
                filename);
            db = new SqlConnection(connectinfo);
            db.Open();

            comm.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            DataSet ds = new DataSet();
            DataSet current = new DataSet();

            sql = String.Format(@"
                    SELECT  DISTINCT    History.BikeID,Capacity,BikeCount
                    FROM    Stations, History
                    WHERE   Stations.StationID={0} AND Stations.StationID = History.StationIDin;
                ", station);

            comm.CommandText = sql;
            adapter.Fill(ds);

            listBox1.Items.Add(string.Format("For Station {0}", station));
            int i = 0;
            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                if (i == 0 && CheckOutSt != 1)
                {
                    listBox1.Items.Add(string.Format("Capacity: {0}", Convert.ToInt32(row["Capacity"])));
                    listBox1.Items.Add(string.Format("BikeCount: {0}", Convert.ToInt32(row["BikeCount"])));
                    
                }
                if (i == 0)
                {
                    listBox1.Items.Add(string.Format("BikeIDs:"));
                }
                string msg = String.Format(@"{0,10}",
                    Convert.ToInt32(row["BikeID"])
                );
                listBox1.Items.Add(msg);
                i = 1;
            }
        }

        //displaycs info about a particular customer as well as their history
        private void Display_CustomerInf(int Customer)
        {
            listBox1.Items.Clear();
            string filename, connectinfo, sql;
            SqlConnection db;
            SqlCommand comm = new SqlCommand();

            filename = "DIVVY.mdf";

            connectinfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;
                AttachDBFilename=|DataDirectory|\{0};Integrated Security=True;",
                filename);
            db = new SqlConnection(connectinfo);
            db.Open();

            comm.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            DataSet ds = new DataSet();
            DataSet current = new DataSet();
            string sql2 = String.Format(@"
                            SELECT  BikeID AS Currently_Out
                            From    History
                            WHERE   CustomerID={0} AND Checkin = NULL;
                    ", Customer);

            sql = String.Format(@"
                    SELECT  BikeID, Checkout, StationIDout,Checkin, StationIDin
                    FROM    History
                    WHERE   CustomerID = {0}
                    ORDER   BY  Checkin DESC;
                    ", Customer);
            comm.CommandText = sql2;
            adapter.Fill(current);
            comm.CommandText = sql;
            adapter.Fill(ds);
            listBox1.Items.Add(string.Format("For Customer ID {0}", Customer));
            listBox1.Items.Add("Currently Checked out bikes:");
            foreach (DataRow row in current.Tables["TABLE"].Rows)
            {
                listBox1.Items.Add(String.Format("{0}", Convert.ToInt32(row["BikeID"])));

            }
            listBox1.Items.Add("");
            listBox1.Items.Add("Customer History:");
            int i = 0;
            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                if (i == 0)
                {
                    listBox1.Items.Add(string.Format("{0,10}{1,40}{2,42}{3,48}{4,42}", "BikeID", "Check Out", "Station Out", "Check In", "Station In"));
                }

                string msg = String.Format(@"{0,10}{1,40}{2,40}{3,40}{4,40}",
                    Convert.ToInt32(row["BikeID"]),
                    Convert.ToString(row["Checkout"]),
                    Convert.ToInt32(row["StationIDout"]),
                    Convert.ToString(row["Checkin"]),
                    Convert.ToInt32(row["StationIDin"])
                );
                this.listBox1.Items.Add(msg);
                i = 1;
            }
        }

        //checkout method. Inserts an entry into the history table
        private void insert_History(int Station, int BikeID, int customer)
        {
            listBox1.Items.Clear();
            string filename, connectinfo, sql;
            SqlConnection db;
            SqlCommand comm = new SqlCommand();

            filename = "DIVVY.mdf";

            connectinfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;
                AttachDBFilename=|DataDirectory|\{0};Integrated Security=True;",
                filename);
            db = new SqlConnection(connectinfo);
            db.Open();

            comm.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            DataSet ds = new DataSet();
            DataSet current = new DataSet();

            sql = String.Format(@"
                    Insert Into 
                                History(BikeID,StationIDout,CustomerID,Checkout)Values({0},{1},{2},Convert(datetime,'{3}',120));
                                ", BikeID, Station, customer,Convert.ToString(DateTime.Now));
            
            comm.CommandText = sql;
            adapter.Fill(current);
            sql = String.Format(@"
                    Update  Stations
                    Set     BikeCount = BikeCount - 1
                    Where   StationID = {0}
            ",Station);
            listBox1.Items.Add("Successfully Checked Out A Bike!");
            db.Close();
        }

        //bike return. Updates the history entry made previously
        private void Return_Bike(int BikeID, int StationIN)
        {
            listBox1.Items.Clear();
            string filename, connectinfo, sql;
            SqlConnection db;
            SqlCommand comm = new SqlCommand();

            filename = "DIVVY.mdf";

            connectinfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;
                AttachDBFilename=|DataDirectory|\{0};Integrated Security=True;",
                filename);
            db = new SqlConnection(connectinfo);
            db.Open();

            comm.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            DataSet ds = new DataSet();
            DataSet current = new DataSet();

            sql = String.Format(@"
                Update  History
                Set     StationIDin = {0},  Checkin = Convert(datetime,'{2}',120)
                WHERE   BikeID = {1}
            ", StationIN,BikeID,Convert.ToString(DateTime.Now));

            comm.CommandText = sql;
            adapter.Fill(ds);
            db.Close();
        }

        //holds data for function calls
        int bikenum=-1;
        int statnum = -1;
        int retbike = -1;
        int retstat = -1;

        //clicking on list box
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listState)
            {
                string id = Convert.ToString(listBox1.GetItemText(listBox1.SelectedItem));
                id = id.Split(':')[0];
                id.Replace(":", "");
                int station = Convert.ToInt32(id);
                if (state == 0)
                {
                    Display_CustomerInf(station);
                }
                if(state==1)
                {
                    Display_StationInf(station);
                }
                listState = false;
                if (state == 2)
                {
                    statnum = station;
                    Display_StationInf(station);
                    listState = true;
                    state = 3;
                    return;
                }
                if(state == 3)
                {
                    string newid = Convert.ToString(listBox1.GetItemText(listBox1.SelectedItem));
                    newid = newid.Split(':')[0];
                    newid.Replace(":", "");
                    bikenum = Convert.ToInt32(newid);
                    ShowCustomers();
                    state = 4;
                    return;
                }
                if (state == 4)
                {
                    string customerID = Convert.ToString(listBox1.GetItemText(listBox1.SelectedItem));
                    customerID = customerID.Split(':')[0];
                    customerID.Replace(":", "");
                    insert_History(statnum, bikenum, Convert.ToInt32(customerID));
                    listState = false;
                    state = 0;
                }
                if(state == 5)
                {
                    string bikenum = listBox1.GetItemText(listBox1.SelectedItem);
                    retbike = Convert.ToInt32(bikenum);
                    ShowStations();
                    state = 6;
                    return;
                }
                if(state == 6)
                {
                    retstat = station;
                    Return_Bike(retbike, retstat);
                    listState = false;
                    state = 0;
                    
                }
            }
        }

        //Clicking customer button shows customers
        private void Customers_Click(object sender, EventArgs e)
        {
            ShowCustomers();
        }

        //search by email
        private void button2_Click(object sender, EventArgs e)
        {
            string entry = textBox1.Text;
            string filename, connectinfo, sql;
            SqlConnection db;
            SqlCommand comm = new SqlCommand();

            filename = "DIVVY.mdf";

            connectinfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;
                AttachDBFilename=|DataDirectory|\{0};Integrated Security=True;",
                filename);
            db = new SqlConnection(connectinfo);
            db.Open();

            comm.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            DataSet ds = new DataSet();

            sql = String.Format(@"
                    SELECT  Customers.CustomerID, Customers.FirstName, Customers.LastName, History.BikeID, History.Checkout, History.StationIDout, History.Checkin, History.StationIDin
                    FROM    Customers, History
                    WHERE   Email = '{0}' AND Customers.CustomerID = History.CustomerID
                    Order BY    History.Checkin DESC
                    ", entry);
            comm.CommandText = sql;
            adapter.Fill(ds);

           
            int i = 0;
            listBox1.Items.Clear();
            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                if (i == 0)
                {
                    listBox1.Items.Add(String.Format("For Email: {0}", entry));
                    string name = String.Format(@"Customer Name: {0} {1}",      
                        Convert.ToString(row["FirstName"]),
                        Convert.ToString(row["LastName"])
                    );
                    
                    listBox1.Items.Add(name);
                    listBox1.Items.Add(string.Format("{0,10}{1,40}{2,42}{3,48}{4,42}", "BikeID", "Check Out", "Station Out", "Check In", "Station In"));
                }

                string msg = String.Format(@"{0,10}{1,40}{2,40}{3,40}{4,40}",
                    Convert.ToInt32(row["BikeID"]),
                    Convert.ToString(row["Checkout"]),
                    Convert.ToInt32(row["StationIDout"]),
                    Convert.ToString(row["Checkin"]),
                    Convert.ToInt32(row["StationIDin"])
                );
                this.listBox1.Items.Add(msg);
                i = 1;
            }

            listState = false;

            db.Close();
          
        }

        //check out a bike
        private void button3_Click(object sender, EventArgs e)
        {
            state = 2;
            ShowStations();
            state = 2;
            listState = true;
            
        }

        //displays all bikes that have not been checked in
        private void Display_Out()
        {
            listBox1.Items.Clear();
            string filename, connectinfo, sql;
            SqlConnection db;
            SqlCommand comm = new SqlCommand();

            filename = "DIVVY.mdf";

            connectinfo = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;
                AttachDBFilename=|DataDirectory|\{0};Integrated Security=True;",
                filename);
            db = new SqlConnection(connectinfo);
            db.Open();

            comm.Connection = db;
            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            DataSet ds = new DataSet();

            sql = String.Format(@"
                    SELECT  BikeID
                    From    History
                    Where   StationIDin IS NULL
                    ");
            comm.CommandText = sql;
            adapter.Fill(ds);
            listBox1.Items.Add("Choose Which Bike is Being Returned");
            listBox1.Items.Add("");
            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                listBox1.Items.Add(String.Format("{0}", Convert.ToInt32(row["BikeID"])));
            }
            db.Close();

        }

        //starts check in process
        private void button4_Click(object sender, EventArgs e)
        {

            Display_Out();
            state = 5;
            listState = true;
        }
    }
}
