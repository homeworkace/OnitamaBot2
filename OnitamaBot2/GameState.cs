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
		public short id;

		public Card(short id)
		{
			this.id = id;
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
		short[] input = new short[41];
		double[] layerA = new double[41];
		double[] layerB = new double[41];
		double[] layerC = new double[41];
		float[] weight = new float[5084];
		float[] bias = new float[123];

		public void Initialise(string filePath) //example: "..//..//..//..//Genomes//test.txt"
		{
			StreamReader theFile = new StreamReader(filePath);
			string data = theFile.ReadToEnd();
			theFile.Close();

			int i = 0;
			foreach (string value in data.Split('\n'))
			{
				if (value == "")
					break;
				if (i < 5084)
					weight[i++] = float.Parse(value);
				else bias[i++ - 5084] = float.Parse(value);
			}
        }
		public void Initialise()
        {
			int i = 0;
			for (; i < 1681; ++i)
				weight[i] = (float)(Random.Shared.NextDouble() * 20 - 10);
			for (; i < 3362; ++i)
				weight[i] = (float)(Random.Shared.NextDouble() * 2 - 1);
			for (; i < 5043; ++i)
				weight[i] = (float)(Random.Shared.NextDouble() * 0.2 - 0.1);
			for (; i < 5084; ++i)
				weight[i] = (float)(Random.Shared.NextDouble() * 0.02 - 0.01);
			for (i = 0; i < 123; ++i)
				bias[i] = (float)(Random.Shared.NextDouble() * 2 - 1);
		}
		public void WriteGenes(string filePath) //example: "..//..//..//..//Genomes//test.txt"
		{
			StreamWriter theFile = new StreamWriter(filePath);
			foreach (float gene in weight)
				theFile.Write(gene + "\n");
			foreach (float gene in bias)
				theFile.Write(gene + "\n");
			theFile.Close();
		}
        public double HandcraftedEvaluate(GameState state)
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
				if (posX[i] != -1)
				{
					//+ sum of (10 / X distance to O daimyo + 5)
					distance = (posX[7] - posX[i]) * (posX[7] - posX[i]) + (posY[7] - posY[i]) * (posY[7] - posY[i]);
					//result += (IsOplayer? -10 : 10) / (distance + 5);
					result += 10 / (distance + 5);
				}
				if (posX[i + 5] != -1)
				{
					//- sum of (10 / O distance to X daimyo + 5)
					distance = (posX[2] - posX[i + 5]) * (posX[2] - posX[i + 5]) + (posY[2] - posY[i + 5]) * (posY[2] - posY[i + 5]);
					//result -= (IsOplayer? -10 : 10) / (distance + 5);
					result -= 10  / (distance + 5);
				}
			}
			//+ 10 / (X daimyo distance to O shrine + 2)
			distance = (2 - posX[2]) * (2 - posX[2]) + (4 - posY[2]) * (4 - posY[2]);
			//result += (IsOplayer? -10 : 10) / (distance + 2);
			result += 10 / (distance + 2);
			//+ 10 / (O daimyo distance to X shrine + 2)
			distance = (2 - posX[7]) * (2 - posX[7]) + (0 - posY[7]) * (0 - posY[7]);
			//result -= (IsOplayer? -10 : 10) / (distance + 2);
			result -= 10 / (distance + 2);

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
						queryState.score = HandcraftedEvaluate(queryState);
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
		public double GeneticEvaluate(GameState state, Card[] cards)
		{
			//(1 if X pawn is on that location, 5 if X daimyo, -1 if O pawn, -5 if O daimyo, 0 if empty)
			//p0, p1, p2, p3, p4,
			//p5, p6, p7, p8, p9,
			//p10, p11, p12, p13, p14,
			//p15, p16, p17, p18, p19,
			//p20, p21, p22, p23, p24, (1 if X pawn is on that location, 5 if X daimyo, -1 if O pawn, -5 if O daimyo, 0 if empty)
			//c0, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13, c14, c15 (2 if owned by X, -2 if owned by O, 1/-1 if turn card depending on whose turn it is, 0 otherwise)
			//there will be 41 input nodes, 3 intermediary layers of 41 nodes, 41 * 41 * 3 + 41 = 5084 weights and 41 * 3 = 123 biases

			//fill the input nodes
			int i = 0;
			for (; i < 5; ++i)
				if (state.position[i] != -1)
					input[state.position[i]] = 1;
			for (; i < 10; ++i)
				if (state.position[i] != -1)
					input[state.position[i]] = -1;
			input[state.position[2]] = 5;
			input[state.position[7]] = -5;
			input[cards[state.position[10]].id + 25] = 2;
			input[cards[state.position[11]].id + 25] = 2;
			input[cards[state.position[12]].id + 25] = (short)(state.Oturn % 2 == 0 ? 1 : -1);
			input[cards[state.position[13]].id + 25] = -2;
			input[cards[state.position[14]].id + 25] = -2;

			//calculate the first node of intermediary layer A
			for (i = 0; i < 41; ++i)
			{
				layerA[i] = bias[i];
				//layerA[i] = 0;
				for (int j = 0; j < 41; ++j)
					layerA[i] += input[j] * weight[i * 41 + j];
			}
			//calculate the first node of intermediary layer B
			for (i = 0; i < 41; ++i)
			{
				layerB[i] = bias[41 + i];
				//layerB[i] = 0;
				for (int j = 0; j < 41; ++j)
					layerB[i] += layerA[j] * weight[1681 + i * 41 + j];
			}
			//calculate the first node of intermediary layer C
			for (i = 0; i < 41; ++i)
			{
				layerC[i] = bias[82 + i];
				//layerC[i] = 0;
				for (int j = 0; j < 41; ++j)
					layerC[i] += layerB[j] * weight[3362 + i * 41 + j];
			}
			//calculate the output node
			double result = 0;
			for (i = 0; i < 41; ++i)
				result += layerC[i] * weight[5043 + i];

			if (Math.Abs(result) > 999.999)
			{
				Console.WriteLine("Score was normalised from " + result);
				result = Math.Sign(result) * 999.999;
			}
			return result;
		}
	}
}