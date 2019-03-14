namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

    #endregion

    /// <summary>
    /// Specifies the kind of filtering to perform for <see cref="ValidationResults.FindAll"/>
    /// </summary>
    public enum TagFilter
    {
        /// <summary>
        /// Include results with the supplied tags.
        /// </summary>
        Include,

        /// <summary>
        /// Ignore results with the supplied tags.
        /// </summary>
        Ignore
    }

    /// <summary>
    /// Represents the result of validating an object.
    /// </summary>
    [Serializable]
    public class ChoValidationResults : IEnumerable<ChoValidationResult>
    {
        private List<ChoValidationResult> _validationResults;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="ChoValidationResults"/> class with the section name.</para>
        /// </summary>
        public ChoValidationResults()
        {
            _validationResults = new List<ChoValidationResult>();
        }

        /// <summary>
        /// <para>Adds a <see cref="ChoValidationResult"/>.</para>
        /// </summary>
        /// <param name="validationResult">The validation result to add.</param>
        public void AddResult(string message)
        {
            if (message == null || message.Length == 0) return;
            _validationResults.Add(new ChoValidationResult(message));
        }

        /// <summary>
        /// <para>Adds a <see cref="ChoValidationResult"/>.</para>
        /// </summary>
        /// <param name="validationResult">The validation result to add.</param>
        public void AddResult(string message, string tag)
        {
            if (message == null || message.Length == 0) return;
            _validationResults.Add(new ChoValidationResult(message, tag));
        }

        /// <summary>
        /// <para>Adds a <see cref="ChoValidationResult"/>.</para>
        /// </summary>
        /// <param name="validationResult">The validation result to add.</param>
        public void AddResult(ChoValidationResult validationResult)
        {
            _validationResults.Add(validationResult);
        }

        public void AddResult(ValidationResult validationResult)
        {
            ChoValidationResult result = new ChoValidationResult(validationResult.ErrorMessage);
            _validationResults.Add(result);
        }

        /// <summary>
        /// <para>Adds a <see cref="ChoValidationResult"/>.</para>
        /// </summary>
        /// <param name="validationResult">The validation result to add.</param>
        public void AddResults(string[] messages)
        {
            if (messages == null || messages.Length == 0) return;
            foreach (string message in messages)
                AddResult(message);
        }

        /// <summary>
        /// <para>Adds all the <see cref="ChoValidationResult"/> instances from <paramref name="sourceValidationResults"/>.</para>
        /// </summary>
        /// <param name="sourceValidationResults">The source for validation results to add.</param>
        public void AddResults(IEnumerable<ChoValidationResult> sourceValidationResults)
        {
            _validationResults.AddRange(sourceValidationResults);
        }

        /// <summary>
        /// Returns a new instance of <see cref="ChoValidationResults"/> that includes the results from the receiver that
        /// match the provided tag names.
        /// </summary>
        /// <param name="tagFilter">The indication of whether to include or ignore the matching results.</param>
        /// <param name="tags">The list of tag names to match.</param>
        /// <returns>A <see cref="ChoValidationResults"/> containing the filtered results.</returns>
        public ChoValidationResults FindAll(TagFilter tagFilter, params string[] tags)
        {
            // workaround for params behavior - a single null parameter will be interpreted 
            // as null array, not as an array with null as element
            if (tags == null)
                tags = new string[] { null };

            ChoValidationResults filteredValidationResults = new ChoValidationResults();

            foreach (ChoValidationResult validationResult in this)
            {
                bool matches = false;

                foreach (string tag in tags)
                {
                    if ((tag == null && validationResult.Tag == null)
                        || (tag != null && tag.Equals(validationResult.Tag)))
                    {
                        matches = true;
                        break;
                    }
                }

                // if ignore, look for !match
                // if include, look for match
                if (matches ^ (tagFilter == TagFilter.Ignore))
                {
                    filteredValidationResults.AddResult(validationResult);
                }
            }

            return filteredValidationResults;
        }

        /// <summary>
        /// Gets the indication of whether the validation represented by the receiver was successful.
        /// </summary>
        /// <remarks>
        /// An unsuccessful validation will be represented by a <see cref="ChoValidationResult"/> instance with
        /// <see cref="ChoValidationResult"/> elements, regardless of these elements' tags.
        /// </remarks>
        public bool IsValid
        {
            get { return _validationResults.Count == 0; }
        }

        /// <summary>
        /// Gets the count of results.
        /// </summary>
        public int Count
        {
            get { return _validationResults.Count; }
        }

        IEnumerator<ChoValidationResult> IEnumerable<ChoValidationResult>.GetEnumerator()
        {
            return _validationResults.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _validationResults.GetEnumerator();
        }

        public new string ToString()
        {
            StringBuilder msg = new StringBuilder();
            foreach (ChoValidationResult validationResult in this)
                msg.Append(validationResult.ToString());

            return msg.ToString();
        }
    }
}
