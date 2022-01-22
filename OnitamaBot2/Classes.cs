using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnitamaBot2
{
	class GameState
	{
		public short[] position = new short[15];
		public bool Oturn;
		public GameState? parent;
		public List<GameState> children = new();
		public double score = 100;
	}
	struct Card
	{
		public List<short> moveList = new();

		public Card(short id)
		{
			switch (id)
			{
				case 0: //Boar
					moveList.Add(-1);
					moveList.Add(1);
                    moveList.Add(5);
					break;
				case 1: //Cobra
					moveList.Add(-4);
					moveList.Add(-1);
					moveList.Add(6);
					break;
				case 2: //Crab
					moveList.Add(-2);
					moveList.Add(2);
					moveList.Add(5);
					break;
				case 3: //Crane
					moveList.Add(-6);
					moveList.Add(-4);
					moveList.Add(5);
					break;
				case 4: //Dragon
					moveList.Add(-6);
					moveList.Add(-4);
					moveList.Add(3);
					moveList.Add(7);
					break;
				case 5: //Eel
					moveList.Add(-6);
					moveList.Add(1);
					moveList.Add(4);
					break;
				case 6: //Elephant
					moveList.Add(-1);
					moveList.Add(1);
					moveList.Add(4);
					moveList.Add(6);
					break;
				case 7: //Frog
					moveList.Add(-4);
					moveList.Add(-2);
					moveList.Add(4);
					break;
				case 8: //Goose
					moveList.Add(-4);
					moveList.Add(-1);
					moveList.Add(1);
					moveList.Add(4);
					break;
				case 9: //Horse
					moveList.Add(-5);
					moveList.Add(-1);
					moveList.Add(5);
					break;
				case 10: //Mantis
					moveList.Add(-5);
					moveList.Add(4);
					moveList.Add(6);
					break;
				case 11: //Monkey
					moveList.Add(-6);
					moveList.Add(-4);
					moveList.Add(4);
					moveList.Add(6);
					break;
				case 12: //Ox
					moveList.Add(-5);
					moveList.Add(1);
					moveList.Add(5);
					break;
				case 13: //Rabbit
					moveList.Add(-6);
					moveList.Add(2);
					moveList.Add(6);
					break;
				case 14: //Rooster
					moveList.Add(-6);
					moveList.Add(-1);
					moveList.Add(1);
					moveList.Add(6);
					break;
				case 15: //Tiger
					moveList.Add(-5);
					moveList.Add(10);
					break;
			}
		}
	}
}
