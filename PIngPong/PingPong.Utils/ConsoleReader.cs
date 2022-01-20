using System;

namespace PingPong.Utils
{
    public class ConsoleReader : IInput
    {
        public string Read()
        {
            return Console.ReadLine();
        }
    }
}
