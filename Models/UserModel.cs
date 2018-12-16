using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;

namespace DBSqlite.Models
{
    public partial class UserModel
    {
        #region private properties

        #endregion

        #region public properties

        [TableFieldName("usrID")]
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
            get { return SQLiteDBStateless.ModelTableFieldNames(typeof(UserModel)); }
        }

        private string ModelValues
        {
            get
            {
                StringBuilder l_StringBuilder = new StringBuilder();

                l_StringBuilder.Append($"{this.ID}");
                l_StringBuilder.Append($",{this.UID}");
                l_StringBuilder.Append($",{this.FirstName}");
                l_StringBuilder.Append($",{this.LastName}");
                l_StringBuilder.Append($",{this.EmailAddress}");

                return l_StringBuilder.ToString();
            }
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
            StringBuilder l_StringBuilder = new StringBuilder();

            l_StringBuilder.Append($"INSERT INTO usrUser_user ({ModelFields}) VALUES ({ModelValues})");
            this.ID = (int)DBSqlite.SQLiteDBStateless.ExecInsertNonQueryReturnID(l_StringBuilder.ToString());
        }

        private void Update()
        {

        }

        private void Delete()
        {

        }

        #endregion
    }
}
