using System;
using System.Data.SqlClient;

namespace Book_Test.Helper {
    public static class DatabaseCleanerEmployee {
        public static int CleanEmployeeDatabase(string connectionString) {
            string _connectionStringTest = connectionString;
            int result = -1;

            using (SqlConnection con = new SqlConnection(_connectionStringTest)) {
                con.Open();
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = @"
                -- USE [TestBookLibrary];

                -- Drop the current table
                -- IF OBJECT_ID('Employee', 'U') IS NOT NULL
                   DROP TABLE [Employee];

                -- Create the Employee table
                CREATE TABLE [Employee] (
                    [EmployeeId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [FirstName] NVARCHAR(60) NOT NULL,
                    [LastName] NVARCHAR(70) NOT NULL,
                    [Address] NVARCHAR(79) NOT NULL,
                    [BirthDate] DATE NULL,
                    [Phone] NVARCHAR(20) NULL,
                    [Email] NVARCHAR(60) NOT NULL
                );

                -- Insert 3 new employees with predefined values
                INSERT INTO [Employee] ([FirstName], [LastName], [Address], [BirthDate], [Phone], [Email])
                VALUES
                    ('Tester1', 'Testersen1', 'Testervej 1', '1990-01-01', '99009901', 'tt@mail1.com'),
                    ('Tester2', 'Testersen2', 'Testervej 2', '1990-01-01', '99009902', 'tt@mail2.com'),
                    ('Tester3', 'Testersen3', 'Testervej 3', '1990-01-01', '99009903', 'tt@mail3.com');
            ";

                result = cmd.ExecuteNonQuery();
            }

            return result;
        }
    }
}
