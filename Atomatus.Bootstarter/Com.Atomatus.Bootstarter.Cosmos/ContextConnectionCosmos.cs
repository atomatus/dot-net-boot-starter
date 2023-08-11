using Microsoft.EntityFrameworkCore;
using System;
using System.Text.RegularExpressions;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Context connection for NoSQL CosmoDB.
    /// </summary>
    internal sealed class ContextConnectionCosmos : ContextConnectionString
    {
        private const string EP_GROUP       = "ep";
        private const string KEY_GROUP      = "key";
        private const string DB_GROUP       = "db";
        private const string REGEX_PATTERN  = @"(?<ep>(?<=Endpoint\=)(.*?)(?=(;|$)))|(?<key>(?<=Key\=)(.*?)(?=(;|$)))|(?<db>(?<=Database\=)(.*?)(?=(;|$)))";

        private const int DEFAULT_PORT      = 443;

        public ContextConnectionCosmos(Builder builder) : base(builder) { }

        private static void CheckMatchGroup(Match match, string group, ref string value)
        {
            var g = match.Groups[group];
            value = (g?.Success ?? false) ? g.Value : value;
        }

        protected internal override DbContextOptionsBuilder Attach(DbContextOptionsBuilder options)
        {
            string accountEndpoint = null;
            string accountKey = null;
            string databaseName = null;

            if (IsValid())
            {
                string connString = GetConnectionString();
                Regex regex = new Regex(REGEX_PATTERN);
                foreach(Match match in regex.Matches(connString))
                {
                    CheckMatchGroup(match, EP_GROUP, ref accountEndpoint);
                    CheckMatchGroup(match, KEY_GROUP, ref accountKey);
                    CheckMatchGroup(match, DB_GROUP, ref databaseName);
                }
            }

            port = port > 0 ? port : DEFAULT_PORT;

            return options.UseCosmos(
                accountEndpoint: accountEndpoint ??= $"{host ?? throw new ArgumentException("AccountEndpoint (Host) can not be null!")}:{port}/",
                accountKey: (accountKey ??= password) ?? throw new ArgumentException("AccountKey (Password) can not be null!"),
                databaseName: databaseName ??= database ?? throw new ArgumentException("Database name can not be null!"));
        }
    }
}
