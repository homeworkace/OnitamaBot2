using OnitamaBot2;

Main();
//for (int i = 0; i < 10; i++)
//{
//PlaygroundAnarchy();
//	Console.Write(i + " thousand...\n");
//}
//Console.WriteLine(Console.BufferHeight);

void PlaygroundAnarchy()
{
	Thread[] counters = new Thread[5];
	Mutex[] consoleCheck = new Mutex[5]; //Pass a Mutex into every Game instance.
	string[] threadOutput = new string[5];
	string[] centralOutput = new string[5];
	int nextGame = 5;
	bool shouldWrite = false;
	bool shouldStop = false;

	Console.BufferHeight = 100;
	for (int i = 0; i < 5; ++i)
	{
		consoleCheck[i] = new Mutex();
		//counters[j] = new Thread(() => PrintAllTheNumbers(j));
	}
	counters[0] = new Thread(() => PrintAllTheNumbers(0, consoleCheck[0], ref threadOutput[0]));
	counters[1] = new Thread(() => PrintAllTheNumbers(1, consoleCheck[1], ref threadOutput[1]));
	counters[2] = new Thread(() => PrintAllTheNumbers(2, consoleCheck[2], ref threadOutput[2]));
	counters[3] = new Thread(() => PrintAllTheNumbers(3, consoleCheck[3], ref threadOutput[3]));
	counters[4] = new Thread(() => PrintAllTheNumbers(4, consoleCheck[4], ref threadOutput[4]));
	for (int i = 0; i < 5; ++i)
	{
		counters[i].Start();
	}
	//Thread.Sleep(1000);
	while (!shouldStop)
	{
		Thread.Sleep(5);
		shouldWrite = false;
		for (int i = 0; i < 5; ++i)
		{
			consoleCheck[i].WaitOne();
			if (centralOutput[i] != threadOutput[i])
			{
				shouldWrite = true;
				centralOutput[i] = threadOutput[i];
				//All the stuff about changing games go here.
				if (centralOutput[i] == "Done" && nextGame < 100)
                {
					int safe_i = i;
					counters[i] = new Thread(() => PrintAllTheNumbers(nextGame++, consoleCheck[safe_i], ref threadOutput[safe_i]));
					counters[i].Start();
                }
			}
			consoleCheck[i].ReleaseMutex();
		}
		if (shouldWrite)
		{
			Console.Clear();
			Console.SetCursorPosition(0, 0);
			for (int i = 0; i < 5; ++i)
				Console.WriteLine(centralOutput[i]);
		}
		shouldStop = true;
		for (int i = 0; i < 5; ++i)
			if (centralOutput[i] != "Done" && centralOutput[i] != "bruh wtf")
				shouldStop = false;
	}
}

void PrintAllTheNumbers(int id, Mutex myCheck, ref string myOutput)
{
	int goal = Random.Shared.Next(500, 1000);
	for (int i = 0; i < goal; ++i)
	{
		Thread.Sleep(10);
		if (myCheck.WaitOne(0))
		{
			myOutput = "Job " + id + ": " + i + "/" + goal;
			myCheck.ReleaseMutex();
		}
	}
	myCheck.WaitOne();
	myOutput = "Done";
	myCheck.ReleaseMutex();
}

