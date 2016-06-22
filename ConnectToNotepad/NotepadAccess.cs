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
            dAdapter = new SqlDataAdapter("SELECT REC_ID, REC_LIST, REC_CONTENT FROM Records", connection);
            SqlCommandBuilder builder = new SqlCommandBuilder(dAdapter);
        }

        public DataSet DisplayAllRecords(DataSet dataSet, string tableName)
        {
            dataAdapter.Fill(dataSet, tableName);
            return dataSet;
        }
        #endregion

        #region SelectCurrentRecord
        public string ShowDBRecord(string recordID, string currentRecord)
        {
            DataTable table = new DataTable();
            SelectRecord selectRecord = new SelectRecord();

            selectRecord.Title = string.Format("SELECT REC_LIST FROM Records WHERE REC_ID = '{0}'", recordID);
            selectRecord.Content = string.Format("SELECT REC_CONTENT FROM Records WHERE REC_ID = '{0}'", recordID);

            connection.Open();
            using (SqlCommand cmd = new SqlCommand(selectRecord.Title, connection))
            {
                SqlDataReader dReader = cmd.ExecuteReader();
                table.Load(dReader);
                dReader.Close();
            }

            using (SqlCommand cmd = new SqlCommand(selectRecord.Content, connection))
            {
                SqlDataReader dReader = cmd.ExecuteReader();
                table.Load(dReader);
                dReader.Close();
            }
            connection.Close();

            if (currentRecord == table.Rows[0]["REC_LIST"].ToString())
                return table.Rows[0]["REC_LIST"].ToString();
            if (currentRecord == table.Rows[1]["REC_CONTENT"].ToString())
                return table.Rows[1]["REC_CONTENT"].ToString();
            else
                return null;
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

        #region RefreshRecordsDB
        public DataSet RefreshRecordsDB(DataSet dataSet, string tableName)
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

        struct SelectRecord
        {
            public string Title{get;set;}
            public string Content{get;set;}
        }
    }
}
