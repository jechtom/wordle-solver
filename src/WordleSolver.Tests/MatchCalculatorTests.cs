using NUnit.Framework;

namespace WordleSolver.Tests
{
    public class MatchCalculatorTests
    {
        [Test]
        public void NoGreenNoYellow()
        {
            Assert.AreEqual((green: 0, yellow: 0), MatchCalculator.CalculateScore("ABCDE", "FGHIJ"));
        }

        [Test]
        public void AllGreen()
        {
            Assert.AreEqual((green: 5, yellow: 0), MatchCalculator.CalculateScore("ABCDE", "ABCDE"));
        }

        [Test]
        public void OneYellow()
        {
            Assert.AreEqual((green: 0, yellow: 1), MatchCalculator.CalculateScore("BYYYY", "ZZZZB"));
        }

        [Test]
        public void TwoYellow()
        {
            Assert.AreEqual((green: 0, yellow: 2), MatchCalculator.CalculateScore("BBYYY", "ZZZZB"));
        }

        [Test]
        public void NoYellowAfterGreenMatch()
        {
            Assert.AreEqual((green: 1, yellow: 0), MatchCalculator.CalculateScore("BBYYB", "ZZZZB"));
        }

        [Test]
        public void YellowAfterGreenMatch()
        {
            Assert.AreEqual((green: 1, yellow: 2), MatchCalculator.CalculateScore("BBYYB", "ZZZBB"));
        }
    }
}