using System;
using System.Drawing;
using System.Windows.Forms;

namespace ApplePicker {

    public partial class Form1 : Form {
        private int rI, rJ;
        static readonly int speed = 8;
        static PictureBox fruit;
        private readonly PictureBox[] snake = new PictureBox[400];
        
        private readonly Label labelScore;
        private int dirX, dirY;
        private int score = 0;
        private readonly int width = 900;
        private readonly int height = 800;
        private readonly int sizeOfSides = 40;
        private enum Direction {
            Left, Right, Up, Down
        }
        Direction direction;

        public Form1() {
            InitializeComponent();
            this.Text = "Apple picker";
            dirX = 1; dirY = 0;
            this.Width = width; this.Height = height;
            //рисуем панель со счетом
            labelScore = new Label {
                Text = "Score: 0",
                Location = new Point(810, 10)
            }; this.Controls.Add(labelScore);
            //рисуем первоночальное положение головы змеи на старте
            snake[0] = new PictureBox {
                Location = new Point(201, 201),
                Image = Image.FromFile("head.jpg"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(sizeOfSides - 1, sizeOfSides - 1)
            }; 
            this.Controls.Add(snake[0]);
            GenerateMap(); //создаем "решетку" поля

            //Создаем "фрукт"
            fruit = new PictureBox {
                Image = Image.FromFile("apple.jpg"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(sizeOfSides, sizeOfSides)
            };
            GenerateFruit();

            this.KeyDown += new KeyEventHandler(KeysPressing); //отслеживаем нажатие клавиш

            timer.Tick += new EventHandler(Update); //делаем таймер
            timer.Interval = (10-speed)*100; //с интервалом 0,5 сек
            timer.Start(); //и запускаем его
        }
        private void GenerateMap() {
            for (int i = 0; i <= width / sizeOfSides; i++) {
                PictureBox pic = new PictureBox {
                    BackColor = Color.Black,
                    Location = new Point(0, sizeOfSides * i),
                    Size = new Size(width - 100, 1)
                };
                this.Controls.Add(pic);
            }
            for (int i = 0; i <= height / sizeOfSides; i++) {
                PictureBox pic = new PictureBox {
                    BackColor = Color.Black,
                    Location = new Point(sizeOfSides * i, 0),
                    Size = new Size(1, height)
                };
                this.Controls.Add(pic);
            }
        }
        private void GenerateFruit() {
            Random random = new Random();
            rI = random.Next(0, height - sizeOfSides);
            int tempI = rI % sizeOfSides;
            rI -= tempI;
            rJ = random.Next(0, height - sizeOfSides);
            int tempJ = rJ % sizeOfSides;
            rJ -= tempJ;
            rJ++; rI++;
            fruit.Location = new Point(rI, rJ);
            this.Controls.Add(fruit);
        }
       
       
        private void CheckEatFruit() {
            if (snake[0].Location.X == rI && snake[0].Location.Y ==rJ) {
                DisplayScore();
                snake[score] = new PictureBox {
                    Location = new Point(snake[score - 1].Location.X + (sizeOfSides * dirX), snake[score - 1].Location.Y - (sizeOfSides * dirY)),
                    Size = new Size(sizeOfSides - 1, sizeOfSides - 1),
                    Image = Image.FromFile("stored-apple.jpg"),
                    SizeMode = PictureBoxSizeMode.StretchImage
                };
                this.Controls.Add(snake[score]);
                GenerateFruit();
            }
        }
        private void CheckBorders() {
            if (snake[0].Location.X < 0 || snake[0].Location.X >= height || snake[0].Location.Y < 0 || snake[0].Location.Y > height) {
                GameOver();
            }
        }
        private void CheckEatSelf() {
            for (int i = 1; i < score; i++) {
                if (snake[0].Location == snake[i].Location)
                    GameOver();
            }
        }
        private void MoveSnake() {
            for (int i = score; i >= 1; i--) {
                snake[i].Location = snake[i - 1].Location;
            }
            snake[0].Location = new Point(snake[0].Location.X + (dirX * sizeOfSides), snake[0].Location.Y + (dirY * sizeOfSides));
            CheckEatSelf();
        }
        private void Update(object sender, EventArgs e) {
            CheckBorders();
            CheckEatFruit();
            MoveSnake();
        }

        private void KeysPressing(object sender, KeyEventArgs e) {
            switch (e.KeyCode.ToString()) {
                case "Right":
                    if(direction != Direction.Left) {
                        direction = Direction.Right;
                        dirX = 1; dirY = 0;
                    }
                    break;

                case "Left":
                    if (direction != Direction.Right) {
                        direction = Direction.Left;
                        dirX = -1; dirY = 0;
                    }
                    break;

                case "Up":
                    if (direction != Direction.Down) {
                        direction = Direction.Up;
                        dirY = -1; dirX = 0;
                    }
                    
                    break;

                case "Down":
                    if (direction != Direction.Up) {
                        direction = Direction.Down;
                        dirY = 1; dirX = 0;
                    }
                    break;

                case "Escape":
                    this.Dispose();
                    break;
            }
        }
        private void DisplayScore() => labelScore.Text = $"Score: {++score}";
        private void GameOver() {
            timer.Stop();
            MessageBox.Show($"You get {score} score");
            this.Close();
        }
    }
}