namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections.Generic;

    using Cinchoo.Core;
    using Cinchoo.Core.Text;

    #endregion NameSpaces

    /// <summary>
    /// Represents the result of an atomic validation.
    /// </summary>
    [Serializable]
    public class ChoValidationResult
    {
        private string _message;
        private string _tag;
        [NonSerialized]
        private object _target;
        [NonSerialized]
        private ChoValidationResults _nestedValidationResults;

        private static readonly IEnumerable<ChoValidationResult> _nonNestedValidationResults = new ChoValidationResult[0];

        public ChoValidationResult(string message)
            : this(message, null, null, _nonNestedValidationResults)
        { }

        public ChoValidationResult(string message, string tag)
            : this(message, tag, null, _nonNestedValidationResults)
        { }

        /// <summary>
        /// Initializes this object with a _message.
        /// </summary>
        public ChoValidationResult(string message, string tag, object target)
            : this(message, tag, target, _nonNestedValidationResults)
        { }


        /// <summary>
        /// Initializes this object with a _message.
        /// </summary>
        public ChoValidationResult(string message, string tag, object target, IEnumerable<ChoValidationResult> nestedValidationResults)
        {
            _message = message;
            _tag = tag;
            _target = target;
            _nestedValidationResults = new ChoValidationResults();
            _nestedValidationResults.AddResults(nestedValidationResults);
        }

        /// <summary>
        /// Gets a _message describing the failure.
        /// </summary>
        public string Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Gets a value characterizing the result.
        /// </summary>
        /// <remarks>
        /// The meaning for a tag is determined by the client code consuming the <see cref="ValidationResults"/>.
        /// </remarks>
        /// <seealso cref="ValidationResults.FindAll"/>
        public string Tag
        {
            get { return _tag; }
        }

        /// <summary>
        /// Gets the object to which the validation rule was applied.
        /// </summary>
        /// <remarks>
        /// This object might not be the object for which validation was requested initially.
        /// </remarks>
        public object Target
        {
            get { return _target; }
        }

        /// <summary>
        /// Gets the nested validation results for a composite failed validation.
        /// </summary>
        public IEnumerable<ChoValidationResult> NestedValidationResults
        {
            get { return _nestedValidationResults; }
        }

        public new string ToString()
        {
            StringBuilder msg = new StringBuilder(Tag == null ? Message : String.Format("[{0}]: {1}", Tag, Message));

            if (_nestedValidationResults != null)
            {
                msg.AppendFormat("{0}{1}", Environment.NewLine, 
                    _nestedValidationResults.ToString().Indent(1));
            }
            return msg.ToString();
        }
    }
}
