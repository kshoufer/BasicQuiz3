using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;


namespace BasicQuiz_v3_
{
    public partial class QuestionPage : Form
    {
        //Lists to temporarily store the database information.
        Question QuestionRow = new Question();
        List<Question> QuestionRowList = new List<Question>();
        UserScore UserScoreRow = new UserScore();
        List<UserScore> UserScoreRowList = new List<UserScore>();
        // User ID and Name hard coded for this demonstration
        int intUserID;
        int intUserAnswerID;
        int intQuestionID;
        string strUserAnwserDescription;
        int intCorrectAnswerID;
        string strCorrectAnswerDescription;
        int intUserQuestionScore;
        int intRowListCount;
        int intTestQuestionNumber = 1;
        int intTotalScore = 0;
        string strQuestionDescription;

        public QuestionPage(int _intUserID)
        {
            InitializeComponent();
            intUserID = _intUserID;
            ClearUserScoreTable();
            ReadQuestionTable();

            foreach (Question objQuestion in QuestionRowList)
            {
                if (objQuestion.intQuestionID == intTestQuestionNumber)
                {
                    lblQuestion.Text = objQuestion.strQuestionDescription;
                    intQuestionID = objQuestion.intQuestionID;
                    strQuestionDescription = objQuestion.strQuestionDescription;
                    radioButton1.Text = objQuestion.strAnswer1Description;
                    radioButton2.Text = objQuestion.strAnswer2Description;
                    radioButton3.Text = objQuestion.strAnswer3Description;
                    intCorrectAnswerID = objQuestion.intCorrectAnswerID;
                    strCorrectAnswerDescription = objQuestion.strCorrectAnswerDescription;

                }
            }// end foreach

            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;

        }// end QuestionPage()


        private void btnNext_Click(object sender, EventArgs e)
        {

            if (radioButton1.Checked)
            {
                intUserAnswerID = 1;
            }
            if (radioButton2.Checked)
            {
                intUserAnswerID = 2;
            }
            if (radioButton3.Checked)
            {
                intUserAnswerID = 3;
            }

            foreach (Question objQuestion in QuestionRowList)
            {
                if (objQuestion.intQuestionID == intTestQuestionNumber)
                {
                    if (objQuestion.intCorrectAnswerID == intUserAnswerID)
                    {
                        lblResult.Text = "Your answer is correct!";
                        intTotalScore++;

                        intUserQuestionScore = 1;
                        lblResult.Visible = true;
                    }
                    else
                    {
                        lblResult.Text = "Your answer is NOT correct.";
                        intUserQuestionScore = 0;
                        lblResult.Visible = true;
                    }

                    switch (intUserAnswerID)
                    {
                        case 1:
                            strUserAnwserDescription = radioButton1.Text;
                            break;
                        case 2:
                            strUserAnwserDescription = radioButton2.Text;
                            break;
                        case 3:
                            strUserAnwserDescription = radioButton3.Text;
                            break;
                    }

                }// end if 

            }// end foreach

            /////////////////////////////////Begin Insert//////////////////////////////////////////////////////
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            conn.ConnectionString = @"Data Source=KEN-HP\SQLSERVER2008R2;Initial Catalog=quiz1;Integrated Security=True";

            try
            {
                conn.Open();

                SqlCommand command = new SqlCommand(
                    "INSERT INTO UserScore (UserID, QuestionID, QuestionDescription, UserAnswerID, UserAnswerDescription, CorrectAnswerID, " +
                        "CorrectAnswerDescription, UserQuestionScore)" + " VALUES (" + intUserID + ", '" + intQuestionID
                        + "', " + "'" + strQuestionDescription + "', " + intUserAnswerID + ", '"
                        + strUserAnwserDescription + "', " + intCorrectAnswerID + ",'" + strCorrectAnswerDescription + "', "
                        + intUserQuestionScore + ")", conn);

                command.ExecuteNonQuery();
            }// end try

            catch (Exception ex)
            {
                MessageBox.Show("Failed to inser to UserScore");
            }
            finally
            {
                conn.Close();
            }
            //////////////////////////////////End Insert////////////////////////////////////////////////

            ReadUserScoreTable();
            dataGridView1.Rows.Clear();

            //send rows to output            
            for (int i = 0; i < intRowListCount; ++i)
            {
                string[] rowItems = new string[5] { 
                    UserScoreRowList[i].intUserID.ToString(),
                    UserScoreRowList[i].intQuestionID.ToString(),
                    UserScoreRowList[i].strUserAnswerDescription,
                    UserScoreRowList[i].strCorrectAnswerDescription,
                    UserScoreRowList[i].intUserQuestionScore.ToString()
                    };
                dataGridView1.Rows.Add(rowItems);
            }

            intTestQuestionNumber++;

            double dblAverageScore;

            if (intTestQuestionNumber > QuestionRowList.Count)
            {
                lblScore.Text = "Your total score is: " + intTotalScore;
                dblAverageScore = (double)intTotalScore / QuestionRowList.Count;
                dblAverageScore = dblAverageScore * 100;
                lblAverageScore.Text = "Your average score is: " + (int)dblAverageScore + "%";
                lblAverageScore.Visible = true;
                lblScore.Visible = true;
                btnNext.Visible = false;
                btnExit.Visible = true;
                lblQuestion.Visible = false;
                radioButton1.Visible = false;
                radioButton2.Visible = false;
                radioButton3.Visible = false;
                
                return;
            }

            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;

            foreach (Question objQuestion in QuestionRowList)
            {
                if (objQuestion.intQuestionID == intTestQuestionNumber)
                {
                    lblQuestion.Text = objQuestion.strQuestionDescription;
                    strQuestionDescription = objQuestion.strQuestionDescription;
                    radioButton1.Text = objQuestion.strAnswer1Description;
                    radioButton2.Text = objQuestion.strAnswer2Description;
                    radioButton3.Text = objQuestion.strAnswer3Description;
                    intCorrectAnswerID = objQuestion.intCorrectAnswerID;
                    strCorrectAnswerDescription = objQuestion.strCorrectAnswerDescription;
                }
            }// end foreach


        } // end btnNext_Click

