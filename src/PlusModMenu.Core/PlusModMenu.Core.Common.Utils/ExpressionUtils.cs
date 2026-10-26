using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PlusModMenu.Core.Common.Utils;

public class ExpressionUtils
{
	private class SwapVisitor : ExpressionVisitor
	{
		private readonly Expression _from;

		private readonly Expression _to;

		public SwapVisitor(Expression from, Expression to)
		{
			_from = from;
			_to = to;
		}

		public override Expression Visit(Expression node)
		{
			if (node != _from)
			{
				return base.Visit(node);
			}
			return _to;
		}
	}

	public static string GetDebugView(Expression? exp)
	{
		if (exp == null)
		{
			return string.Empty;
		}
		return (typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(exp) as string) ?? string.Empty;
	}

	public static TMemberInfo? GetMemberInfo<TMemberInfo>(Expression? expression) where TMemberInfo : MemberInfo
	{
		while (true)
		{
			switch (expression?.NodeType)
			{
			case ExpressionType.Convert:
				expression = ((UnaryExpression)expression).Operand;
				break;
			case ExpressionType.Call:
				expression = ((MethodCallExpression)expression).Object;
				break;
			case ExpressionType.Constant:
				return ((ConstantExpression)expression).Value as TMemberInfo;
			default:
				throw new ArgumentException("The expression is not accessing a member");
			}
		}
	}

	public static PropertyInfo GetProperty<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression)
	{
		return ((expression.Body as MemberExpression)?.Member as PropertyInfo) ?? throw new ArgumentException("Expression must directly access a property.", "expression");
	}

	public static MethodInfo GetMethod<TEntity, TMethod>(Expression<Func<TEntity, TMethod>> expression)
	{
		return GetMemberInfo<MethodInfo>(expression.Body) ?? throw new ArgumentException("Expression must directly access a method.", "expression");
	}

	public static Expression? Switch(Expression from, Expression to, Expression of)
	{
		return new SwapVisitor(from, to).Visit(of);
	}

	public static TFunc CompileStaticMethod<TFunc>(MethodInfo staticMethod, params Type[] parameterTypes)
	{
		if (staticMethod == null)
		{
			throw new ArgumentNullException("staticMethod");
		}
		if (!staticMethod.IsStatic)
		{
			throw new InvalidOperationException("The provided method is not static.");
		}
		IEnumerable<ParameterExpression> enumerable = parameterTypes.Select(Expression.Parameter).ToArray();
		return Expression.Lambda<TFunc>(Expression.Call(null, staticMethod, enumerable), enumerable).Compile();
	}

	public static TFunc CompileFixedInstanceMethod<TFunc>(MethodInfo method, object instance)
	{
		if (method == null)
		{
			throw new ArgumentNullException("method");
		}
		if (instance == null)
		{
			throw new ArgumentNullException("instance");
		}
		return Expression.Lambda<TFunc>(Expression.Call(Expression.Constant(instance), method), Array.Empty<ParameterExpression>()).Compile();
	}

	public static TFunc CompileDynamicInstanceMethod<TFunc, TInstance>(MethodInfo method)
	{
		if (method == null)
		{
			throw new ArgumentNullException("method");
		}
		ParameterExpression parameterExpression = Expression.Parameter(typeof(TInstance));
		return Expression.Lambda<TFunc>(Expression.Call(parameterExpression, method), new ParameterExpression[1] { parameterExpression }).Compile();
	}
}