void Main()
{
	string sessionName = "Downtown";
	//Ang Mo Kio
	//Bedok
	//Clementi
	//Downtown
	//East Coast
	//
	Exhibition();
	//PassAndPlay();
	//Evaluator writer = new();
	//writer.TestInitialise();
	//writer.WriteGenes("..//..//..//..//Genomes//10for10.txt");
	//ScoreSomeStates();
	return;
	while (true)
		try
		{
			Training(sessionName);
		}
		catch (Exception ex)
		{
			if (!Directory.Exists("..//..//..//..//Genomes//" + sessionName + "//Error Logs//"))
				Directory.CreateDirectory("..//..//..//..//Genomes//" + sessionName + "//Error Logs//");
			StreamWriter theError = new StreamWriter("..//..//..//..//Genomes//" + sessionName + "//Error Logs//" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".txt");
			theError.WriteLine(ex.Message);
			theError.WriteLine(ex.StackTrace);
			theError.Close();
		}
}
void Exhibition()
{
	Thread gameThread;
	Mutex consoleCheck = new(); //Pass a Mutex into every Game instance.
	string threadOutput = "";
	string centralOutput = "";
	string logOutput = "";
	bool shouldWrite = false;
	bool shouldStop = false;

	Console.BufferHeight = 100;
	Evaluator botA = new Evaluator();
	//botA.Initialise("..//..//..//..//Genomes//Downtown//472_B.txt");
	botA.input[0] = 1;
	botA.input[1] = 1;
	//botA.input[2] = 1;
	Evaluator botB = new Evaluator();
	//botB.Initialise("..//..//..//..//Genomes//Clementi//0_A.txt");
	botB.input[0] = 1;
	//botB.input[1] = 1;
	//botB.input[2] = 1;
	//Shuffle the cards first
	int[] cardNumbers = new int[5];
	cardNumbers[0] = Random.Shared.Next(16);
	cardNumbers[1] = Random.Shared.Next(16);
	while (cardNumbers[1] == cardNumbers[0])
		cardNumbers[1] = Random.Shared.Next(16);
	cardNumbers[2] = Random.Shared.Next(16);
	while (cardNumbers[2] == cardNumbers[0] || cardNumbers[2] == cardNumbers[1])
		cardNumbers[2] = Random.Shared.Next(16);
	cardNumbers[3] = Random.Shared.Next(16);
	while (cardNumbers[3] == cardNumbers[0] || cardNumbers[3] == cardNumbers[1] || cardNumbers[3] == cardNumbers[2])
		cardNumbers[3] = Random.Shared.Next(16);
	cardNumbers[4] = Random.Shared.Next(16);
	while (cardNumbers[4] == cardNumbers[0] || cardNumbers[4] == cardNumbers[1] || cardNumbers[4] == cardNumbers[2] || cardNumbers[4] == cardNumbers[3])
		cardNumbers[4] = Random.Shared.Next(16);

	//1st round
	Game theGame = new();
	theGame.NewGame(botA, botB, new[] { new Card((short)cardNumbers[0]), new Card((short)cardNumbers[1]), new Card((short)cardNumbers[2]), new Card((short)cardNumbers[3]), new Card((short)cardNumbers[4]) });
	theGame.checker = consoleCheck;
	gameThread = new Thread(() => theGame.TheGame(ref threadOutput));
	gameThread.Start();
	while (!shouldStop)
	{
		Thread.Sleep(5);
		shouldWrite = false;
		consoleCheck.WaitOne();
		if (centralOutput != threadOutput)
		{
			shouldWrite = true;
			centralOutput = threadOutput;
		}
		consoleCheck.ReleaseMutex();
		if (shouldWrite)
		{
			Console.Clear();
			Console.SetCursorPosition(0, 0);
			Console.WriteLine(centralOutput);
			logOutput += centralOutput;
		}
		shouldStop = true;
		if (centralOutput != "Done")
			shouldStop = false;
	}

	shouldStop = false;
	threadOutput = "";
	//2nd round - same setup, but sides are changed
	Game theGame2 = new();
	theGame2.NewGame(botB, botA, new[] { new Card((short)cardNumbers[0]), new Card((short)cardNumbers[1]), new Card((short)cardNumbers[2]), new Card((short)cardNumbers[3]), new Card((short)cardNumbers[4]) });
	theGame2.checker = consoleCheck;
	gameThread = new Thread(() => theGame2.TheGame(ref threadOutput));
	gameThread.Start();
	while (!shouldStop)
	{
		Thread.Sleep(5);
		shouldWrite = false;
		consoleCheck.WaitOne();
		if (centralOutput != threadOutput)
		{
			shouldWrite = true;
			centralOutput = threadOutput;
		}
		consoleCheck.ReleaseMutex();
		if (shouldWrite)
		{
			Console.Clear();
			Console.SetCursorPosition(0, 0);
			Console.WriteLine(centralOutput);
			logOutput += centralOutput;
		}
		shouldStop = true;
		if (centralOutput != "Done")
			shouldStop = false;
	}

	StreamWriter gameLog = new StreamWriter("..//..//..//..//Genomes//Exhibitions//DistanceAndCohesion_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".txt");
	gameLog.WriteLine(logOutput);
	gameLog.Close();
}

void ScoreSomeStates()
{
	Evaluator[] theBot = new Evaluator[4];
	string[] botStrings = new string[theBot.Length];
	theBot[0] = new();
	//theBot[0].Initialise("..//..//..//..//Genomes//Downtown//472_B.txt");
	theBot[0].input[0] = 1;
	//theBot[0].input[1] = 1;
	theBot[0].input[2] = 1;
	botStrings[0] = "Distance + Control";
	theBot[1] = new();
	theBot[1].input[0] = 1;
	//theBot[1].input[1] = 1;
	//theBot[1].input[2] = 1;
	botStrings[1] = "Distance";
	theBot[2] = new();
	theBot[2].input[0] = 1;
	theBot[2].input[1] = 1;
	//theBot[2].input[2] = 1;
	botStrings[2] = "Distance + Cohesion";
	theBot[3] = new();
	theBot[3].input[0] = 1;
	theBot[3].input[1] = 1;
	theBot[3].input[2] = 1;
	botStrings[3] = "Distance + Cohesion + Control";

	//Starting position
	GameState pos = new();
	pos.Oturn = 0;
	pos.position[0] = 0;
	pos.position[1] = 1;
	pos.position[2] = 2;
	pos.position[3] = 3;
	pos.position[4] = 4;
	pos.position[5] = 24;
	pos.position[6] = 23;
	pos.position[7] = 22;
	pos.position[8] = 21;
	pos.position[9] = 20;
	pos.position[10] = 0;
	pos.position[11] = 1;
	pos.position[12] = 2;
	pos.position[13] = 3;
	pos.position[14] = 4;
	Card[] theCards = new Card[5] { new Card(12), new Card(2), new Card(3), new Card(8), new Card(14)};
	Console.WriteLine("\nStarting position:");
	Console.WriteLine(Game.PrintBoard(pos));
	for (int i = 0; i < theBot.Length; ++i)
    {
		if (theBot[i].IsInitialised)
			Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].GeneticEvaluate(pos, theCards));
		else Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].HandcraftedEvaluate(pos, theCards));
	}

	//First capture
	pos.Oturn = 1;
	pos.position[0] = 6;
	pos.position[1] = 8;
	pos.position[2] = 2;
	pos.position[3] = 9;
	pos.position[4] = 10;
	pos.position[5] = 16;
	pos.position[6] = 18;
	pos.position[7] = 22;
	pos.position[8] = 19;
	pos.position[9] = -1;
	pos.position[10] = 0;
	pos.position[11] = 1;
	pos.position[12] = 2;
	pos.position[13] = 4;
	pos.position[14] = 3;
	Console.WriteLine("\nFirst capture:");
	Console.WriteLine(Game.PrintBoard(pos));
	for (int i = 0; i < theBot.Length; ++i)
	{
		if (theBot[i].IsInitialised)
			Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].GeneticEvaluate(pos, theCards));
		else Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].HandcraftedEvaluate(pos, theCards));
	}

	//3-on-3
	pos.Oturn = 0;
	pos.position[0] = 8;
	pos.position[1] = 13;
	pos.position[2] = 3;
	pos.position[3] = -1;
	pos.position[4] = -1;
	pos.position[5] = 12;
	pos.position[6] = 17;
	pos.position[7] = 22;
	pos.position[8] = -1;
	pos.position[9] = -1;
	pos.position[10] = 0;
	pos.position[11] = 3;
	pos.position[12] = 4;
	pos.position[13] = 2;
	pos.position[14] = 1;
	Console.WriteLine("\n3-on-3:");
	Console.WriteLine(Game.PrintBoard(pos));
	for (int i = 0; i < theBot.Length; ++i)
	{
		if (theBot[i].IsInitialised)
			Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].GeneticEvaluate(pos, theCards));
		else Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].HandcraftedEvaluate(pos, theCards));
	}

	//Last turn
	pos.Oturn = 0;
	pos.position[0] = -1;
	pos.position[1] = -1;
	pos.position[2] = 3;
	pos.position[3] = -1;
	pos.position[4] = -1;
	pos.position[5] = -1;
	pos.position[6] = -1;
	pos.position[7] = 6;
	pos.position[8] = -1;
	pos.position[9] = -1;
	pos.position[10] = 2;
	pos.position[11] = 1;
	pos.position[12] = 4;
	pos.position[13] = 3;
	pos.position[14] = 0;
	Console.WriteLine("\nLast turn:");
	Console.WriteLine(Game.PrintBoard(pos));
	for (int i = 0; i < theBot.Length; ++i)
	{
		if (theBot[i].IsInitialised)
			Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].GeneticEvaluate(pos, theCards));
		else Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].HandcraftedEvaluate(pos, theCards));
	}

	//Another start
	pos.Oturn = 0;
	pos.position[0] = 0;
	pos.position[1] = 1;
	pos.position[2] = 2;
	pos.position[3] = 3;
	pos.position[4] = 4;
	pos.position[5] = 24;
	pos.position[6] = 23;
	pos.position[7] = 22;
	pos.position[8] = 21;
	pos.position[9] = 20;
	pos.position[10] = 0;
	pos.position[11] = 1;
	pos.position[12] = 2;
	pos.position[13] = 3;
	pos.position[14] = 4;
	theCards = new Card[5] { new Card(14), new Card(1), new Card(2), new Card(9), new Card(4) };
	Console.WriteLine("\nAnother start:");
	Console.WriteLine(Game.PrintBoard(pos));
	for (int i = 0; i < theBot.Length; ++i)
	{
		if (theBot[i].IsInitialised)
			Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].GeneticEvaluate(pos, theCards));
		else Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].HandcraftedEvaluate(pos, theCards));
	}

	//Shock action
	pos.Oturn = 0;
	pos.position[0] = 0;
	pos.position[1] = 6;
	pos.position[2] = 1;
	pos.position[3] = 8;
	pos.position[4] = -1;
	pos.position[5] = 7;
	pos.position[6] = 17;
	pos.position[7] = 22;
	pos.position[8] = 21;
	pos.position[9] = 23;
	pos.position[10] = 1;
	pos.position[11] = 4;
	pos.position[12] = 3;
	pos.position[13] = 0;
	pos.position[14] = 2;
	Console.WriteLine("\nShock action:");
	Console.WriteLine(Game.PrintBoard(pos));
	for (int i = 0; i < theBot.Length; ++i)
	{
		if (theBot[i].IsInitialised)
			Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].GeneticEvaluate(pos, theCards));
		else Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].HandcraftedEvaluate(pos, theCards));
	}

	//Ballsy
	pos.Oturn = 0;
	pos.position[0] = 6;
	pos.position[1] = 12;
	pos.position[2] = 1;
	pos.position[3] = -1;
	pos.position[4] = -1;
	pos.position[5] = 11;
	pos.position[6] = 18;
	pos.position[7] = 24;
	pos.position[8] = 23;
	pos.position[9] = -1;
	pos.position[10] = 0;
	pos.position[11] = 2;
	pos.position[12] = 3;
	pos.position[13] = 1;
	pos.position[14] = 4;
	Console.WriteLine("\nBallsy:");
	Console.WriteLine(Game.PrintBoard(pos));
	for (int i = 0; i < theBot.Length; ++i)
	{
		if (theBot[i].IsInitialised)
			Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].GeneticEvaluate(pos, theCards));
		else Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].HandcraftedEvaluate(pos, theCards));
	}

	//2 steps from hell
	pos.Oturn = 1;
	pos.position[0] = 7;
	pos.position[1] = -1;
	pos.position[2] = 2;
	pos.position[3] = -1;
	pos.position[4] = -1;
	pos.position[5] = 16;
	pos.position[6] = -1;
	pos.position[7] = 6;
	pos.position[8] = -1;
	pos.position[9] = -1;
	pos.position[10] = 0;
	pos.position[11] = 4;
	pos.position[12] = 3;
	pos.position[13] = 1;
	pos.position[14] = 2;
	Console.WriteLine("\n2 steps from hell:");
	Console.WriteLine(Game.PrintBoard(pos));
	for (int i = 0; i < theBot.Length; ++i)
	{
		if (theBot[i].IsInitialised)
			Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].GeneticEvaluate(pos, theCards));
		else Console.WriteLine("    " + botStrings[i] + ": " + theBot[i].HandcraftedEvaluate(pos, theCards));
	}
}

