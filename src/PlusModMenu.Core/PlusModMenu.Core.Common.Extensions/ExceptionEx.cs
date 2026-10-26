using System;
using System.Collections.Generic;

namespace PlusModMenu.Core.Common.Extensions;

public static class ExceptionEx
{
	public static IEnumerable<Exception> InnerExceptionsAndSelf(this Exception? ex)
	{
		while (ex != null)
		{
			yield return ex;
			ex = ex.InnerException;
		}
	}
}
