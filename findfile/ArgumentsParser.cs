using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace findfile
{
    internal class ArgumentsParser
    {
        private readonly string[] m_Args;
        private readonly StringBuilder m_Errors = new StringBuilder();
        private List<string> m_Dirs = new List<string>();

        public bool CompareSize { get; private set; }
        public ReadOnlyCollection<string> Dirs { get { return m_Dirs.AsReadOnly(); } }

        public string ErrorMessage
        {
            get { return m_Errors.ToString(); }
        }

        public bool HasInvalidArgs { get { return !string.IsNullOrEmpty(ErrorMessage); } }

        public ArgumentsParser(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            m_Args = args;
        }

        public bool ParseArguments()
        {
            if (m_Args.Length == 0)
            {
                m_Errors.Append("At least one argument must be specified");
            }
            foreach (var arg in m_Args)
            {
                if (arg == "-size")
                {
                    CompareSize = true;
                }
                else
                {
                    string dir = arg;
                    if (Directory.Exists(dir))
                    {
                        m_Dirs.Add(dir);
                    }
                    else
                    {
                        m_Errors.AppendFormat("Directory '{0}' does not exists\r\n", dir);
                    }
                }
            }
            return !HasInvalidArgs;
        }
    }
}