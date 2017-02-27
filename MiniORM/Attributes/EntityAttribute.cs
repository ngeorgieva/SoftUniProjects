namespace MiniORM.Attributes
{
    using System;
    public class EntityAttribute : Attribute
    {
        public string TableName { get; set; }
    }
}
