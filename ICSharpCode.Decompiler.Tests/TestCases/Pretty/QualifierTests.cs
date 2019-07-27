using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Decompiler.Tests.Pretty
{
	internal class QualifierTests
	{
		private struct Test
		{
			private int dummy;

			private void DeclaringType(QualifierTests instance)
			{
				instance.NoParameters();
			}

			private void DeclaringType()
			{
				StaticNoParameteres();
				Parameter(null);
				StaticParameter(null);
				// The unnecessary cast is added, because we add casts before we add the qualifier.
				// normally it's preferable to have casts over having qualifiers,
				// this is an ugly edge case.
				QualifierTests.StaticParameter((object)null);
			}

			private void Parameter(object o)
			{

			}

			private static void StaticParameter(object o)
			{
			}

			private void Parameter(QualifierTests test)
			{
				Delegate(Parameter);
				Delegate(StaticParameter);
				Delegate(test.Parameter);
				Delegate(QualifierTests.StaticParameter);
			}

			private static void StaticParameter(QualifierTests test)
			{
			}

			private static void DeclaringTypeStatic()
			{
			}

			private void DeclaringTypeConflict(QualifierTests instance)
			{
				DeclaringType();
				instance.DeclaringType();
				fieldConflict();
				instance.fieldConflict = 5;
			}

			private void DeclaringTypeConflict()
			{
				DeclaringTypeStatic();
				QualifierTests.DeclaringTypeStatic();
			}

			private void fieldConflict()
			{

			}

			private void Delegate(Action<object> action)
			{

			}
		}

		internal class Parent
		{
			public virtual void Virtual()
			{

			}

			public virtual void NewVirtual()
			{

			}

			public void New()
			{

			}

			public void BaseOnly()
			{

			}
		}

		internal class Child : Parent
		{
			public override void Virtual()
			{
				base.Virtual();
			}

			public new void NewVirtual()
			{
				base.NewVirtual();
			}

			public new void New()
			{
				base.New();
			}

			public void BaseQualifiers()
			{
				Virtual();
				base.Virtual();
				NewVirtual();
				base.NewVirtual();
				New();
				base.New();
				BaseOnly();
			}
		}

		private int fieldConflict;
		private int innerConflict;

		private void NoParameters()
		{
			Delegate(Parameter);
			Delegate(StaticParameter);
		}

		private static void StaticNoParameteres()
		{

		}

		private void Parameter(object o)
		{

		}

		private static void StaticParameter(object o)
		{

		}

		private void DeclaringType()
		{

		}

		private static void DeclaringTypeStatic()
		{

		}

		private void conflictWithParameter()
		{

		}

		private void conflictWithVariable(int val)
		{

		}

		private void Conflicts(int conflictWithParameter)
		{
			this.conflictWithParameter();
		}

		private void Conflicts()
		{
			int conflictWithVariable = 5;
			this.conflictWithVariable(conflictWithVariable);
			// workaround for missing identifiers in il
			Capturer(() => conflictWithVariable);
		}

		private void Capturing()
		{
			int fieldConflict = 5;
			Capturer(() => this.fieldConflict + fieldConflict);
			Capturer(delegate {
				int innerConflict = 5;
				return this.fieldConflict + fieldConflict + Capturer2(() => this.innerConflict + innerConflict + this.fieldConflict + fieldConflict);
			});
		}

		private void Capturer(Func<int> func)
		{

		}

		private int Capturer2(Func<int> func)
		{
			return 0;
		}

		private void Delegate(Action<object> action)
		{

		}
	}

	internal static class ZExt
	{
		public static void Do(this int test)
		{

		}
		public static void Do(this object test)
		{

		}
#if CS72
		public static void Do(this ref DateTime test)
		{

		}
#endif

		public static void Do2(this int test, DateTime date)
		{
			test.Do();
			((IEnumerable<int>)null).Any();
			((object)null).Do();
#if CS72
			date.Do();
#endif
		}
	}
}
