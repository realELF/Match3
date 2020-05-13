using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3.Scripts
{
    class Gameplay : Scene
    {
        private enum GameplayState { AllRest, AnimationOfSwap, AnimationOfBackSwap, GameOver }


        private const int PLAY_TIME = 60;
        public const int FIELD_SIZE = 8;
        public const int LEFT_INDENT = 200;
        public const int TOP_INDENT = 100;

        public readonly Element[,] field;
        public readonly List<Point> fallingColumns;

        private GameplayState gameplayState;
        private Point?[] clickedElements;

        public ushort points;
        private TimeSpan startTime;
        private float deltaPosition;

        private Button gameOverButton;
        public readonly List<Effect> effects;

        public Gameplay()
        {
            gameplayState = GameplayState.AllRest;
            startTime = DateTime.Now.TimeOfDay;
            points = 0;

            field = new Element[FIELD_SIZE, FIELD_SIZE];
            for (int x = 0; x < FIELD_SIZE; x++)
                for (int y = 0; y < FIELD_SIZE; y++)
                    field[x, y] = new Element(Element.ElementType.Usual);

            fallingColumns = new List<Point>();
            effects = new List<Effect>();

            clickedElements = new Point?[2];

            FieldCheck();
        }

        private double GetLeftTime()
        {
            double leftTime = PLAY_TIME - DateTime.Now.TimeOfDay.TotalSeconds + startTime.TotalSeconds;
            if (leftTime < 0)
                return 0;
            else
                return leftTime;
        }

        #region Механика игры
        public bool FieldCheck()
        {
            List<Point> garbage = new List<Point>();

            Element.ElementType bonusElement=Element.ElementType.Usual;

            //По горизонтали
            Color lastColor;
            byte counter = 0;
            for (int y = 0; y < FIELD_SIZE; y++)
            {
                lastColor = field[0, y].color;//Можно было и field[x - 1, y].color

                for (int x = 1; x < FIELD_SIZE; x++)
                {
                    Color color = field[x, y].color;
                    if (color == lastColor)
                    {
                        counter++;
                        if (counter == 3)
                            bonusElement = Element.ElementType.HorizontalLine;
                        if (counter == 4)
                            bonusElement = Element.ElementType.Bomb;
                    }
                    if (color != lastColor)
                    {
                        if (counter >= 2)
                            for (int dX = counter; dX >= 0; dX--)
                            {
                                garbage.Add(new Point(x - 1 - dX, y));
                            }

                        counter = 0;
                        lastColor = color;
                    }
                }
                if (counter >= 2)
                    for (int dX = counter; dX >= 0; dX--)
                    {
                        garbage.Add(new Point(FIELD_SIZE - 1 - dX, y));
                    }

                counter = 0;
            }

            //По вертикали
            for (int x = 0; x < FIELD_SIZE; x++)
            {
                lastColor = field[x, 0].color;

                for (int y = 1; y < FIELD_SIZE; y++)
                {
                    Color color = field[x, y].color;
                    if (color == lastColor)
                    {
                        counter++;
                        if (counter == 3 && bonusElement!=Element.ElementType.Bomb)
                            bonusElement = Element.ElementType.VerticalLine;
                        if (counter == 4)
                            bonusElement = Element.ElementType.Bomb;
                    }
                    if (color != lastColor)
                    {
                        if (counter >= 2)
                            for (int dY = counter; dY >= 0; dY--)
                            {
                                garbage.Add(new Point(x, y - 1 - dY));
                            }

                        counter = 0;
                        lastColor = color;
                    }
                }
                if (counter >= 2)
                    for (int dY = counter; dY >= 0; dY--)
                    {
                        garbage.Add(new Point(x, FIELD_SIZE - 1 - dY));
                    }

                counter = 0;
            }

            DeleteElements(garbage);

            if (clickedElements[1] != null && field[clickedElements[1].Value.X, clickedElements[1].Value.Y]==null && bonusElement != Element.ElementType.Usual)
            {
                field[clickedElements[1].Value.X, clickedElements[1].Value.Y] = new Element(bonusElement);

                //Обновление падения
                RefreshColumn((Point)clickedElements[1]);
            }

            return garbage.Count > 0;
        }

        public void RefreshColumn(Point point)
        {
            for (int i = fallingColumns.Count - 1; i >= 0; i--)
            {
                if (fallingColumns[i].X == point.X)
                {
                    if (fallingColumns[i].Y > point.Y)
                    {
                        if (field[point.X, point.Y] != null)
                            field[point.X, point.Y].inFall = true;
                        return;
                    }
                    else
                    {
                        int y = fallingColumns[i].Y;
                        //int y = point.Y;
                        if (point.Y == fallingColumns[i].Y)
                        {
                            for (; y >= 0; y--)
                            {
                                if (field[fallingColumns[i].X, y] == null)
                                    break;
                            }
                            if (y == 0)
                                fallingColumns.RemoveAt(i);
                            else
                                fallingColumns[i] = new Point(fallingColumns[i].X, y);
                        }
                        else
                            fallingColumns[i] = new Point(fallingColumns[i].X, point.Y);

                        return;
                    }
                }
            }

            fallingColumns.Add(point);
        }

        public void DeleteElements(List<Point> elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];

                bool addElement = true;
                for (int j = 0; j < fallingColumns.Count; j++)
                {
                    if (element.X == fallingColumns[j].X)
                    {
                        if (element.Y > fallingColumns[j].Y)
                            fallingColumns[j] = element;

                        addElement = false;
                        break;
                    }
                }
                if (addElement)
                    fallingColumns.Add(element);

                if (field[element.X, element.Y] != null)
                {
                    field[element.X, element.Y].Destroy(element, this);
                }
                else
                    if (clickedElements[1] != null)
                        field[element.X, element.Y] = new Element(Element.ElementType.Bomb);
            }
        }

        public void StartDrop()
        {
            for (int i = 0; i < fallingColumns.Count; i++)
            {
                for (int y = fallingColumns[i].Y - 1; y >= 0; y--)
                {
                    if (field[fallingColumns[i].X, y] != null)
                    {
                        field[fallingColumns[i].X, y].inFall = true;

                        field[fallingColumns[i].X, y + 1] = field[fallingColumns[i].X, y];
                        field[fallingColumns[i].X, y] = null;
                    }
                }
                field[fallingColumns[i].X, 0] = new Element(Element.ElementType.Usual);
                field[fallingColumns[i].X, 0].inFall = true;
            }

            deltaPosition = Match3.Instance.ContentContainer.cell.Height;
        }

        public void CheckDrop()
        {
            for (int i = fallingColumns.Count - 1; i >= 0; i--)
            {
                bool deleteColumn = true;
                for (int y = fallingColumns[i].Y; y >= 0; y--)
                {
                    if (field[fallingColumns[i].X, y] == null)
                    {
                        deleteColumn = false;
                        fallingColumns[i] = new Point(fallingColumns[i].X, y);
                        break;
                    }

                    field[fallingColumns[i].X, y].inFall = false;
                }
                if (deleteColumn)
                    fallingColumns.RemoveAt(i);
            }
        }

        public void CheckClickedElement()
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Texture2D cellTexture = Match3.Instance.ContentContainer.cell;
                if (mouseState.X >= LEFT_INDENT && mouseState.X <= LEFT_INDENT + FIELD_SIZE * cellTexture.Width
                    && mouseState.Y >= TOP_INDENT && mouseState.Y <= TOP_INDENT + FIELD_SIZE * cellTexture.Height)
                {
                    Point clickedElement;
                    clickedElement = new Point((mouseState.X - LEFT_INDENT) / cellTexture.Width, (mouseState.Y - TOP_INDENT) / cellTexture.Height);

                    if (clickedElements[0] == null)
                        clickedElements[0] = clickedElement;
                    else
                        if (clickedElements[0] != clickedElement)
                            //Убирает выделение если не равно, т.е. не подходит для ==0
                            if (Math.Abs(clickedElements[0].Value.X - clickedElement.X) + Math.Abs(clickedElements[0].Value.Y - clickedElement.Y) == 1)
                            {
                                clickedElements[1] = clickedElement;

                                gameplayState = GameplayState.AnimationOfSwap;
                                deltaPosition = cellTexture.Width;// TODO: Доработать, если, спрайт клетки будет другим [cellTexture.Width!=cellTexture.Height]
                            }
                            else
                                clickedElements[0] = null;
                }
                else
                    for (int i = 0; i < 2; i++)
                        clickedElements[i] = null;
                ////Можно было как-то так
                //if (point.Value.X < 0 && point.Value.X >= FIELD_SIZE && point.Value.Y < 0 && point.Value.Y >= FIELD_SIZE)
                //    lastClickedElement = null;
            }
        }

        private void SwapElements(Point a0, Point a1)
        {
            Element tempElement = field[a0.X,a0.Y];
            field[a0.X, a0.Y] = field[a1.X, a1.Y];
            field[a1.X, a1.Y] = tempElement;
        }

        public override void Update(GameTime gameTime)
        {
            if (gameOverButton == null)
            {
                if (GetLeftTime() == 0)
                {
                    gameOverButton = new Button(() => Match3.Instance.scene = new MainMenu(), new Vector2(300, 400), "Ok");
                    return;
                }

                if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.R))
                    Match3.Instance.scene = new MainMenu();

                if (effects.Count > 0)
                    for (int e = effects.Count - 1; e >= 0; e--)
                        effects[e].Update(this, gameTime);
                else
                {
                    float deltaMilliseconds = gameTime.ElapsedGameTime.Milliseconds / 1000f;

                    if (fallingColumns.Count > 0)
                        if (deltaPosition > 0)
                        {
                            deltaPosition -= Element.FALL_SPEED * deltaMilliseconds;

                            if (deltaPosition <= 0)
                            {
                                CheckDrop();

                                if (fallingColumns.Count == 0)
                                {
                                    //System.Threading.Thread.Sleep(2000);
                                    FieldCheck();
                                }
                            }
                        }
                        else
                            StartDrop();
                    else
                        if (deltaPosition > 0)
                            deltaPosition -= Element.FALL_SPEED * deltaMilliseconds;
                        else
                            if (clickedElements[1] == null)
                                CheckClickedElement();
                            else
                            {
                                if (gameplayState == GameplayState.AnimationOfSwap)
                                {
                                    SwapElements((Point)clickedElements[0], (Point)clickedElements[1]);
                                    bool tempBool = FieldCheck();

                                    if (tempBool)
                                    {
                                        gameplayState = GameplayState.AllRest;
                                        for (int i = 0; i < 2; i++)
                                            clickedElements[i] = null;
                                    }
                                    else
                                    {
                                        SwapElements((Point)clickedElements[0], (Point)clickedElements[1]);
                                        gameplayState = GameplayState.AnimationOfBackSwap;
                                        deltaPosition = Match3.Instance.ContentContainer.cell.Width;// TODO: Доработать, если, спрайт клетки будет другим [cellTexture.Width!=cellTexture.Height]
                                    }
                                }
                                else
                                {
                                    gameplayState = GameplayState.AllRest;
                                    for (int i = 0; i < 2; i++)
                                        clickedElements[i] = null;
                                }
                            }
                }
            }
            else
                gameOverButton.Update();
        }
        #endregion

        public override void Draw()
        {
            Match3.Instance.SpriteBatch.DrawString(Match3.Instance.ContentContainer.font, string.Format("{0:F0} seconds left", GetLeftTime()), new Vector2(280, 60), Color.PaleVioletRed);
            Match3.Instance.SpriteBatch.DrawString(Match3.Instance.ContentContainer.font, "Points: " + points, new Vector2(300, 20), Color.White);

            for (int e = effects.Count - 1; e >= 0; e--)
                effects[e].Draw();

            Texture2D cellTexture = Match3.Instance.ContentContainer.cell;
            for (int x = 0; x < FIELD_SIZE; x++)
                for (int y = 0; y < FIELD_SIZE; y++)
                {
                    Color cellColor = Color.White;

                    ////    Куда падает
                    for (int i = 0; i < fallingColumns.Count; i++)
                        if (x == fallingColumns[i].X && y == fallingColumns[i].Y)
                            cellColor = Color.Red;

                    for (int i = 0; i < 2; i++)
                        if (clickedElements[i] != null && x == clickedElements[i].Value.X && y == clickedElements[i].Value.Y)
                        {
                            cellColor = Color.BlueViolet;
                            break;
                        }

                    Vector2 cellPosition = new Vector2(cellTexture.Width * x + LEFT_INDENT, cellTexture.Height * y + TOP_INDENT);
                    Match3.Instance.SpriteBatch.Draw(cellTexture, cellPosition, cellColor);

                    if (field[x, y] != null)
                    {
                        //Для анимации
                        float deltaX = 0;
                        float deltaY = 0;
                        if (field[x, y].inFall)
                            deltaY = deltaPosition;
                        else
                            if (cellColor == Color.BlueViolet&&(gameplayState == GameplayState.AnimationOfSwap || gameplayState == GameplayState.AnimationOfBackSwap))
                            {
                                int numberOfClickedElement = 0;
                                int numberOfAnotherClickedElement = 1;

                                if (field[x, y] == field[clickedElements[1].Value.X, clickedElements[1].Value.Y])
                                {
                                    numberOfClickedElement = 1;
                                    numberOfAnotherClickedElement = 0;
                                }

                                if (clickedElements[numberOfClickedElement].Value.X == clickedElements[numberOfAnotherClickedElement].Value.X)
                                {
                                    float tempValue;
                                    if (gameplayState == GameplayState.AnimationOfSwap)
                                        tempValue = cellTexture.Height - deltaPosition;
                                    else
                                        tempValue = deltaPosition;
                                    deltaY = (clickedElements[numberOfClickedElement].Value.Y - clickedElements[numberOfAnotherClickedElement].Value.Y) * tempValue;
                                }
                                else
                                {
                                    float tempValue;
                                    if (gameplayState == GameplayState.AnimationOfSwap)
                                        tempValue = cellTexture.Width - deltaPosition;
                                    else
                                        tempValue = deltaPosition;
                                    deltaX = (clickedElements[numberOfClickedElement].Value.X - clickedElements[numberOfAnotherClickedElement].Value.X) * tempValue;
                                }
                            }

                        Vector2 trianglePos = new Vector2(
                                cellPosition.X + 3 - deltaX
                                , cellPosition.Y + 4 - deltaY
                                );
                        field[x, y].Draw(trianglePos);
                    }
                }

            if (gameOverButton != null)
            {
                Match3.Instance.SpriteBatch.DrawString(Match3.Instance.ContentContainer.font, "Game Over", new Vector2(340, 400), Color.Red, 0, Vector2.One, 1, SpriteEffects.None, 0.2f);
                gameOverButton.Draw();
            }
        }
    }
}