using System;
using System.Diagnostics;
using System.Threading;

namespace Minesweeper {
    public partial class Form1 : Form {
        Button[] buttonArray = new Button[100]; //communication between button UI and int matrix backend
        int[,] matrix = new int[10,10]; //calculation and build
        Random random = new Random();
        int bombCount = 0;
        int flagCount = 0;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        int clock;

        private void TimerEventProcessor(Object anObject, EventArgs eventArgs) {
            clock--;
            if (clock <= 0) {
                timer.Stop();

                //end the game
                GameOverLabel.Text = "YOU LOSE!";
                GameOverLabel.Visible = true;
                PlayAgainLabel.Visible = true;
                ContinueButton.Visible = true;
                ExitButton.Visible = true;
                flagCount = 0;
                timer.Stop();
                for (int i = 0; i < buttonArray.Length; i++) {
                    if (buttonArray[i].Text.Contains("X") && buttonArray[i].Text.Contains("5")) {
                        buttonArray[i].ForeColor = Color.White;
                        buttonArray[i].Text = "X";
                        flagCount += 1;
                    }
                    else if (buttonArray[i].Text.Contains("5")) {
                        buttonArray[i].ForeColor = Color.Firebrick;
                        buttonArray[i].Text = "X";
                    }
                    else {
                        buttonArray[i].Text = " ";
                    }
                }
                flagCountLabel.Text = "Flags: "+flagCount;

            }
            else {
                //reset event clock
                timer.Stop();
                timer.Start();
            }

            if(clock % 60 < 10) {
                timerLabel.Text = clock / 60 + ":0" + clock % 60;
            }
            else {
                timerLabel.Text = clock / 60 + ":" + clock % 60;
            }
        }

        public Form1() {
            InitializeComponent();
            timer.Interval = 1000;
            timer.Start();
            timer.Tick += new EventHandler(TimerEventProcessor);
            flowLayoutPanel1.Controls.CopyTo(buttonArray, 0);
            
            //builds game grid, bombs, etc.
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 10; j++) {
                    if (random.Next(100) < 30) { //difficulty setting
                        matrix.SetValue(5, i, j);
                        bombCount++; 
                    }
                    else {
                        matrix.SetValue(7, i, j); //flag value, needed for next step
                    }
                }
            }

