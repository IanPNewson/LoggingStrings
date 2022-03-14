using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LoggingStrings
{
    [InterpolatedStringHandler]
    public ref struct LoggingStringInterpolator
    {

        private System.Runtime.CompilerServices.DefaultInterpolatedStringHandler _defaultHandler;

        private StringBuilder _label = new StringBuilder();

        private Dictionary<string, object> _labels = new Dictionary<string, object>();

        public Dictionary<string, object> Labels { get => _labels; }

        public LoggingStringInterpolator(int literalLength, int formattedLength)
        {
            _defaultHandler = new System.Runtime.CompilerServices.DefaultInterpolatedStringHandler(literalLength, formattedLength);
        }


        public void AppendLiteral(string s)
        {
            _label.Append(s);
            _defaultHandler.AppendLiteral(s);
        }

        public void AppendFormatted<T>(T t, string? format = null)
        {
            _labels.Add(_label.ToString(), t);
            _label.Clear();
        }

        public override string ToString()
        {
            return _defaultHandler.ToStringAndClear();
        }

        #region operators

        public static implicit operator string(LoggingStringInterpolator handler) => handler.ToString();

        #endregion

    }
}
