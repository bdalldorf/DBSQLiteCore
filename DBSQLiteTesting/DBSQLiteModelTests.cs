using DBSqlite;
using DBSqlite.Models;
using NUnit.Framework;

namespace DBSqlLiteTests
{
    public class UserModelTest
    {
        [Test]
        public void ReturnTrueIfModelTableFieldsAreCorrect()
        {
            string UserModelFIelds = SQLiteDBStateless.ModelFieldNames(typeof(UserModel));
            Assert.AreEqual("usrID, usrUID, usrFirstName, usrLastName, usrEmailAddress", UserModelFIelds);
        }

        [Test]
        public void ReturnTrueIfModelValuesAreCorrect()
        {
            UserModel UserModel = new UserModel()
            {
                ID = 1,
                UID = "Testert1",
                FirstName = "Test",
                LastName = "Tester",
                EmailAddress = "test@testing.com"
            };

            string UserModelFIelds = SQLiteDBStateless.ModelFieldValues(UserModel);
            Assert.AreEqual("1, 'Testert1', 'Test', 'Tester', 'test@testing.com'", UserModelFIelds);
        }

        [Test]
        public void ReturnTrueIfUpdateFieldsAreCorrect()
        {
            UserModel UserModel = new UserModel()
            {
                ID = 1,
                UID = "Testert1",
                FirstName = "Test",
                LastName = "Tester",
                EmailAddress = "test@testing.com"
            };

            string UserUpdateFields = SQLiteDBStateless.GenerateUpdateFields(UserModel);
            Assert.AreEqual("usrUID = 'Testert1', usrFirstName = 'Test', usrLastName = 'Tester', usrEmailAddress = 'test@testing.com'", UserUpdateFields);
        }

        [Test]
        public void ReturnTrueIfInsertFieldsAreCorrect()
        {
            UserModel UserModel = new UserModel()
            {
                ID = 1,
                UID = "Testert1",
                FirstName = "Test",
                LastName = "Tester",
                EmailAddress = "test@testing.com"
            };

            string UserUpdateFields = SQLiteDBStateless.GenerateInsertFields(UserModel);
            Assert.AreEqual("(usrUID, usrFirstName, usrLastName, usrEmailAddress) VALUES ('Testert1', 'Test', 'Tester', 'test@testing.com')", UserUpdateFields);
        }

        [Test]
        public void ReturnTrueIfModelTableNameIsCorrect()
        {
            UserModel UserModel = new UserModel();
            Assert.AreEqual("usrUser_usr", UserModel.TableName());
        }
    }
}