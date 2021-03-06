﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GoWareWMS
{
    public partial class LogInForm : Form
    {
        private Client client;
        private Manager manager;
        private DBConnect db_connect;
        /// <summary>
        /// Main form when running the model
        /// </summary>
        public LogInForm()
        {
            InitializeComponent();
            db_connect = new DBConnect();
            client = new Client();
            manager = new Manager();
        }
        /// <summary>
        /// When the client log in button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_client_login_Click(object sender, EventArgs e)
        {
            client.Username = textBox_username_client.Text;
            client.Password = textBox_pwd_client.Text;
            bool logInSucc = false;
            // Check the username
            if (client.Username == "")
            {
                MessageBox.Show("Please enter the Username.");
                logInSucc = false;
            } 
            else if (! checkText(client.Username))
            {
                MessageBox.Show("Invalid Username.");
                logInSucc = false;
            }
            // Check the password
            else if (client.Password == "")
            {
                MessageBox.Show("Please enter the Password.");
                logInSucc = false;
            }
            else {
                if (db_connect.OpenConnection())
                {
                    string mysql_cmd = "SELECT * FROM client " +
                          "WHERE username = @name " +
                          "AND password = @pwd;";
                    MySqlCommand cmd = new MySqlCommand(mysql_cmd, db_connect.Connection);
                    cmd.Parameters.AddWithValue("@name", client.Username);
                    cmd.Parameters.AddWithValue("@pwd", client.Password);
                    MySqlDataReader dataReader = cmd.ExecuteReader();
                    // If the client can be found in the database, log in succeed
                    while (dataReader.Read())
                    {
                        if (dataReader["username"].ToString() == client.Username
                            && dataReader["password"].ToString() == client.Password)
                        {
                            client.ID = dataReader["id_client"].ToString();
                            client.Type = dataReader["usertype"].ToString();
                            client.Email = dataReader["email"].ToString();
                            if (client.Type == "personal")
                            {
                                client.Firstname = dataReader["firstname"].ToString();
                                client.Middlename = dataReader["middlename"].ToString();
                                client.Lastname = dataReader["lastname"].ToString();
                                client.Sex = dataReader["sex"].ToString();
                            }
                            else if (client.Type == "corporate")
                            {
                                client.Companyname = dataReader["companyname"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Client type error!");
                                this.Close();
                            }
                            logInSucc = true;
                            break;
                        }
                        else
                        {
                            MessageBox.Show("DB Error!");
                        }
                    }
                    if (!logInSucc)
                    {
                        MessageBox.Show("The Username or the Password is incorrect");
                        textBox_pwd_client.Clear();
                    }
                }
                else
                {
                    logInSucc = false;
                    MessageBox.Show(db_connect.Message);
                }
            }
            // If login succeed, switch to the client main form
            if (logInSucc)
            {
                switchToClientMainForm();
            }
            if (!db_connect.CloseConnection())
            {
                MessageBox.Show(db_connect.Message);
            }
        }
        /// <summary>
        /// Open the client form and hide the login form
        /// </summary>
        private void switchToClientMainForm()
        {
            ClientMainForm clientMainForm = new ClientMainForm(client);
            clientMainForm.logInForm = this;
            clientMainForm.Show();
            this.Hide();
        }
        /// <summary>
        /// When the manager log in button is clickec
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_manager_login_Click(object sender, EventArgs e)
        {
            manager.Username = textBox_username_manager.Text;
            manager.Password = textBox_pwd_manager.Text;
            bool logInSucc = false;
            // Check the username
            if (manager.Username == "")
            {
                MessageBox.Show("Please enter the Username.");
                logInSucc = false;
            }
            else if (!checkText(manager.Username))
            {
                MessageBox.Show("Invalid Username.");
                logInSucc = false;
            }
            // Check the password
            else if (manager.Password == "")
            {
                MessageBox.Show("Please enter the Password.");
                logInSucc = false;
            }
            else
            {
                if (db_connect.OpenConnection())
                {
                    string mysql_cmd = "SELECT * FROM client " +
                          "WHERE username = @name " +
                          "AND password = @pwd;";
                    MySqlCommand cmd = new MySqlCommand(mysql_cmd, db_connect.Connection);
                    cmd.Parameters.AddWithValue("@name", manager.Username);
                    cmd.Parameters.AddWithValue("@pwd", manager.Password);
                    MySqlDataReader dataReader = cmd.ExecuteReader();
                    while (dataReader.Read())
                    {
                        // If manager can be found in the database
                        if (dataReader["username"].ToString() == manager.Username
                            && dataReader["password"].ToString() == manager.Password)
                        {
                            manager.ID = dataReader["id_manager"].ToString();
                            manager.Email = dataReader["email"].ToString();
                            manager.Firstname = dataReader["firstname"].ToString();
                            manager.Middlename = dataReader["middlename"].ToString();
                            manager.Lastname = dataReader["lastname"].ToString();
                            manager.Sex = dataReader["sex"].ToString();
                            logInSucc = true;
                            break;
                        }
                        else
                        {
                            MessageBox.Show("DB Error!");
                        }
                    }
                    if (!logInSucc)
                    {
                        MessageBox.Show("The Username or the Password is incorrect");
                        textBox_pwd_manager.Clear();
                    }
                }
                else
                {
                    logInSucc = false;
                    MessageBox.Show(db_connect.Message);
                }
            }
            // If login succeed, switch to the manager main form
            if (logInSucc)
            {
                switchToManagerMainForm();
            }
            if (!db_connect.CloseConnection())
            {
                MessageBox.Show(db_connect.Message);
            }
        }
        /// <summary>
        /// Open the manager form and hide the login form
        /// </summary>
        public void switchToManagerMainForm()
        {
            ManagerMainForm managerMainForm = new ManagerMainForm(manager);
            managerMainForm.logInForm = this;
            managerMainForm.Show();
            this.Hide();
        }
        /// <summary>
        /// Check the validity of the text input
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool checkText(string text)
        {
            foreach (char c in text)
            {
                int n = (int)c;
                if (!((n >= 48 && n <= 57) ||
                    (n >= 65 && n <= 90) ||
                    (n >= 97 && n <= 122)))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// When the tab is changed between client login tab and manager login tab, refresh the textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl.SelectedIndex)
            {
                case 0:
                    textBox_pwd_client.Clear();
                    textBox_username_client.Clear();
                    break;
                case 1:
                    textBox_pwd_manager.Clear();
                    textBox_username_manager.Clear();
                    break;
            }
        }
        /// <summary>
        /// When client register button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_client_register_Click(object sender, EventArgs e)
        {
            ClientRegisterForm clientRegisterForm = new ClientRegisterForm();
            clientRegisterForm.Show();
        }
        /// <summary>
        /// When manager register button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_manager_register_Click(object sender, EventArgs e)
        {
            ManagerRegisterForm managerRegisterForm = new ManagerRegisterForm();
            managerRegisterForm.Show();
        }
    }
}
