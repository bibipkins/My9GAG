using System.Diagnostics;

namespace My9GAG.Logic.Logger
{
    public class ConsoleLogger : ILogger
    {
        public void Log(params string[] info)
        {
            Debug.WriteLine("");
            Debug.WriteLine("----------------------------------------");

            foreach (var item in info)
            {
                Debug.WriteLine(item);
            }

            Debug.WriteLine("----------------------------------------");
            Debug.WriteLine("");
        }
    }
}
