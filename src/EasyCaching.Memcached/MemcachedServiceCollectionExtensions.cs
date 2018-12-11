﻿namespace EasyCaching.Memcached
{
    using EasyCaching.Core;
    using EasyCaching.Core.Internal;
    using Enyim.Caching.Configuration;
    using Enyim.Caching.Memcached;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Linq;

    /// <summary>
    /// Memcached service collection extensions.
    /// </summary>
    public static class MemcachedServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the default memcached.
        /// </summary>
        /// <returns>The default redis cache.</returns>
        /// <param name="services">Services.</param>
        /// <param name="providerAction">Options.</param>
        public static IServiceCollection AddDefaultMemcached(
            this IServiceCollection services,
            Action<MemcachedOptions> providerAction)
        {
            ArgumentCheck.NotNull(services, nameof(services));
            ArgumentCheck.NotNull(providerAction, nameof(providerAction));

            services.AddOptions();
            services.Configure(providerAction);

            services.AddSingleton<IEasyCachingProviderFactory, DefaultEasyCachingProviderFactory>();
            services.TryAddSingleton<ITranscoder, EasyCachingTranscoder>();
            services.TryAddSingleton<IMemcachedKeyTransformer, DefaultKeyTransformer>();
            services.TryAddSingleton<IEasyCachingSerializer, DefaultBinaryFormatterSerializer>();
            services.AddSingleton<IMemcachedClientConfiguration, EasyCachingMemcachedClientConfiguration>(x =>
            {
                var options = x.GetRequiredService<IOptionsMonitor<MemcachedOptions>>();
                var loggerFactory = x.GetRequiredService<ILoggerFactory>();
                var transcoder = x.GetRequiredService<ITranscoder>();
                var transformer = x.GetRequiredService<IMemcachedKeyTransformer>();
                return new EasyCachingMemcachedClientConfiguration(loggerFactory, options, transcoder, transformer);
            });

            services.TryAddSingleton<EasyCachingMemcachedClient>(x=> 
            {
                var loggerFactory = x.GetRequiredService<ILoggerFactory>();
                var config = x.GetRequiredService<IMemcachedClientConfiguration>();
                return new EasyCachingMemcachedClient(EasyCachingConstValue.DefaultMemcachedName, loggerFactory, config);
            });

            services.AddSingleton<IEasyCachingProvider, DefaultMemcachedCachingProvider>();

            return services;
        }

        /// <summary>
        /// Adds the default memcached.
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// "easycaching": {
        ///     "memcached":{
        ///         "CachingProviderType": 3,
        ///         "MaxRdSecond": 120,
        ///         "Order": 2,
        ///         "dbconfig": {            
        ///             "Servers": [
        ///                 {
        ///                 "Address": "memcached",
        ///                 "Port": 11211
        ///                 }
        ///             ],
        ///             "socketPool": {
        ///                 "minPoolSize": "5",
        ///                 "maxPoolSize": "25",
        ///                 "connectionTimeout": "00:00:15",
        ///                 "receiveTimeout": "00:00:15",
        ///                 "deadTimeout": "00:00:15",
        ///                 "queueTimeout": "00:00:00.150"
        ///             } 
        ///         }
        ///     }
        /// }      
        /// ]]>
        /// </example>
        /// <returns>The default memcached.</returns>
        /// <param name="services">Services.</param>
        /// <param name="configuration">Configuration.</param>
        public static IServiceCollection AddDefaultMemcached(
           this IServiceCollection services,
            IConfiguration configuration)
        {
            var cacheConfig = configuration.GetSection(EasyCachingConstValue.MemcachedSection);
            services.Configure<MemcachedOptions>(cacheConfig);

            //var memcachedConfig = configuration.GetSection(EasyCachingConstValue.ConfigChildSection);
            //services.Configure<EasyCachingMemcachedClientOptions>(memcachedConfig);

            services.AddSingleton<IEasyCachingProviderFactory, DefaultEasyCachingProviderFactory>();
            services.TryAddSingleton<ITranscoder, EasyCachingTranscoder>();
            services.TryAddSingleton<IMemcachedKeyTransformer, DefaultKeyTransformer>();
            services.TryAddSingleton<IEasyCachingSerializer, DefaultBinaryFormatterSerializer>();
            services.AddSingleton<IMemcachedClientConfiguration, EasyCachingMemcachedClientConfiguration>(x =>
            {
                var options = x.GetRequiredService<IOptionsMonitor<MemcachedOptions>>();
                var loggerFactory = x.GetRequiredService<ILoggerFactory>();
                var transcoder = x.GetRequiredService<ITranscoder>();
                var transformer = x.GetRequiredService<IMemcachedKeyTransformer>();
                return new EasyCachingMemcachedClientConfiguration(loggerFactory, options, transcoder, transformer);
            });

            services.TryAddSingleton<EasyCachingMemcachedClient>(x =>
            {
                var loggerFactory = x.GetRequiredService<ILoggerFactory>();
                var config = x.GetRequiredService<IMemcachedClientConfiguration>();
                return new EasyCachingMemcachedClient(EasyCachingConstValue.DefaultMemcachedName, loggerFactory, config);
            });

            services.TryAddSingleton<IEasyCachingProvider, DefaultMemcachedCachingProvider>();
            return services;
        }

        /// <summary>
        /// Adds the default memcached.
        /// </summary>
        /// <returns>The default memcached.</returns>
        /// <param name="services">Services.</param>
        /// <param name="name">Name.</param>
        /// <param name="providerAction">Provider action.</param>
        public static IServiceCollection AddDefaultMemcached(
           this IServiceCollection services,
           string name,
           Action<MemcachedOptions> providerAction)
        {
            ArgumentCheck.NotNull(services, nameof(services));
            ArgumentCheck.NotNullOrWhiteSpace(name, nameof(name));
            ArgumentCheck.NotNull(providerAction, nameof(providerAction));

            services.AddOptions();
            services.Configure(name, providerAction);

            services.AddSingleton<IEasyCachingProviderFactory, DefaultEasyCachingProviderFactory>();
            services.TryAddSingleton<ITranscoder, EasyCachingTranscoder>();
            services.TryAddSingleton<IMemcachedKeyTransformer, DefaultKeyTransformer>();
            services.TryAddSingleton<IEasyCachingSerializer, DefaultBinaryFormatterSerializer>();
            services.AddSingleton<EasyCachingMemcachedClientConfiguration>(x =>
            {
                var optionsMon = x.GetRequiredService<IOptionsMonitor<MemcachedOptions>>();
                var options = optionsMon.Get(name);
                var loggerFactory = x.GetRequiredService<ILoggerFactory>();
                var transcoder = x.GetRequiredService<ITranscoder>();
                var transformer = x.GetRequiredService<IMemcachedKeyTransformer>();
                return new EasyCachingMemcachedClientConfiguration(name, loggerFactory, options, transcoder, transformer);
            });

            services.TryAddSingleton<EasyCachingMemcachedClient>(x =>
            {
                var loggerFactory = x.GetRequiredService<ILoggerFactory>();
                var configs = x.GetServices<EasyCachingMemcachedClientConfiguration>();
                var config = configs.FirstOrDefault(y => y.Name.Equals(name));
                return new EasyCachingMemcachedClient(name, loggerFactory, config);
            });

            services.AddSingleton<IEasyCachingProvider, DefaultMemcachedCachingProvider>();

            return services;
        }

        /// <summary>
        /// Adds the default memcached.
        /// </summary>
        /// <returns>The default memcached.</returns>
        /// <param name="services">Services.</param>
        /// <param name="name">Name.</param>
        /// <param name="configuration">Configuration.</param>
        public static IServiceCollection AddDefaultMemcached(
          this IServiceCollection services,
           string name,
           IConfiguration configuration)
        {
            var cacheConfig = configuration.GetSection(EasyCachingConstValue.MemcachedSection);
            services.Configure<MemcachedOptions>(name,cacheConfig);

            services.AddSingleton<IEasyCachingProviderFactory, DefaultEasyCachingProviderFactory>();
            services.TryAddSingleton<ITranscoder, EasyCachingTranscoder>();
            services.TryAddSingleton<IMemcachedKeyTransformer, DefaultKeyTransformer>();
            services.TryAddSingleton<IEasyCachingSerializer, DefaultBinaryFormatterSerializer>();
            services.AddSingleton<EasyCachingMemcachedClientConfiguration>(x =>
            {
                var optionsMon = x.GetRequiredService<IOptionsMonitor<MemcachedOptions>>();
                var options = optionsMon.Get(name);
                var loggerFactory = x.GetRequiredService<ILoggerFactory>();
                var transcoder = x.GetRequiredService<ITranscoder>();
                var transformer = x.GetRequiredService<IMemcachedKeyTransformer>();
                return new EasyCachingMemcachedClientConfiguration(name, loggerFactory, options, transcoder, transformer);
            });

            services.TryAddSingleton<EasyCachingMemcachedClient>(x =>
            {
                var loggerFactory = x.GetRequiredService<ILoggerFactory>();
                var configs = x.GetServices<EasyCachingMemcachedClientConfiguration>();
                var config = configs.FirstOrDefault(y => y.Name.Equals(name));
                return new EasyCachingMemcachedClient(name, loggerFactory, config);
            });

            services.TryAddSingleton<IEasyCachingProvider, DefaultMemcachedCachingProvider>();
            return services;
        }
    }
}