        private void ReadUserScoreTable()
        {
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            conn.ConnectionString = @"Data Source=KEN-HP\SQLSERVER2008R2;Initial Catalog=quiz1;Integrated Security=True";

            try
            {
                conn.Open();

                SqlCommand command = new SqlCommand(
                    "SELECT * FROM UserScore;",
                    conn);

                SqlDataReader reader = command.ExecuteReader();

                intRowListCount = 0;
                UserScoreRowList.Clear();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        UserScore ur = new UserScore();
                        ur.intRowID = (int)reader[0];
                        ur.intUserID = (int)reader[1];
                        ur.intQuestionID = (int)reader[2];
                        ur.strQuestionDescription = (string)reader[3];
                        ur.intUserAnswerID = (int)reader[4];
                        ur.strUserAnswerDescription = (string)reader[5];
                        ur.intCorrectAnswerID = (int)reader[6];
                        ur.strCorrectAnswerDescription = (string)reader[7];
                        ur.intUserQuestionScore = (int)reader[8];
                        UserScoreRowList.Add(ur);
                        intRowListCount++;
                    }
                }// end if (reader.HasRows)
                else
                {
                    Console.WriteLine("No rows found.");
                }

                reader.Close();

            }// end try

            catch (Exception ex)
            {
                MessageBox.Show("Failed to read user score");
            }
            finally
            {
                conn.Close();
            }
        } // end ReadUserScoreTable

        private void ReadQuestionTable()
        {
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            conn.ConnectionString = @"Data Source=KEN-HP\SQLSERVER2008R2;Initial Catalog=quiz1;Integrated Security=True";

            try
            {
                conn.Open();

                SqlCommand command = new SqlCommand(
                    "SELECT * FROM Question;",
                    conn);

                SqlDataReader reader = command.ExecuteReader();

                intRowListCount = 0;

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Question qr = new Question();
                        qr.intQuestionID = (int)reader[0];
                        qr.strQuestionDescription = (string)reader[1];
                        qr.intAnswer1ID = (int)reader[2];
                        qr.strAnswer1Description = (string)reader[3];
                        qr.intAnswer2ID = (int)reader[4];
                        qr.strAnswer2Description = (string)reader[5];
                        qr.intAnswer3ID = (int)reader[6];
                        qr.strAnswer3Description = (string)reader[7];
                        qr.intCorrectAnswerID = (int)reader[8];
                        qr.strCorrectAnswerDescription = (string)reader[9];
                        QuestionRowList.Add(qr);
                        intRowListCount++;
                    }
                }// end if (reader.HasRows)
                else
                {
                    Console.WriteLine("No rows found.");
                }

                reader.Close();

            }// end try

            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to data source");
            }
            finally
            {
                conn.Close();
            }
        } // end ReadQuestionTable

        private void ClearUserScoreTable()
        {
            System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection();
            conn.ConnectionString = @"Data Source=KEN-HP\SQLSERVER2008R2;Initial Catalog=quiz1;Asynchronous Processing=True;Integrated Security=True";

            try
            {
                conn.Open();

                SqlCommand command = new SqlCommand(
                    "DELETE FROM UserScore;",
                    conn);

                command.ExecuteNonQuery();

            }// end try

            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to data source");
            }
            finally
            {
                conn.Close();
            }
        } // end DeleteQuestionTable

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }// end class QuestionPage
}// end namespace BasicQuiz_v2_
