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
		public short Oturn;
		public GameState? parent;
		public List<GameState> children = new();
		public double score = 100;
	}
	struct Card
	{
		public List<short> moveList = new();
		public string name = "";

		public Card(short id)
		{
			switch (id)
			{
				case 0: //Boar
					moveList.Add(-1);
					moveList.Add(1);
                    moveList.Add(5);
					name = "Boar";
					break;
				case 1: //Cobra
					moveList.Add(-4);
					moveList.Add(-1);
					moveList.Add(6);
					name = "Cobra";
					break;
				case 2: //Crab
					moveList.Add(-2);
					moveList.Add(2);
					moveList.Add(5);
					name = "Crab";
					break;
				case 3: //Crane
					moveList.Add(-6);
					moveList.Add(-4);
					moveList.Add(5);
					name = "Crane";
					break;
				case 4: //Dragon
					moveList.Add(-6);
					moveList.Add(-4);
					moveList.Add(3);
					moveList.Add(7);
					name = "Dragon";
					break;
				case 5: //Eel
					moveList.Add(-6);
					moveList.Add(1);
					moveList.Add(4);
					name = "Eel";
					break;
				case 6: //Elephant
					moveList.Add(-1);
					moveList.Add(1);
					moveList.Add(4);
					moveList.Add(6);
					name = "Elephant";
					break;
				case 7: //Frog
					moveList.Add(-4);
					moveList.Add(-2);
					moveList.Add(4);
					name = "Frog";
					break;
				case 8: //Goose
					moveList.Add(-4);
					moveList.Add(-1);
					moveList.Add(1);
					moveList.Add(4);
					name = "Goose";
					break;
				case 9: //Horse
					moveList.Add(-5);
					moveList.Add(-1);
					moveList.Add(5);
					name = "Horse";
					break;
				case 10: //Mantis
					moveList.Add(-5);
					moveList.Add(4);
					moveList.Add(6);
					name = "Mantis";
					break;
				case 11: //Monkey
					moveList.Add(-6);
					moveList.Add(-4);
					moveList.Add(4);
					moveList.Add(6);
					name = "Monkey";
					break;
				case 12: //Ox
					moveList.Add(-5);
					moveList.Add(1);
					moveList.Add(5);
					name = "Ox";
					break;
				case 13: //Rabbit
					moveList.Add(-6);
					moveList.Add(2);
					moveList.Add(6);
					name = "Rabbit";
					break;
				case 14: //Rooster
					moveList.Add(-6);
					moveList.Add(-1);
					moveList.Add(1);
					moveList.Add(6);
					name = "Rooster";
					break;
				case 15: //Tiger
					moveList.Add(-5);
					moveList.Add(10);
					name = "Tiger";
					break;
			}
		}
	}
	class Evaluator
	{
		bool IsOplayer = false;
		int[] posX = new int[10];
		int[] posY = new int[10];

		public double Evaluate(GameState state)
		{
			//tentative algorithm:
			//1. sum of distances to enemy daimyo - sum of enemy distances to daimyo
			//2. daimyo distance to enemy shrine - enemy daimyo distance to shrine
			//X win = +1000, O win = -1000
			if (Game.CheckWin(state) != 0)
				return Game.CheckWin(state) * (IsOplayer ? -1000 : 1000);

			//work out the position of every piece in x-y coordinates
			for (int i = 0; i < 10; i++)
            {
				posX[i] = state.position[i] % 5;
				posY[i] = state.position[i] / 5;
			}

			double result = 0;
            float distance;
            for (int i = 0; i < 5; ++i)
            {
				//+ sum of (5 / X distance to O daimyo)
				distance = (posX[7] - posX[i]) * (posX[7] - posX[i]) + (posY[7] - posY[i]) * (posY[7] - posY[i]);
				//result += (IsOplayer? -5 : 5) / distance;
				result += 5 / distance;
				//- sum of (5 / O distance to X daimyo)
				distance = (posX[2] - posX[i + 5]) * (posX[2] - posX[i + 5]) + (posY[2] - posY[i + 5]) * (posY[2] - posY[i + 5]);
				//result -= (IsOplayer ? -5 : 5) / distance;
				result -= 5 / distance;
			}
			//+ 3 / X daimyo distance to O shrine
			distance = (2 - posX[2]) * (2 - posX[2]) + (4 - posY[2]) * (4 - posY[2]);
			//result += (IsOplayer ? -3 : 3) / distance;
			result += 3 / distance;
			//+ 3 / O daimyo distance to X shrine
			distance = (2 - posX[7]) * (2 - posX[7]) + (0 - posY[7]) * (0 - posY[7]);
			//result -= (IsOplayer ? -3 : 3) / distance;
			result -= 3 / distance;

			return result;
		}

		public void EvaluateFrom(GameState state)
        {
			List<short> indices = new();
			GameState queryState = state;

			//if there's a child, go down
			indices.Add(0);
			while (state.children.Count != indices[0])
				if (queryState.children.Count > indices.Last())
				{
					queryState = queryState.children[indices.Last()];
					indices.Add(0);
				}
				//if there's no child, evaluate and go back up
				else
				{
					if (queryState.children.Count == 0)
						queryState.score = Evaluate(queryState);
					if (indices[indices.Count - 2] == 0)
						queryState.parent.score = queryState.score;
					else if (queryState.Oturn % 2 == 0 && queryState.score < queryState.parent.score)
						queryState.parent.score = queryState.score;
					else if (queryState.Oturn % 2 == 1 && queryState.score > queryState.parent.score)
						queryState.parent.score = queryState.score;
					queryState = queryState.parent;
					indices.RemoveAt(indices.Count - 1);
					indices[indices.Count - 1] += 1;
				}
        }
	}
}