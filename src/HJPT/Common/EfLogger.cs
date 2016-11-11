using System;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Csys.Common
{
    public class EfLogger
    {
        
    }

    public class DbLoggerProvider : ILoggerProvider
    {
        private readonly Action<string> _writeAction;

        public DbLoggerProvider(Action<string> writeAction)
        {
            _writeAction = writeAction;
        }

        private static readonly string[] _whitelist =
        {
            typeof(Microsoft.EntityFrameworkCore.Storage.Internal.RelationalCommandBuilderFactory).FullName,
            //typeof(Microsoft.EntityFrameworkCore.Storage.Internal.SqlCeDatabaseConnection).FullName
        };

        public ILogger CreateLogger(string name)
        {
            if (_whitelist.Contains(name))
            {
                return new DbSimpleLogger(_writeAction);
            }

            return NullLogger.Instance;
        }

        public void Dispose()
        {
        }
    }


    internal class DbSimpleLogger : ILogger
    {
        private readonly Action<string> _writeAction;

        public DbSimpleLogger(Action<string> writeAction)
        {
            _writeAction = writeAction;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);
            _writeAction(message);
        }

        public IDisposable BeginScope<TState>(TState state) => null;
    }

    internal class NullLogger : ILogger
    {
        public static NullLogger Instance { get; } = new NullLogger();
        public IDisposable BeginScope<TState>(TState state) => null;
        public bool IsEnabled(LogLevel logLevel) => false;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }
    }

}