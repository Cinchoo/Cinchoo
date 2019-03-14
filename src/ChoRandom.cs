namespace Cinchoo.Core
{
    #region NameSpaces

    using System;

    #endregion NameSpaces

    //Uses Linear congruential generator algorithm
    public class ChoRandom : Random
    {
        #region Constants

        const ulong m = 1664525;
        const ulong c = 1013904223;

        #endregion Constants

        #region Instance Data members (Private)

        private ulong seed;

        #endregion Instance Data members (Private)

        #region Constructors

        private static readonly ChoRandom _random;

        #endregion Constructors

        #region Constructors

        public ChoRandom(int seed)
        {
            this.seed = (ulong)seed;
        }

        static ChoRandom()
        {
            _random = new ChoRandom((int)DateTime.Now.Ticks);
        }

        #endregion Constructors

        #region Random Overrides

        public override int Next()
        {
            unchecked
            {
                seed = seed * m + c;
            }

            return (int)(seed & 0xFFFFFFFFu);
        }

        #endregion Random Overrides

        #region Shared Members (Public)

        public static int NextRandom()
        {
            return _random.Next();
        }

        public static int NextRandom(int maxValue)
        {
            return _random.Next(maxValue);
        }

        public static int NextRandom(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        #endregion Shared Members (Public)
    }
}
