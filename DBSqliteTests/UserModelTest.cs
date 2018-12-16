using DBSqlite.Models;
using NUnit.Framework;
using System.Reflection;
using DBSqlite;
using System.Text;

namespace Tests
{
    public class UserModelTest
    {
        [Test]
        public void ReturnTrueIfModeltableNamesAreCorrect()
        {
            string UserModel = SQLiteDBStateless.ModelTableFieldNames(typeof(UserModel));

            Assert.AreEqual("(usrID, usrUID, usrFirstName, usrLastName, usrEmailAddress)", UserModel);
        }
    }
}