using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Cosmos, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c51a009f369b01e2686a4cb3d5330d7610cacc2db1c6b8692c745162831f2809286f15ce0bff54a0f24c35f0f498ae59059ae05e78aa4f2b36c4b75ea03c6de0b5696ae1ed6024cb1a2139e8d4c6a50bfcfec4f651e2411f66c123078bfe8d58ff21e5021462011188759a9b35ec1feee26137c41ec11a67037b993c41fb8bad")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Postgres, PublicKey=002400000480000094000000060200000024000052534131000400000100010081883cc4a796c90d1d02a587f661e5ce61a7bc27c2780f8df196d3462290fc6fcc25cf6d539defb830264c657d1f8af10565d32ae101e70135911176e3ed2a9370dce6a10457e630736002e278ca656561b6a8afa890c9133ba190cf8b1dd553cf77afeb24f5638fd567b1cb7b3ec4850735e13fdfe6f8058ac58ae4f255ebb6")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Sqlite, PublicKey=002400000480000094000000060200000024000052534131000400000100010081883cc4a796c90d1d02a587f661e5ce61a7bc27c2780f8df196d3462290fc6fcc25cf6d539defb830264c657d1f8af10565d32ae101e70135911176e3ed2a9370dce6a10457e630736002e278ca656561b6a8afa890c9133ba190cf8b1dd553cf77afeb24f5638fd567b1cb7b3ec4850735e13fdfe6f8058ac58ae4f255ebb6")]
[assembly: InternalsVisibleTo("Com.Atomatus.Bootstarter.Sqlserver, PublicKey=002400000480000094000000060200000024000052534131000400000100010079ca8479c5a52b5b284eff5e26078cea9e5916c0b506ed2cca26012d01cdc1d0636d835a1992394f726d792ea7e23b8da849a9e530b7a837731ca5a5a112fcdf4db8448d505bb66dbc687b486252be73366f7747325775503bab93b1829d763e4cd0c82ad4443d8c4eedaf534bffd42fed1f12923288754c012385e6422f47d8")]
namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Context Connection extensions for context string
    /// </summary>
    public static class ContextConnectionStringExtensions
    {
        private static bool OnBuildFromConnectionStringCallback(ContextConnection.Builder builder, out ContextConnection conn)
        {
            var aux = new ContextConnectionString(builder);
            bool isValid = aux.IsValid();
            conn = isValid ? aux : default;
            return isValid;
        }

        /// <summary>
        /// Define configuration and add callback to build it 
        /// as connection string explicit for defined database structure in appseting.json.
        /// </summary>
        /// <param name="builder">current builder</param>
        /// <param name="configuration">configuration values</param>
        /// <returns>current buider</returns>
        internal static ContextConnection.Builder Configuration(this ContextConnection.Builder builder, IConfiguration configuration)
        {
            return builder
                .AddConfiguration(configuration)
                .AddBuildCallback(OnBuildFromConnectionStringCallback);
        }

        /// <summary>
        /// Define configuration from service provider and add callback to build it 
        /// as connection string explicit for defined database structure in appseting.json.
        /// </summary>
        /// <param name="builder">current builder</param>
        /// <param name="provider">service provider that contains IConfiguration</param>
        /// <returns>current builder</returns>
        internal static ContextConnection.Builder Configuration(this ContextConnection.Builder builder, IServiceProvider provider)
        {
            return builder.Configuration((provider != null ? provider.GetService<IConfiguration>() :
                 throw new ArgumentNullException(nameof(provider), "Service provider can not be null!"))/* ??
                 throw new ArgumentException("Service provider is do not attaching IConfiguration!")*/);
        }

        /// <summary>
        /// Define connection string key to recovery connection string from configurations (appsetings.json).
        /// </summary>
        /// <param name="builder">current builder</param>
        /// <param name="connectionStringKey">connection string access key</param>
        /// <returns>current builder</returns>
        public static ContextConnection.Builder ConnectionStringKey(this ContextConnection.Builder builder, string connectionStringKey)
        {
            return builder
                .AddConnectionStringKey(connectionStringKey)
                .AddBuildCallback(OnBuildFromConnectionStringCallback);
        }
    }
}
