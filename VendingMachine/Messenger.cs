using System;

namespace VendingMachine.Core
{
    public interface IMessenger
    {
        void Show(string message);
        void Clear();
    }

    public class Messenger : IMessenger
    {
        private static readonly Messenger _default = new Messenger();

        public static Messenger Default { get { return _default; } }

        public void Show(string message) { Console.WriteLine("{0}: {1}", DateTime.Now.ToShortTimeString(), message); }

        public void Clear() { Show(string.Empty); }
    }
}
