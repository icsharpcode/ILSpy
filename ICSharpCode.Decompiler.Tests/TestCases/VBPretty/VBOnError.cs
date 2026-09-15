using System;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

public class VBOnError
{
#if OPT
	public static void ResumeNext()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 1;
						goto IL_0007;
					case 65:
						{
							num = num2;
							switch (num3)
							{
								case 1:
									break;
								default:
									goto end_IL_0000;
							}
							int num4 = num + 1;
							num = 0;
							switch (num4)
							{
								case 1:
									break;
								case 2:
									goto IL_0007;
								case 3:
									goto end_IL_0000_2;
								default:
									goto end_IL_0000;
								case 4:
									goto end_IL_0000_3;
							}
							goto default;
						}
						IL_0007:
						num2 = 2;
						Console.WriteLine("A");
						break;
						end_IL_0000_2:
						break;
				}
				num2 = 3;
				Console.WriteLine("B");
				break;
				end_IL_0000:;
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 65;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_3:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#elif (LEGACY_VBC && !OPT)
	public static void ResumeNext()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 1;
						goto IL_0007;
					case 68:
						{
							num = num2;
							switch (num3)
							{
								case 1:
									break;
								default:
									goto end_IL_0000;
							}
							int num4 = num + 1;
							num = 0;
							switch (num4)
							{
								case 1:
									break;
								case 2:
									goto IL_0007;
								case 3:
									goto end_IL_0000_2;
								default:
									goto end_IL_0000;
								case 4:
									goto end_IL_0000_3;
							}
							goto default;
						}
						IL_0007:
						num2 = 2;
						Console.WriteLine("A");
						break;
						end_IL_0000_2:
						break;
				}
				num2 = 3;
				Console.WriteLine("B");
				break;
				end_IL_0000:;
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 68;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_3:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#else
	public static void ResumeNext()
	{
		int try0001_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0001_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = -2;
						goto IL_000b;
					case 71:
						{
							num = num2;
							switch ((num3 <= -2) ? 1 : num3)
							{
								case 1:
									break;
								default:
									goto end_IL_0001;
							}
							int num4 = num + 1;
							num = 0;
							switch (num4)
							{
								case 1:
									break;
								case 2:
									goto IL_000b;
								case 3:
									goto end_IL_0001_2;
								default:
									goto end_IL_0001;
								case 4:
									goto end_IL_0001_3;
							}
							goto default;
						}
						IL_000b:
						num2 = 2;
						Console.WriteLine("A");
						break;
						end_IL_0001_2:
						break;
				}
				num2 = 3;
				Console.WriteLine("B");
				break;
				end_IL_0001:;
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0001_dispatch = 71;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0001_3:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#endif

#if (LEGACY_VBC && OPT)
	public static int ResumeNextWithResult()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		int num5 = default;
		int num6 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 1;
						goto IL_0007;
					case 64:
						{
							num = num2;
							switch (num3)
							{
								case 1:
									break;
								default:
									goto end_IL_0000;
							}
							int num4 = num + 1;
							num = 0;
							switch (num4)
							{
								case 1:
									break;
								case 2:
									goto IL_0007;
								case 3:
									goto IL_000c;
								case 4:
									goto end_IL_0000_2;
								default:
									goto end_IL_0000;
								case 5:
									goto end_IL_0000_3;
							}
							goto default;
						}
						IL_000c:
						num2 = 3;
						num5 /= 0;
						break;
						IL_0007:
						num2 = 2;
						num5 = 1;
						goto IL_000c;
						end_IL_0000_2:
						break;
				}
				num2 = 4;
				num6 = num5;
				break;
				end_IL_0000:;
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 64;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_3:
			break;
		}
		int result = num6;
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
		return result;
	}
#elif (LEGACY_VBC && !OPT)
	public static int ResumeNextWithResult()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		int num5 = default;
		int num6 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 1;
						goto IL_0007;
					case 67:
						{
							num = num2;
							switch (num3)
							{
								case 1:
									break;
								default:
									goto end_IL_0000;
							}
							int num4 = num + 1;
							num = 0;
							switch (num4)
							{
								case 1:
									break;
								case 2:
									goto IL_0007;
								case 3:
									goto IL_000c;
								case 4:
									goto end_IL_0000_2;
								default:
									goto end_IL_0000;
								case 5:
									goto end_IL_0000_3;
							}
							goto default;
						}
						IL_000c:
						num2 = 3;
						num5 /= 0;
						break;
						IL_0007:
						num2 = 2;
						num5 = 1;
						goto IL_000c;
						end_IL_0000_2:
						break;
				}
				num2 = 4;
				num6 = num5;
				break;
				end_IL_0000:;
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 67;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_3:
			break;
		}
		int result = num6;
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
		return result;
	}
