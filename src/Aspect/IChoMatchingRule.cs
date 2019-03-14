namespace Microsoft.Practices.EnterpriseLibrary.PolicyInjection
{
    #region NameSpaces

    using System.Reflection;

    #endregion

    /// <summary>
    /// This interface is implemented by the matching rule classes.
    /// A Matching rule is used to see if a particular policy should
    /// be applied to a class member.
    /// </summary>
    public interface IChoMatchingRule
    {
        /// <summary>
        /// Tests to see if this rule applies to the given member.
        /// </summary>
        /// <param name="member">Member to test.</param>
        /// <returns>true if the rule applies, false if it doesn't.</returns>
        bool Matches(MethodBase member);
    }
}
