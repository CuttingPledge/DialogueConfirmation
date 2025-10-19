using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace DialogueConfirmation
{
    internal class DialogueChange
    {
        #region Variables
        public bool firstClick = false;
        public DialogueBox? currentDiag;
        public int previousResponse = -1;
        public int currentResponse = -1;
        public int questionX;
        public int questionY;
        public ClickableComponent okButton;
        private ModEntry _mod;
        private Texture2D okButtonTexture;
        #endregion

        public DialogueChange(ModEntry mod)
        {
            _mod = mod;
            okButtonTexture = Game1.mouseCursors;
            okButton = new(new Rectangle(Game1.viewport.Width / 2, Game1.viewport.Height / 2, 200, 200), "OKButton")
            {
                myID = -2,
                downNeighborID = 0,
                upNeighborID = 0
            };
        }

        #region EventHandlers
        // Looks for dialogue boxes
        public void Display_MenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is DialogueBox NewMenu)
            {
                firstClick = false;
                currentDiag = NewMenu;

                if (currentDiag.isQuestion)
                {
                    questionX = currentDiag.x;
                    currentResponse = -1;
                }
            }
            else if (e.OldMenu is DialogueBox OldMenu)
            {
                currentDiag = null;
                currentResponse = -1;
            }
        }

        // Receives player input and passes to HandleQuestion method based on options
        public void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (currentDiag != null)
            {
                // All questions will have a list of responses
                if (currentDiag.responses.Count() > 0)
                {
                    // For some reason "dialogues" doesn't always have entries? The game's sleep question has it, but some other questions do not
                    if (currentDiag.dialogues.Count != 0 && currentDiag.dialogues[0] == "Go to sleep for the night?")
                    {
                        if (_mod.Config.ConfirmSleepDialogue)
                        {
                            this.HandleQuestion(e.Button);
                        }
                    }
                    else if (currentDiag.responses[0].responseText == "Shop" || currentDiag.responses[0].responseText == "Supplies Shop" || currentDiag.responses[0].responseText == "Donate To Museum")
                    {
                        if (_mod.Config.ConfirmShopDialogue)
                        {
                            this.HandleQuestion(e.Button);
                        }
                    }
                    else if (currentDiag.dialogues.Count != 0 && currentDiag.dialogues[0] == "Choose destination:")
                    {
                        if (_mod.Config.ConfirmMinecart)
                        {
                            this.HandleQuestion(e.Button);
                        }
                    }
                    else if (currentDiag.responses[0].responseText == "Leave the mine")
                    {
                        if (_mod.Config.ConfirmMineLadder)
                        {
                            this.HandleQuestion(e.Button);
                        }
                    }
                    else if (currentDiag.dialogues.Count != 0 && currentDiag.dialogues[0].Substring(0, 4) == "Eat " && currentDiag.responses[0].responseText == "Yes")
                    {
                        if (_mod.Config.ConfirmEating)
                        {
                            this.HandleQuestion(e.Button);
                        }
                    }
                    else if (currentDiag.dialogues.Count != 0 && currentDiag.dialogues[0] == "Select channel:")
                    {
                        if (_mod.Config.ConfirmTV)
                        {
                            this.HandleQuestion(e.Button);
                        }
                    }
                    else if (currentDiag.dialogues.Count != 0 && currentDiag.dialogues[0] == "Please select a number:" || currentDiag.dialogues[0] == "Please make a selection:")
                    {
                        if (_mod.Config.ConfirmPhone)
                        {
                            this.HandleQuestion(e.Button);
                        }
                    }
                    else if (_mod.Config.ConfirmQuestionAnswer)
                    {
                        this.HandleQuestion(e.Button);
                    }
                }
                // Requires double-clicking to move to next dialogue (disabled by default in options)
                else if (firstClick && currentDiag.dialogueContinuedOnNextPage)
                {
                    firstClick = false;
                }
                else if (!firstClick && currentDiag.characterIndexInDialogue == currentDiag.getCurrentString().Length && _mod.Config.DoubleClickRegularDialogue)
                {
                    _mod.SupressKeyInput(e.Button);
                    firstClick = true;
                }
            }
        }
        #endregion

        #region Methods
        // Draws the arrow on the side of the selected question
        public void DrawArrow(object? sender, RenderedActiveMenuEventArgs e)
        {
            if (currentDiag != null)
            {
                if (currentDiag.isQuestion && !currentDiag.transitioning && currentResponse != -1)
                {
                    e.SpriteBatch.Draw(Game1.mouseCursors, new Rectangle(questionX - 40, questionY - 60, 44, 44), new Rectangle(12, 204, 44, 44), Color.White);
                }
            }
        }

        // Handles player input to require double-click to answer a question
        private void HandleQuestion(SButton sButton)
        {
            if (currentDiag != null)
            {
                try
                {
                    if (Game1.options.gamepadControls) // Controller support
                    {
                        if (sButton == SButton.LeftThumbstickUp || sButton == SButton.LeftThumbstickDown)
                        {
                            // Player choosing dialogue option
                        }
                        else if (sButton == SButton.ControllerA || sButton == SButton.ControllerB)
                        {
                            previousResponse = currentResponse;
                            currentResponse = currentDiag.selectedResponse;
                            questionX = currentDiag.x;
                            if (currentResponse != -1)
                            {
                                questionY = currentDiag.y - (currentDiag.heightForQuestions - currentDiag.height) + SpriteText.getHeightOfString(currentDiag.getCurrentString(), currentDiag.width) + 48;
                                for (int i = 0; i <= currentResponse; i++)
                                {
                                    questionY += SpriteText.getHeightOfString(currentDiag.responses[i].responseText, currentDiag.width - 16) + 16;
                                }
                            }
                            if (previousResponse != currentResponse || currentResponse == -1)
                            {
                                _mod.SupressKeyInput(sButton);
                            }
                        }
                        else
                        {
                            _mod.SupressKeyInput(sButton);
                        }
                    }
                    else // Mouse and keyboard
                    {
                        previousResponse = currentResponse;
                        currentResponse = currentDiag.selectedResponse;
                        questionX = currentDiag.x;
                        if (currentResponse != -1)
                        {
                            questionY = currentDiag.y - (currentDiag.heightForQuestions - currentDiag.height) + SpriteText.getHeightOfString(currentDiag.getCurrentString(), currentDiag.width - 16) + 48;
                            for (int i = 0; i <= currentResponse; i++)
                            {
                                questionY += SpriteText.getHeightOfString(currentDiag.responses[i].responseText, currentDiag.width - 16) + 16;
                            }
                        }
                        if (previousResponse != currentResponse || currentResponse == -1)
                        {
                            _mod.SupressKeyInput(sButton);
                        }
                        if (sButton != SButton.MouseLeft && sButton != SButton.MouseRight && sButton != SButton.T)
                        {
                            _mod.SupressKeyInput(sButton);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _mod.Monitor.Log($" An error has occurred while handling the question. " + ex, LogLevel.Error);
                }
                finally
                {

                }
            }
        }
        #endregion
    }
}
