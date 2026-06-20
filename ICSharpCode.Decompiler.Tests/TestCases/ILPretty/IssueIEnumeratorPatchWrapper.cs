using System;
using System.Collections;
using IFix;
using IFix.Core;
using XUtliPoolLib;

namespace IFix
{
	public class ILFixDynamicMethodWrapper
	{
		public IEnumerator __Gen_Wrap_44(object inst)
		{
			return null;
		}
	}

	public class WrappersManagerImpl
	{
		public static bool IsPatched(int id)
		{
			return false;
		}

		public static ILFixDynamicMethodWrapper GetPatch(int id)
		{
			return null;
		}
	}
}

namespace IFix.Core
{
	public class XIDAttribute : Attribute
	{
		private int id;

		public XIDAttribute(int id)
		{
			this.id = id;
		}
	}
}

namespace XUpdater
{
	public class XUpdater
	{
		public XVersionNewData _server;

		public string _version;

		public XVersionNewData _client;

		public XVersionNewData _buildin;

		public bool _need_play_cg;

		public IXVideo _video;

		[XID(373)]
		private IEnumerator Finish()
		{
			if (WrappersManagerImpl.IsPatched(373))
			{
				return WrappersManagerImpl.GetPatch(373).__Gen_Wrap_44(this);
			}
			if (_server != null)
			{
				_version = _server.ToString();
			}
			_server = null;
			_client = null;
			_buildin = null;
			if (_need_play_cg && _video != null)
			{
				_video.Play(A_1: false);
				yield return null;
				while (_video.isPlaying)
				{
					yield return null;
				}
			}
			yield break;
		}
	}
}

namespace XUtliPoolLib
{
	public interface IXVideo
	{
		bool isPlaying { get; }

		void Play(bool A_1);
	}

	public class XVersionNewData
	{
	}
}
