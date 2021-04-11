﻿using Microsoft.Extensions.Configuration;
using System;

namespace Com.Atomatus.Bootstarter.Context.Configuration.Connection
{
    public abstract class ContextConnectionParameters : IDisposable
    {
        protected string database;
        protected string host;
        protected int port;
        protected string user;
        protected string password;
        protected int timeout;
        protected int commandTimeout;
        protected int idleLifetime;
        protected int minPoolSize;
        protected int maxPoolSize;
        protected string applicationName;

        protected IConfiguration configuration;
        protected string connectionStringKey;

        ~ContextConnectionParameters()
        {
            this.Dispose();
        }

        protected ContextConnectionParameters(ContextConnectionParameters other)
        {
            this.database               = other.database;
            this.host                   = other.host;
            this.port                   = other.port;
            this.user                   = other.user;
            this.password               = other.password;
            this.timeout                = other.timeout;
            this.commandTimeout         = other.commandTimeout;
            this.idleLifetime           = other.idleLifetime;
            this.minPoolSize            = other.minPoolSize;
            this.maxPoolSize            = other.maxPoolSize;
            this.applicationName        = other.applicationName;            
            this.configuration          = other.configuration;
            this.connectionStringKey    = other.connectionStringKey;
        }

        protected ContextConnectionParameters() { }

        protected virtual void OnDispose() { }

        private void Dispose()
        {
            this.OnDispose();
            this.database               = null;
            this.host                   = null;
            this.user                   = null;
            this.password               = null;
            this.applicationName        = null;
            this.configuration          = null;
            this.connectionStringKey    = null;
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}