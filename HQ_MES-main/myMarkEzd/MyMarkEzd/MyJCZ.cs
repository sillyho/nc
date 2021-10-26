using Laser_JCZ;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMarkEzd
{
	[ClassInterface(ClassInterfaceType.None)]
	public class MyJCZ : IMyJCZ
	{
		public string GetLastError()
		{
			return MarkJcz.GetLastError();
		}

		public bool InitLaserMark()
		{
			if (MarkJcz.InitLaser())
			{
				return true;
			}
			return false;
		}

		public bool LoadEzdFile(string strEzd)
		{
			if (MarkJcz.LoadEzdFile(strEzd))
			{
				return true;
			}
			return false;
		}

		public bool ChangeTextByName(string strName, string strText)
		{
			if (MarkJcz.ChangeTextByName(strName, strText))
			{
				return true;
			}
			return false;
		}

		public void ShowPreviewBmp(PictureBox pictureBox)
		{
			MarkJcz.ShowPreviewBmp(pictureBox);
		}

		public bool Mark(bool bFly = false)
		{
			if (MarkJcz.Mark(bFly))
			{
				return true;
			}
			return false;
		}

		public bool MarkEntity(string strEntityName)
		{
			if (MarkJcz.MarkEntity(strEntityName))
			{
				return true;
			}
			return false;
		}

		public bool CenterRotateEnt(string strEntName, double dAngle)
		{
			if (MarkJcz.RoTateEnt(strEntName, dAngle))
			{
				return true;
			}
			return false;
		}

		public bool GetCenterPoint(string strEntityName, out double dx, out double dy)
		{
			dx = 0.0;
			dy = 0.0;
			double dx2 = 0.0;
			double dy2 = 0.0;
			if (MarkJcz.GetEntPos(strEntityName, ref dx2, ref dy2))
			{
				dx = dx2;
				dy = dy2;
				return true;
			}
			return false;
		}

		public bool RotateEnt(string strEntName, double dx, double dy, double dAngle)
		{
			if (MarkJcz.RoTateEnt(strEntName, dx, dy, dAngle))
			{
				return true;
			}
			return false;
		}

		public bool CloseEZD()
		{
			if (MarkJcz.Close())
			{
				return true;
			}
			return false;
		}

		public bool ReadPort(int nPort)
		{
			return MarkJcz.ReadPort(nPort);
		}
		public int ReadPort()
        {
			int nStatus = 0;
			MarkJcz.ReadPort(ref nStatus);

			return nStatus;
        }

		public bool TreggerReadPort(int nPort)
		{
			Task.Factory.StartNew(delegate
			{
				bool flag = false;
				bool flag2 = false;
				while (true)
				{
					flag = MarkJcz.ReadPort(nPort);
					if (flag && !flag2)
					{
						break;
					}
					flag2 = flag;
					Thread.Sleep(5);
				}
				return true;
			}).Wait(50);
			return false;
		}

		public bool MoveEnt(string pEntName, double dMovex, double dMovey)
		{
			return MarkJcz.MoveEnt(pEntName, dMovex, dMovey);
		}

		public bool CopyEnt(string strSourceName, string strDesName)
		{
			return MarkJcz.CopyEnt(strSourceName, strDesName);
		}

		public void StopMark()
		{
			MarkJcz.StopMark();
		}

		public void SetOutPort(int nPort, int nState, int nMillisecond)
		{
			bool flag = (nState == 1) ? true : false;
			MarkJcz.WritePort(nPort, flag);
			Thread.Sleep(nMillisecond);
			MarkJcz.WritePort(nPort, !flag);
		}
	}
}
