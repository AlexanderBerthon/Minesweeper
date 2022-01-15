/**
 * This project is an attempt to replicate the game 'minesweeper' 
 * there are 100 cells total and a portion of those cells contain a hidden bomb
 * the goal of the game is to carefully clear each cell and find the bombs without triggering them
 * when all safe spaces are found, you win
 * if any bomb is triggered, you lose
 * safe cells that are cleared will display hints if there is a bomb nearby, indicated by a number value 0-4
 * a 0 value represents no bombs adjacent to the safe tile while a 4 value means all 4 adjacent tiles are bombs
 * use these hints to clear the board safely and mark all the bombs
 * 
 * The game is currently limited to a 10x10 grid
 * The games difficulty can be changed by modifying the range of the bomb assignment value. located in the grid instantiation section.
 * The more broad the range, the more bombs will be assigned and the harder the game will be.
 */
namespace MatrixProjectUI {
    /**
     * This class represents the UI Form, all game code is within this class.. but it could/should be separated into it's own file
     */
    public partial class Form1 : Form {
        Button[] buttonArray = new Button[100]; //communication between button UI and int matrix backend
        int[,] matrix = new int[10,10]; //calculation and build
        Random random = new Random();
        int bombCount = 0;
        int flagCount = 0;

        /**
         * This method does all of the building and setup for the game
         * Creates the matrix of safe/bomb spaces
         * Pre-solves the matrix and hides it from the user
         * Assigns values to be written to the UI elements
         */
        public Form1() {
            InitializeComponent();
            flowLayoutPanel1.Controls.CopyTo(buttonArray, 0);

            //builds game grid, bombs, etc.
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 10; j++) {
                    if (random.Next(100) < 25) { //difficulty setting
                        matrix.SetValue(5, i, j);
                        bombCount++; 
                    }
                    else {
                        matrix.SetValue(7, i, j); //flag value, needed for next step
                    }
                }
            }
            //solves the game grid, assigns values to non-bomb spaces based on proximity
            for (int i = 0; i < Math.Sqrt(matrix.Length); i++) {
                for (int j = 0; j < Math.Sqrt(matrix.Length); j++) {
                    if (matrix[i, j] == 7) {
                        matrix[i, j] = 0;
                        //check 4 adjacent cells
                        try {
                            if (matrix[i - 1, j] == 5) {
                                matrix[i, j]++;
                            }
                        }
                        catch (IndexOutOfRangeException) { }
                        try {
                            if (matrix[i, j - 1] == 5) {
                                matrix[i, j]++;
                            }
                        }
                        catch (IndexOutOfRangeException) { }
                        try {
                            if (matrix[i + 1, j] == 5) {
                                matrix[i, j]++;
                            }
                        }
                        catch (IndexOutOfRangeException) { }
                        try {
                            if (matrix[i, j + 1] == 5) {
                                matrix[i, j]++;
                            }
                        }
                        catch (IndexOutOfRangeException) { }
                    }
                }
            }
            //Transfers data from int matrix to text on button grid / UI
            List<String> temp = new List<String>();
            for (int i = 0; i < Math.Sqrt(matrix.Length); i++) {
                for (int j = 0; j < Math.Sqrt(matrix.Length); j++) {
                    temp.Add(""+matrix[i, j]);
                }
            }

