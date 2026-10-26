using System;
using System.Collections;

namespace PlusModMenu.Core.Common;

public class EnumeratorHelper
{
	public static IEnumerator ThrowingIterator(IEnumerator enumerator, Action<Exception> exceptionAction)
	{
		while (true)
		{
			object current;
			try
			{
				if (!enumerator.MoveNext())
				{
					break;
				}
				current = enumerator.Current;
			}
			catch (Exception obj)
			{
				exceptionAction(obj);
				break;
			}
			yield return current;
		}
	}
}
