namespace Eleks.Demo.UnitTests
{
    public static class TAssert
    {
        public static bool ArraysEqual(string[] expected, string[] actual)
        {
            if (expected == actual) return true;
            if (expected.Length != actual.Length) return false;
            for (int i = 0; i < expected.Length; ++i)
            {
                if (expected[i] != actual[i]) return false;
            }
            return true;
        }
    }
}