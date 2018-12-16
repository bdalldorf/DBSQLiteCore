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
            DataTable DataTable = SQLiteDBStateless.ExecDataTable($"SELECT * FROM usrUser_user WHERE usrID = {id}");

            if (DataTable.Rows.Count == 1)
            {
                LoadByUserModelDataRow(DataTable.Rows[0]);
            }
        }

        private void LoadByUserModelDataRow(DataRow dataRow)
        {
            this.ID = SQLiteDBCommon.GetValueIntFromSql(dataRow[this.ID.TableField()]);
            this.UID = SQLiteDBCommon.GetValueStringFromSql(dataRow[this.UID.TableField()]);
            this.FirstName = SQLiteDBCommon.GetValueStringFromSql(dataRow[this.FirstName.TableField()]);
            this.LastName = SQLiteDBCommon.GetValueStringFromSql(dataRow[this.LastName.TableField()]);
            this.EmailAddress = SQLiteDBCommon.GetValueStringFromSql(dataRow[this.EmailAddress.TableField()]);
        }

        #endregion

        #region Additional Methods

        private string ModelFields
        {
            get { return SQLiteDBStateless.ModelFieldNames(typeof(UserModel)); }
        }

        private string ModelValues
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
            this.ID = (int)DBSqlite.SQLiteDBStateless.
                ExecInsertNonQueryReturnID($"INSERT INTO {this.TableName()} {SQLiteDBStateless.GenerateInsertFields(this)}");
        }

        private void Update()
        {
            DBSqlite.SQLiteDBStateless.ExecNonQuery($"UPDATE {this.TableName()} {SQLiteDBStateless.GenerateUpdateFields(this)} WHERE usrID = {this.ID}");
        }

        private void Delete()
        {
            DBSqlite.SQLiteDBStateless.
                ExecInsertNonQueryReturnID($"DELETE FROM {this.TableName()} WHERE usrID = {this.ID}");
        }

        #endregion
    }
}