#elif (!LEGACY_VBC && OPT)
	public static int ResumeNextWithResult()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		int num5 = default;
		int result = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 1;
						goto IL_0007;
					case 63:
						{
							num = num2;
							switch (num3)
							{
								case 1:
									break;
								default:
									goto end_IL_0000;
							}
							int num4 = num + 1;
							num = 0;
							switch (num4)
							{
								case 1:
									break;
								case 2:
									goto IL_0007;
								case 3:
									goto IL_000c;
								case 4:
									goto end_IL_0000_2;
								default:
									goto end_IL_0000;
								case 5:
									goto end_IL_0000_3;
							}
							goto default;
						}
						IL_000c:
						num2 = 3;
						num5 /= 0;
						break;
						IL_0007:
						num2 = 2;
						num5 = 1;
						goto IL_000c;
						end_IL_0000_2:
						break;
				}
				num2 = 4;
				result = num5;
				break;
				end_IL_0000:;
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 63;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_3:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
		return result;
	}
#else
	public static int ResumeNextWithResult()
	{
		int try0001_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		int num5 = default;
		int result = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0001_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = -2;
						goto IL_000b;
					case 69:
						{
							num = num2;
							switch ((num3 <= -2) ? 1 : num3)
							{
								case 1:
									break;
								default:
									goto end_IL_0001;
							}
							int num4 = num + 1;
							num = 0;
							switch (num4)
							{
								case 1:
									break;
								case 2:
									goto IL_000b;
								case 3:
									goto IL_0010;
								case 4:
									goto end_IL_0001_2;
								default:
									goto end_IL_0001;
								case 5:
									goto end_IL_0001_3;
							}
							goto default;
						}
						IL_000b:
						num2 = 2;
						num5 = 1;
						goto IL_0010;
						IL_0010:
						num2 = 3;
						num5 /= 0;
						break;
						end_IL_0001_2:
						break;
				}
				num2 = 4;
				result = num5;
				break;
				end_IL_0001:;
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0001_dispatch = 69;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0001_3:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
		return result;
	}
#endif

#if (LEGACY_VBC && OPT)
	public static void ResumeNextErrNumber()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 1;
						goto IL_0007;
					case 109:
						{
							num = num2;
							switch (num3)
							{
								case 1:
									break;
								default:
									goto end_IL_0000;
							}
							int num4 = num + 1;
							num = 0;
							switch (num4)
							{
								case 1:
									break;
								case 2:
									goto IL_0007;
								case 3:
									goto IL_0013;
								case 4:
									goto IL_0022;
								case 5:
									goto end_IL_0000_2;
								default:
									goto end_IL_0000;
								case 6:
								case 7:
									goto end_IL_0000_3;
							}
							goto default;
						}
						IL_0022:
						num2 = 4;
						Console.WriteLine(Information.Err().Description);
						break;
						IL_0007:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0013;
						IL_0013:
						num2 = 3;
						if (Information.Err().Number == 0)
						{
							goto end_IL_0000_3;
						}
						goto IL_0022;
						end_IL_0000_2:
						break;
				}
				num2 = 5;
				Information.Err().Clear();
				break;
				end_IL_0000:;
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 109;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_3:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#elif (LEGACY_VBC && !OPT)
	public static void ResumeNextErrNumber()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 1;
						goto IL_0007;
					case 112:
						{
							num = num2;
							switch (num3)
							{
								case 1:
									break;
								default:
									goto end_IL_0000;
							}
							int num4 = num + 1;
							num = 0;
							switch (num4)
							{
								case 1:
									break;
								case 2:
									goto IL_0007;
								case 3:
									goto IL_0013;
								case 4:
									goto IL_0022;
								case 5:
									goto end_IL_0000_2;
								default:
									goto end_IL_0000;
								case 6:
								case 7:
									goto end_IL_0000_3;
							}
							goto default;
						}
						IL_0022:
						num2 = 4;
						Console.WriteLine(Information.Err().Description);
						break;
						IL_0007:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0013;
						IL_0013:
						num2 = 3;
						if (Information.Err().Number == 0)
						{
							goto end_IL_0000_3;
						}
						goto IL_0022;
						end_IL_0000_2:
						break;
				}
				num2 = 5;
				Information.Err().Clear();
				break;
				end_IL_0000:;
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 112;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_3:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#elif (!LEGACY_VBC && OPT)
	public static void ResumeNextErrNumber()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 1;
						goto IL_0007;
					case 104:
						{
							num = num2;
							switch (num3)
							{
								case 1:
									break;
								default:
									goto end_IL_0000;
							}
							int num4 = num + 1;
							num = 0;
							switch (num4)
							{
								case 1:
									break;
								case 2:
									goto IL_0007;
								case 3:
									goto IL_0013;
								case 4:
									goto IL_0021;
								case 5:
									goto end_IL_0000_2;
								default:
									goto end_IL_0000;
								case 6:
									goto end_IL_0000_3;
							}
							goto default;
						}
						IL_0021:
						num2 = 4;
						Console.WriteLine(Information.Err().Description);
						break;
						IL_0007:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0013;
						IL_0013:
						num2 = 3;
						if (Information.Err().Number == 0)
						{
							goto end_IL_0000_3;
						}
						goto IL_0021;
						end_IL_0000_2:
						break;
				}
				num2 = 5;
				Information.Err().Clear();
				break;
				end_IL_0000:;
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 104;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_3:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#else
	public static void ResumeNextErrNumber()
	{
		int try0001_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0001_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = -2;
						goto IL_000b;
					case 122:
						{
							num = num2;
							switch ((num3 <= -2) ? 1 : num3)
							{
								case 1:
									break;
								default:
									goto end_IL_0001;
							}
							int num4 = num + 1;
							num = 0;
							switch (num4)
							{
								case 1:
									break;
								case 2:
									goto IL_000b;
								case 3:
									goto IL_0018;
								case 4:
									goto IL_002b;
								case 5:
									goto end_IL_0001_2;
								default:
									goto end_IL_0001;
								case 6:
								case 7:
									goto end_IL_0001_3;
							}
							goto default;
						}
						IL_000b:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0018;
						IL_0018:
						num2 = 3;
						if (Information.Err().Number == 0)
						{
							goto end_IL_0001_3;
						}
						goto IL_002b;
						IL_002b:
						num2 = 4;
						Console.WriteLine(Information.Err().Description);
						break;
						end_IL_0001_2:
						break;
				}
				num2 = 5;
				Information.Err().Clear();
				break;
				end_IL_0001:;
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0001_dispatch = 122;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0001_3:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#endif

