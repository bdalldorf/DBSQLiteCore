using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using System.Text;
using System.Linq;

namespace DBSqlite
{
    public static class SQLiteDBStateless
    {
        private static SQLiteConnection OpenConnection()
        {
            SQLiteConnection SQLiteConnection = new SQLiteConnection("" + new SQLiteConnectionStringBuilder
            {
                DataSource = "test.db"
            });

            SQLiteConnection.Open();

            return SQLiteConnection;
        }

        private static SQLiteTransaction BeginTransaction()
        {
            return OpenConnection().BeginTransaction();
        }

        private static SQLiteCommand Command(string sql, SQLiteConnection sqliteConnection)
        {
            return new SQLiteCommand(sql, sqliteConnection);
        }

        private static SQLiteCommand Command(string sql, SQLiteTransaction sqliteTransaction)
        {
            return new SQLiteCommand(sql, sqliteTransaction.Connection, sqliteTransaction);
        }

        private static SQLiteDataAdapter DataAdapter(string sql, SQLiteConnection sqliteConnection)
        {
            return new SQLiteDataAdapter(sql, sqliteConnection);
        }

        public static int ExecNonQuery(string sql)
        {
            int Results = -1;
            SQLiteConnection SQLiteConnection = null;

            try
            {
                SQLiteConnection = OpenConnection();

                Results = Command(sql, SQLiteConnection).ExecuteNonQuery();
            }
            catch (Exception exception)
            {
            }
            finally
            {
                if (SQLiteConnection != null)
                    SQLiteConnection.Close();
            }

            return Results;
        }