void PassAndPlay()
{
	Thread gameThread;
	Mutex consoleCheck = new(); //Pass a Mutex into every Game instance.
	string threadOutput = "";
	string centralOutput = "";
	bool shouldWrite = false;
	bool shouldStop = false;

	Console.BufferHeight = 100;
	Game theGame = new();
	theGame.checker = consoleCheck;
	Evaluator customBot = new Evaluator();
	//customBot.Initialise("..//..//..//..//Genomes//Clementi//183_B.txt");
	theGame.NewGame(customBot, null, Array.Empty<Card>());
	//theGame.NewGame(new Evaluator(), null, Array.Empty<Card>());
	Console.Clear();
	gameThread = new Thread(() => theGame.TheGame(ref threadOutput));
	gameThread.Start();
	//Thread.Sleep(1000);
	while (!shouldStop)
	{
		Thread.Sleep(5);
		shouldWrite = false;
		consoleCheck.WaitOne();
		if (centralOutput != threadOutput)
		{
			shouldWrite = true;
			centralOutput = threadOutput;
		}
		consoleCheck.ReleaseMutex();
		if (shouldWrite)
		{
			Console.Clear();
			Console.SetCursorPosition(0, 0);
			Console.WriteLine(centralOutput);
		}
		shouldStop = true;
		if (centralOutput != "Done")
			shouldStop = false;
	}
}
void Training(string filePath)
{
	Thread[] counters = new Thread[5];
	Mutex[] consoleCheck = new Mutex[5]; //Pass a Mutex into every Game instance.
	string[] threadOutput = new string[5];
	string[] centralOutput = new string[5];
	bool shouldWrite = false;
	bool shouldStop = false;
	int nextGame = counters.Length;
	int generation = 1;

	Console.BufferHeight = 100;
	double generationScore = 0;
	float roundScore = 0;
	//Make all the bots
	Evaluator[] botA = new Evaluator[nextGame];
	Evaluator[] botB = new Evaluator[nextGame];
	if (!Directory.Exists("..//..//..//..//Genomes//" + filePath))
		Directory.CreateDirectory("..//..//..//..//Genomes//" + filePath);
	if (File.Exists("..//..//..//..//Genomes//" + filePath + "//Last completed generation.txt"))
	{
		StreamReader theFile = new StreamReader("..//..//..//..//Genomes//" + filePath + "//Last completed generation.txt");
		string[] LCG = theFile.ReadToEnd().Split('\n');
		theFile.Close();
		for (int i = 0; i < counters.Length; ++i)
		{
			botA[i] = new();
			botA[i].Initialise("..//..//..//..//Genomes//" + filePath + "//" + int.Parse(LCG[0]) + "_A.txt");
			botB[i] = new();
			botB[i].Initialise("..//..//..//..//Genomes//" + filePath + "//" + int.Parse(LCG[1]) + "_B.txt");
		}
		generation = Math.Max(int.Parse(LCG[0]), int.Parse(LCG[1])) + 1;
	}
	else
	{
		StreamWriter theFile = new StreamWriter("..//..//..//..//Genomes//" + filePath + "//Last completed generation.txt");
		theFile.WriteLine("0\n0");
		theFile.Close();
		botA[0] = new();
		botA[0].Initialise();
		botA[0].WriteGenes("..//..//..//..//Genomes//" + filePath + "//0_A.txt");
		botB[0] = new();
		botB[0].Initialise();
		botB[0].WriteGenes("..//..//..//..//Genomes//" + filePath + "//0_B.txt");
		for (int i = 1; i < counters.Length; ++i)
		{
			botA[i] = new();
			botA[i].Initialise("..//..//..//..//Genomes//" + filePath + "//0_A.txt");
			botB[i] = new();
			botB[i].Initialise("..//..//..//..//Genomes//" + filePath + "//0_B.txt");
		}
    }
	//Make all the games
	Game[] gamesInGeneration = new Game[20];
	for (int i = 0; i < gamesInGeneration.Length; i += 2)
	{
		//Shuffle the cards first
		//int[] cardNumbers = new int[5] { 0, 1, 2, 3, 4 };
		int[] cardNumbers = new int[5];
		cardNumbers[0] = Random.Shared.Next(16);
		cardNumbers[1] = Random.Shared.Next(16);
		while (cardNumbers[1] == cardNumbers[0])
			cardNumbers[1] = Random.Shared.Next(16);
		cardNumbers[2] = Random.Shared.Next(16);
		while (cardNumbers[2] == cardNumbers[0] || cardNumbers[2] == cardNumbers[1])
			cardNumbers[2] = Random.Shared.Next(16);
		cardNumbers[3] = Random.Shared.Next(16);
		while (cardNumbers[3] == cardNumbers[0] || cardNumbers[3] == cardNumbers[1] || cardNumbers[3] == cardNumbers[2])
			cardNumbers[3] = Random.Shared.Next(16);
		cardNumbers[4] = Random.Shared.Next(16);
		while (cardNumbers[4] == cardNumbers[0] || cardNumbers[4] == cardNumbers[1] || cardNumbers[4] == cardNumbers[2] || cardNumbers[4] == cardNumbers[3])
			cardNumbers[4] = Random.Shared.Next(16);

		//1st round
		Game theGame = new();
		theGame.NewGame(botA[0], botB[0], new[] { new Card((short)cardNumbers[0]), new Card((short)cardNumbers[1]), new Card((short)cardNumbers[2]), new Card((short)cardNumbers[3]), new Card((short)cardNumbers[4]) });
		gamesInGeneration[i] = theGame;
		theGame.gameInGeneration = i;

		//2nd round - same setup, but sides are changed
		theGame = new();
		theGame.NewGame(botB[0], botA[0], new[] { new Card((short)cardNumbers[0]), new Card((short)cardNumbers[1]), new Card((short)cardNumbers[2]), new Card((short)cardNumbers[3]), new Card((short)cardNumbers[4]) });
		gamesInGeneration[i + 1] = theGame;
		theGame.gameInGeneration = i + 1;
	}
	for (int i = 0; i < counters.Length; ++i)
	{
		consoleCheck[i] = new Mutex();
		gamesInGeneration[i].checker = consoleCheck[i];
		if (i % 2 == 0)
		{
			gamesInGeneration[i].botX = botA[i];
			botA[i].theGame = gamesInGeneration[i];
			gamesInGeneration[i].botO = botB[i];
			botB[i].theGame = gamesInGeneration[i];
		}
		else
		{
			gamesInGeneration[i].botX = botB[i];
			botB[i].theGame = gamesInGeneration[i];
			gamesInGeneration[i].botO = botA[i];
			botA[i].theGame = gamesInGeneration[i];
		}
		counters[i] = new Thread(() => gamesInGeneration[i].TheGame(ref threadOutput[i]));
		counters[i].Start();
		//Putting a delay here prevents a race condition somehow.
		Thread.Sleep(100);
	}
	//Thread.Sleep(1000);
	while (!shouldStop)
	{
		Thread.Sleep(5);
		shouldWrite = false;
		for (int i = 0; i < counters.Length; ++i)
		{
			consoleCheck[i].WaitOne();
			if (centralOutput[i] != threadOutput[i])
			{
				shouldWrite = true;
				centralOutput[i] = threadOutput[i];
				//All the stuff about changing games go here.
				if (centralOutput[i] == "Done")
				{
					int safe_i = i;
					if (botA[safe_i].theGame.gameInGeneration % 2 == 0)
						generationScore += botA[safe_i].theGame.result;
					else generationScore -= botA[safe_i].theGame.result;
					if (nextGame < gamesInGeneration.Length)
					{
						gamesInGeneration[nextGame].checker = consoleCheck[safe_i];
						if (nextGame % 2 == 0)
						{
							gamesInGeneration[nextGame].botX = botA[safe_i];
							botA[safe_i].theGame = gamesInGeneration[nextGame];
							gamesInGeneration[nextGame].botO = botB[safe_i];
							botB[safe_i].theGame = gamesInGeneration[nextGame];
						}
						else
						{
							gamesInGeneration[nextGame].botX = botB[safe_i];
							botB[safe_i].theGame = gamesInGeneration[nextGame];
							gamesInGeneration[nextGame].botO = botA[safe_i];
							botA[safe_i].theGame = gamesInGeneration[nextGame];
						}
						centralOutput[safe_i] = "";
						threadOutput[safe_i] = "";
						counters[safe_i] = new Thread(() => gamesInGeneration[nextGame++].TheGame(ref threadOutput[safe_i]));
						counters[safe_i].Start();
					}
				}
			}
			consoleCheck[i].ReleaseMutex();
		}
		if (shouldWrite)
		{
			Console.Clear();
			Console.SetCursorPosition(0, 0);
			Console.WriteLine("Score in generation " + generation + ": " + generationScore);
			for (int i = 0; i < counters.Length; ++i)
				Console.WriteLine(centralOutput[i]);
		}
		shouldStop = true;
		for (int i = 0; i < counters.Length; ++i)
			if (centralOutput[i] != "Done" && centralOutput[i] != "bruh wtf")
				shouldStop = false;
	}

	//Generation done - evolve the loser
	if (generationScore == 0)
		return;
	else if (generationScore > 0)
	{
		//botB[0].Evolve(botA[0], Math.Abs(generationScore) * 20 / gamesInGeneration.Length, (float)Math.Abs(generationScore) / 2);
		botB[0].Evolve(botA[0], Math.Abs(generationScore) * 20 / gamesInGeneration.Length);
		botB[0].WriteGenes("..//..//..//..//Genomes//" + filePath + "//" + generation + "_B.txt");
		for (int i = 1; i < botB.Length; ++i)
			botB[i].Initialise("..//..//..//..//Genomes//" + filePath + "//" + generation + "_B.txt");
		StreamReader LastCompletedGenerationR = new StreamReader("..//..//..//..//Genomes//" + filePath + "//Last completed generation.txt");
		int LCG_botA = int.Parse(LastCompletedGenerationR.ReadToEnd().Split('\n')[0]);
		LastCompletedGenerationR.Close();
		StreamWriter LastCompletedGenerationW = new StreamWriter("..//..//..//..//Genomes//" + filePath + "//Last completed generation.txt");
		LastCompletedGenerationW.WriteLine(LCG_botA);
		LastCompletedGenerationW.WriteLine(generation);
		LastCompletedGenerationW.Close();
	}
	else
	{
		//botA[0].Evolve(botB[0], Math.Abs(generationScore) * 20 / gamesInGeneration.Length, (float)Math.Abs(generationScore) / 2);
		botA[0].Evolve(botB[0], Math.Abs(generationScore) * 20 / gamesInGeneration.Length);
		botA[0].WriteGenes("..//..//..//..//Genomes//" + filePath + "//" + generation + "_A.txt");
		for (int i = 1; i < botA.Length; ++i)
			botA[i].Initialise("..//..//..//..//Genomes//" + filePath + "//" + generation + "_A.txt");
		StreamReader LastCompletedGenerationR = new StreamReader("..//..//..//..//Genomes//" + filePath + "//Last completed generation.txt");
		int LCG_botB = int.Parse(LastCompletedGenerationR.ReadToEnd().Split('\n')[1]);
		LastCompletedGenerationR.Close();
		StreamWriter LastCompletedGenerationW = new StreamWriter("..//..//..//..//Genomes//" + filePath + "//Last completed generation.txt");
		LastCompletedGenerationW.WriteLine(generation);
		LastCompletedGenerationW.WriteLine(LCG_botB);
		LastCompletedGenerationW.Close();
	}
	generation += 1;
}

