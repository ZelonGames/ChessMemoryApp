using ChessMemoryApp.Model.Memory_Techniques;
using ChessMemoryApp.Model.UI_Components;

namespace ChessMemoryApp;

public partial class MemoryPalacePage : ContentPage
{
	public MemoryPalacePage()
	{
		InitializeComponent();
	}

	public void OnClickedGetWords(object sender, EventArgs e)
	{
		var numberToWordConverter = new NumberToWordConverter();
        numberToWordConverter.LoadWordsFromFile("MemoryPalace/swe_wordlist.txt");
		MoveNotationButtonGenerator.GenerateButtons(textBoxMoves.Text, layoutWordButtons);
	}
}