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
		public bool IsInitialised = false;
		int[] posX = new int[10];
		int[] posY = new int[10];
		public short[] input = new short[41];
		double[] layerA = new double[41];
		double[] layerB = new double[41];
		double[] layerC = new double[41];
		float[] weight = new float[5084];
		float[] bias = new float[123];
		public Game theGame;

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

			IsInitialised = true;
		}
		public void Initialise()
		{
			int i = 0;
			/*for (; i < 1681; ++i)
				weight[i] = (float)(Random.Shared.NextDouble() * 20 - 10);
			for (; i < 3362; ++i)
				weight[i] = (float)(Random.Shared.NextDouble() * 2 - 1);
			for (; i < 5043; ++i)
				weight[i] = (float)(Random.Shared.NextDouble() * 0.2 - 0.1);
			for (; i < 5084; ++i)
				weight[i] = (float)(Random.Shared.NextDouble() * 0.02 - 0.01);*/
			for (; i < 5084; ++i)
				weight[i] = (float)(Random.Shared.NextDouble() * 2 - 1);
			for (i = 0; i < 123; ++i)
				bias[i] = (float)(Random.Shared.NextDouble() * 2 - 1);

			IsInitialised = true;
		}
		public void TestInitialise()
		{
			int i = 0;
			for (; i < 1681; ++i)
			{
				/*int bruh = i % 41;
				if (bruh == 20 || bruh == 21 || bruh == 22 || bruh == 23 || bruh == 24 || bruh == 28 || bruh == 29)
					weight[i] = -1;
				else weight[i] = 1;*/
				weight[i] = 5;
			}
			/*for (; i < 3362; ++i)
				weight[i] = 1;
			for (; i < 5043; ++i)
				weight[i] = 0.1F;
			for (; i < 5084; ++i)
				weight[i] = 0.01F;*/
			for (; i < 5084; ++i)
				weight[i] = 1;
			for (i = 0; i < 123; ++i)
				bias[i] = 0;
		}
		public void WriteGenes(string filePath) //example: "..//..//..//..//Genomes//test.txt"
		{
			StreamWriter theFile = new StreamWriter(filePath);
			foreach (float gene in weight)
				theFile.Write(gene + "\n");
				//theFile.Write(BitConverter.ToString(BitConverter.GetBytes(gene)));
			foreach (float gene in bias)
				theFile.Write(gene + "\n");
				//theFile.Write(BitConverter.ToString(BitConverter.GetBytes(gene)));
			theFile.Close();
		}
		public double HandcraftedEvaluate(GameState state, Card[] cards)
		{
			//tentative algorithm:
			//1. sum of distances to enemy daimyo - sum of enemy distances to daimyo
			//2. daimyo distance to enemy shrine - enemy daimyo distance to shrine
			//X win = +1000, O win = -1000
			if (Game.CheckWin(state) != 0)
				return Game.CheckWin(state) * (IsOplayer ? -1200 : 1200) + (IsOplayer ? state.Oturn : -state.Oturn);

			double result = 0;
			float distance;
			//Distance (input[0])
			if (input[0] > 0)
			{
				//work out the position of every piece in x-y coordinates
				for (int i = 0; i < 10; i++)
				{
					posX[i] = state.position[i] % 5;
					posY[i] = state.position[i] / 5;
				}

				for (int i = 0; i < 5; ++i)
				{
					if (posX[i] != -1)
					{
						//+ sum of (10 / X distance to O daimyo + 5)
						distance = (posX[7] - posX[i]) * (posX[7] - posX[i]) + (posY[7] - posY[i]) * (posY[7] - posY[i]);
						result += 10 / (distance + 5);
					}
					if (posX[i + 5] != -1)
					{
						//- sum of (10 / O distance to X daimyo + 5)
						distance = (posX[2] - posX[i + 5]) * (posX[2] - posX[i + 5]) + (posY[2] - posY[i + 5]) * (posY[2] - posY[i + 5]);
						result -= 10 / (distance + 5);
					}
				}
				//+ 10 / (X daimyo distance to O shrine + 2)
				distance = (2 - posX[2]) * (2 - posX[2]) + (4 - posY[2]) * (4 - posY[2]);
				result += 10 / (distance + 2);
				//+ 10 / (O daimyo distance to X shrine + 2)
				distance = (2 - posX[7]) * (2 - posX[7]) + (0 - posY[7]) * (0 - posY[7]);
				result -= 10 / (distance + 2);
			}

			//Cohesion (input[1]) (distance is weight)
			if (input[1] > 0)
			{
				for (int h = 0; h < 2; ++h)
				{
					for (int i = h * 5; i < 5 + h * 5; ++i)
					{
						//the daimyo and the dead bear no weight
						if (i == 2 + h * 5 || state.position[i] == -1)
							continue;
						//all other pieces have a starting weight of 0.2
						distance = 0.1f;
						//every enemy move that can capture the piece gives it extra weight
						for (int j = 5 - h * 5; j < 10 - h * 5; ++j)
						{
							if (state.position[j] == -1)
								continue;
							for (int k = 0; k < cards[state.position[13 - h * 3]].moveList.Count; ++k)
							{
								int horizontal = (state.position[j] % 5) + (h * 2 - 1) * (((((cards[state.position[13 - h * 3]].moveList[k] + 2) % 5) - 4) % 5) + 2);
								if (horizontal < 0 || horizontal > 4)
									continue;
								if (state.position[j] + (h * 2 - 1) * cards[state.position[13 - h * 3]].moveList[k] == state.position[i])
								{
									distance += 0.25f;
									break;
								}
							}
							for (int k = 0; k < cards[state.position[14 - h * 3]].moveList.Count; ++k)
							{
								int horizontal = (state.position[j] % 5) + (h * 2 - 1) * (((((cards[state.position[14 - h * 3]].moveList[k] + 2) % 5) - 4) % 5) + 2);
								if (horizontal < 0 || horizontal > 4)
									continue;
								if (state.position[j] + (h * 2 - 1) * cards[state.position[14 - h * 3]].moveList[k] == state.position[i])
								{
									distance += 0.25f;
									break;
								}
							}
						}
						//every move that protects this piece gives a weighted score
						for (int j = h * 5; j < 5 + h * 5; ++j)
						{
							if (j == i || state.position[j] == -1)
								continue;
							for (int k = 0; k < cards[state.position[10 + h * 3]].moveList.Count; ++k)
							{
								int horizontal = (state.position[j] % 5) + (1 - h * 2) * (((((cards[state.position[10 + h * 3]].moveList[k] + 2) % 5) - 4) % 5) + 2);
								if (horizontal < 0 || horizontal > 4)
									continue;
								if (state.position[j] + (1 - h * 2) * cards[state.position[10 + h * 3]].moveList[k] == state.position[i])
								{
									result += (1 - h * 2) * distance;
									break;
								}
							}
							for (int k = 0; k < cards[state.position[11 + h * 3]].moveList.Count; ++k)
							{
								int horizontal = (state.position[j] % 5) + (1 - h * 2) * (((((cards[state.position[11 + h * 3]].moveList[k] + 2) % 5) - 4) % 5) + 2);
								if (horizontal < 0 || horizontal > 4)
									continue;
								if (state.position[j] + (1 - h * 2) * cards[state.position[11 + h * 3]].moveList[k] == state.position[i])
								{
									result += (1 - h * 2) * distance;
									break;
								}
							}
						}
					}
				}
			}

			//Control (input[2])
			if (input[2] > 0)
			{
				for (int h = 0; h < 2; ++h)
				{
					for (int i = h * 5; i < 5 + h * 5; ++i)
					{
						if (state.position[i] == -1)
							continue;
						for (int j = 0; j < cards[state.position[10 + h * 3]].moveList.Count; ++j)
						{
							distance = state.position[i] + (1 - h * 2) * cards[state.position[10 + h * 3]].moveList[j];
							if (distance < 0 || distance > 24)
								continue;
							int horizontal = (state.position[i] % 5) + (1 - h * 2) * (((((cards[state.position[10 + h * 3]].moveList[j] + 2) % 5) - 4) % 5) + 2);
							if (horizontal < 0 || horizontal > 4)
								continue;
							for (int k = h * 5; k < 5 + h * 5; ++k)
							{
								if (k == i || state.position[k] == -1)
									continue;
								if (distance == state.position[k])
								{
									distance = -1;
									break;
								}
							}
							if (distance != -1)
							{
								result += (1 - h * 2) * 0.04;
								if (i == 2 + h * 5)
									result += (1 - h * 2) * 0.08;
							}
							if (distance == state.position[7 - h * 5])
								result += (1 - h * 2) * 0.012;
						}
						for (int j = 0; j < cards[state.position[11 + h * 3]].moveList.Count; ++j)
						{
							distance = state.position[i] + (1 - h * 2) * cards[state.position[11 + h * 3]].moveList[j];
							if (distance < 0 || distance > 24)
								continue;
							int horizontal = (state.position[i] % 5) + (1 - h * 2) * (((((cards[state.position[11 + h * 3]].moveList[j] + 2) % 5) - 4) % 5) + 2);
							if (horizontal < 0 || horizontal > 4)
								continue;
							for (int k = h * 5; k < 5 + h * 5; ++k)
							{
								if (k == i || state.position[k] == -1)
									continue;
								if (k == i)
									continue;
								if (distance == state.position[k])
								{
									distance = -1;
									break;
								}
							}
							if (distance != -1)
							{
								result += (1 - h * 2) * 0.04;
								if (i == 2 + h * 5)
									result += (1 - h * 2) * 0.08;
							}
							if (distance == state.position[7 - h * 5])
								result += (1 - h * 2) * 0.012;
						}
					}
				}
			}

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
						if (IsInitialised)
							queryState.score = GeneticEvaluate(queryState, theGame.cardList);
						else queryState.score = HandcraftedEvaluate(queryState, theGame.cardList);
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
			if (Game.CheckWin(state) != 0)
				return Game.CheckWin(state) * (IsOplayer ? -1200 : 1200) + (IsOplayer ? state.Oturn : -state.Oturn);

			//fill the input nodes
			int i = 0;
			for (; i < 41; ++i)
				input[i] = 0;
			i = 0;
			for (; i < 5; ++i)
				if (state.position[i] != -1)
					input[state.position[i]] = 1;
			for (; i < 10; ++i)
				if (state.position[i] != -1)
					input[state.position[i]] = -1;
			if (state.position[2] != -1)
				input[state.position[2]] = 5;
			if (state.position[7] != -1)
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
				for (int j = 0; j < 41; ++j)
					layerA[i] += input[j] * weight[i * 41 + j];
				layerA[i] = Math.Tanh(layerA[i] / 13);
			}
			//calculate the first node of intermediary layer B
			for (i = 0; i < 41; ++i)
			{
				layerB[i] = bias[41 + i];
				for (int j = 0; j < 41; ++j)
					layerB[i] += layerA[j] * weight[1681 + i * 41 + j];
				layerB[i] = Math.Tanh(layerB[i] / 20);
			}
			//calculate the first node of intermediary layer C
			for (i = 0; i < 41; ++i)
			{
				layerC[i] = bias[82 + i];
				for (int j = 0; j < 41; ++j)
					layerC[i] += layerB[j] * weight[3362 + i * 41 + j];
				layerC[i] = Math.Tanh(layerC[i] / 20);
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

		public void Evolve(Evaluator target, double weight, float noise = 0.25f)
		{
			int i = 0;
			for (; i < 5084; ++i)
			{
				this.weight[i] += (float)((target.weight[i] - this.weight[i]) * Math.Tanh(weight * (Random.Shared.NextSingle() * 0.5245253083117 + 0.75)));
				this.weight[i] += Random.Shared.NextSingle() * 2 * noise - noise;
				if (this.weight[i] > 10)
					this.weight[i] = 10;
				else if (this.weight[i] < -10)
					this.weight[i] = -10;
			}
			for (i = 0; i < 123; ++i)
			{
				bias[i] += (float)((target.bias[i] - bias[i]) * Math.Tanh(weight * (Random.Shared.NextSingle() * 0.5245253083117 + 0.75)));
				bias[i] += Random.Shared.NextSingle() * 20 * noise - 10 * noise;
				if (bias[i] > 100)
					bias[i] = 100;
				else if (bias[i] < -100)
					bias[i] = -100;
			}
		}
	}
}