        public static bool ExecNonQueryTransaction(List<string> sqlStatements)
        {
            SQLiteTransaction SQLiteTransaction = null;
            StringBuilder l_Results = new StringBuilder();

            try
            {
                SQLiteTransaction = BeginTransaction();

                foreach (string sqlStatement in sqlStatements)
                {
                    Command(sqlStatement, SQLiteTransaction).ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                if (SQLiteTransaction != null)
                {
                    SQLiteTransaction.Rollback();
                    SQLiteTransaction.Dispose();
                    SQLiteTransaction = null;
                    return false;
                }
            }
            finally
            {
                if (SQLiteTransaction != null)
                {
                    SQLiteTransaction.Commit();
                }
            }

            return true;
        }

        public static long ExecInsertNonQueryReturnID(string sql)
        {
            long RowID = SQLiteDBCommon.EmptyLong;
            SQLiteTransaction SQLiteTransaction = null;
            StringBuilder l_Results = new StringBuilder();

            try
            {
                SQLiteTransaction = BeginTransaction();

                Command(sql, SQLiteTransaction).ExecuteNonQuery();

                RowID = SQLiteTransaction.Connection.LastInsertRowId;
            }
            catch (Exception exception)
            {
                if (SQLiteTransaction != null)
                {
                    SQLiteTransaction.Rollback();
                    SQLiteTransaction.Dispose();
                    SQLiteTransaction = null;
                }
            }
            finally
            {
                if (SQLiteTransaction != null)
                {
                    SQLiteTransaction.Commit();
                }
            }

            return RowID;
        }

        public static object ExecScalar(string sql)
        {
            {
                SQLiteConnection SQLiteConnection = null;
                object ScalarReturnObject = null;

                try
                {
                    SQLiteConnection = OpenConnection();

                    ScalarReturnObject = Command(sql, SQLiteConnection).ExecuteScalar();
                }
                catch (Exception exception)
                {
                }
                finally
                {
                    if (SQLiteConnection != null)
                        SQLiteConnection.Close();
                }

                return ScalarReturnObject;
            }
        }

        public static SQLiteDataReader ExecDataReader(string sql)
        {
            SQLiteConnection SQLiteConnection = null;
            SQLiteDataReader SQLiteDataReader = null;

            try
            {
                SQLiteConnection = OpenConnection();

                SQLiteDataReader = Command(sql, SQLiteConnection).ExecuteReader();
            }
            catch (Exception exception)
            {
            }
            finally
            {
                if (SQLiteConnection != null)
                    SQLiteConnection.Close();
            }

            return SQLiteDataReader;
        }

        public static DataTable ExecDataTable(string sql)
        {
            {
                SQLiteConnection SQLiteConnection = null;
                SQLiteDataAdapter SQLiteDataAdapter = null;
                DataTable DataTable = new DataTable();

                try
                {
                    SQLiteConnection = OpenConnection();

                    SQLiteDataAdapter = DataAdapter(sql, SQLiteConnection);
                    SQLiteDataAdapter.Fill(DataTable);
                }
                catch (Exception exception)
                {
                }
                finally
                {
                    if (SQLiteConnection != null)
                        SQLiteConnection.Close();
                }

                return DataTable;
            }
        }

        public static string ModelFieldNames(Type model)
        {
            StringBuilder StringBuilder = new StringBuilder();

            foreach (FieldInfo Field in model.GetFields(System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.GetField | BindingFlags.Instance))
            {
                string Value = (string)Field.CustomAttributes.Where(customAttributes => customAttributes.AttributeType == typeof(TableFieldNameAttribute)).First().ConstructorArguments.First().Value;
                StringBuilder.Append(StringBuilder.Length == 0 ? $"{Value}" : $", {Value}");
            }

            return StringBuilder.ToString();
        }

        public static string ModelFieldValues(object model)
        {
            StringBuilder StringBuilder = new StringBuilder();

            foreach (var properties in model.GetType().GetFields(System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.GetField | BindingFlags.Instance))
            {
                object Value = properties.GetValue(model);
                StringBuilder.Append(StringBuilder.Length == 0 ? $"{SQLiteDBCommon.SetValueForSql(Value)}" : $", {SQLiteDBCommon.SetValueForSql(Value)}");
            }

            return StringBuilder.ToString();
        }

        public static string GenerateInsertFields(IDatabaseModel model)
        {
            StringBuilder StringBuilderFields = new StringBuilder();
            StringBuilder StringBuilderValues = new StringBuilder();

            foreach (FieldInfo Field in model.GetType().GetFields(System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.GetField | BindingFlags.Instance))
            {
                CustomAttributeData l_ExcludeFromUpdate = Field.CustomAttributes.FirstOrDefault(customAttributes => customAttributes.AttributeType == typeof(TableFieldExcludeFromUpdateAttribute));

                if (l_ExcludeFromUpdate != null && Convert.ToBoolean(l_ExcludeFromUpdate.ConstructorArguments.First().Value))
                    continue;

                CustomAttributeData l_TableFieldName = Field.CustomAttributes.FirstOrDefault(customAttributes => customAttributes.AttributeType == typeof(TableFieldNameAttribute));

                if (l_TableFieldName == null)
                    continue;

                string FieldName = (string)l_TableFieldName.ConstructorArguments.First().Value;
                object FieldValue = Field.GetValue(model);

                StringBuilderFields.Append(StringBuilderFields.Length == 0 ? $"({FieldName}" : $", {FieldName}");
                StringBuilderValues.Append(StringBuilderValues.Length == 0 ? $"{SQLiteDBCommon.SetValueForSql(FieldValue)}" : $", {SQLiteDBCommon.SetValueForSql(FieldValue)}");
            }

            return StringBuilderFields.Append($") VALUES ({StringBuilderValues.ToString()})").ToString();
        }

        public static string GenerateUpdateFields(IDatabaseModel model)
        {
          StringBuilder StringBuilder = new StringBuilder();

          foreach (FieldInfo Field in model.GetType().GetFields(System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.GetField | BindingFlags.Instance))
            {
                CustomAttributeData l_ExcludeFromUpdate = Field.CustomAttributes.FirstOrDefault(customAttributes => customAttributes.AttributeType == typeof(TableFieldExcludeFromUpdateAttribute));

                if (l_ExcludeFromUpdate != null && Convert.ToBoolean(l_ExcludeFromUpdate.ConstructorArguments.First().Value))
                    continue;

                CustomAttributeData l_TableFieldName = Field.CustomAttributes.FirstOrDefault(customAttributes => customAttributes.AttributeType == typeof(TableFieldNameAttribute));

                if (l_TableFieldName == null)
                    continue;

                string FieldName = (string)l_TableFieldName.ConstructorArguments.First().Value;
                object FieldValue = Field.GetValue(model);
                StringBuilder.Append(StringBuilder.Length == 0 ? $"{FieldName} = {SQLiteDBCommon.SetValueForSql(FieldValue)}" : $", {FieldName} = {SQLiteDBCommon.SetValueForSql(FieldValue)}");
            }

            return StringBuilder.ToString();
        }
    }
}
