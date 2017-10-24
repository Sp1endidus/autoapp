using System;

namespace AutoApp
{
    class ArgsReader
    {
        public object Value { get; set; }
        public string Argument { get; set; }
        
        public ArgsReader(string argument, object defaultValue)
        {
            Argument = argument;
            Value = defaultValue;
        }

        public void ReadArgs(string[] args, Type type)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (CheckArg(args[i]) && i + 1 < args.Length)
                {
                    Value = args[i + 1];
                    CheckType(type);
                }
            }
        }

        public void ReadArgs(string[] args)
        {
            ReadArgs(args, typeof(object));
        }

        private bool CheckArg(string arg)
        {
            if (arg.ToLower() == "-" + Argument || arg.ToLower() == "/" + Argument)
            {
                return true;
            }

            return false;
        }

        public void CheckType(Type type)
        {
            Convert.ChangeType(Value, type);
        }
    }
}
