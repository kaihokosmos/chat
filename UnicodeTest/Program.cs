using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8; // damit die Console nicht nur 

			Console.WriteLine("ĂüýĉĮīğŇ¶®@ÎâĆćŶƝƢƌƝǺȌȵɔɖɦɣɡЩϬϩДАϽ");
			string meinText = "Hallöchen, Dü ÄÖÜäüöß ĂüýĉĮīğŇ¶®@ÎâĆćŶƝƢƌƝǺȌȵɔɖɦɣɡЩϬϩДАϽ!!!";

			byte[] bytes = Encoding.UTF8.GetBytes(meinText);
			string meinText2 = Encoding.UTF8.GetString(bytes);

			Console.WriteLine(meinText2);

			if (meinText == meinText2)
			{
				Console.WriteLine("Ok");
			}
			else
			{
				Console.WriteLine("Nicht ok");
			}


			Console.ReadKey();

		}
	}
}
