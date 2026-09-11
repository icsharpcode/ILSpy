using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;

public class Issue3659
{
	public static void Func(ref Issue3659 obj, object value)
	{
	}

	public static int ShowMessage(string a, int b, string c, object d, int e)
	{
		return 0;
	}

	internal void VBFunction(object value)
	{
#if OPT || LEGACY_VBC
		int try0000_dispatch = -1;
#else
		int try0001_dispatch = -1;
#endif
		int num2 = default;
		int num = default;
		while (true)
		{
			try
			{
				/*Note: ILSpy has introduced the following switch to emulate a goto from catch-block to try-block*/;
#if OPT || LEGACY_VBC
				switch (try0000_dispatch)
#else
				switch (try0001_dispatch)
#endif
				{
					default:
					{
						ProjectData.ClearProjectError();
						num2 = 2;
						Issue3659 obj = this;
						Func(ref obj, RuntimeHelpers.GetObjectValue(value));
#if OPT || LEGACY_VBC
						goto end_IL_0000;
#else
						goto end_IL_0001;
#endif
					}
#if OPT || LEGACY_VBC
					case 45:
#else
					case 50:
#endif
						num = -1;
						switch (num2)
						{
							case 2:
								ShowMessage("VBFunction", 0, "Exception", null, 0);
#if OPT || LEGACY_VBC
								goto end_IL_0000;
#else
								goto end_IL_0001;
#endif
						}
						break;
				}
			}
			catch (Exception ex) when ((num2 != 0) & (num == 0))
			{
				ProjectData.SetProjectError(ex);
#if OPT || LEGACY_VBC
				try0000_dispatch = 45;
#else
				try0001_dispatch = 50;
#endif
				continue;
			}
			throw ProjectData.CreateProjectError(-2146828237);
			continue;
#if OPT || LEGACY_VBC
			end_IL_0000:
#else
			end_IL_0001:
#endif
			break;
		}
		if (num != 0)
		{
			ProjectData.ClearProjectError();
		}
	}
}
