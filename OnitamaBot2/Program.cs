using OnitamaBot2;

theGame();

void theGame()
{
    newGame();
	
	/*while (1)
	{
		if (checkWin(currentState))
		{
			eventCode = 1;
			//for (short i = 0; i < 8; ++i)
			//worker[i].join();
			break;
		}
		short theMove = 1;
		thread worker = thread(MCSearch, &theMove);
		cout << "Computing your moves... Type 0 to stop the calculations or type anything else to view the current scores.\n";
		while (1)
		{
			cin >> theMove;
			cout << "Scores:\n";
			for (short i = 0; i < currentState->children.size(); ++i)
				//cout << (float)i->wins / i->playouts << "% (" << i->wins << "/" << i->playouts << ")\n";
				cout << i << ": " << currentState->children[i]->score << "\n";
			if (!theMove)
				break;
		}
		worker.join();
		if (currentState->Oturn)
			cout << "O";
		else cout << "X";
		cout << ", make your next move:\n";
	jizz:
		cin >> theMove;
		if (theMove == -1)
		{
			cout << "Tree dump:\n";
			treeDump();
			goto jizz;
		}
		else if (theMove == -2)
		{
			if (currentState->parent)
				currentState = currentState->parent;
			else goto jizz;
		}
		else if (theMove == -3)
		{
			eventCode = 2;
			goto jizz;
		}
		else if (theMove == -4)
		{
			eventCode = 0;
			goto jizz;
		}
		else if (theMove == -5)
		{
			goto jizz;
		}
		else if (theMove == -6) //the debug command
		{
			cout << jobQueue.size() << "\n";
		}
		else if (theMove >= currentState->children.size() || theMove < 0)
		{
			cout << "Not an option:\n";
			goto jizz;
		}
		else
		{
			currentState = currentState->children[theMove];
		}
	}*/
}

void newGame()
{
	GameState currentState;
	Card[] cardList = new Card[5];
	int choice = 0;
    Console.Write("X starts first.\nPlace the corresponding numbers of the 5 cards in play, side by side, in this order: X's first card, X's second card, turn card, O's first card, O's second card.\nReference:\n00: Boar\n01: Cobra\n02: Crab\n03: Crane\n04: Dragon\n05: Eel\n06: Elephant\n07: Frog\n08: Goose\n09: Horse\n10: Mantis\n11: Monkey\n12: Ox\n13: Rabbit\n14: Rooster\n15: Tiger\nExample: X starts with Boar (00) and Cobra (01), O starts with Crane (03) and Dragon (04). X always starts first, so the turn card, Crab (03) belongs to X. The input is thus: 0001020304.\n");
	Int32.TryParse(Console.ReadLine(), out choice); //1505030914
	for (short i = 0; i < 5; ++i)
		cardList[i] = new Card((short)((int)(choice / Math.Pow(10, 8 - i * 2)) % 100)); //To isolate the 2 digits we want, divide by a power of ten then floor to get rid of every digit to the right, then mod 100 to get rid of every digit to the left.

	GameState pos = new();
	pos.Oturn = false;
	pos.parent = null;
	pos.position[0] = 0;
	pos.position[1] = 1;
	pos.position[2] = 2;
	pos.position[3] = 3;
	pos.position[4] = 4;
	pos.position[5] = 20;
	pos.position[6] = 21;
	pos.position[7] = 22;
	pos.position[8] = 23;
	pos.position[9] = 24;
	pos.position[10] = 0;
	pos.position[11] = 1;
	pos.position[12] = 2;
	pos.position[13] = 3;
	pos.position[14] = 4;
	currentState = pos;

	printBoard(currentState);
	checkMoves(currentState, cardList);
}

string print(GameState state)
{
	string result = "";
	foreach (int i in state.position)
		result += i + ", ";
	return result;
}

void printBoard(GameState state)
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
					if (i < 5)
					{
						if (i == 2)
							Console.Write("X");
						else Console.Write("x");
					}
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

short checkWin(GameState curState)
{
	if (curState.position[7] < 0)
	{
		Console.WriteLine("X Stone.");
		return 1;
	}
	else if (curState.position[2] == 22)
	{
		Console.WriteLine("X Stream.");
		return 1;
	}
	else if (curState.position[2] < 0)
	{
		Console.WriteLine("O Stone.");
		return -1;
	}
	else if (curState.position[7] < 2)
	{
		Console.WriteLine("O Stream.");
		return -1;
	}
	else return 0;
}