            for (int i = 0; i<temp.Count; i++) {
                if (temp[i] == "0") {
                    buttonArray[i].Text = "?  ";
                }
                else {
                    buttonArray[i].Text = "? " + temp[i];
                }
            }
            label1.Text = "Total Bombs: " + bombCount;
            label2.Text = "Flags: " + flagCount;
        }

        /**
         * This method builds the flow layout. Responsible for houseing the 100 buttons and their data. 
         * Important to build but I don't need to modify it in any way, so no extra code within.
         */
        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) {
        }

        /**
         * This method is responsible for a majority of the work / gameplay. 
         * It is an event handler method that is run when the user LEFT clicks on a cell
         * When clicked, the data on the corresponding UI element is retrieved. ie the button / cell
         * The UI button data is extracted and compared to the matrix to determine what action to take
         * If the clicked button/cell was a bomb. the game is over and the player loses.
         * If the clicked button/cell was not a bomb, then that cells information is displayed.
         * The surrounding 8 cells are also all checked in the same way.
         * If a surrounding cell is a bomb, it's information is not revealed
         * If a surrounding cell is not a bomb, it's information is revealed
         */
        private void button_Click(object sender, EventArgs e) {
            Button btn = (Button)sender; //can't access sender data without instantiating it. for.. reasons...
            if (btn.Text.Length == 1) {
                //Prevents player from clicking an already revealed cell and gaming the system.
            }
            else { //messy code to relate correct button object to data in grid backend. Jankyness due to pre-generated UI that I can't modify
                if (matrix[int.Parse(btn.Name.Substring(btn.Name.Length - 2, 1)), int.Parse(btn.Name.Substring(btn.Name.Length - 1))] == 5) {
                    label1.Text = "GAME OVER";
                    for (int i = 0; i < buttonArray.Length; i++) {
                        if (buttonArray[i].Text.Contains("5")) {
                            buttonArray[i].Text = "X";
                        }
                        else {
                            buttonArray[i].Text = "";
                        }
                        buttonArray[i].Enabled = false;
                        buttonArray[i].BackColor = Color.White;
                    }
                }
                else {

                    int goodFlag = 0;

                    if (btn.Text.Contains("X")) {
                        flagCount--;
                        label2.Text = "Flags: " + flagCount;
                    }
                    btn.Text = btn.Text.Substring(btn.Text.Length - 1); //reveal current cell
                    Button nextBtn;

                    /**
                    code is messy, but I think this is the 'best' way to do it.
                    if I made a case:switch statement I could avoid having to run all 8 checks each time
                    but I would have to repeat the same code even more times, so it would look even worse to save a total of..
                    math..
                    9% reduction of if statements from this method.
                    so.. not really worth it? an if statement check is already basically 0 processing power and O(1)
                    9% would be a negligible efficiency gain in exchange for an extra 100 lines of code
                    maybe I could still make a helper function to make this code a bit more readable though..
                    can't. unless you send the button? 
                    */
                    try {
                        nextBtn = buttonArray[btn.TabIndex - 10];   //top
                        if (matrix[int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 2, 1)), int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 1))] != 5) {
                            if (nextBtn.Text.Contains("X")) {
                                flagCount--;
                                label2.Text = "Flags: " + flagCount;
                            }
                            nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                        }
                    }
                    catch (IndexOutOfRangeException) { }
                    try {
                        nextBtn = buttonArray[btn.TabIndex + 10];   //bot
                        if (matrix[int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 2, 1)), int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 1))] != 5) {
                            if (nextBtn.Text.Contains("X")) {
                                flagCount--;
                                label2.Text = "Flags: " + flagCount;
                            }
                            nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                        }
                    }
                    catch (IndexOutOfRangeException) { }
                    //prevent left cells from wrapping
                    if (int.Parse(btn.Name.Substring(btn.Name.Length - 1)) != 0) {
                        try {
                            nextBtn = buttonArray[btn.TabIndex - 1];    //left
                            if (matrix[int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 2, 1)), int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 1))] != 5) {
                                if (nextBtn.Text.Contains("X")) {
                                    flagCount--;
                                    label2.Text = "Flags: " + flagCount;
                                }
                                nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                            }
                        }
                        catch (IndexOutOfRangeException) { }
                        try {
                            nextBtn = buttonArray[btn.TabIndex - 11];   //top_left
                            if (matrix[int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 2, 1)), int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 1))] != 5) {
                                if (nextBtn.Text.Contains("X")) {
                                    flagCount--;
                                    label2.Text = "Flags: " + flagCount;
                                }
                                nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                            }
                        }
                        catch (IndexOutOfRangeException) { }
                        try {
                            nextBtn = buttonArray[btn.TabIndex + 9];    //bot_left
                            if (matrix[int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 2, 1)), int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 1))] != 5) {
                                if (nextBtn.Text.Contains("X")) {
                                    flagCount--;
                                    label2.Text = "Flags: " + flagCount;
                                }
                                nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                            }
                        }
                        catch (IndexOutOfRangeException) { }
                    }
                    //prevent right cells from wrapping
                    if (int.Parse(btn.Name.Substring(btn.Name.Length - 1)) != 9) {
                        try {
                            nextBtn = buttonArray[btn.TabIndex + 1];    //right
                            if (matrix[int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 2, 1)), int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 1))] != 5) {
                                if (nextBtn.Text.Contains("X")) {
                                    flagCount--;
                                    label2.Text = "Flags: " + flagCount;
                                }
                                nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                            }
                        }
                        catch (IndexOutOfRangeException) { }
                        try {
                            nextBtn = buttonArray[btn.TabIndex - 9];    //top_right
                            if (matrix[int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 2, 1)), int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 1))] != 5) {
                                if (nextBtn.Text.Contains("X")) {
                                    flagCount--;
                                    label2.Text = "Flags: " + flagCount;
                                }
                                nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                            }
                        }
                        catch (IndexOutOfRangeException) { }
                        try {
                            nextBtn = buttonArray[btn.TabIndex + 11];   //bot_right
                            if (matrix[int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 2, 1)), int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 1))] != 5) {
                                if (nextBtn.Text.Contains("X")) {
                                    flagCount--;
                                    label2.Text = "Flags: " + flagCount;
                                }
                                nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                            }
                        }
                        catch (IndexOutOfRangeException) { }
                    }
                    //win condition check
                    if (bombCount == flagCount) {
                        for (int i = 0; i < buttonArray.Length; i++) {
                            if (buttonArray[i].Text.Length > 1) {
                                goodFlag++;
                            }
                        }
                        if (bombCount == goodFlag) {
                            label1.Text = "YOU WIN!";
                            for (int i = 0; i < buttonArray.Length; i++) {
                                buttonArray[i].Enabled = false;
                                buttonArray[i].BackColor = Color.White;
                            }
                        }
                    }
                }
            }
        }

        /**
         * This method is responsible for right click / flag functionality
         * It is an event handler method intentionally separate from the previous _Click handler
         * _Click does not support right click actions while _MouseDown does
         * Left click functionality is kept separate for usability and positive gameplay experience reasons.
         * By keeping it in the _Click method, left click actions are able to be canceled if the user misclicks or fat fingers a
         * cell on accident by holding down left click and only releasing it on the cell that they want to play.
         * the _MouseDown method will not allow this flexibility
         * 
         * The right click funtionality by contrast can be reverted by simply right clicking the cell again, as it is just a visual
         * flag value and does not reveal any data in and of itself. 
         * 
         * As for what this method does exactly, it simply displays a flag value to the user so that they may more easily keep track
         * of cells they believe to be bombs.
         * any cell can be marked, even if it is a safe space.
         * it is up to the player to determine the correct safe and non-safe spaces
         */
        private void Button_MouseDown(object sender, MouseEventArgs e) {
            Button btn = (Button)sender;
            int goodFlag = 0;


            if (e.Button == MouseButtons.Right && btn.Text.Length>1) {
                if (btn.Text.Contains("X")){
                    btn.Text = "?" + btn.Text.Substring(1);
                    flagCount--;
                }
                else{
                    btn.Text = "X" + btn.Text.Substring(1);
                    flagCount++;
                }
                label2.Text = "Flags: " + flagCount;
            }

            //win condition check
            if (bombCount == flagCount) {
                for (int i = 0; i < buttonArray.Length; i++) {
                    if (buttonArray[i].Text.Length > 1) {
                        goodFlag++;
                    }
                }
                if (bombCount == goodFlag) {
                    label1.Text = "YOU WIN!";
                    for (int i = 0; i < buttonArray.Length; i++) {
                        buttonArray[i].Enabled = false;
                        buttonArray[i].BackColor = Color.White;
                    }
                }
            }
        }
    }
}