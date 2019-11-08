using System;
using System.Collections.Generic;
using System.Text;

namespace My9GAG.Logic.Logger
{
    public interface ILogger
    {
        void LogIntoConsole(params string[] info);
    }
}
