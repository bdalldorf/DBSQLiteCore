using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;

namespace DBSqlite.Models
{
    [TableName("usrUser_usr")]
    public partial class UserModel : IDatabaseModel
    {
        #region private properties

        #endregion

        #region public properties

        [TableFieldName("usrID")]
        [TableFieldExcludeFromUpdate(true)]
        [TableFieldExcludeFromInsert(true)]
        public int ID;
        [TableFieldName("usrUID")]
        public string UID;
        [TableFieldName("usrFirstName")]
        public string FirstName;
        [TableFieldName("usrLastName")]
        public string LastName;
        [TableFieldName("usrEmailAddress")]
        public string EmailAddress;

        #endregion

        #region Constructors

        public UserModel() { }

        public UserModel(int id)
        {
            DataTable DataTable = SQLiteDBStateless.ExecDataTable($"SELECT * FROM {this.TableName()} WHERE usrID = {id}");

            if (DataTable.Rows.Count == 1)
            {
                LoadByUserModelDataRow(DataTable.Rows[0]);
            }
        }

        private void LoadByUserModelDataRow(DataRow dataRow)
        {
            this.ID = SQLiteDBCommon.GetValueIntFromSql(GetDatabaseTableFieldName(dataRow, nameof(this.ID)));
            this.UID = SQLiteDBCommon.GetValueStringFromSql(GetDatabaseTableFieldName(dataRow, nameof(this.UID))); ;
            this.FirstName = SQLiteDBCommon.GetValueStringFromSql(GetDatabaseTableFieldName(dataRow, nameof(this.FirstName)));
            this.LastName = SQLiteDBCommon.GetValueStringFromSql(GetDatabaseTableFieldName(dataRow, nameof(this.LastName)));
            this.EmailAddress = SQLiteDBCommon.GetValueStringFromSql(GetDatabaseTableFieldName(dataRow, nameof(this.EmailAddress)));
        }

        private object GetDatabaseTableFieldName(DataRow dataRow, string fieldName) => dataRow[SQLiteDBStateless.GetDatabaseTableFieldName(this, fieldName)];


        #endregion

        #region Additional Methods

        private List<string> ModelFields
        {
            get { return SQLiteDBStateless.ModelFieldNames(typeof(UserModel)); }
        }

        private List<object> ModelValues
        {
            get { return SQLiteDBStateless.ModelFieldValues(this); }
        }

        #endregion

        #region CRUD Methods

        public void Save()
        {
            if (this.ID.IsEmpty())
                Insert();
            else
                Update();

        }

        private void Insert()
        {
            this.ID = (int)SQLiteDBStateless.ExecInsertNonQueryReturnID(SQLiteDBStateless.GenerateStandardInsertStatement(this));
        }

        private void Update()
        {
            SQLiteDBStateless.ExecNonQuery(SQLiteDBStateless.GenerateStandardUpdateStatement(this, nameof(this.ID), this.ID));
        }

        public void Delete()
        {
            SQLiteDBStateless.ExecNonQuery(SQLiteDBStateless.GenerateStandardDeleteStatement(this, nameof(this.ID), this.ID));
        }

        #endregion
    }
}
