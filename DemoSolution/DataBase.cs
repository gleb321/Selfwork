using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using NewVariant.Exceptions;
using NewVariant.Interfaces;

namespace DemoSolution {
    public class DataBase : IDataBase {
        public void CreateTable<T>() where T : IEntity {
            var tableType = typeof(T);
            if (_tables.ContainsKey(tableType))
                throw new DataBaseException($"Table already exists {tableType.Name}!");

            _tables[tableType] = new List<T>();
        }
        
        public void InsertInto<T>(Func<T> getEntity) where T : IEntity {
            var tableType = typeof(T);
            if (!_tables.ContainsKey(tableType))
                throw new DataBaseException($"Unknown table {tableType.Name}!");

            ((List<T>)_tables[tableType]).Add(getEntity.Invoke());
        }

        public IEnumerable<T> GetTable<T>() where T : IEntity {
            var tableType = typeof(T);
            if (!_tables.ContainsKey(tableType))
                throw new DataBaseException($"Unknown table {tableType.Name}!");

            return (List<T>) _tables[tableType];
        }

        public void Serialize<T>(string path) where T : IEntity {
            var tableType = typeof(T);
            if (!_tables.ContainsKey(tableType))
                throw new DataBaseException($"Unknown table {tableType.Name}!");

            var serializedTable = JsonSerializer.Serialize(_tables[tableType]);
            File.WriteAllText(path, serializedTable);
        }

        public void Deserialize<T>(string path) where T : IEntity {
            var tableType = typeof(T);
            if (!_tables.ContainsKey(tableType))
                throw new DataBaseException($"Unknown table {tableType.Name}!");

            var serializedTable = File.ReadAllText(path);
            _tables[tableType] = JsonSerializer.Deserialize<List<T>>(serializedTable);
        }
        
        private readonly IDictionary<Type, object> _tables = new Dictionary<Type, object>();
    }
}