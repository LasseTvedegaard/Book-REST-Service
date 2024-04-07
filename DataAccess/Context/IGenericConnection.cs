using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Context {
    public interface IGenericConnection<T> : IConnection where T : IConnection { }
    }

