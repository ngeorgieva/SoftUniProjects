namespace MiniORM
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Attributes;

    public class EntityManager : IDbContext
    {
        private SqlConnection connection;
        private string connectionString;
        private bool isCode;

        public EntityManager(string connectionString, bool isCode)
        {
            this.connectionString = connectionString;
            this.isCode = isCode;
        }

        public bool Persist(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Cannot persist null entity");
            }

            if (this.isCode && !this.CheckIfTableExists(entity.GetType()))
            {
                this.CreateTable(entity.GetType());
            }

            Type entityType = entity.GetType();
            FieldInfo idInfo = GetId(entityType);
            int id = (int) idInfo.GetValue(entity);

            if (id <= 0)
            {
                return this.Insert(entity, idInfo);
            }

            return this.Update(entity, idInfo);
        }

        private bool Update(object entity, FieldInfo idInfo)
        {
            int numberOfAffectedRows = 0;
            string updateString = this.PrepareEntityUpdateString(entity, idInfo);
            using (connection = new SqlConnection(this.connectionString))
            {
                this.connection.Open();
                SqlCommand updateCommand = new SqlCommand(updateString, this.connection);
                numberOfAffectedRows = updateCommand.ExecuteNonQuery();
            }

            return numberOfAffectedRows > 0;
        }

        private string PrepareEntityUpdateString(object entity, FieldInfo idInfo)
        {
            StringBuilder updateString = new StringBuilder();
            updateString.Append($"UPDATE {GetTableName(entity.GetType())} SET ");
            StringBuilder settings = new StringBuilder();

            FieldInfo[] columnFields = entity.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(x => x.IsDefined(typeof(ColumnAttribute))).ToArray();

            foreach (FieldInfo columnField in columnFields)
            {
                settings.Append($"{this.GetColumnName(columnField)} = '{columnField.GetValue(entity)}', ");
            }

            settings.Remove(settings.Length - 2, 2);
            updateString.Append(settings);

            updateString.Append($" WHERE Id = {idInfo.GetValue(entity)}");

            return updateString.ToString();

        }

        private bool Insert(object entityType, FieldInfo idInfo)
        {
            int numberOfAffectedRows = 0;
            string insertionString = this.PrepareEntityInsertionString(entityType);
            using (connection = new SqlConnection(this.connectionString))
            {
                this.connection.Open();
                SqlCommand insertionCommand = new SqlCommand(insertionString, this.connection);
                numberOfAffectedRows = insertionCommand.ExecuteNonQuery();

                string query = $"SELECT MAX(Id) FROM {this.GetTableName(entityType.GetType())}";
                SqlCommand getLastIdCommand = new SqlCommand(query, this.connection);
                int id = (int) getLastIdCommand.ExecuteScalar();

                idInfo.SetValue(entityType, id);
            }
            
            return numberOfAffectedRows > 0;
        }

        private string PrepareEntityInsertionString(object entity)
        {
            StringBuilder insertionString = new StringBuilder();
            StringBuilder columnNames = new StringBuilder();
            StringBuilder valueString = new StringBuilder();

            insertionString.Append($"INSERT INTO {this.GetTableName(entity.GetType())}(");
            FieldInfo[] columnFields = entity.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(x => x.IsDefined(typeof (ColumnAttribute))).ToArray();

            foreach (FieldInfo columnField in columnFields)
            {
                columnNames.Append($"{this.GetColumnName(columnField)}, ");
                valueString.Append($"'{columnField.GetValue(entity)}', ");
            }
            columnNames = columnNames.Remove(columnNames.Length - 2, 2);
            valueString = valueString.Remove(valueString.Length - 2, 2);
            insertionString.Append(columnNames);
            insertionString.Append(") Values(");
            insertionString.Append(valueString);
            insertionString.Append(")");

            return insertionString.ToString();
        }


        private void CreateTable(Type entity)
        {
            string creationString = PrepareTableCreationString(entity);
            using (this.connection = new SqlConnection(this.connectionString))
            {
                this.connection.Open();
                SqlCommand command = new SqlCommand(creationString, this.connection);
                command.ExecuteNonQuery();
            }
        }

        private string PrepareTableCreationString(Type entity)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"CREATE TABLE {GetTableName(entity)} (");
            builder.Append("Id INT PRIMARY KEY IDENTITY, ");
            FieldInfo[] columnsInfo = entity.GetFields(BindingFlags.Instance | BindingFlags.NonPublic).Where(x => x.IsDefined(typeof(ColumnAttribute))).ToArray();
            //string[] columnNames = columnsInfo.Select(info => info.GetCustomAttribute<ColumnAttribute>().ColumnName).ToArray();

            foreach (FieldInfo columnField in columnsInfo)
            {
                builder.Append($"{this.GetColumnName(columnField)} {this.GetTypeToDb(columnField)}, ");
            }
            builder.Remove(builder.Length - 2, 2);
            builder.Append(")");

            return builder.ToString();
        }

        private string GetTypeToDb(FieldInfo field)
        {
            switch (field.FieldType.Name)
            {
                case "Int32":
                    return "int";
                case "String":
                    return "varchar(max)";
                case "DateTime":
                    return "datetime";
                case "Boolean":
                    return "bit";
                case "Single":
                case "Double":
                case "Decimal":
                    return "decimal(10, 4)";
                default: throw new ArgumentException("No such type present. Try extending the framework.");
            }
        }

        private bool CheckIfTableExists(Type type)
        {
            string query =
                $"SELECT COUNT(name) FROM sys.sysobjects WHERE [Name] = '{GetTableName(type)}' AND [xtype] = 'U'";

            int numberOfTables = 0;

            using (this.connection = new SqlConnection(this.connectionString))
            {
                this.connection.Open();
                SqlCommand command = new SqlCommand(query, this.connection);
                numberOfTables = (int)command.ExecuteScalar();
            }

            return numberOfTables > 0;
        }

        public T FindById<T>(int id)
        {
            T wantedObject = default(T);
            string query = $"SELECT * FROM {this.GetTableName(typeof (T))} WHERE Id = @Id";
            using (connection = new SqlConnection(this.connectionString))
            {
                this.connection.Open();
                SqlCommand command = new SqlCommand(query, this.connection);
                command.Parameters.AddWithValue("@Id", id);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        throw new InvalidOperationException($"No entity with id {id} was found.");
                    }
                    reader.Read();
                    wantedObject = this.CreateEntity<T>(reader);
                }
            }

            return wantedObject;
        }

        private T CreateEntity<T>(SqlDataReader reader)
        {
            object[] columns = new object[reader.FieldCount];
            reader.GetValues(columns);

            Type[] types = new Type[columns.Length - 1];
            object[] fieldValues = new object[columns.Length - 1];

            for (int i = 1; i < columns.Length; i++)
            {
                types[i - 1] = columns[i].GetType();
                fieldValues[i - 1] = columns[i];
            }

            T createdObject = (T) typeof (T).GetConstructor(types).Invoke(fieldValues);
            FieldInfo idInfo = createdObject.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(x => x.IsDefined(typeof (IdAttribute)));

            idInfo.SetValue(createdObject, columns[0]);

            return createdObject;
        }

        public IEnumerable<T> FindAll<T>()
        {
            return this.FindAll<T>(null);
        }

        public IEnumerable<T> FindAll<T>(string where)
        {
            List<T> entities = new List<T>();
            string selectionString = $"SELECT * FROM {this.GetTableName(typeof (T))} WHERE 1=1 ";

            if (where != null)
            {
                selectionString += "AND ";
                selectionString += where;
            }

            using (this.connection = new SqlConnection(this.connectionString))
            {
                this.connection.Open();

                SqlCommand selectionCommand = new SqlCommand(selectionString, this.connection);
                SqlDataReader reader = selectionCommand.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        entities.Add(this.CreateEntity<T>(reader));
                    }
                }
            }

            return entities;
        }

        public T FindFirst<T>()
        {
            return this.FindFirst<T>(null);
        }

        public T FindFirst<T>(string where)
        {
            T result = default(T);

            string selectionString = $"SELECT TOP 1 * FROM {this.GetTableName(typeof (T))} WHERE 1=1 AND ";

            if (where != null)
            {
                selectionString += where;
            }

            using (this.connection = new SqlConnection(this.connectionString))
            {
                this.connection.Open();
                SqlCommand selectionCommand = new SqlCommand(selectionString, this.connection);
                SqlDataReader reader = selectionCommand.ExecuteReader();
                reader.Read();
                result = this.CreateEntity<T>(reader);
            }

            return result;
        }

        public void Delete<T>(object entity)
        {
            var firstOrDefault =
                entity.GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .FirstOrDefault(x => x.IsDefined(typeof (IdAttribute)));

            if (firstOrDefault == null)
            {
                throw new NullReferenceException("The given entity has no fiels with attribute Id!");
            }

            this.DeleteById<T>((int)firstOrDefault.GetValue(entity));
        }

        public void DeleteById<T>(int id)
        {
            string deletionString = $"DELETE FROM {this.GetTableName(typeof (T))} WHERE Id = @id";

            using (this.connection = new SqlConnection(this.connectionString))
            {
                this.connection.Open();
                SqlCommand command = new SqlCommand(deletionString, this.connection);
                command.Parameters.AddWithValue("@id", id);
                int numberOfDeletedItems = command.ExecuteNonQuery();

                if (numberOfDeletedItems == 0)
                {
                    throw new ArgumentException($"Id {numberOfDeletedItems} not found!");
                }
            }
        }

        private FieldInfo GetId(Type entity)
        {
            if (entity == null)
            {
                throw new ArgumentException("Cannot get id for null type");
            }

            FieldInfo id =
                entity.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .FirstOrDefault(x => x.IsDefined(typeof (IdAttribute)));

            if (id == null)
            {
                throw new ArgumentException("No id field was found in the current class.");
            }

            return id;
        }

        private string GetTableName(Type entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Table is null");
            }

            if (!entity.IsDefined(typeof (EntityAttribute)))
            {
                throw new ArgumentException("Cannot get table name of entity!");
            }

            string tableName = entity.GetCustomAttribute<EntityAttribute>().TableName;

            if (tableName == null)
            {
                throw new ArgumentNullException("Table name cannot be null!");
            }

            return tableName;
        }

        private string GetColumnName(FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("Field cannot be null.");
            }

            if (!field.IsDefined(typeof (ColumnAttribute)))
            {
                return field.Name;
            }

            string columnName = field.GetCustomAttribute<ColumnAttribute>().ColumnName;
            if (columnName == null)
            {
                throw new ArgumentNullException("Column name cannot be null.");
            }

            return columnName;
        }
    }
}

