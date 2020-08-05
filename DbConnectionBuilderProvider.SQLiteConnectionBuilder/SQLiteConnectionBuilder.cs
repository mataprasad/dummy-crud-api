using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DbConnectionBuilderProvider.SQLiteConnectionBuilder
{
    public class SQLiteConnectionBuilder : IDbConnectionBuilder
    {
        private IConfiguration configuration;
        private IWebHostEnvironment env;

        public SQLiteConnectionBuilder(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.configuration = configuration;
            this.env = env;
        }

        public IDbConnection GetConnection()
        {
            var connectionString = new SQLiteConnectionStringBuilder();
            connectionString.DataSource = Path.Combine(this.env.ContentRootPath, "Data", this.configuration["SQLiteFileName"]);
            return new SQLiteConnection(connectionString.ToString());
        }
    }
}
