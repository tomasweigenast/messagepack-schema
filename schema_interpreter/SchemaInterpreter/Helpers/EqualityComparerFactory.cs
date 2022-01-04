using System;
using System.Collections.Generic;

namespace SchemaInterpreter.Helpers
{
    public class EqualityComparerFactory
	{
		private sealed class Impl<T> : IEqualityComparer<T>
		{
			private readonly Func<T, T, bool> Eq;
			private readonly Func<T, int> HashFunc;

			public Impl(Func<T, T, bool> eq, Func<T, int> hashFunc)
			{
				Eq = eq;
				HashFunc = hashFunc;
			}

			public bool Equals(T left, T right)
			{
				return Eq(left, right);
			}

			public int GetHashCode(T obj)
			{
				return HashFunc(obj);
			}

		}

		public static IEqualityComparer<T> Create<T>(Func<T, T, bool> eq, Func<T, int> hashFunc)
			=> new Impl<T>(eq, hashFunc);
	}
}
