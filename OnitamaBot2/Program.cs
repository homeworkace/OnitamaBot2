using OnitamaBot2;

theGame();

void theGame()
{
    newGame();
}

void newGame()
{
	GameState currentState;
	Card[] cardList = new Card[5];
	int choice = 0;
    Console.WriteLine("X starts first.\nPlace the corresponding numbers of the 5 cards in play, side by side, in this order: X's first card, X's second card, turn card, O's first card, O's second card.\nReference:\n00: Boar\n01: Cobra\n02: Crab\n03: Crane\n04: Dragon\n05: Eel\n06: Elephant\n07: Frog\n08: Goose\n09: Horse\n10: Mantis\n11: Monkey\n12: Ox\n13: Rabbit\n14: Rooster\n15: Tiger\nExample: X starts with Boar (00) and Cobra (01), O starts with Crane (03) and Dragon (04). X always starts first, so the turn card, Crab (03) belongs to X. The input is thus: 0001020304.\n");
	Int32.TryParse(Console.ReadLine(), out choice);
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
	checkMoves(currentState);
}

void checkMoves(GameState curState)
{
	string curPosition = "";
	for (int i = 0; i < curState.position.Length; i++)
		curPosition = curPosition + ", " + curState.position[i];
	Console.WriteLine(curPosition);
}