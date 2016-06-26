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
        private SqlConnection connection = new SqlConnection(@"Data Source=ROMAKHYPC\SQLEXPRESS;Initial Catalog=Notepad;Integrated Security=True");
        private SqlDataAdapter dataAdapter = null;

        #region Connection
        public void OpenConnnection()
        {
            connection.Open();
            GetAllRecords();
        }

        public void CloseConnection()
        {
            connection.Close();
        }
        #endregion

        #region GetAllRecords
        private void GetAllRecords()
        {
            dataAdapter = new SqlDataAdapter("SELECT REC_ID, REC_LIST, REC_CONTENT FROM Records", connection);
        }

        public DataSet DisplayAllRecords(DataSet dataSet, string tableName)
        {
            dataAdapter.Fill(dataSet, tableName);
            return dataSet;
        }
        #endregion

        #region GetCurrentRecord
        public string GetCurrentRecord(string recordID, string field)
        {
            DataTable table = new DataTable();
            String query = null;

            if (field == "title")
                query = string.Format("SELECT REC_LIST FROM Records WHERE REC_ID = '{0}'", recordID);
            if (field == "content")
                query = string.Format("SELECT REC_CONTENT FROM Records WHERE REC_ID = '{0}'", recordID);

            connection.Open();
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                SqlDataReader dReader = cmd.ExecuteReader();
                table.Load(dReader);
                dReader.Close();
            }
            connection.Close();

            return table.Rows[0][0].ToString();
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

        #region RefreshRecords
        public DataSet RefreshRecords(DataSet dataSet, string tableName)
        {
            dataSet.Tables[0].Clear();
            DisplayAllRecords(dataSet, tableName);
            return dataSet;
        }
        #endregion

        #region UpdateRecord
        public void UpdateRecords(string editTitle, string editContent, string id)
        {
            string sqlUpdate = string.Format("UPDATE Records SET REC_LIST = '{0}', REC_CONTENT = '{1}' WHERE REC_ID = '{2}'",
                editTitle, editContent, id);

            using(SqlCommand cmd = new SqlCommand(sqlUpdate, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region DeleteRecord
        public void DeleteRecord(string id)
        {
            string sqlDelete = string.Format("DELETE FROM Records WHERE REC_ID = '{0}'", id);

            using(SqlCommand cmd = new SqlCommand(sqlDelete, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
        #endregion
    }
}
