using SignalR.Dynamic.API.Common;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Dynamic.API.Interfaces
{
    public interface IRepository<T>
    {
        T Get(int ID);
        void Add(T t);
        IEnumerable<T> All { get; }
        void Delete(int id);
        event Action<T> OnAdd;
        event Action<T> OnChange;
        event Action<IEnumerable<T>> OnRemove;
    }
}
