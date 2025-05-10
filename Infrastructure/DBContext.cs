using Npgsql;
using Microsoft.Extensions.Configuration;
using System.IO;
using Infrastructure;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Infrastructure
{
    public static class DBContext
    {
        private static NpgsqlConnection _connection;
        private static readonly object _lock = new();

        public static NpgsqlConnection GetConnection()
        {
            if (_connection == null)
            {   
                lock (_lock)
                {
                    if (_connection == null)
                    {
                        var configuration = LoadConfiguration();
                        string connectionString = configuration.GetConnectionString("PostgresConnection");
                        _connection = new NpgsqlConnection(connectionString);
                    }
                }
            }

            return _connection;
        }

        private static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            return builder.Build();
        }
    }
}
