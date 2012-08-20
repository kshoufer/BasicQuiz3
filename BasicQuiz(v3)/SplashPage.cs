using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BasicQuiz_v3_
{
    public partial class SplashPage : Form
    {
        bool bolMatch = false;
        int intUserID;
        string strUserName;
        string strUserPassword;
        
        public SplashPage()
        {
            InitializeComponent();
        }

        private void SplashPage_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReadUserProfileTable();
            
            if (!bolMatch)
            {
                MessageBox.Show("The User Name and/or Passord Does Not Match" + "\n" + "Try Again");
                txtPassword.Text = "";
                txtUserName.Text = "";
                txtUserName.Focus();
            }
        }

        private void ReadUserProfileTable()
        {
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            conn.ConnectionString = @"Data Source=KEN-HP\SQLSERVER2008R2;Initial Catalog=quiz1;Integrated Security=True";

            try
            {
                conn.Open();

                SqlCommand command = new SqlCommand(
                    "SELECT * FROM UserProfile;",
                    conn);

                SqlDataReader reader = command.ExecuteReader();


                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        strUserName = (string)reader[1];
                        strUserPassword = (string)reader[2];
                        if (strUserName.ToString() == txtUserName.Text && strUserPassword == txtPassword.Text)
                        {
                            bolMatch = true;
                            intUserID = (int)reader[0];
                            QuestionPage qp = new QuestionPage(intUserID);
                            qp.Visible = true;
                            this.Hide();
                        }
                    }// end while
                }
                else
                {
                    Console.WriteLine("No rows found");
                }// end if (reader.HasRows)

                reader.Close();

            }// end try

            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to UserProfile");
            }
            finally
            {
                conn.Close();
            }

            
        } // end ReadUserProfileTable

    }
}
