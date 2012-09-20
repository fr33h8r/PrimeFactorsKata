using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Xunit.Extensions;

namespace PrimeFactorsKata
{
    public interface IPrimeFactorsProvider
    {
        List<int> Generate(int number);
    }

    class TestProvider : IPrimeFactorsProvider
    {
        private readonly int arg;
        private List<int> expected;

        public TestProvider(int arg, List<int> expected)
        {
            this.arg = arg;
            this.expected = expected;
        }

        public List<int> Generate(int number)
        {
            return number == arg ? expected : null;
        }
    }

    public class AzurePrimeFactorsProvider : IPrimeFactorsProvider
    {
        private readonly string username;

        public AzurePrimeFactorsProvider(string username)
        {
            this.username = username;
        }

        public List<int> Generate(int number)
        {
            return new [] {number + 1}.ToList();
        }
    }


    public class SimplePrimeFactorsProvider : IPrimeFactorsProvider
    {
        public virtual List<int> Generate(int number)
        {
            Console.Out.WriteLine("- $1 from Pavel");
            var r = number;
            Func<int, int> countFor = ii => Enumerable.Range(1, int.MaxValue)
                                                .TakeWhile(j =>
                                                               {
                                                                   if (r % ii != 0 || r <= 0)
                                                                       return false;
                                                                   r /= ii;
                                                                   return true;
                                                               }).Count();

            return Enumerable.Range(2, number)
                .SelectMany(i => Enumerable.Repeat(i, countFor(i)))
                .ToList();
        }
    }

    public class PrimeFactorsTests
    {
        [Fact]
        public void should_take_1_return_empty()
        {
            //arrange
            var factors = new SimplePrimeFactorsProvider();
            //act
            var result = factors.Generate(2);
            //assert
            result.Should().BeEquivalentTo(new List<int> { 2 });
        }

        [Theory]
        [InlineData(2, new[] { 2 })]
        [InlineData(3, new[] { 3 })]
        [InlineData(4, new[] { 2, 2 })]
        [InlineData(5, new[] { 5 })]
        [InlineData(6, new[] { 2, 3 })]
        [InlineData(8, new[] { 2, 2, 2 })]
        [InlineData(9, new[] { 3, 3 })]
        [InlineData(13195, new[] { 5, 7, 13, 29 })]
        public void should_take_vakue_return_list(int value, int[] expected)
        {
            //arrange
            var factors = new SimplePrimeFactorsProvider();
            //act
            var result = factors.Generate(value);
            //assert
            result.Should().BeEquivalentTo(expected.ToList());
        }
    }

    public class SimpleNum
    {
        private readonly IPrimeFactorsProvider fact;

        public SimpleNum(IPrimeFactorsProvider fact)
        {
            this.fact = fact;
        }

        public int GetGreatestDivisor(int num)
        {
            var list = fact.Generate(num);
            return list.Max();
        }
    }



    public class SimpleNumTests
    {
        [Fact]
        void should_take_greatest_divisor()
        {
            //arrange
            var myPrimeFactorProvider = Substitute.For<IPrimeFactorsProvider>();

            myPrimeFactorProvider.Generate(99)
                .Returns(new List<int> {1,2,3,5,6,7});

            //act
            var snum = new SimpleNum(myPrimeFactorProvider);


            var result = snum.GetGreatestDivisor(99);
            //assert
            result.Should().Be(7);
        }
    }
}