#if LEGACY_VBC || OPT
	public static void GoToHandler()
	{
		int try0000_dispatch = -1;
		int num2 = default;
		int num = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num2 = 2;
						Console.WriteLine("Body");
						goto end_IL_0000;
					case 36:
						num = -1;
						switch (num2)
						{
							case 2:
								Console.WriteLine(Information.Err().Description);
								goto end_IL_0000;
						}
						break;
				}
			}
			catch (Exception ex) when ((num2 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 36;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#else
	public static void GoToHandler()
	{
		int try0001_dispatch = -1;
		int num2 = default;
		int num = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0001_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num2 = 2;
						Console.WriteLine("Body");
						goto end_IL_0001;
					case 42:
						num = -1;
						switch (num2)
						{
							case 2:
								Console.WriteLine(Information.Err().Description);
								goto end_IL_0001;
						}
						break;
				}
			}
			catch (Exception ex) when ((num2 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0001_dispatch = 42;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0001:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#endif

#if (LEGACY_VBC && OPT)
	public static void GoToHandlerResumeNext()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				int num4;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 2;
						goto IL_0007;
					case 120:
						{
							num = num2;
							switch (num3)
							{
								case 2:
									break;
								case 1:
									goto IL_004c;
								default:
									goto end_IL_0000;
							}
							goto IL_0024;
						}
						IL_0024:
						num2 = 5;
						Console.WriteLine(Information.Err().Number);
						goto IL_0035;
						IL_0035:
						num2 = 6;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						goto IL_004c;
						IL_0013:
						num2 = 3;
						Console.WriteLine("B");
						goto end_IL_0000_2;
						IL_004c:
						num4 = num + 1;
						num = 0;
						switch (num4)
						{
							case 1:
								break;
							case 2:
								goto IL_0007;
							case 3:
								goto IL_0013;
							case 5:
								goto IL_0024;
							case 6:
								goto IL_0035;
							default:
								goto end_IL_0000;
							case 4:
							case 7:
								goto end_IL_0000_2;
						}
						goto default;
						IL_0007:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0013;
						end_IL_0000:
						break;
				}
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 120;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_2:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#elif (LEGACY_VBC && !OPT)
	public static void GoToHandlerResumeNext()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				int num4;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 2;
						goto IL_0007;
					case 125:
						{
							num = num2;
							switch (num3)
							{
								case 2:
									break;
								case 1:
									goto IL_0051;
								default:
									goto end_IL_0000;
							}
							goto IL_0024;
						}
						IL_0024:
						num2 = 5;
						Console.WriteLine(Information.Err().Number);
						goto IL_0035;
						IL_0035:
						num2 = 6;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						goto IL_0051;
						IL_0013:
						num2 = 3;
						Console.WriteLine("B");
						goto end_IL_0000_2;
						IL_0051:
						num4 = num + 1;
						num = 0;
						switch (num4)
						{
							case 1:
								break;
							case 2:
								goto IL_0007;
							case 3:
								goto IL_0013;
							case 5:
								goto IL_0024;
							case 6:
								goto IL_0035;
							default:
								goto end_IL_0000;
							case 4:
							case 7:
								goto end_IL_0000_2;
						}
						goto default;
						IL_0007:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0013;
						end_IL_0000:
						break;
				}
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 125;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_2:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#elif (!LEGACY_VBC && OPT)
	public static void GoToHandlerResumeNext()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				int num4;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 2;
						goto IL_0007;
					case 117:
						{
							num = num2;
							switch (num3)
							{
								case 2:
									break;
								case 1:
									goto IL_0049;
								default:
									goto end_IL_0000;
							}
							goto IL_0021;
						}
						IL_0021:
						num2 = 5;
						Console.WriteLine(Information.Err().Number);
						goto IL_0032;
						IL_0032:
						num2 = 6;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						goto IL_0049;
						IL_0013:
						num2 = 3;
						Console.WriteLine("B");
						goto end_IL_0000_2;
						IL_0049:
						num4 = num + 1;
						num = 0;
						switch (num4)
						{
							case 1:
								break;
							case 2:
								goto IL_0007;
							case 3:
								goto IL_0013;
							case 5:
								goto IL_0021;
							case 6:
								goto IL_0032;
							default:
								goto end_IL_0000;
							case 4:
							case 7:
								goto end_IL_0000_2;
						}
						goto default;
						IL_0007:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0013;
						end_IL_0000:
						break;
				}
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 117;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_2:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#else
	public static void GoToHandlerResumeNext()
	{
		int try0001_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				int num4;
				switch (try0001_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 2;
						goto IL_000a;
					case 127:
						{
							num = num2;
							switch ((num3 <= -2) ? 1 : num3)
							{
								case 2:
									break;
								case 1:
									goto IL_0053;
								default:
									goto end_IL_0001;
							}
							goto IL_0027;
						}
						IL_000a:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0017;
						IL_0053:
						num4 = num + 1;
						num = 0;
						switch (num4)
						{
							case 1:
								break;
							case 2:
								goto IL_000a;
							case 3:
								goto IL_0017;
							case 5:
								goto IL_0027;
							case 6:
								goto IL_0039;
							default:
								goto end_IL_0001;
							case 4:
							case 7:
								goto end_IL_0001_2;
						}
						goto default;
						IL_0027:
						num2 = 5;
						Console.WriteLine(Information.Err().Number);
						goto IL_0039;
						IL_0039:
						num2 = 6;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						goto IL_0053;
						IL_0017:
						num2 = 3;
						Console.WriteLine("B");
						goto end_IL_0001_2;
						end_IL_0001:
						break;
				}
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0001_dispatch = 127;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0001_2:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#endif

#if (LEGACY_VBC && OPT)
	public static void GoToHandlerResume(int retries)
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				int num4;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 2;
						goto IL_0007;
					case 115:
						{
							num = num2;
							switch (num3)
							{
								case 2:
									break;
								case 1:
									goto IL_003f;
								default:
									goto end_IL_0000;
							}
							goto IL_0018;
						}
						IL_003f:
						num4 = num + 1;
						goto IL_0042;
						IL_0018:
						num2 = 4;
						retries = checked(retries - 1);
						goto IL_001f;
						IL_001f:
						num2 = 5;
						if (retries <= 0)
						{
							goto end_IL_0000_2;
						}
						goto IL_0025;
						IL_0025:
						num2 = 6;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						num4 = num;
						goto IL_0042;
						IL_0007:
						num2 = 2;
						Console.WriteLine("Body");
						goto end_IL_0000_2;
						IL_0042:
						num = 0;
						switch (num4)
						{
							case 1:
								break;
							case 2:
								goto IL_0007;
							case 4:
								goto IL_0018;
							case 5:
								goto IL_001f;
							case 6:
								goto IL_0025;
							default:
								goto end_IL_0000;
							case 3:
							case 7:
							case 8:
							case 9:
								goto end_IL_0000_2;
						}
						goto default;
						end_IL_0000:
						break;
				}
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 115;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_2:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#elif (LEGACY_VBC && !OPT)
	public static void GoToHandlerResume(int retries)
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				int num4;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 2;
						goto IL_0007;
					case 117:
						{
							num = num2;
							switch (num3)
							{
								case 2:
									break;
								case 1:
									goto IL_0041;
								default:
									goto end_IL_0000;
							}
							goto IL_0018;
						}
						IL_0041:
						num4 = num + 1;
						goto IL_0044;
						IL_0018:
						num2 = 4;
						retries = checked(retries - 1);
						goto IL_001f;
						IL_001f:
						num2 = 5;
						if (retries <= 0)
						{
							goto end_IL_0000_2;
						}
						goto IL_0025;
						IL_0025:
						num2 = 6;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						num4 = num;
						goto IL_0044;
						IL_0007:
						num2 = 2;
						Console.WriteLine("Body");
						goto end_IL_0000_2;
						IL_0044:
						num = 0;
						switch (num4)
						{
							case 1:
								break;
							case 2:
								goto IL_0007;
							case 4:
								goto IL_0018;
							case 5:
								goto IL_001f;
							case 6:
								goto IL_0025;
							default:
								goto end_IL_0000;
							case 3:
							case 7:
							case 8:
							case 9:
								goto end_IL_0000_2;
						}
						goto default;
						end_IL_0000:
						break;
				}
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 117;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_2:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#elif (!LEGACY_VBC && OPT)
	public static void GoToHandlerResume(int retries)
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				int num4;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 2;
						goto IL_0007;
					case 104:
						{
							num = num2;
							switch (num3)
							{
								case 2:
									break;
								case 1:
									goto IL_003c;
								default:
									goto end_IL_0000;
							}
							goto IL_0015;
						}
						IL_003c:
						num4 = num + 1;
						goto IL_003f;
						IL_0015:
						num2 = 4;
						retries = checked(retries - 1);
						goto IL_001c;
						IL_001c:
						num2 = 5;
						if (retries <= 0)
						{
							goto end_IL_0000_2;
						}
						goto IL_0022;
						IL_0022:
						num2 = 6;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						num4 = num;
						goto IL_003f;
						IL_0007:
						num2 = 2;
						Console.WriteLine("Body");
						goto end_IL_0000_2;
						IL_003f:
						num = 0;
						switch (num4)
						{
							case 1:
								break;
							case 2:
								goto IL_0007;
							case 4:
								goto IL_0015;
							case 5:
								goto IL_001c;
							case 6:
								goto IL_0022;
							default:
								goto end_IL_0000;
							case 3:
							case 7:
								goto end_IL_0000_2;
						}
						goto default;
						end_IL_0000:
						break;
				}
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 104;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_2:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#else
	public static void GoToHandlerResume(int retries)
	{
		int try0001_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				int num4;
				switch (try0001_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 2;
						goto IL_000a;
					case 122:
						{
							num = num2;
							switch ((num3 <= -2) ? 1 : num3)
							{
								case 2:
									break;
								case 1:
									goto IL_004a;
								default:
									goto end_IL_0001;
							}
							goto IL_001a;
						}
						IL_000a:
						num2 = 2;
						Console.WriteLine("Body");
						goto end_IL_0001_2;
						IL_004d:
						num = 0;
						switch (num4)
						{
							case 1:
								break;
							case 2:
								goto IL_000a;
							case 4:
								goto IL_001a;
							case 5:
								goto IL_0021;
							case 6:
								goto IL_002b;
							default:
								goto end_IL_0001;
							case 3:
							case 7:
							case 8:
								goto end_IL_0001_2;
						}
						goto default;
						IL_004a:
						num4 = num + 1;
						goto IL_004d;
						IL_001a:
						num2 = 4;
						retries = checked(retries - 1);
						goto IL_0021;
						IL_0021:
						num2 = 5;
						if (retries <= 0)
						{
							goto end_IL_0001_2;
						}
						goto IL_002b;
						IL_002b:
						num2 = 6;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						num4 = num;
						goto IL_004d;
						end_IL_0001:
						break;
				}
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0001_dispatch = 122;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0001_2:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#endif

#if (LEGACY_VBC && OPT)
	public static void GoToHandlerResumeLabel()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				int num4;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 2;
						goto IL_0007;
					case 128:
						{
							num = num2;
							switch (num3)
							{
								case 2:
									break;
								case 1:
									goto IL_0050;
								default:
									goto end_IL_0000;
							}
							goto IL_0024;
						}
						IL_0050:
						num4 = num + 1;
						num = 0;
						switch (num4)
						{
							case 1:
								break;
							case 2:
								goto IL_0007;
							case 3:
							case 7:
								goto IL_0013;
							case 5:
								goto IL_0024;
							case 6:
								goto IL_0035;
							default:
								goto end_IL_0000;
							case 4:
							case 8:
								goto end_IL_0000_2;
						}
						goto default;
						IL_0024:
						num2 = 5;
						Console.WriteLine(Information.Err().Description);
						goto IL_0035;
						IL_0035:
						num2 = 6;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						num = 0;
						goto IL_0013;
						IL_0007:
						num2 = 2;
						Console.WriteLine("Body");
						goto IL_0013;
						IL_0013:
						num2 = 3;
						Console.WriteLine("Done");
						goto end_IL_0000_2;
						end_IL_0000:
						break;
				}
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 128;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_2:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#elif (LEGACY_VBC && !OPT)
	public static void GoToHandlerResumeLabel()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				int num4;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 2;
						goto IL_0007;
					case 131:
						{
							num = num2;
							switch (num3)
							{
								case 2:
									break;
								case 1:
									goto IL_0053;
								default:
									goto end_IL_0000;
							}
							goto IL_0024;
						}
						IL_0053:
						num4 = num + 1;
						num = 0;
						switch (num4)
						{
							case 1:
								break;
							case 2:
								goto IL_0007;
							case 3:
							case 7:
								goto IL_0013;
							case 5:
								goto IL_0024;
							case 6:
								goto IL_0035;
							default:
								goto end_IL_0000;
							case 4:
							case 8:
								goto end_IL_0000_2;
						}
						goto default;
						IL_0024:
						num2 = 5;
						Console.WriteLine(Information.Err().Description);
						goto IL_0035;
						IL_0035:
						num2 = 6;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						num = 0;
						goto IL_0013;
						IL_0007:
						num2 = 2;
						Console.WriteLine("Body");
						goto IL_0013;
						IL_0013:
						num2 = 3;
						Console.WriteLine("Done");
						goto end_IL_0000_2;
						end_IL_0000:
						break;
				}
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 131;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_2:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#elif (!LEGACY_VBC && OPT)
	public static void GoToHandlerResumeLabel()
	{
		int try0000_dispatch = -1;
		int num2 = default;
		int num = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num2 = 2;
						Console.WriteLine("Body");
						break;
					case 69:
						num = -1;
						switch (num2)
						{
							case 2:
								Console.WriteLine(Information.Err().Description);
								ProjectData.ClearProjectError();
								if (num == 0)
								{
									throw ProjectData.CreateProjectError(-2146828268);
								}
								num = 0;
								break;
							default:
								goto end_IL_0000;
						}
						break;
				}
				Console.WriteLine("Done");
				break;
				end_IL_0000:;
			}
			catch (Exception ex) when ((num2 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 69;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#else
	public static void GoToHandlerResumeLabel()
	{
		int try0001_dispatch = -1;
		int num2 = default;
		int num = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0001_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num2 = 2;
						Console.WriteLine("Body");
						break;
					case 78:
						num = -1;
						switch (num2)
						{
							case 2:
								Console.WriteLine(Information.Err().Description);
								ProjectData.ClearProjectError();
								if (num == 0)
								{
									throw ProjectData.CreateProjectError(-2146828268);
								}
								num = 0;
								break;
							default:
								goto end_IL_0001;
						}
						break;
				}
				Console.WriteLine("Done");
				break;
				end_IL_0001:;
			}
			catch (Exception ex) when ((num2 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0001_dispatch = 78;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#endif

#if OPT
	public static void GoToZero()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 1;
						goto IL_0007;
					case 76:
						{
							num = num2;
							switch (num3)
							{
								case 1:
									break;
								default:
									goto end_IL_0000;
							}
							int num4 = num + 1;
							num = 0;
							switch (num4)
							{
								case 1:
									break;
								case 2:
									goto IL_0007;
								case 3:
									goto IL_0013;
								case 4:
									goto end_IL_0000_2;
								default:
									goto end_IL_0000;
								case 5:
									goto end_IL_0000_3;
							}
							goto default;
						}
						IL_0013:
						ProjectData.ClearProjectError();
						num3 = 0;
						break;
						IL_0007:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0013;
						end_IL_0000_2:
						break;
				}
				num2 = 4;
				Console.WriteLine("B");
				break;
				end_IL_0000:;
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 76;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_3:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#elif (LEGACY_VBC && !OPT)
	public static void GoToZero()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 1;
						goto IL_0007;
					case 79:
						{
							num = num2;
							switch (num3)
							{
								case 1:
									break;
								default:
									goto end_IL_0000;
							}
							int num4 = num + 1;
							num = 0;
							switch (num4)
							{
								case 1:
									break;
								case 2:
									goto IL_0007;
								case 3:
									goto IL_0013;
								case 4:
									goto end_IL_0000_2;
								default:
									goto end_IL_0000;
								case 5:
									goto end_IL_0000_3;
							}
							goto default;
						}
						IL_0013:
						ProjectData.ClearProjectError();
						num3 = 0;
						break;
						IL_0007:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0013;
						end_IL_0000_2:
						break;
				}
				num2 = 4;
				Console.WriteLine("B");
				break;
				end_IL_0000:;
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 79;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_3:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#else
	public static void GoToZero()
	{
		int try0001_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0001_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = -2;
						goto IL_000b;
					case 83:
						{
							num = num2;
							switch ((num3 <= -2) ? 1 : num3)
							{
								case 1:
									break;
								default:
									goto end_IL_0001;
							}
							int num4 = num + 1;
							num = 0;
							switch (num4)
							{
								case 1:
									break;
								case 2:
									goto IL_000b;
								case 3:
									goto IL_0018;
								case 4:
									goto end_IL_0001_2;
								default:
									goto end_IL_0001;
								case 5:
									goto end_IL_0001_3;
							}
							goto default;
						}
						IL_000b:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0018;
						IL_0018:
						ProjectData.ClearProjectError();
						num3 = 0;
						break;
						end_IL_0001_2:
						break;
				}
				num2 = 4;
				Console.WriteLine("B");
				break;
				end_IL_0001:;
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0001_dispatch = 83;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0001_3:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#endif

#if LEGACY_VBC || OPT
	public static void GoToMinusOne()
	{
		int try0000_dispatch = -1;
		int num2 = default;
		int num = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num2 = 2;
						Console.WriteLine("A");
						goto end_IL_0000;
					case 57:
						num = -1;
						switch (num2)
						{
							case 2:
								ProjectData.ClearProjectError();
								num = 0;
								ProjectData.ClearProjectError();
								num2 = 3;
								Console.WriteLine("B");
								goto end_IL_0000;
							case 3:
								Console.WriteLine("C");
								goto end_IL_0000;
						}
						break;
				}
			}
			catch (Exception ex) when ((num2 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 57;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#else
	public static void GoToMinusOne()
	{
		int try0001_dispatch = -1;
		int num2 = default;
		int num = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0001_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num2 = 2;
						Console.WriteLine("A");
						goto end_IL_0001;
					case 67:
						num = -1;
						switch (num2)
						{
							case 2:
								ProjectData.ClearProjectError();
								num = 0;
								ProjectData.ClearProjectError();
								num2 = 3;
								Console.WriteLine("B");
								goto end_IL_0001;
							case 3:
								Console.WriteLine("C");
								goto end_IL_0001;
						}
						break;
				}
			}
			catch (Exception ex) when ((num2 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0001_dispatch = 67;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0001:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#endif

#if (LEGACY_VBC && OPT)
	public static void SwitchHandlers()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				int num4;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 2;
						goto IL_0007;
					case 176:
						{
							num = num2;
							switch (num3)
							{
								case 2:
									break;
								case 3:
									goto IL_004c;
								case 1:
									goto IL_0074;
								default:
									goto end_IL_0000;
							}
							goto IL_002b;
						}
						IL_002b:
						num2 = 6;
						Console.WriteLine("Handler1");
						goto IL_0037;
						IL_0037:
						num2 = 7;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						goto IL_0074;
						IL_001a:
						num2 = 4;
						Console.WriteLine("B");
						goto end_IL_0000_2;
						IL_0074:
						num4 = num + 1;
						num = 0;
						switch (num4)
						{
							case 1:
								break;
							case 2:
								goto IL_0007;
							case 3:
								goto IL_0013;
							case 4:
								goto IL_001a;
							case 6:
								goto IL_002b;
							case 7:
								goto IL_0037;
							case 8:
							case 9:
								goto IL_004c;
							case 10:
								goto IL_0059;
							default:
								goto end_IL_0000;
							case 5:
							case 11:
								goto end_IL_0000_2;
						}
						goto default;
						IL_004c:
						num2 = 9;
						Console.WriteLine("Handler2");
						goto IL_0059;
						IL_0059:
						num2 = 10;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						goto IL_0074;
						IL_0013:
						ProjectData.ClearProjectError();
						num3 = 3;
						goto IL_001a;
						IL_0007:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0013;
						end_IL_0000:
						break;
				}
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 176;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_2:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#elif (LEGACY_VBC && !OPT)
	public static void SwitchHandlers()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				int num4;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 2;
						goto IL_0007;
					case 183:
						{
							num = num2;
							switch (num3)
							{
								case 2:
									break;
								case 3:
									goto IL_004e;
								case 1:
									goto IL_007b;
								default:
									goto end_IL_0000;
							}
							goto IL_002b;
						}
						IL_002b:
						num2 = 6;
						Console.WriteLine("Handler1");
						goto IL_0037;
						IL_0037:
						num2 = 7;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						goto IL_007b;
						IL_001a:
						num2 = 4;
						Console.WriteLine("B");
						goto end_IL_0000_2;
						IL_007b:
						num4 = num + 1;
						num = 0;
						switch (num4)
						{
							case 1:
								break;
							case 2:
								goto IL_0007;
							case 3:
								goto IL_0013;
							case 4:
								goto IL_001a;
							case 6:
								goto IL_002b;
							case 7:
								goto IL_0037;
							case 8:
							case 9:
								goto IL_004e;
							case 10:
								goto IL_005b;
							default:
								goto end_IL_0000;
							case 5:
							case 11:
								goto end_IL_0000_2;
						}
						goto default;
						IL_004e:
						num2 = 9;
						Console.WriteLine("Handler2");
						goto IL_005b;
						IL_005b:
						num2 = 10;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						goto IL_007b;
						IL_0013:
						ProjectData.ClearProjectError();
						num3 = 3;
						goto IL_001a;
						IL_0007:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0013;
						end_IL_0000:
						break;
				}
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 183;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_2:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#elif (!LEGACY_VBC && OPT)
	public static void SwitchHandlers()
	{
		int try0000_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				int num4;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 2;
						goto IL_0007;
					case 165:
						{
							num = num2;
							switch (num3)
							{
								case 2:
									break;
								case 3:
									goto IL_0049;
								case 1:
									goto IL_006d;
								default:
									goto end_IL_0000;
							}
							goto IL_0028;
						}
						IL_0028:
						num2 = 6;
						Console.WriteLine("Handler1");
						goto IL_0034;
						IL_0034:
						num2 = 7;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						goto IL_006d;
						IL_001a:
						num2 = 4;
						Console.WriteLine("B");
						goto end_IL_0000_2;
						IL_006d:
						num4 = num + 1;
						num = 0;
						switch (num4)
						{
							case 1:
								break;
							case 2:
								goto IL_0007;
							case 3:
								goto IL_0013;
							case 4:
								goto IL_001a;
							case 6:
								goto IL_0028;
							case 7:
								goto IL_0034;
							case 8:
								goto IL_0049;
							case 9:
								goto IL_0055;
							default:
								goto end_IL_0000;
							case 5:
							case 10:
								goto end_IL_0000_2;
						}
						goto default;
						IL_0049:
						num2 = 8;
						Console.WriteLine("Handler2");
						goto IL_0055;
						IL_0055:
						num2 = 9;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						goto IL_006d;
						IL_0013:
						ProjectData.ClearProjectError();
						num3 = 3;
						goto IL_001a;
						IL_0007:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0013;
						end_IL_0000:
						break;
				}
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 165;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000_2:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#else
	public static void SwitchHandlers()
	{
		int try0001_dispatch = -1;
		int num3 = default;
		int num = default;
		int num2 = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				int num4;
				switch (try0001_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num3 = 2;
						goto IL_000a;
					case 184:
						{
							num = num2;
							switch ((num3 <= -2) ? 1 : num3)
							{
								case 2:
									break;
								case 3:
									goto IL_0055;
								case 1:
									goto IL_0080;
								default:
									goto end_IL_0001;
							}
							goto IL_002f;
						}
						IL_000a:
						num2 = 2;
						Console.WriteLine("A");
						goto IL_0017;
						IL_0017:
						ProjectData.ClearProjectError();
						num3 = 3;
						goto IL_001f;
						IL_002f:
						num2 = 6;
						Console.WriteLine("Handler1");
						goto IL_003c;
						IL_003c:
						num2 = 7;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						goto IL_0080;
						IL_001f:
						num2 = 4;
						Console.WriteLine("B");
						goto end_IL_0001_2;
						IL_0080:
						num4 = num + 1;
						num = 0;
						switch (num4)
						{
							case 1:
								break;
							case 2:
								goto IL_000a;
							case 3:
								goto IL_0017;
							case 4:
								goto IL_001f;
							case 6:
								goto IL_002f;
							case 7:
								goto IL_003c;
							case 8:
								goto IL_0055;
							case 9:
								goto IL_0062;
							default:
								goto end_IL_0001;
							case 5:
							case 10:
								goto end_IL_0001_2;
						}
						goto default;
						IL_0055:
						num2 = 8;
						Console.WriteLine("Handler2");
						goto IL_0062;
						IL_0062:
						num2 = 9;
						ProjectData.ClearProjectError();
						if (num == 0)
						{
							throw ProjectData.CreateProjectError(-2146828268);
						}
						goto IL_0080;
						end_IL_0001:
						break;
				}
			}
			catch (Exception ex) when ((num3 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0001_dispatch = 184;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0001_2:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
#endif

#if (LEGACY_VBC && OPT)
	public static int GoToHandlerWithResult()
	{
		int try0000_dispatch = -1;
		int num2 = default;
		int num3;
		int num = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num2 = 2;
						num3 = int.Parse("x");
						goto end_IL_0000;
					case 24:
						num = -1;
						switch (num2)
						{
							case 2:
								num3 = -1;
								goto end_IL_0000;
						}
						break;
				}
			}
			catch (Exception ex) when ((num2 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 24;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000:
			break;
		}
		int result = num3;
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
		return result;
	}
#elif (LEGACY_VBC && !OPT)
	public static int GoToHandlerWithResult()
	{
		int try0000_dispatch = -1;
		int num2 = default;
		int num3;
		int num = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num2 = 2;
						num3 = int.Parse("x");
						goto end_IL_0000;
					case 26:
						num = -1;
						switch (num2)
						{
							case 2:
								num3 = -1;
								goto end_IL_0000;
						}
						break;
				}
			}
			catch (Exception ex) when ((num2 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 26;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000:
			break;
		}
		int result = num3;
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
		return result;
	}
#elif (!LEGACY_VBC && OPT)
	public static int GoToHandlerWithResult()
	{
		int try0000_dispatch = -1;
		int num2 = default;
		int result;
		int num = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0000_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num2 = 2;
						result = int.Parse("x");
						goto end_IL_0000;
					case 24:
						num = -1;
						switch (num2)
						{
							case 2:
								result = -1;
								goto end_IL_0000;
						}
						break;
				}
			}
			catch (Exception ex) when ((num2 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0000_dispatch = 24;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0000:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
		return result;
	}
#else
	public static int GoToHandlerWithResult()
	{
		int try0001_dispatch = -1;
		int num2 = default;
		int result;
		int num = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
				switch (try0001_dispatch)
				{
					default:
						ProjectData.ClearProjectError();
						num2 = 2;
						result = int.Parse("x");
						goto end_IL_0001;
					case 30:
						num = -1;
						switch (num2)
						{
							case 2:
								result = -1;
								goto end_IL_0001;
						}
						break;
				}
			}
			catch (Exception ex) when ((num2 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
				try0001_dispatch = 30;
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
			end_IL_0001:
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
		return result;
	}
#endif
}
