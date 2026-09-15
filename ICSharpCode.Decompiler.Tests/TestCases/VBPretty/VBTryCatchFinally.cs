using System;
using System.IO;

using Microsoft.VisualBasic.CompilerServices;

public class VBTryCatchFinally
{
	private static bool Condition()
	{
		return true;
	}

	public static void TryFinally()
	{
		try
		{
			Console.WriteLine("Try");
		}
		finally
		{
			Console.WriteLine("Finally");
		}
	}

	public static void TryCatchBare()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception projectError)
		{
			ProjectData.SetProjectError(projectError);
			Console.WriteLine("Catch");
			ProjectData.ClearProjectError();
		}
	}

	public static void TryCatchVariable()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Console.WriteLine(ex2.Message);
			ProjectData.ClearProjectError();
		}
	}

	public static void TryCatchSpecificType()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (IOException ex)
		{
			ProjectData.SetProjectError(ex);
			IOException ex2 = ex;
			Console.WriteLine(ex2.Message);
			ProjectData.ClearProjectError();
		}
	}

	public static void TryCatchUnusedVariable()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (InvalidOperationException ex)
		{
			ProjectData.SetProjectError(ex);
			InvalidOperationException ex2 = ex;
			Console.WriteLine("Catch");
			ProjectData.ClearProjectError();
		}
	}

	public static void TryCatchWhen()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception projectError) when (((Func<bool>)delegate {
			// Could not convert BlockContainer to single expression
			ProjectData.SetProjectError(projectError);
			return Condition();
		}).Invoke())
		{
			Console.WriteLine("Catch");
			ProjectData.ClearProjectError();
		}
	}

	public static void TryCatchVariableWhen()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception ex) when (((Func<bool>)delegate {
			// Could not convert BlockContainer to single expression
			ProjectData.SetProjectError(ex);
			return ex.Message != null;
		}).Invoke())
		{
			Console.WriteLine(ex.Message);
			ProjectData.ClearProjectError();
		}
	}

	public static void TryCatchSpecificTypeWhen()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (IOException ex) when (((Func<bool>)delegate {
			// Could not convert BlockContainer to single expression
			ProjectData.SetProjectError(ex);
			return Condition();
		}).Invoke())
		{
			Console.WriteLine(ex.Message);
			ProjectData.ClearProjectError();
		}
	}

	public static void TryCatchExistingLocal()
	{
		Exception value = null;
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			value = ex;
			Console.WriteLine("Catch");
			ProjectData.ClearProjectError();
		}
		Console.WriteLine(value);
	}

	public static void TryCatchExistingLocalWhen()
	{
		Exception value = null;
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception ex) when (((Func<bool>)delegate {
			// Could not convert BlockContainer to single expression
#if LEGACY_VBC
			value = ex;
			ProjectData.SetProjectError(ex);
#else
			ProjectData.SetProjectError(ex);
			value = ex;
#endif
			return ex.Message != null;
		}).Invoke())
		{
			Console.WriteLine("Catch");
			ProjectData.ClearProjectError();
		}
		Console.WriteLine(value);
	}

	public static void TryCatchFinally()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Console.WriteLine(ex2.Message);
			ProjectData.ClearProjectError();
		}
		finally
		{
			Console.WriteLine("Finally");
		}
	}

	public static void TryCatchBareFinally()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception projectError)
		{
			ProjectData.SetProjectError(projectError);
			Console.WriteLine("Catch");
			ProjectData.ClearProjectError();
		}
		finally
		{
			Console.WriteLine("Finally");
		}
	}

	public static void TryCatchWhenFinally()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception ex) when (((Func<bool>)delegate {
			// Could not convert BlockContainer to single expression
			ProjectData.SetProjectError(ex);
			return Condition();
		}).Invoke())
		{
			Console.WriteLine(ex.Message);
			ProjectData.ClearProjectError();
		}
		finally
		{
			Console.WriteLine("Finally");
		}
	}

	public static void TryMultipleCatch()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (FileNotFoundException ex)
		{
			ProjectData.SetProjectError(ex);
			FileNotFoundException ex2 = ex;
			Console.WriteLine(ex2.FileName);
			ProjectData.ClearProjectError();
		}
		catch (IOException ex3)
		{
			ProjectData.SetProjectError(ex3);
			IOException ex4 = ex3;
			Console.WriteLine(ex4.Message);
			ProjectData.ClearProjectError();
		}
		catch (Exception projectError)
		{
			ProjectData.SetProjectError(projectError);
			Console.WriteLine("Catch");
			ProjectData.ClearProjectError();
		}
	}

	public static void TryMultipleCatchFinally()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (FileNotFoundException ex)
		{
			ProjectData.SetProjectError(ex);
			FileNotFoundException ex2 = ex;
			Console.WriteLine(ex2.FileName);
			ProjectData.ClearProjectError();
		}
		catch (IOException ex3)
		{
			ProjectData.SetProjectError(ex3);
			IOException ex4 = ex3;
			Console.WriteLine(ex4.Message);
			ProjectData.ClearProjectError();
		}
		finally
		{
			Console.WriteLine("Finally");
		}
	}

	public static void TryMultipleCatchWhen()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (IOException ex) when (((Func<bool>)delegate {
			// Could not convert BlockContainer to single expression
			ProjectData.SetProjectError(ex);
			return Condition();
		}).Invoke())
		{
			Console.WriteLine(ex.Message);
			ProjectData.ClearProjectError();
		}
		catch (Exception projectError) when (((Func<bool>)delegate {
			// Could not convert BlockContainer to single expression
			ProjectData.SetProjectError(projectError);
			return Condition();
		}).Invoke())
		{
			Console.WriteLine("Catch When");
			ProjectData.ClearProjectError();
		}
		catch (Exception ex2)
		{
			ProjectData.SetProjectError(ex2);
			Exception ex3 = ex2;
			Console.WriteLine(ex3.Message);
			ProjectData.ClearProjectError();
		}
	}

	public static void EmptyTryFinally()
	{
		try
		{
		}
		finally
		{
			Console.WriteLine("Finally");
		}
	}

	public static void EmptyCatch()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception projectError)
		{
			ProjectData.SetProjectError(projectError);
			ProjectData.ClearProjectError();
		}
	}

	public static void EmptyFinally()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Console.WriteLine(ex2.Message);
			ProjectData.ClearProjectError();
		}
