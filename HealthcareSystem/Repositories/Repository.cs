using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystem.Repositories
{
    public class Repository<T>
    {
        private readonly List<T> items = new();

        public void Add(T item) => items.Add(item);

        public List<T> GetAll() => items;

        public T? GetById(Func<T, bool> predicate) =>
            items.FirstOrDefault(predicate);

        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if (item != null)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }
    }
}
