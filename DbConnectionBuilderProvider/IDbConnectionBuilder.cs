using System;
using System.Data;

namespace DbConnectionBuilderProvider
{
    public interface IDbConnectionBuilder
    {
        IDbConnection GetConnection();
    }
}