#if !OPT || LEGACY_VBC
		finally
		{
		}
#endif
	}

	public static void ExitTryInTry(bool b)
	{
		try
		{
			if (!b)
			{
				Console.WriteLine("Try");
			}
		}
		catch (Exception projectError)
		{
			ProjectData.SetProjectError(projectError);
			Console.WriteLine("Catch");
			ProjectData.ClearProjectError();
		}
		Console.WriteLine("End");
	}

	public static void ExitTryInCatch(bool b)
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception projectError)
		{
			ProjectData.SetProjectError(projectError);
			if (b)
			{
				ProjectData.ClearProjectError();
			}
			else
			{
				Console.WriteLine("Catch");
				ProjectData.ClearProjectError();
			}
		}
		Console.WriteLine("End");
	}

	public static void ExitTryWithFinally(bool b)
	{
		try
		{
			if (!b)
			{
				Console.WriteLine("Try");
			}
		}
		finally
		{
			Console.WriteLine("Finally");
		}
		Console.WriteLine("End");
	}

	public static void Rethrow()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Console.WriteLine(ex2.Message);
			throw;
		}
	}

	public static void ThrowNew()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception innerException = ex;
			throw new InvalidOperationException("Catch", innerException);
		}
	}

	public static int ReturnFromTry()
	{
		int result;
		try
		{
			result = 1;
		}
		catch (Exception projectError)
		{
			ProjectData.SetProjectError(projectError);
			result = 2;
			ProjectData.ClearProjectError();
		}
		finally
		{
			Console.WriteLine("Finally");
		}
		return result;
	}

	public static int ReturnAfterTry()
	{
#if !(LEGACY_VBC && OPT)
		int result;
#endif
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			Console.WriteLine(ex2.Message);
#if LEGACY_VBC && OPT
			int result = -1;
			ProjectData.ClearProjectError();
			return result;
		}
		return 0;
#else
			result = -1;
			ProjectData.ClearProjectError();
#if LEGACY_VBC
			goto IL_0032;
#elif OPT
			goto IL_0029;
#else
			goto IL_0031;
#endif
		}
		result = 0;
#if LEGACY_VBC
		goto IL_0032;
		IL_0032:
#elif OPT
		goto IL_0029;
		IL_0029:
#else
		goto IL_0031;
		IL_0031:
#endif
		return result;
#endif
	}

	public static void NestedTryInTry()
	{
		try
		{
			try
			{
				Console.WriteLine("Inner Try");
			}
			catch (IOException ex)
			{
				ProjectData.SetProjectError(ex);
				IOException ex2 = ex;
				Console.WriteLine(ex2.Message);
				ProjectData.ClearProjectError();
			}
		}
		catch (Exception ex3)
		{
			ProjectData.SetProjectError(ex3);
			Exception ex4 = ex3;
			Console.WriteLine(ex4.Message);
			ProjectData.ClearProjectError();
		}
	}

	public static void NestedTryInCatch()
	{
		try
		{
			Console.WriteLine("Try");
		}
		catch (Exception ex)
		{
			ProjectData.SetProjectError(ex);
			Exception ex2 = ex;
			try
			{
				Console.WriteLine(ex2.Message);
			}
			catch (Exception ex3)
			{
				ProjectData.SetProjectError(ex3);
				Exception ex4 = ex3;
				Console.WriteLine(ex4.Message);
				ProjectData.ClearProjectError();
			}
			ProjectData.ClearProjectError();
		}
	}

	public static void NestedTryInFinally()
	{
		try
		{
			Console.WriteLine("Try");
		}
		finally
		{
			try
			{
				Console.WriteLine("Inner Try");
			}
			catch (Exception projectError)
			{
				ProjectData.SetProjectError(projectError);
				Console.WriteLine("Inner Catch");
				ProjectData.ClearProjectError();
			}
		}
	}

	public static void TryInLoop()
	{
		int num = 0;
		do
		{
			try
			{
				switch (num)
				{
					case 5:
						break;
					case 8:
						return;
					default:
						Console.WriteLine(num);
						break;
				}
			}
			catch (Exception projectError)
			{
				ProjectData.SetProjectError(projectError);
				Console.WriteLine("Catch");
				ProjectData.ClearProjectError();
			}
			finally
			{
				Console.WriteLine("Finally");
			}
			num = checked(num + 1);
		} while (num <= 9);
	}
}
