using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business
{
    public class MockLogService : ILogService
    {

        public void Visit()
        {
        }

        public void Visit(string message)
        {
        }

        public void Info(string message)
        {
        }

        public void Info(string format, params object[] args)
        {
        }

        public void Warning(string message)
        {
        }

        public void Warning(string format, params object[] args)
        {
        }

        public void Error(string message)
        {
        }

        public void Error(string format, params object[] args)
        {
        }

        public void Error(Exception ex)
        {
        }

        public void Error(string source, Exception ex)
        {
        }
    }
}
