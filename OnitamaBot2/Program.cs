using OnitamaBot2;

Training();
//PassAndPlay(); //Cobra, Monkey, Goose, Boar, Rabbit
/*Evaluator botA = new();
botA.Initialise("..//..//..//..//Genomes//testX.txt");
Evaluator botB = new();
botB.Initialise("..//..//..//..//Genomes//testO.txt");
botB.Evolve(botA, 5);
botB.WriteGenes("..//..//..//..//Genomes//DemonSpawn.txt");*/

void PassAndPlay()
{
	Game theGame = new();
	theGame.NewGame(new Evaluator(), null, Array.Empty<Card>());
	theGame.TheGame();
}
void Training()
{
	double generationScore = 0;
	float roundScore = 0;
	Evaluator botA = new();
	botA.Initialise("..//..//..//..//Genomes//testX.txt");
	Evaluator botB = new();
	botB.Initialise("..//..//..//..//Genomes//testO.txt");

	for (int generation = 0; generation < 3; generation++)
	{
		for (int i = 0; i < 50; ++i)
		{
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
			roundScore = theGame.TheGame();
			generationScore += roundScore;
			Console.WriteLine("Round " + (1 + i * 2) + ": " + roundScore + ", total score: " + generationScore);

			//2nd round - same setup, but sides are changed
			theGame = new();
			theGame.NewGame(botB, botA, new[] { new Card((short)cardNumbers[0]), new Card((short)cardNumbers[1]), new Card((short)cardNumbers[2]), new Card((short)cardNumbers[3]), new Card((short)cardNumbers[4]) });
			roundScore = theGame.TheGame();
			generationScore -= roundScore;
			Console.WriteLine("Round " + (2 + i * 2) + ": " + -roundScore + ", total score: " + generationScore);

			/*if (i % 5 == 4)
			{
				//Console.Write("State of the game at move " + currentState.Oturn + ". Type \"0\" to quit, anything else to continue: ");
				Console.Write("Type \"0\" to quit, anything else to continue: ");
				if (Console.ReadLine() == "0")
					break;
			}*/
		}

		//Generation done - evolve the loser
		if (generationScore == 0)
			return;
		else if (generationScore > 0)
		{
			botB.Evolve(botA, Math.Abs(generationScore));
			botB.WriteGenes("..//..//..//..//Genomes//" + (generation + 1) + "_A.txt");
		}
		else
		{
			botA.Evolve(botB, Math.Abs(generationScore));
			botA.WriteGenes("..//..//..//..//Genomes//" + (generation + 1) + "_B.txt");
		}
	}
}

/*Evaluator theBot = new();
theBot.Initialise();
//theBot.WriteGenes("..//..//..//..//Genomes//test.txt");
//theBot.Initialise("..//..//..//..//Genomes//test.txt");
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
Console.WriteLine(theBot.GeneticEvaluate(pos, new[] { new Card(0), new Card(1), new Card(2), new Card(3), new Card(4) }));
Console.WriteLine("bruh");
//float bruh = 5;
//Console.WriteLine(bruh / 16 + bruh / 17 + bruh / 25 + bruh / 32);*/

