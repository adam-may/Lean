using System;
using System.Linq;
using NUnit.Framework;
using QuantConnect.Indicators;

namespace QuantConnect.Tests.Indicators
{
    [TestFixture]
    public class IndicatorBaseTests
    {
        private TestableIndicatorBase _indicatorBase;
        private const int HistorySize = 3;

        [SetUp]
        public void SetUp()
        {
            _indicatorBase = new TestableIndicatorBase();
        }

        [Test]
        public void PreviousReturnsErrorIfRememberNotCalled()
        {
            Assert.IsNull(_indicatorBase.Previous);
        }

        [Test]
        public void CurrentWorksIfNotRemembering()
        {
            var inputValues = new[] {1, 2, 3, 4};
            var indicatorDataPoints = inputValues.Select(x => new IndicatorDataPoint(new DateTime(2020, 1, x), x)).ToList();

            foreach (var indicatorDataPoint in indicatorDataPoints)
            {
                _indicatorBase.Update(indicatorDataPoint);
            }

            Assert.IsNull(_indicatorBase.Previous);
            Assert.AreEqual(indicatorDataPoints.Last(), _indicatorBase.Current);
        }

        [Test]
        public void ResetWorksIfNotRemembering()
        {
            Assert.DoesNotThrow(() => _indicatorBase.Reset());
        }

        [Test]
        public void RememberSetsUpRollingWindowSize()
        {
            _indicatorBase.Remember(HistorySize);
            Assert.AreEqual(HistorySize, _indicatorBase.Previous.Size);
            Assert.AreEqual(0, _indicatorBase.Previous.Count);
        }

        [Test]
        public void CorrectPreviousValues()
        {
            _indicatorBase.Remember(HistorySize);

            var inputValues = new[] {1, 2, 3, 4};
            var indicatorDataPoints = inputValues.Select(x => new IndicatorDataPoint(new DateTime(2020, 1, x), x)).ToList();

            foreach (var indicatorDataPoint in indicatorDataPoints)
            {
                _indicatorBase.Update(indicatorDataPoint);
            }

            Assert.AreEqual(3, _indicatorBase.Previous.Count);
            Assert.AreEqual(4, _indicatorBase.Previous.Samples);
            CollectionAssert.AreEqual(Enumerable.Reverse(indicatorDataPoints).Take(HistorySize), _indicatorBase.Previous);
        }

        [Test]
        public void ResetWorksOnPreviousValues()
        {
            _indicatorBase.Remember(HistorySize);

            var inputValues = new[] {1, 2, 3, 4};
            var indicatorDataPoints = inputValues.Select(x => new IndicatorDataPoint(new DateTime(2020, 1, x), x)).ToList();

            foreach (var indicatorDataPoint in indicatorDataPoints)
            {
                _indicatorBase.Update(indicatorDataPoint);
            }

            Assert.AreEqual(3, _indicatorBase.Previous.Count);
            Assert.AreEqual(4, _indicatorBase.Previous.Samples);
            CollectionAssert.AreEqual(Enumerable.Reverse(indicatorDataPoints).Take(HistorySize), _indicatorBase.Previous);

            _indicatorBase.Reset();

            Assert.AreEqual(0, _indicatorBase.Previous.Count);
            Assert.AreEqual(0, _indicatorBase.Previous.Samples);
            CollectionAssert.AreEqual(new IndicatorDataPoint[] {}, _indicatorBase.Previous);
        }
    }

    public class TestableIndicatorBase : IndicatorBase<IndicatorDataPoint>
    {
        public TestableIndicatorBase()
            : base("Test")
        {
        }

        public override bool IsReady => true;

        protected override decimal ComputeNextValue(IndicatorDataPoint input) => input.Value;
    }
}