class Game
{
	GameState currentState;
	public Card[] cardList;
	Queue<GameState> jobQueue = new();
	public Evaluator botX, botO;
	public Mutex checker;
	public float result;
	public int gameInGeneration;
	void CheckMoves(GameState curState)
	{
		//for each piece
		for (short i = 0; i < 5; ++i)
		{
			//skip if dead (-1)
			if (curState.position[i + (curState.Oturn % 2) * 5] < 0)
				continue;
			//for each possible move from the 1st card
			for (short j = 0; j < cardList[curState.position[10 + (curState.Oturn % 2) * 3]].moveList.Count; ++j)
			{
				int result = curState.position[i + (curState.Oturn % 2) * 5] + (1 - (curState.Oturn % 2) * 2) * cardList[curState.position[10 + (curState.Oturn % 2) * 3]].moveList[j];
				//if piece goes out of bounds vertically
				if (result < 0 || result > 24)
					continue;
				//if piece lands on allies
				short k = 0;
				for (; k < 5; ++k)
				{
					if (k == i)
						continue;
					if (curState.position[k + (curState.Oturn % 2) * 5] == result)
						break;
				}
				if (k != 5)
					continue;
				//if piece goes out of bounds horizontally
				int horizontal = (curState.position[i + (curState.Oturn % 2) * 5] % 5) + (1 - (curState.Oturn % 2) * 2) * (((((cardList[curState.position[10 + (curState.Oturn % 2) * 3]].moveList[j] + 2) % 5) - 4) % 5) + 2);
				if (horizontal < 0 || horizontal > 4)
					continue;
				//if execution reaches, this move is legal
				GameState pos = new();
				pos.Oturn = (short)(curState.Oturn + 1);
				pos.parent = curState;
				pos.score = 0;
				for (k = 0; k < 15; ++k)
					pos.position[k] = curState.position[k];
				pos.position[i + (curState.Oturn % 2) * 5] = (short)result;
				for (k = 0; k < 5; ++k)
					if (pos.position[k + 5 - (curState.Oturn % 2) * 5] == result)
					{
						pos.position[k + 5 - (curState.Oturn % 2) * 5] = -1;
						break;
					}
				short swapCard = pos.position[12];
				pos.position[12] = pos.position[10 + (curState.Oturn % 2) * 3];
				pos.position[10 + (curState.Oturn % 2) * 3] = swapCard;
				curState.children.Add(pos);
				if (CheckWin(pos) == 0)
					jobQueue.Enqueue(pos);
				else pos.score = CheckWin(pos) * 1000;
				//Console.WriteLine(curState.children.Count - 1);
				//PrintBoard(pos);
				//Console.WriteLine(Print(pos));
			}
			//for each possible move from the 2nd card
			for (short j = 0; j < cardList[curState.position[11 + (curState.Oturn % 2) * 3]].moveList.Count; ++j)
			{
				int result = curState.position[i + (curState.Oturn % 2) * 5] + (1 - (curState.Oturn % 2) * 2) * cardList[curState.position[11 + (curState.Oturn % 2) * 3]].moveList[j];
				//if piece goes out of bounds vertically
				if (result < 0 || result > 24)
					continue;
				//if piece lands on allies
				short k = 0;
				for (; k < 5; ++k)
				{
					if (k == i)
						continue;
					if (curState.position[k + (curState.Oturn % 2) * 5] == result)
						break;
				}
				if (k != 5)
					continue;
				//if piece goes out of bounds horizontally
				int horizontal = (curState.position[i + (curState.Oturn % 2) * 5] % 5) + (1 - (curState.Oturn % 2) * 2) * (((((cardList[curState.position[11 + (curState.Oturn % 2) * 3]].moveList[j] + 2) % 5) - 4) % 5) + 2);
				if (horizontal < 0 || horizontal > 4)
					continue;
				//if execution reaches, this move is legal
				GameState pos = new();
				pos.Oturn = (short)(curState.Oturn + 1);
				pos.parent = curState;
				pos.score = 0;
				for (k = 0; k < 15; ++k)
					pos.position[k] = curState.position[k];
				pos.position[i + (curState.Oturn % 2) * 5] = (short)result;
				for (k = 0; k < 5; ++k)
					if (pos.position[k + 5 - (curState.Oturn % 2) * 5] == result)
					{
						pos.position[k + 5 - (curState.Oturn % 2) * 5] = -1;
						break;
					}
				short swapCard = pos.position[12];
				pos.position[12] = pos.position[11 + (curState.Oturn % 2) * 3];
				pos.position[11 + (curState.Oturn % 2) * 3] = swapCard;
				curState.children.Add(pos);
				if (CheckWin(pos) == 0)
					jobQueue.Enqueue(pos);
				else pos.score = CheckWin(pos) * 1000;
				//Console.WriteLine(curState.children.Count - 1);
				//PrintBoard(pos);
				//Console.WriteLine(Print(pos));
			}
		}
	}
	static public short CheckWin(GameState curState)
	{
		if (curState.position[7] < 0)
		{
			//Console.Write("X Stone.\n");
			return 1;
		}
		else if (curState.position[2] == 22)
		{
			//Console.Write("X Stream.\n");
			return 1;
		}
		else if (curState.position[2] < 0)
		{
			//Console.Write("O Stone.\n");
			return -1;
		}
		else if (curState.position[7] == 2)
		{
			//Console.Write("O Stream.\n");
			return -1;
		}
		else return 0;
	}
	public GameState NewGame(Evaluator botX, Evaluator ?botO, Card[] presetCards)
	{
		int choice = 0;
		if (presetCards.Length == 0)
		{
			Console.Write("X starts first.\nPlace the corresponding numbers of the 5 cards in play, side by side, in this order: X's first card, X's second card, turn card, O's first card, O's second card.\nReference:\n00: Boar\n01: Cobra\n02: Crab\n03: Crane\n04: Dragon\n05: Eel\n06: Elephant\n07: Frog\n08: Goose\n09: Horse\n10: Mantis\n11: Monkey\n12: Ox\n13: Rabbit\n14: Rooster\n15: Tiger\nExample: X starts with Boar (00) and Cobra (01), O starts with Crane (03) and Dragon (04). X always starts first, so the turn card, Crab (03) belongs to X. The input is thus: 0001020304.\n");
			while (!int.TryParse(Console.ReadLine(), out choice)) //Eel, Monkey, Goose, Crane, Cobra, 0511080301
				Console.Write("Please enter a number: ");
			cardList = new Card[5];
			for (short i = 0; i < 5; ++i)
				cardList[i] = new Card((short)((int)(choice / Math.Pow(10, 8 - i * 2)) % 100)); //To isolate the 2 digits we want, divide by a power of ten then floor to get rid of every digit to the right, then mod 100 to get rid of every digit to the left.
		}
		else cardList = presetCards;

		GameState pos = new();
		pos.Oturn = 0;
		pos.parent = null;
		pos.score = 0;
		pos.position[0] = 0;
		pos.position[1] = 1;
		pos.position[2] = 2;
		pos.position[3] = 3;
		pos.position[4] = 4;
		pos.position[5] = 24;
		pos.position[6] = 23;
		pos.position[7] = 22;
		pos.position[8] = 21;
		pos.position[9] = 20;
		pos.position[10] = 0;
		pos.position[11] = 1;
		pos.position[12] = 2;
		pos.position[13] = 3;
		pos.position[14] = 4;
		currentState = pos;

		this.botX = botX;
		botX.theGame = this;
		this.botO = botO;
		if (botO is not null)
			botO.theGame = this;
		CheckMoves(currentState);
		return currentState;
	}
	string Print(GameState state)
	{
		string result = "";
		foreach (int i in state.position)
			result += i + ", ";
		//for (int i = 10; i < 15; ++i)
		//	result += state.position[i] + ", ";
		result = result.Remove(result.Length - 2, 2);
		return result;
	}
	static public string PrintBoard(GameState state)
	{
		string result = "_______\n";
		for (short y = 0; y < 5; ++y)
		{
			result += "|";
			for (short x = 0; x < 5; ++x)
			{
				short i = 0;
				for (; i < 10; ++i)
				{
					if (state.position[i] == (4 - y) * 5 + x)
					{
						if (i == 2)
							result += "X";
						else if (i < 5)
							result += "x";
						else if (i == 7)
							result += "O";
						else result += "o";
						break;
					}
				}
				if (i == 10)
					result += " ";
			}
			result += "|\n";
		}
		return result + "_______\n";
	}
	public float TheGame(ref string output)
	{
		short theMove = 1;

		while (CheckWin(currentState) == 0 && currentState.Oturn < 200)
		{
			try
			{
				while (jobQueue.Peek().Oturn - currentState.Oturn < 4)
				{
					GameState stateQuery = jobQueue.Dequeue();
					//Only work on jobs for positions still on the tree.
					if (stateQuery.parent != null)
						CheckMoves(stateQuery);
					if (jobQueue.Count == 0)
						break;
				}
			}
			catch (Exception ex)
            {
				if (!Directory.Exists("..//..//..//..//Genomes//Clementi//Error Logs//"))
					Directory.CreateDirectory("..//..//..//..//Genomes//Clementi//Error Logs//");
				StreamWriter theError = new StreamWriter("..//..//..//..//Genomes//Clementi//Error Logs//" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".txt");
				theError.WriteLine(ex.Message);
				theError.WriteLine(ex.StackTrace);
				theError.WriteLine(Print(currentState));
				theError.WriteLine("Turn " + currentState.Oturn + ", Game " + gameInGeneration);
				foreach (Card c in cardList)
					theError.WriteLine(c.id);
				theError.Close();
				botX.WriteGenes("..//..//..//..//Genomes//Clementi//Error Logs//botX_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".txt");
				botO.WriteGenes("..//..//..//..//Genomes//Clementi//Error Logs//botO_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".txt");
				throw;
            }
			if (theMove >= 0)
				if (botO is null || currentState.Oturn % 2 == 0)
					botX.EvaluateFrom(currentState);
				else botO.EvaluateFrom(currentState);
			if (botO is null)
			{
				checker.WaitOne();
				output += PrintBoard(currentState);
				if (currentState.Oturn % 2 == 1)
					output += "O";
				else output += "X";
				output += ", make your next move.\n-1 for more intuitive input, -2 to go back one move, -3 to show the suggested move, -4 to dump state tree, -5 to show all possible moves: ";
				checker.ReleaseMutex();
				while (!short.TryParse(Console.ReadLine(), out theMove))
				{
					checker.WaitOne();
					output += "\nPlease enter a number: ";
					checker.ReleaseMutex();
				}
			}
			else
			{
				//if botO exists, training mode is enabled. we dont need any player involvement
				checker.WaitOne();
				output = PrintBoard(currentState) + "Game " + (gameInGeneration + 1) + ": State of the game at move " + currentState.Oturn;
				output += "\nBot X thinks the score is " + botX.HandcraftedEvaluate(currentState, cardList);
				output += "\nBot O thinks the score is " + botO.HandcraftedEvaluate(currentState, cardList);
				checker.ReleaseMutex();
				theMove = 0;
				for (int i = 1; i < currentState.children.Count; ++i)
				{
					if (currentState.Oturn % 2 == 0 && currentState.children[i].score > currentState.children[theMove].score)
						theMove = (short)i;
					else if (currentState.Oturn % 2 == 1 && currentState.children[i].score < currentState.children[theMove].score)
						theMove = (short)i;
				}
			}
			if (theMove == -1) //More intuitive selection.
			{
				int cardChoice, pieceChoice;
				checker.WaitOne();
				output += "\nSelect a card (0 for " + cardList[currentState.position[10 + (currentState.Oturn % 2) * 3]].name + ", 1 for " + cardList[currentState.position[11 + (currentState.Oturn % 2) * 3]].name + "). -1 to cancel: ";
				checker.ReleaseMutex();
				while (!int.TryParse(Console.ReadLine(), out cardChoice))
				{
					checker.WaitOne();
					output += "\nPlease enter a number: ";
					checker.ReleaseMutex();
				}
				if (cardChoice == -1)
					continue;
				cardChoice += 10 + (currentState.Oturn % 2) * 3;
				checker.WaitOne();
				output += "\n_______\n";
				for (short y = 0; y < 5; ++y)
				{
					output += "|";
					for (short x = 0; x < 5; ++x)
					{
						short i = 0;
						for (; i < 5; ++i)
						{
							if (currentState.position[i + (currentState.Oturn % 2) * 5] == (4 - y) * 5 + x)
							{
								output += i;
								break;
							}
						}
						if (i == 5)
							output += " ";
					}
					output += "|\n";
				}
				output += "_______\nSelect the piece you wish to move. -1 to cancel: ";
				checker.ReleaseMutex();
				while (!int.TryParse(Console.ReadLine(), out pieceChoice))
				{
					checker.WaitOne();
					output += "\nPlease enter a number: ";
					checker.ReleaseMutex();
				}
				if (pieceChoice == -1)
					continue;
				pieceChoice += (currentState.Oturn % 2) * 5;
				List<short> possibleMoves = new();
				for (short i = 0; i < currentState.children.Count; ++i)
					if (currentState.children[i].position[12] == currentState.position[cardChoice] && currentState.position[pieceChoice] != currentState.children[i].position[pieceChoice])
						possibleMoves.Add(i);
				checker.WaitOne();
				if (possibleMoves.Count == 0)
				{
					output = "No possible moves exist with this card and piece.\n";
					continue;
				}
				output = "_______\n";
				for (short y = 0; y < 5; ++y)
				{
					output += "|";
					for (short x = 0; x < 5; ++x)
					{
						if (currentState.position[pieceChoice] == (4 - y) * 5 + x)
						{
							if (pieceChoice == 2)
								output += "X";
							else if (pieceChoice < 5)
								output += "x";
							else if (pieceChoice == 7)
								output += "O";
							else output += "o";
							continue;
						}
						bool noMove = true;
						foreach (short i in possibleMoves)
							if (currentState.children[i].position[pieceChoice] == (4 - y) * 5 + x)
							{
								output += i;
								noMove = false;
								break;
							}
						if (noMove)
							output += " ";
					}
					output += "|\n";
				}
				output += "_______ Move ID's for your desired moves are shown above.\n";
				checker.ReleaseMutex();
			}
			else if (theMove == -2) //Go back one move.
			{
				if (currentState.parent is not null)
					currentState = currentState.parent;
			}
			else if (theMove == -3) //Print the suggested move.
			{
				int bestCandidate = 0;
				for (int i = 1; i < currentState.children.Count; ++i)
				{
					if (currentState.Oturn % 2 == 0 && currentState.children[i].score > currentState.children[bestCandidate].score)
						bestCandidate = i;
					else if (currentState.Oturn % 2 == 1 && currentState.children[i].score < currentState.children[bestCandidate].score)
						bestCandidate = i;
				}
				checker.WaitOne();
				output += "\n" + bestCandidate + " use " + cardList[currentState.children[bestCandidate].position[12]].name + ": " + currentState.children[bestCandidate].score + "\n" + PrintBoard(currentState.children[bestCandidate]);
				checker.ReleaseMutex();
			}
			else if (theMove == -4) //Dump state tree.
			{
				checker.WaitOne();
				output = "Tree dump:\n" + TreeDump();
				checker.ReleaseMutex();
			}
			else if (theMove == -5) //Print all possible options
			{
				for (int i = 0; i < currentState.children.Count; ++i)
				{
					checker.WaitOne();
					output += i + ": " + Print(currentState.children[i]) + " / " + currentState.children[i].score + "\n" + PrintBoard(currentState.children[i]);
					checker.ReleaseMutex();
				}
			}
			else if (theMove == -6) //the debug command
			{
				output += Print(currentState);
				for (int i = 0; i < cardList.Length; ++i)
					output += ", " + cardList[i].id;
			}
			else if (theMove >= currentState.children.Count || theMove < 0)
			{
				checker.WaitOne();
				output += "Not an option.\n";
				checker.ReleaseMutex();
			}
			else
			{
				//PRUNING: Set aside the branch that contains the move that was actually made.
				GameState chosenMove = currentState.children[theMove];
				currentState.children.Remove(chosenMove);
				GameState stateQuery = currentState;
				while (currentState.children.Count > 0)
				{
					//For every other branch: if it has children, go down the first child.
					while (stateQuery.children.Count > 0)
						stateQuery = stateQuery.children[0];
					//If it has no children, go back up one level, get the first child to null its parent, null the first child, then remove the first child.
					stateQuery = stateQuery.parent;
					stateQuery.children[0].parent = null;
					stateQuery.children[0] = null;
					stateQuery.children.RemoveAt(0);
				} 
				currentState.children.Add(chosenMove);
				currentState = chosenMove;
				//output = "";
				/*if (botO is null)
				{
					checker.WaitOne();
					output = PrintBoard(currentState);
					checker.ReleaseMutex();
				}*/
			}
		}
		//Thread.Sleep(1000); //If the 2nd-to-last move is shown too fast.
		checker.WaitOne();
		output = PrintBoard(currentState);
		if (botO is not null)
			output += "Game " + (gameInGeneration + 1) + ": ";
		if (CheckWin(currentState) == 0)
			output += "Draw!";
		else
		{
			result = (float)CheckWin(currentState) / (currentState.Oturn / 2);
			output += (CheckWin(currentState) > 0 ? "X" : "O") + " wins at turn " + currentState.Oturn + " for " + (gameInGeneration % 2 == 0 ? result : -result) + " points";
		}
		checker.ReleaseMutex();
		Thread.Sleep(3000);
		output = "Done";
		return result;
	}
	/*void TrainingGame(Card[] startingCards, Evaluator botX, Evaluator botO)
	{
		cardList = startingCards;
		GameState currentState = NewGame();

		while (CheckWin(currentState) == 0)
		{

		}
	}*/
	string TreeDump()
	{
		//go down, right, then up
		List<short> stack = new();
		string result = "";
		//find the root
		GameState curNode = currentState;
		while (curNode.parent is not null)
			curNode = curNode.parent;
		do
		{
			//print
			for (short k = 0; k < stack.Count; ++k)
				result += "| ";
			result += "|- (" + Print(curNode) + ") " + curNode.score + "\n";
			//go down
			if (curNode.children.Count > 0)
			{
				curNode = curNode.children[0];
				stack.Add(1);
			}
			else
			{
				while (curNode.parent.children.Count == stack.Last())
				{
					//go up
					curNode = curNode.parent;
					if (stack.Count == 1)
						return result;
					stack.RemoveAt(stack.Count - 1);
				}
				//go right
				curNode = curNode.parent.children[stack.Last()];
				++stack[stack.Count - 1];
			}
		} while (stack.Count > 0);
		return result;
	}
}