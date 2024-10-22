using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public interface ILogService
    {
        void Visit();
        void Visit(string message);
        void Info(string message);
        void Info(string format, params object[] args);
        void Warning(string message);
        void Warning(string format, params object[] args);
        void Error(string message);
        void Error(string format, params object[] args);
        void Error(Exception ex);
        void Error(string source, Exception ex);
    }
}
