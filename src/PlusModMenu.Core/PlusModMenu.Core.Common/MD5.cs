using System;
using System.Security.Cryptography;
using System.Text;

namespace PlusModMenu.Core.Common;

public class MD5
{
	public static string Calculate(string input)
	{
		System.Security.Cryptography.MD5 mD = System.Security.Cryptography.MD5.Create();
		byte[] bytes = Encoding.ASCII.GetBytes(input);
		return BitConverter.ToString(mD.ComputeHash(bytes)).Replace("-", "");
	}
}