            //set dynamic time
            clock = 4 * bombCount;

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
            bombCountLabel.Text = "Total Bombs: " + bombCount;
            flagCountLabel.Text = "Flags: " + flagCount;
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) {
        }

        //player click event handler
        private void button_Click(object sender, EventArgs e) {
            Button btn = (Button)sender;
            //clicked on already revealed cell, do nothing to prevent cheating
            if (btn.Text.Length == 1) {
            }
            else {
                //clicked on a bomb
                if (matrix[int.Parse(btn.Name.Substring(btn.Name.Length - 2, 1)), int.Parse(btn.Name.Substring(btn.Name.Length - 1))] == 5) {
                    GameOverLabel.Text = "YOU LOSE!";
                    GameOverLabel.Visible = true;
                    PlayAgainLabel.Visible = true;
                    ContinueButton.Visible = true;
                    ExitButton.Visible = true;
                    flagCount = 0;
                    timer.Stop();
                    for (int i = 0; i < buttonArray.Length; i++) {
                        if (buttonArray[i].Text.Contains("X") && buttonArray[i].Text.Contains("5")) {
                            buttonArray[i].ForeColor = Color.White;
                            buttonArray[i].Text = "X";
                            flagCount += 1;
                        }
                        else if (buttonArray[i].Text.Contains("5")) {
                            buttonArray[i].ForeColor = Color.Firebrick;
                            buttonArray[i].Text = "X";
                        }
                        else {
                            buttonArray[i].Text = " ";
                        }
                    }
                    flagCountLabel.Text = "Flags: " + flagCount;
                }
                //clicked on a valid cell
                else {
                    int goodFlag = 0;

                    //fixes flag count if player clicks on a cell they marked as a bomb but it wasn't a bomb
                    if (btn.Text.Contains("X")) {
                        flagCount--;
                        flagCountLabel.Text = "Flags: " + flagCount;
                    }

                    //reveal current cell
                    btn.Text = btn.Text.Substring(btn.Text.Length - 1);
                    btn.ForeColor = Color.White;
                    Button nextBtn;

                    //reveal surrounding cells as needed
                    try {
                        nextBtn = buttonArray[btn.TabIndex - 10];   //top
                        if (matrix[int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 2, 1)), int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 1))] != 5) {
                            if (nextBtn.Text.Contains("X")) {
                                flagCount--;
                                flagCountLabel.Text = "Flags: " + flagCount;
                            }
                            nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                            nextBtn.ForeColor = Color.White;
                        }
                    }
                    catch (IndexOutOfRangeException) { }
                    try {
                        nextBtn = buttonArray[btn.TabIndex + 10];   //bot
                        if (matrix[int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 2, 1)), int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 1))] != 5) {
                            if (nextBtn.Text.Contains("X")) {
                                flagCount--;
                                flagCountLabel.Text = "Flags: " + flagCount;
                            }
                            nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                            nextBtn.ForeColor = Color.White;
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
                                    flagCountLabel.Text = "Flags: " + flagCount;
                                }
                                nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                                nextBtn.ForeColor = Color.White;
                            }
                        }
                        catch (IndexOutOfRangeException) { }
                        try {
                            nextBtn = buttonArray[btn.TabIndex - 11];   //top_left
                            if (matrix[int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 2, 1)), int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 1))] != 5) {
                                if (nextBtn.Text.Contains("X")) {
                                    flagCount--;
                                    flagCountLabel.Text = "Flags: " + flagCount;
                                }
                                nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                                nextBtn.ForeColor = Color.White;
                            }
                        }
                        catch (IndexOutOfRangeException) { }
                        try {
                            nextBtn = buttonArray[btn.TabIndex + 9];    //bot_left
                            if (matrix[int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 2, 1)), int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 1))] != 5) {
                                if (nextBtn.Text.Contains("X")) {
                                    flagCount--;
                                    flagCountLabel.Text = "Flags: " + flagCount;
                                }
                                nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                                nextBtn.ForeColor = Color.White;
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
                                    flagCountLabel.Text = "Flags: " + flagCount;
                                }
                                nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                                nextBtn.ForeColor = Color.White;
                            }
                        }
                        catch (IndexOutOfRangeException) { }
                        try {
                            nextBtn = buttonArray[btn.TabIndex - 9];    //top_right
                            if (matrix[int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 2, 1)), int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 1))] != 5) {
                                if (nextBtn.Text.Contains("X")) {
                                    flagCount--;
                                    flagCountLabel.Text = "Flags: " + flagCount;
                                }
                                nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                                nextBtn.ForeColor = Color.White;
                            }
                        }
                        catch (IndexOutOfRangeException) { }
                        try {
                            nextBtn = buttonArray[btn.TabIndex + 11];   //bot_right
                            if (matrix[int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 2, 1)), int.Parse(nextBtn.Name.Substring(nextBtn.Name.Length - 1))] != 5) {
                                if (nextBtn.Text.Contains("X")) {
                                    flagCount--;
                                    flagCountLabel.Text = "Flags: " + flagCount;
                                }
                                nextBtn.Text = nextBtn.Text.Substring(nextBtn.Text.Length - 1);
                                nextBtn.ForeColor = Color.White;
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
                            GameOverLabel.Text = "YOU WIN!";
                            GameOverLabel.Visible = true;
                            PlayAgainLabel.Visible = true;
                            ContinueButton.Visible = true;
                            ExitButton.Visible = true;
                            timer.Stop();
                        }
                    }
                }
            }
        }

        /**
         * right click event handler / flag function
         * this is an event handler intentionally separate from the previous _Click handler
         * _Click does not support right click actions while _MouseDown does
         * left click functionality is kept separate to improve gameplay experience
         * by keeping it in the _Click method, left click actions are able to be canceled if the user misclicks or fat fingers a
         * cell on accident by holding down left click and only releasing it on the cell that they want to play.
         * the _MouseDown method will not allow this flexibility
         * 
         * the right click funtionality by contrast can be reverted by simply right clicking the cell again, as it is just a visual
         * flag value and does not reveal any data itself 
         */
        private void Button_MouseDown(object sender, MouseEventArgs e) {
            Button btn = (Button)sender;
            int goodFlag = 0;


            if (e.Button == MouseButtons.Right && btn.Text.Length>1) {
                if (btn.Text.Contains("X")){
                    btn.Text = "?" + btn.Text.Substring(1);
                    btn.ForeColor = Color.Firebrick;
                    flagCount--;
                }
                else{
                    btn.Text = "X" + btn.Text.Substring(1);
                    btn.ForeColor = Color.White;
                    flagCount++;
                }
                flagCountLabel.Text = "Flags: " + flagCount;
            }

            //win condition check
            if (bombCount == flagCount) {
                for (int i = 0; i < buttonArray.Length; i++) {
                    if (buttonArray[i].Text.Length > 1) {
                        goodFlag++;
                    }
                }
                if (bombCount == goodFlag) {
                    GameOverLabel.Text = "YOU WIN!";
                    GameOverLabel.Visible = true;
                    PlayAgainLabel.Visible = true;
                    ContinueButton.Visible = true;
                    ExitButton.Visible = true;
                    timer.Stop();
                }
            }
        }

        //restart game
        private void ContinueButton_Click(object sender, EventArgs e) {
            bombCount = 0;
            flagCount = 0;

            //builds game grid, bombs, etc.
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 10; j++) {
                    if (random.Next(100) < 30) { //difficulty setting
                        matrix.SetValue(5, i, j);
                        bombCount++;
                    }
                    else {
                        matrix.SetValue(7, i, j); //flag value, needed for next step
                    }
                }
            }

            clock = 4 * bombCount;

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
                    temp.Add("" + matrix[i, j]);
                }
            }

            for (int i = 0; i < temp.Count; i++) {
                if (temp[i] == "0") {
                    buttonArray[i].Text = "?  ";
                }
                else {
                    buttonArray[i].Text = "? " + temp[i];
                }
            }
            bombCountLabel.Text = "Total Bombs: " + bombCount;
            flagCountLabel.Text = "Flags: " + flagCount;
            timerLabel.Text = "2:00";

            GameOverLabel.Visible = false;
            PlayAgainLabel.Visible = false;
            ContinueButton.Visible = false;
            ExitButton.Visible = false;

            for (int i = 0; i < buttonArray.Length; i++) {
                buttonArray[i].ForeColor = Color.Firebrick;
                buttonArray[i].BackColor = Color.FromArgb(30, 30, 30);
            }


            timer.Start();
        }

        private void ExitButton_Click(object sender, EventArgs e) {
            Application.Exit();
        }

    }
}