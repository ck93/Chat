using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Data;

namespace Chat
{
    class mysql
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        private string port;

        public void Initialize(string server, string database, string uid, string password, string port)
        { 
            this.server = server;
            this.uid = uid;
            this.password = password;
            this.port = port;
            this.database = database;
            string connectionString = "Data Source=" + server + ";" + "port=" + port + ";" + "Database=" + database + ";" + "User Id=" + uid + ";" + "Password=" + password + ";" + "CharSet = utf8"; ;
            connection = new MySqlConnection(connectionString);            
        }

        //open connection to database  
        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based on the error number.  
                //The two most common error numbers when connecting are as follows:  
                //0: Cannot connect to server.  
                //1045: Invalid user name and/or password.  
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        //Close connection  
        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public DataTable GetSchema(string str, string[] restri)
        {
            return connection.GetSchema(str, restri);
        }
        public DataTable GetSchema(string str)
        {
            return connection.GetSchema(str);
        }
        // Get Database List  

        //Insert statement  
        public void Insert(string name, string password, string status, string IP, int port)
        {
            string query = "INSERT INTO user VALUES('"+name+"','"+password+"','"+status+"','"+IP+"','"+port+"','0')";

            //open connection  
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor  
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Execute command  
                cmd.ExecuteNonQuery();
                //close connection  
                this.CloseConnection();
                MessageBox.Show("注册成功！");
            }
            else
                MessageBox.Show("连接服务器失败！");
        }

        //Update statement  
        public void Update(string query)
        {
            //string query = "UPDATE user SET name='Joe', age='22' WHERE name='John Smith'";

            //Open connection  
            if (this.OpenConnection() == true)
            {
                //create mysql command  
                MySqlCommand cmd = new MySqlCommand();
                //Assign the query using CommandText  
                cmd.CommandText = query;
                //Assign the connection using Connection  
                cmd.Connection = connection;
                //Execute query  
                cmd.ExecuteNonQuery();
                //close connection  
                this.CloseConnection();
            }
        }

        //Delete statement  
        public void Delete(string name)
        {
            string query = "DELETE FROM user WHERE name='"+ name +"'";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        //Select statement  
        public string[] Select(string name)
        {
            string query = "SELECT * FROM user where name = '" + name + "'";
            //Create a list to store the result  
            string[] list = new string[6];
            if (this.OpenConnection() == true)
            {
                //Create Command  
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command  
                MySqlDataReader dataReader = cmd.ExecuteReader();
                //Read the data and store them in the list  
                while (dataReader.Read())
                {
                    list[0] = dataReader["name"].ToString();
                    list[1] = dataReader["password"].ToString();
                    list[2] = dataReader["status"].ToString();
                    list[3] = dataReader["IP"].ToString();
                    list[4] = dataReader["port"].ToString();
                    list[5] = dataReader["onlinetime"].ToString();
                }
                //close Data Reader  
                dataReader.Close();
                //close Connection  
                this.CloseConnection();
                //return list to be displayed  
                return list;
            }
            else
            {
                MessageBox.Show("数据库连接失败！");
                return list;
            }
        }

        public List<string>[] SelectAll()
        {
            string query = "SELECT * FROM user";

            //Create a list to store the result  
            List<string>[] list = new List<string>[4];
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();
            list[3] = new List<string>();

            //Open connection  
            if (this.OpenConnection() == true)
            {
                //Create Command  
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command  
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list  
                while (dataReader.Read())
                {
                    list[0].Add(dataReader["name"] + "");
                    list[1].Add(dataReader["status"] + "");
                    list[2].Add(dataReader["IP"] + "");
                    list[3].Add(dataReader["port"] + "");
                }
                //close Data Reader  
                dataReader.Close();
                //close Connection  
                this.CloseConnection();
                //return list to be displayed  
                return list;
            }
            else
            {
                return list;
            }
        }
        //Count statement  
        public int Count(string name)
        {
            string query = "SELECT Count(*) FROM user where name ='" + name + "'"; ;
            int Count = -1;
            if (this.OpenConnection() == true)
            {
                //MessageBox.Show(connection.ServerVersion + "|" + connection.State + "|" + connection.ServerThread);
                //Create Mysql Command  
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //ExecuteScalar will return one value  
                Count = int.Parse(cmd.ExecuteScalar() + "");
                //close Connection  
                this.CloseConnection();
                return Count;
            }
            else
            {
                //MessageBox.Show(connection.ServerVersion + "|" + connection.State);
                MessageBox.Show("数据库连接失败！");
                return Count;
            }
        }

        
    }
}