class Game
{
	GameState currentState;
	public Card[] cardList;
	Queue<GameState> jobQueue = new();
	public Evaluator botX, botO;
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
		PrintBoard(currentState);
		return currentState;
	}
	string Print(GameState state)
	{
		string result = "";
		foreach (int i in state.position)
			result += i + ", ";
		//for (int i = 10; i < 15; ++i)
		//	result += state.position[i] + ", ";
		result.Remove(result.Length - 2, 2);
		return result;
	}
	void PrintBoard(GameState state)
	{
		Console.Write("_______\n");
		for (short y = 0; y < 5; ++y)
		{
			Console.Write("|");
			for (short x = 0; x < 5; ++x)
			{
				short i = 0;
				for (; i < 10; ++i)
				{
					if (state.position[i] == (4 - y) * 5 + x)
					{
						if (i == 2)
							Console.Write("X");
						else if (i < 5)
							Console.Write("x");
						else if (i == 7)
							Console.Write("O");
						else Console.Write("o");
						break;
					}
				}
				if (i == 10)
					Console.Write(" ");
			}
			Console.Write("|\n");
		}
		Console.Write("_______\n");
	}
	public float TheGame()
	{
		short theMove = 1;

		while (CheckWin(currentState) == 0 && currentState.Oturn < 200)
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
			if (botO is null || currentState.Oturn % 2 == 0)
				botX.EvaluateFrom(currentState);
			else botO.EvaluateFrom(currentState);
			if (botO is null)
			{
				if (currentState.Oturn % 2 == 1)
					Console.Write("O");
				else Console.Write("X");
				Console.Write(", make your next move.\n-1 for more intuitive input, -2 to go back one move, -3 to show the suggested move, -4 to dump state tree, -5 to show all possible moves: ");
				while (!short.TryParse(Console.ReadLine(), out theMove))
					Console.Write("Please enter a number: ");
            }
			else
			{
				//if botO exists, training mode is enabled. we dont need any player involvement
				PrintBoard(currentState);
				Console.WriteLine("State of the game at move " + currentState.Oturn);
				/*if (currentState.Oturn % 10 == 0)
				{
					//Console.Write("State of the game at move " + currentState.Oturn + ". Type \"0\" to quit, anything else to continue: ");
					Console.Write("Type \"0\" to quit, anything else to continue: ");
					if (Console.ReadLine() == "0")
						break;
				}*/
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
				Console.Write("Select a card (0 for " + cardList[currentState.position[10 + (currentState.Oturn % 2) * 3]].name + ", 1 for " + cardList[currentState.position[11 + (currentState.Oturn % 2) * 3]].name + "). -1 to cancel: ");
				while (!int.TryParse(Console.ReadLine(), out cardChoice))
					Console.Write("Please enter a number: ");
				if (cardChoice == -1)
					continue;
				cardChoice += 10 + (currentState.Oturn % 2) * 3;
				Console.Write("_______\n");
				for (short y = 0; y < 5; ++y)
				{
					Console.Write("|");
					for (short x = 0; x < 5; ++x)
					{
						short i = 0;
						for (; i < 5; ++i)
						{
							if (currentState.position[i + (currentState.Oturn % 2) * 5] == (4 - y) * 5 + x)
							{
								Console.Write(i);
								break;
							}
						}
						if (i == 5)
							Console.Write(" ");
					}
					Console.Write("|\n");
				}
				Console.Write("_______\nSelect the piece you wish to move. -1 to cancel: ");
				while (!int.TryParse(Console.ReadLine(), out pieceChoice))
					Console.Write("Please enter a number: ");
				if (pieceChoice == -1)
					continue;
				pieceChoice += (currentState.Oturn % 2) * 5;
				List<short> possibleMoves = new();
				for (short i = 0; i < currentState.children.Count; ++i)
					if (currentState.children[i].position[12] == currentState.position[cardChoice] && currentState.position[pieceChoice] != currentState.children[i].position[pieceChoice])
						possibleMoves.Add(i);
				if (possibleMoves.Count == 0)
                {
					Console.Write("No possible moves exist with this card and piece.\n");
					continue;
                }
				Console.Write("_______\n");
				for (short y = 0; y < 5; ++y)
				{
					Console.Write("|");
					for (short x = 0; x < 5; ++x)
					{
						if (currentState.position[pieceChoice] == (4 - y) * 5 + x)
                        {
							if (pieceChoice == 2)
								Console.Write("X");
							else if (pieceChoice < 5)
								Console.Write("x");
							else if (pieceChoice == 7)
								Console.Write("O");
							else Console.Write("o");
							continue;
						}
						bool noMove = true;
						foreach (short i in possibleMoves)
							if (currentState.children[i].position[pieceChoice] == (4 - y) * 5 + x)
							{
								Console.Write(i);
								noMove = false;
								break;
							}
						if (noMove)
							Console.Write(" ");
					}
					Console.Write("|\n");
				}
				Console.Write("_______ Move ID's for your desired moves are shown above.\n");
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
				Console.WriteLine(bestCandidate + " use " + cardList[currentState.children[bestCandidate].position[12]].name + ": " + currentState.children[bestCandidate].score);
				PrintBoard(currentState.children[bestCandidate]);
			}
			else if (theMove == -4) //Dump state tree.
			{
				Console.Write("Tree dump:\n");
				TreeDump();
			}
			else if (theMove == -5) //Print all possible options
			{
				for (int i = 0; i < currentState.children.Count; ++i)
                {
					Console.WriteLine(i + ": " + Print(currentState.children[i]) + " / " + currentState.children[i].score);
					PrintBoard(currentState.children[i]);
				}
			}
			else if (theMove == -6) //the debug command
			{
			}
			else if (theMove >= currentState.children.Count || theMove < 0)
			{
				Console.Write("Not an option.\n");
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
				if (botO is null)
					PrintBoard(currentState);
			}
		}
		PrintBoard(currentState);
		if (CheckWin(currentState) == 0)
		{
			Console.Write("Draw!\n");
			return 0;
		}
		else Console.WriteLine((CheckWin(currentState) > 0 ? "X" : "O") + " wins at turn " + currentState.Oturn);
		return (float)CheckWin(currentState) / (currentState.Oturn / 2);
	}
	/*void TrainingGame(Card[] startingCards, Evaluator botX, Evaluator botO)
    {
		cardList = startingCards;
		GameState currentState = NewGame();

		while (CheckWin(currentState) == 0)
        {

        }
    }*/
	void TreeDump()
	{
		//go down, right, then up
		List<short> stack = new();
		//find the root
		GameState curNode = currentState;
		while (curNode.parent is not null)
			curNode = curNode.parent;
		do
		{
			//print
			for (short k = 0; k < stack.Count; ++k)
				Console.Write("| ");
			Console.Write("|- (" + Print(curNode) + ") " + curNode.score + "\n");
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
						return;
					stack.RemoveAt(stack.Count - 1);
				}
				//go right
				curNode = curNode.parent.children[stack.Last()];
				++stack[stack.Count - 1];
			}
		} while (stack.Count > 0);
	}
}