void checkMoves(GameState curState, Card[] cardList)
{
	//for each piece
	for (short i = 0; i < 5; ++i)
	{
		//skip if dead (-1)
		if (curState.position[curState.Oturn ? i + 5 : i] < 0)
			continue;
		//for each possible move from the 1st card
		for (short j = 0; j < cardList[curState.position[curState.Oturn ? 13 : 10]].moveList.Count(); ++j)
		{
			int result = curState.position[curState.Oturn ? i + 5 : i] - (curState.Oturn ? 1 : -1) * cardList[curState.position[curState.Oturn ? 13 : 10]].moveList[j];
			//if piece goes out of bounds vertically
			if (result < 0 || result > 24)
				continue;
			//if piece lands on allies
			short k = 0;
			for (; k < 5; ++k)
			{
				if (k == i)
					continue;
				if (curState.position[curState.Oturn ? k + 5 : k] == result)
					break;
			}
			if (k != 5)
				continue;
			//if piece goes out of bounds horizontally
			int horizontal = (curState.position[curState.Oturn ? i + 5 : i] % 5) - (curState.Oturn ? 1 : -1) * (((((cardList[curState.position[curState.Oturn ? 13 : 10]].moveList[j] + 2) % 5) - 4) % 5) + 2);
			if (horizontal < 0 || horizontal > 4)
				continue;
			//if execution reaches, this move is legal
			GameState pos = new();
			pos.Oturn = !curState.Oturn;
			pos.parent = curState;
			pos.score = 100;
			for (k = 0; k < 15; ++k)
				pos.position[k] = curState.position[k];
			pos.position[curState.Oturn ? i + 5 : i] = (short)result;
			/*for (k = 5; k < 10; ++k)
				if (pos.position[curState.Oturn ? k + 5 : k] == result)
				{
					pos.position[curState.Oturn ? k + 5 : k] = -1;
					break;
				}*/
			for (k = 0; k < 5; ++k)
				if (pos.position[curState.Oturn ? k : k + 5] == result)
				{
					pos.position[curState.Oturn ? k : k + 5] = -1;
					break;
				}
			short swapCard = pos.position[12];
			pos.position[12] = pos.position[curState.Oturn ? 13 : 10];
			pos.position[curState.Oturn ? 13 : 10] = swapCard;
			curState.children.Add(pos);
			Console.WriteLine(curState.children.Count() - 1);
			printBoard(pos);
			Console.WriteLine(print(pos));
		}
		//for each possible move from the 2nd card
		for (short j = 0; j < cardList[curState.position[curState.Oturn ? 14 : 11]].moveList.Count(); ++j)
		{
			int result = curState.position[curState.Oturn ? i + 5 : i] - (curState.Oturn ? 1 : -1) * cardList[curState.position[curState.Oturn ? 14 : 11]].moveList[j];
			//if piece goes out of bounds vertically
			if (result < 0 || result > 24)
				continue;
			//if piece lands on allies
			short k = 0;
			for (; k < 5; ++k)
			{
				if (k == i)
					continue;
				if (curState.position[curState.Oturn ? k + 5 : k] == result)
					break;
			}
			if (k != 5)
				continue;
			//if piece goes out of bounds horizontally
			int horizontal = (curState.position[curState.Oturn ? i + 5 : i] % 5) - (curState.Oturn ? 1 : -1) * (((((cardList[curState.position[curState.Oturn ? 14 : 11]].moveList[j] + 2) % 5) - 4) % 5) + 2);
			if (horizontal < 0 || horizontal > 4)
				continue;
			//if execution reaches, this move is legal
			GameState pos = new();
			pos.Oturn = !curState.Oturn;
			pos.parent = curState;
			pos.score = 100;
			for (k = 0; k < 15; ++k)
				pos.position[k] = curState.position[k];
			pos.position[curState.Oturn ? i + 5 : i] = (short)result;
			/*for (k = 5; k < 10; ++k)
				if (pos.position[curState.Oturn ? k + 5 : k] == result)
				{
					pos.position[curState.Oturn ? k + 5 : k] = -1;
					break;
				}*/
			for (k = 0; k < 5; ++k)
				if (pos.position[curState.Oturn ? k : k + 5] == result)
				{
					pos.position[curState.Oturn ? k : k + 5] = -1;
					break;
				}
			short swapCard = pos.position[12];
			pos.position[12] = pos.position[curState.Oturn ? 14 : 11];
			pos.position[curState.Oturn ? 14 : 11] = swapCard;
			curState.children.Add(pos);
			Console.WriteLine(curState.children.Count() - 1);
			printBoard(pos);
			Console.WriteLine(print(pos));
		}
	}
}