using System;

namespace ModMenu.Settings;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public class OrderAttribute : Attribute
{
	public enum OrderMode
	{
		BeforeUnordered,
		AfterUnordered
	}

	public int Order { get; }

	public OrderMode Mode { get; }

	public OrderAttribute(int order, OrderMode mode = OrderMode.BeforeUnordered)
	{
		Order = order;
		Mode = mode;
	}
}
