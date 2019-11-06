using System.Diagnostics;

namespace My9GAG.Logic.Logger
{
    public class My9GAGLogger : ILogger
    {
        public void LogIntoConsole(params string[] info)
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
