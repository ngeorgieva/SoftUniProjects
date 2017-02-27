namespace MiniORM.Entities
{
    using System;
    using Attributes;

    [Entity(TableName = "Users")]
    public class User
    {
        [Id]
        private int id;

        [Column(ColumnName = "Username")]
        private string username;

        [Column(ColumnName = "Pass")]
        private string password;

        [Column(ColumnName = "Age")]
        private int age;

        [Column(ColumnName = "RegistrationDate")]
        private DateTime registrationDate;

        private string email;

        public User(string username, string password, int age, DateTime registrationDate)
        {
            this.Username = username;
            this.Password = password;
            this.Age = age;
            this.RegistrationDate = registrationDate;
        }

        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        public string Username
        {
            get { return this.username; }
            set { this.username = value; }
        }

        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        public int Age
        {
            get { return this.age; }
            set { this.age = value; }
        }

        public DateTime RegistrationDate
        {
            get { return this.registrationDate; }
            set { this.registrationDate = value; }
        }
    }
}
