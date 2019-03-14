namespace Cinchoo.Core
{
    #region NameSpaces

    using System;
    using System.Collections.Generic;
    using System.Text;

    #endregion NameSpaces  

    public class ChoPair<TLeft, TRight>
    {
        private TLeft _left;
        private TRight _right;

        internal ChoPair(TLeft left, TRight right)
        {
            _left = left;
            _right = right;
        }

        public override bool Equals(object obj)
        {
            ChoPair<TLeft, TRight> other = obj as ChoPair<TLeft, TRight>;

            if (other == null)
                return false;

            return (Equals(_left, other._left) && Equals(_right, other._right));
        }

        public override int GetHashCode()
        {
            int hashForType = _left == null ? 0 : _left.GetHashCode();
            int hashForID = _right == null ? 0 : _right.GetHashCode();
            return hashForType ^ hashForID;
        }

        public TLeft Left
        {
            get { return _left; }
        }

        public TRight Right
        {
            get { return _right; }
        }
    }
}
