using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ConnectToNotepad
{
    public class NotepadAccess
    {
        private SqlConnection connection = null;
        private SqlDataAdapter dataAdapter = null;

        #region Connection
        public void OpenConnnection(string connectString)
        {
            connection = new SqlConnection(connectString);
            AdapterSelectAll(out dataAdapter);
            connection.Open();
        }

        public void CloseConnection()
        {
            connection.Close();
        }
        #endregion

        #region SelectAll
        // private method to set SqlDataAdapter and access all records
        private void AdapterSelectAll(out SqlDataAdapter dAdapter)
        {
            dAdapter = new SqlDataAdapter("SELECT REC_LIST, REC_CONTENT FROM Records", connection);
            SqlCommandBuilder builder = new SqlCommandBuilder(dAdapter);
        }

        public DataSet DisplayAllRecords(DataSet dataSet, string tableName)
        {
            dataAdapter.Fill(dataSet, tableName);
            return dataSet;
        }
        #endregion

        #region Select_REC_LIST
        private void AdapterSelectRecordList(out SqlDataAdapter dAdapterRecList)
        {
            dAdapterRecList = new SqlDataAdapter("SELECT REC_LIST FROM Records", connection);
            SqlCommandBuilder builder = new SqlCommandBuilder(dAdapterRecList);
        }

        public string ShowDBTitle(string title)
        {
            DataTable table = new DataTable();
            string sqlSelectTitle = string.Format("SELECT REC_LIST FROM Records WHERE REC_LIST = '{0}'", title);
            string connString = @"Data Source=ROMAKHYPC\SQLEXPRESS;Initial Catalog=Notepad;Integrated Security=True";

            connection = new SqlConnection(connString);
            connection.Open();

            using (SqlCommand cmd = new SqlCommand(sqlSelectTitle, connection))
            {
                SqlDataReader dReader = cmd.ExecuteReader();
                table.Load(dReader);
                dReader.Close();
            }
            return table.Rows[0]["REC_LIST"].ToString();
        }
        #endregion

        #region InsertRecord
        public void InsertRecord(string title, string content)
        {
            string sqlInsert = string.Format("INSERT INTO Records (REC_LIST, REC_CONTENT) VALUES (@REC_LIST, @REC_CONTENT)");

            using (SqlCommand command = new SqlCommand(sqlInsert, connection))
            {
                command.Parameters.AddWithValue("@REC_LIST", title);
                command.Parameters.AddWithValue("@REC_CONTENT", content);

                command.ExecuteNonQuery();
            }
        }
        #endregion      

        #region Select_REC_CONTENT
        public string ShowDBContent(string content) 
        {
            DataTable table = new DataTable();
            string sqlSelectContent = string.Format("SELECT REC_CONTENT FROM Records WHERE REC_CONTENT = '{0}'", content);
            string connString = @"Data Source=ROMAKHYPC\SQLEXPRESS;Initial Catalog=Notepad;Integrated Security=True";

            connection = new SqlConnection(connString);
            connection.Open();

            using (SqlCommand cmd = new SqlCommand(sqlSelectContent, connection))
            {
                SqlDataReader dReader = cmd.ExecuteReader();
                table.Load(dReader);
                dReader.Close();
            }
            return table.Rows[0]["REC_CONTENT"].ToString();
        }
        #endregion

        public DataSet RefreshRecordsDB(DataSet dataSet, string tableName)
        {
            dataSet.Tables[0].Clear();
            DisplayAllRecords(dataSet, tableName);
            return dataSet;
        }

        #region UpdateRecord
        public void UpdateRecords(string editTitle, string editContent)
        {
            string sqlUpdate = string.Format("UPDATE Records SET REC_LIST = '{0}', REC_CONTENT = '{1}' WHERE REC_LIST = '{2}'",
                editTitle, editContent, editTitle);

            using(SqlCommand cmd = new SqlCommand(sqlUpdate, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region DeleteRecord
        public void DeleteRecord(string title)
        {
            string sqlDelete = string.Format("DELETE FROM Records WHERE REC_LIST = '{0}'", title);

            using(SqlCommand cmd = new SqlCommand(sqlDelete, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
        #endregion
    }
}
