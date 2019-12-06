using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelForEach
{
	class Program
	{
		static void Main(string[] args)
		{
			string[] names = { "Thomas", "Peter", "Anton", "Berta", "Cäsar", "Dora", "Email", "Friedrich" };

			// Nicht-parallel
			Console.WriteLine("Nicht-parallel (for-Schleife)");
			for (int i = 0; i < names.Length; i++)
			{
				Console.WriteLine(names[i].ToUpper());

				if (names[i] == "Berta")
				{
					break;
				}
			}

			Console.WriteLine();
			Console.WriteLine("Parallel"); // Startindex, Länge
										   // Parallel
			Parallel.For(0, names.Length, i =>
			{
				Console.WriteLine(names[i].ToUpper());

				if (names[i] == "Berta")
				{
					return; // break geht syntaktisch nicht; return geht syntaktisch, aber hat keinen Effekt,
							// da return hier nur einen Thread abbricht, aber die anderen nicht; diese laufen weiter
				}
			});

			Console.WriteLine();
			Console.WriteLine("Parallel mit loopState");
			while (true) // um durch "Enter" in der Konsole die Parallel-Schleife immer wieder auslösen zu können
			{
				Parallel.For(0, names.Length, (i, loopState) =>
				{
					if (!loopState.IsStopped) // Werte, d. hier gestoppt werden, werden nicht ausgegeben; während des Prozesses
											  // kann ein gestarteter Thread1 gestoppt werden (wird nicht ausgegeben), wenn der
											  // loopState.Break() ausgelöst wurde, bevor Thread1 zu (!loopState.IsStopped) erreicht.
					{
						Console.WriteLine("{0}: {1}", i, names[i].ToUpper());
						if (names[i] == "Berta")
						{
							loopState.Break(); // Threads (nicht "Berta"), die schon gestartet wurden, können nach "Berta" noch
											   // ausgegeben werden; die Schleife wird nach der aktuellen Iteration beendet
											   // loopState.Stop(); // die Parallelschleife wird beendet, sobald das im System möglich ist
						}
					}
				});

				Console.ReadLine();
			}


			// Console.ReadKey();
		}
	}
}
