namespace Cinchoo.Core.Text
{
    #region NameSpaces

    using System;
    using System.Text;

    using Cinchoo.Core.Collections;

    #endregion NameSpaces

    public sealed class ChoStringMsgBuilderCollection : IDisposable
    {
        #region Instance Data Members (Private)

        private StringBuilder _msg = new StringBuilder();
        private ChoNotNullableArrayList _tokens = new ChoNotNullableArrayList();

        #endregion

        #region Constructors

        public ChoStringMsgBuilderCollection(string header)
        {
            _msg.AppendFormat("{0}{1}", header, Environment.NewLine);
        }

        public ChoStringMsgBuilderCollection(string header, params object[] args)
        {
            _msg.AppendFormat("{0}{1}", header, Environment.NewLine);
        }

        #endregion

        #region Instance Members (Public)

        public void Append(params string[] tokens)
        {
            if (tokens == null || tokens.Length == 0) return;

            _tokens.AddRange(tokens);
        }

        public new string ToString()
        {
            _msg.AppendFormat("[{0}", Environment.NewLine);
            if (_tokens != null && _tokens.Count > 0)
            {
                foreach (string token in _tokens)
                    _msg.AppendFormat("\t{0}{1}", token, Environment.NewLine);
            }
            else
            {
                _msg.AppendFormat("\t-- Empty --{0}", Environment.NewLine);
            }
            _msg.AppendFormat("]{0}", Environment.NewLine);
            return _msg.ToString();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _msg = null;
            _tokens = null;
        }

        #endregion

        #region Finalizers

        ~ChoStringMsgBuilderCollection()
        {
            Dispose();
        }

        #endregion
    }
}
