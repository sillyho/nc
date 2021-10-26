using System.Windows.Forms;

namespace MyMarkEzd
{
	public interface IMyJCZ
	{
		bool InitLaserMark();

		bool LoadEzdFile(string strEzd);

		bool ChangeTextByName(string strName, string strText);

		void ShowPreviewBmp(PictureBox pictureBox);

		bool Mark(bool bFly);

		bool MarkEntity(string strEntityName);

		string GetLastError();

		bool CenterRotateEnt(string strEntName, double dAngle);

		bool GetCenterPoint(string strEntityName, out double dx, out double dy);

		bool RotateEnt(string strEntName, double dx, double dy, double dAngle);

		bool CloseEZD();

		bool TreggerReadPort(int nPort);

		bool MoveEnt(string pEntName, double dMovex, double dMovey);

		bool CopyEnt(string strSourceName, string strDesName);

		void StopMark();

		void SetOutPort(int nPort, int nState, int nMillisecond);
	}
}
