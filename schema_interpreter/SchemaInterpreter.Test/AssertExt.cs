using System;
using System.Linq.Expressions;
using Xunit;

namespace SchemaInterpreter.Test
{
    public static class AssertExt
    {
        public static void True(params Expression<Func<bool>>[] conditions)
        {
            foreach(var condition in conditions)
            {
                Assert.True(condition.Compile()(), $"Condition: {condition}");
            }
        }

        public static void True<T>(T value, params Expression<Func<T, bool>>[] conditions)
        {
            foreach (var condition in conditions)
            {
                Assert.True(condition.Compile()(value), $"Condition: {condition}");
            }
        }
    }
}