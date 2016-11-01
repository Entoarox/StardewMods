// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.DialogueBox
// Assembly: Stardew Valley, Version=1.0.6117.32354, Culture=neutral, PublicKeyToken=null
// MVID: A228046D-FF51-468B-B935-E371C5FE297A
// Assembly location: D:\Applications\Steam\steamapps\common\Stardew Valley\Stardew Valley.exe

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;

using System;
using System.Collections.Generic;

using Entoarox.Framework.Reflection;

namespace Entoarox.Framework
{
    public class TitleMenuDialogue : IClickableMenu
    {
        private List<string> dialogues = new List<string>();
        private Stack<string> characterDialoguesBrokenUp = new Stack<string>();
        private List<Response> responses = new List<Response>();
        private Rectangle friendshipJewel = Rectangle.Empty;
        private int transitionX = -1;
        private int safetyTimer = 750;
        private int selectedResponse = -1;
        private bool transitioning = true;
        private bool transitioningBigger = true;
        private string hoverText = "";
        public const int portraitBoxSize = 74;
        public const int nameTagWidth = 102;
        public const int nameTagHeight = 18;
        public const int portraitPlateWidth = 115;
        public const int nameTagSideMargin = 5;
        public const float transitionRate = 3f;
        public const int characterAdvanceDelay = 30;
        public const int safetyDelay = 750;
        private Dialogue characterDialogue;
        public static int questionFinishPauseTimer;
        private bool activatedByGamePad;
        private int x;
        private int y;
        private int transitionY;
        private int transitionWidth;
        private int transitionHeight;
        private int characterAdvanceTimer;
        private int characterIndexInDialogue;
        private int heightForQuestions;
        private int newPortaitShakeTimer;
        private int gamePadIntroTimer;
        private bool isQuestion;
        private TemporaryAnimatedSprite dialogueIcon;

        public static void ShowTitleMenuDialogue(string msg)
        {
            FieldHelper.SetField(Game1.activeClickableMenu, "subMenu", new TitleMenuDialogue(msg));
            List<ClickableTextureComponent> buttons = FieldHelper.GetField<List<ClickableTextureComponent>>(Game1.activeClickableMenu, "buttons");
            foreach (ClickableTextureComponent button in buttons)
                button.visible = false;
            FieldHelper.GetField<ClickableTextureComponent>(Game1.activeClickableMenu, "backButton").visible = false;
        }
        public TitleMenuDialogue(string msg) : this(msg, true)
        {
            FieldHelper.GetField<ClickableTextureComponent>(Game1.activeClickableMenu, "backButton").visible = false;
        }
        private TitleMenuDialogue(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.activatedByGamePad = Game1.isAnyGamePadButtonBeingPressed();
            this.gamePadIntroTimer = 1000;
        }

        private TitleMenuDialogue(string dialogue, bool self)
        {
            this.activatedByGamePad = Game1.isAnyGamePadButtonBeingPressed();
            if (this.activatedByGamePad)
            {
                Game1.mouseCursorTransparency = 0.0f;
                this.gamePadIntroTimer = 1000;
            }
            this.dialogues.AddRange((IEnumerable<string>)dialogue.Split('#'));
            this.width = Math.Min(1200, SpriteText.getWidthOfString(dialogue) + Game1.tileSize);
            this.height = SpriteText.getHeightOfString(dialogue, this.width - Game1.pixelZoom * 5) + Game1.pixelZoom;
            this.x = (int)Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0).X;
            this.y = Game1.viewport.Height - this.height - Game1.tileSize;
            this.setUpIcons();
        }

        private TitleMenuDialogue(string dialogue, List<Response> responses)
        {
            this.activatedByGamePad = Game1.isAnyGamePadButtonBeingPressed();
            this.gamePadIntroTimer = 1000;
            this.dialogues.Add(dialogue);
            this.responses = responses;
            this.isQuestion = true;
            this.width = 1200;
            this.setUpQuestions();
            this.height = this.heightForQuestions;
            this.x = (int)Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0).X;
            this.y = Game1.viewport.Height - this.height - Game1.tileSize;
            this.setUpIcons();
            this.characterIndexInDialogue = dialogue.Length - 1;
        }

        private TitleMenuDialogue(Dialogue dialogue)
        {
            this.characterDialogue = dialogue;
            this.activatedByGamePad = Game1.isAnyGamePadButtonBeingPressed();
            if (this.activatedByGamePad)
            {
                Game1.mouseCursorTransparency = 0.0f;
                this.gamePadIntroTimer = 1000;
            }
            this.width = 1200;
            this.height = 6 * Game1.tileSize;
            this.x = (int)Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0).X;
            this.y = Game1.viewport.Height - this.height - Game1.tileSize;
            this.friendshipJewel = new Rectangle(this.x + this.width - Game1.tileSize, this.y + Game1.tileSize * 4, 11 * Game1.pixelZoom, 11 * Game1.pixelZoom);
            this.characterDialoguesBrokenUp.Push(dialogue.getCurrentDialogue());
            this.checkDialogue(dialogue);
            this.newPortaitShakeTimer = this.characterDialogue.getPortraitIndex() == 1 ? 250 : 0;
            this.setUpForGamePadMode();
        }

        private TitleMenuDialogue(List<string> dialogues)
        {
            this.activatedByGamePad = Game1.isAnyGamePadButtonBeingPressed();
            if (this.activatedByGamePad)
            {
                Game1.mouseCursorTransparency = 0.0f;
                this.gamePadIntroTimer = 1000;
            }
            this.dialogues = dialogues;
            this.width = Math.Min(1200, SpriteText.getWidthOfString(dialogues[0]) + Game1.tileSize);
            this.height = SpriteText.getHeightOfString(dialogues[0], this.width - Game1.pixelZoom * 4);
            this.x = (int)Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0).X;
            this.y = Game1.viewport.Height - this.height - Game1.tileSize;
            this.setUpIcons();
        }

        public override bool autoCenterMouseCursorForGamepad()
        {
            return false;
        }

        private void playOpeningSound()
        {
            Game1.playSound("breathin");
        }

        public override void setUpForGamePadMode()
        {
            if (!Game1.options.gamepadControls || !this.activatedByGamePad && Game1.lastCursorMotionWasMouse)
                return;
            this.gamePadControlsImplemented = true;
            if (this.isQuestion)
            {
                int num = 0;
                string currentString = this.getCurrentString();
                if (currentString != null && currentString.Length > 0)
                    num = SpriteText.getHeightOfString(currentString, 999999);
                Game1.setMousePosition(this.x + this.width - Game1.tileSize * 2, this.y + num + Game1.tileSize);
            }
            else
                Game1.mouseCursorTransparency = 0.0f;
        }

        public void closeDialogue()
        {
            FieldHelper.SetField(Game1.activeClickableMenu, "subMenu", null);
            List<ClickableTextureComponent> buttons = FieldHelper.GetField<List<ClickableTextureComponent>>(Game1.activeClickableMenu, "buttons");
            foreach (ClickableTextureComponent button in buttons)
                button.visible = true;
            FieldHelper.GetField<ClickableTextureComponent>(Game1.activeClickableMenu, "backButton").visible = true;
        }

        public void finishTyping()
        {
            this.characterIndexInDialogue = this.getCurrentString().Length - 1;
        }

        public void beginOutro()
        {
            this.transitioning = true;
            this.transitioningBigger = false;
            Game1.playSound("breathout");
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            this.receiveLeftClick(x, y, playSound);
        }

        private void tryOutro()
        {
            if (Game1.activeClickableMenu == null || !Game1.activeClickableMenu.Equals((object)this))
                return;
            this.beginOutro();
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.options.doesInputListContain(Game1.options.actionButton, key))
            {
                this.receiveLeftClick(0, 0, true);
            }
            else
            {
                if (!this.isQuestion || Game1.eventUp || this.characterDialogue != null)
                    return;
                if (Game1.options.doesInputListContain(Game1.options.menuButton, key))
                {
                    if (this.responses != null && this.responses.Count > 0 && Game1.currentLocation.answerDialogue(this.responses[this.responses.Count - 1]))
                        Game1.playSound("smallSelect");
                    this.selectedResponse = -1;
                    this.tryOutro();
                }
                else
                {
                    if (key != Keys.Y || this.responses == null || (this.responses.Count <= 0 || !this.responses[0].responseKey.Equals("Yes")) || !Game1.currentLocation.answerDialogue(this.responses[0]))
                        return;
                    Game1.playSound("smallSelect");
                    this.selectedResponse = -1;
                    this.tryOutro();
                }
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (this.transitioning)
                return;
            if (this.characterIndexInDialogue < this.getCurrentString().Length - 1)
            {
                this.characterIndexInDialogue = this.getCurrentString().Length - 1;
            }
            else
            {
                if (this.safetyTimer > 0)
                    return;
                if (this.isQuestion)
                {
                    if (this.selectedResponse == -1)
                        return;
                    DialogueBox.questionFinishPauseTimer = Game1.eventUp ? 600 : 200;
                    this.transitioning = true;
                    this.transitionX = -1;
                    this.transitioningBigger = true;
                    if (this.characterDialogue != null)
                    {
                        this.characterDialoguesBrokenUp.Pop();
                        this.characterDialogue.chooseResponse(this.responses[this.selectedResponse]);
                        this.characterDialoguesBrokenUp.Push("");
                        Game1.playSound("smallSelect");
                    }
                    else
                    {
                        Game1.dialogueUp = false;
                        if (Game1.eventUp)
                        {
                            Game1.playSound("smallSelect");
                            Game1.currentLocation.currentEvent.answerDialogue(Game1.currentLocation.lastQuestionKey, this.selectedResponse);
                            this.selectedResponse = -1;
                            this.tryOutro();
                            return;
                        }
                        if (Game1.currentLocation.answerDialogue(this.responses[this.selectedResponse]))
                            Game1.playSound("smallSelect");
                        this.selectedResponse = -1;
                        this.tryOutro();
                        return;
                    }
                }
                else if (this.characterDialogue == null)
                {
                    this.dialogues.RemoveAt(0);
                    if (this.dialogues.Count == 0)
                    {
                        this.closeDialogue();
                    }
                    else
                    {
                        this.width = Math.Min(1200, SpriteText.getWidthOfString(this.dialogues[0]) + Game1.tileSize);
                        this.height = SpriteText.getHeightOfString(this.dialogues[0], this.width - Game1.pixelZoom * 4);
                        this.x = (int)Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0).X;
                        this.y = Game1.viewport.Height - this.height - Game1.tileSize * 2;
                        this.xPositionOnScreen = x;
                        this.yPositionOnScreen = y;
                        this.setUpIcons();
                    }
                }
                this.characterIndexInDialogue = 0;
                if (this.characterDialogue != null)
                {
                    int portraitIndex = this.characterDialogue.getPortraitIndex();
                    if (this.characterDialoguesBrokenUp.Count == 0)
                    {
                        this.beginOutro();
                        return;
                    }
                    this.characterDialoguesBrokenUp.Pop();
                    if (this.characterDialoguesBrokenUp.Count == 0)
                    {
                        if (!this.characterDialogue.isCurrentStringContinuedOnNextScreen)
                            this.beginOutro();
                        this.characterDialogue.exitCurrentDialogue();
                    }
                    if (!this.characterDialogue.isDialogueFinished() && this.characterDialogue.getCurrentDialogue().Length > 0 && this.characterDialoguesBrokenUp.Count == 0)
                        this.characterDialoguesBrokenUp.Push(this.characterDialogue.getCurrentDialogue());
                    this.checkDialogue(this.characterDialogue);
                    if (this.characterDialogue.getPortraitIndex() != portraitIndex)
                        this.newPortaitShakeTimer = this.characterDialogue.getPortraitIndex() == 1 ? 250 : 50;
                }
                if (!this.transitioning)
                    Game1.playSound("smallSelect");
                this.setUpIcons();
                this.safetyTimer = 750;
                if (this.getCurrentString() == null || this.getCurrentString().Length > 20)
                    return;
                this.safetyTimer -= 200;
            }
        }

        private void setUpIcons()
        {
            this.dialogueIcon = (TemporaryAnimatedSprite)null;
            if (this.isQuestion)
                this.setUpQuestionIcon();
            else if (this.characterDialogue != null && (this.characterDialogue.isCurrentStringContinuedOnNextScreen || this.characterDialoguesBrokenUp.Count > 1))
                this.setUpNextPageIcon();
            else if (this.dialogues != null && this.dialogues.Count > 1)
                this.setUpNextPageIcon();
            else
                this.setUpCloseDialogueIcon();
            this.setUpForGamePadMode();
            if (this.getCurrentString() == null || this.getCurrentString().Length > 20)
                return;
            this.safetyTimer -= 200;
        }

        public override void performHoverAction(int mouseX, int mouseY)
        {
            this.hoverText = "";
            if (!this.transitioning && this.characterIndexInDialogue >= this.getCurrentString().Length - 1)
            {
                base.performHoverAction(mouseX, mouseY);
                if (this.isQuestion)
                {
                    int num1 = this.selectedResponse;
                    int num2 = this.y - (this.heightForQuestions - this.height) + SpriteText.getHeightOfString(this.getCurrentString(), this.width) + Game1.pixelZoom * 12;
                    for (int index = 0; index < this.responses.Count; ++index)
                    {
                        SpriteText.getHeightOfString(this.responses[index].responseText, this.width);
                        if (mouseY >= num2 && mouseY < num2 + SpriteText.getHeightOfString(this.responses[index].responseText, this.width))
                        {
                            this.selectedResponse = index;
                            break;
                        }
                        num2 += SpriteText.getHeightOfString(this.responses[index].responseText, this.width) + Game1.pixelZoom * 4;
                    }
                    if (this.selectedResponse != num1)
                        Game1.playSound("Cowboy_gunshot");
                }
            }
            if (Game1.eventUp || this.friendshipJewel.Equals(Rectangle.Empty) || (!this.friendshipJewel.Contains(mouseX, mouseY) || this.characterDialogue == null) || (this.characterDialogue.speaker == null || !Game1.player.friendships.ContainsKey(this.characterDialogue.speaker.name)))
                return;
            this.hoverText = string.Concat(new object[4]
            {
        (object) Game1.player.getFriendshipHeartLevelForNPC(this.characterDialogue.speaker.name),
        (object) "/",
        this.characterDialogue.speaker.name.Equals(Game1.player.spouse) ? (object) "12" : (object) "10",
        (object) "<"
            });
        }

        private void setUpQuestionIcon()
        {
            Vector2 position = new Vector2((float)(this.x + this.width - 10 * Game1.pixelZoom), (float)(this.y + this.height - 11 * Game1.pixelZoom));
            this.dialogueIcon = new TemporaryAnimatedSprite(Game1.mouseCursors, new Rectangle(330, 357, 7, 13), 100f, 6, 999999, position, false, false, 0.89f, 0.0f, Color.White, (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, true)
            {
                yPeriodic = true,
                yPeriodicLoopTime = 1500f,
                yPeriodicRange = (float)(Game1.tileSize / 8)
            };
        }

        private void setUpCloseDialogueIcon()
        {
            Vector2 position = new Vector2((float)(this.x + this.width - 10 * Game1.pixelZoom), (float)(this.y + this.height - 11 * Game1.pixelZoom));
            if (this.isPortraitBox())
                position.X -= (float)(115 * Game1.pixelZoom + 8 * Game1.pixelZoom);
            this.dialogueIcon = new TemporaryAnimatedSprite(Game1.mouseCursors, new Rectangle(289, 342, 11, 12), 80f, 11, 999999, position, false, false, 0.89f, 0.0f, Color.White, (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, true);
        }

        private void setUpNextPageIcon()
        {
            Vector2 position = new Vector2((float)(this.x + this.width - 10 * Game1.pixelZoom), (float)(this.y + this.height - 10 * Game1.pixelZoom));
            if (this.isPortraitBox())
                position.X -= (float)(115 * Game1.pixelZoom + 8 * Game1.pixelZoom);
            this.dialogueIcon = new TemporaryAnimatedSprite(Game1.mouseCursors, new Rectangle(232, 346, 9, 9), 90f, 6, 999999, position, false, false, 0.89f, 0.0f, Color.White, (float)Game1.pixelZoom, 0.0f, 0.0f, 0.0f, true)
            {
                yPeriodic = true,
                yPeriodicLoopTime = 1500f,
                yPeriodicRange = (float)(Game1.tileSize / 8)
            };
        }

        private void checkDialogue(Dialogue d)
        {
            this.isQuestion = false;
            string str1 = "";
            if (this.characterDialoguesBrokenUp.Count == 1)
                str1 = SpriteText.getSubstringBeyondHeight(this.characterDialoguesBrokenUp.Peek(), this.width - 115 * Game1.pixelZoom - 5 * Game1.pixelZoom, this.height - Game1.pixelZoom * 4);
            if (str1.Length > 0)
            {
                string str2 = this.characterDialoguesBrokenUp.Pop().Replace(Environment.NewLine, "");
                this.characterDialoguesBrokenUp.Push(str1.Trim());
                this.characterDialoguesBrokenUp.Push(str2.Substring(0, str2.Length - str1.Length + 1).Trim());
            }
            else if (d.getCurrentDialogue().Length == 0)
                this.beginOutro();
            if (!d.isCurrentDialogueAQuestion())
                return;
            this.responses = d.getResponseOptions();
            this.isQuestion = true;
            this.setUpQuestions();
        }

        private void setUpQuestions()
        {
            int widthConstraint = this.width - Game1.pixelZoom * 4;
            this.heightForQuestions = SpriteText.getHeightOfString(this.getCurrentString(), widthConstraint);
            foreach (Response response in this.responses)
                this.heightForQuestions += SpriteText.getHeightOfString(response.responseText, widthConstraint) + Game1.pixelZoom * 4;
            this.heightForQuestions += Game1.pixelZoom * 10;
        }

        public bool isPortraitBox()
        {
            if (this.characterDialogue != null && this.characterDialogue.speaker != null && (this.characterDialogue.speaker.Portrait != null && this.characterDialogue.showPortrait))
                return Game1.options.showPortraits;
            return false;
        }

        public void drawBox(SpriteBatch b, int xPos, int yPos, int boxWidth, int boxHeight)
        {
            if (xPos <= 0)
                return;
            b.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos, boxWidth, boxHeight), new Rectangle?(new Rectangle(306, 320, 16, 16)), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(xPos, yPos - 5 * Game1.pixelZoom, boxWidth, 6 * Game1.pixelZoom), new Rectangle?(new Rectangle(275, 313, 1, 6)), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(xPos + 3 * Game1.pixelZoom, yPos + boxHeight, boxWidth - 5 * Game1.pixelZoom, 8 * Game1.pixelZoom), new Rectangle?(new Rectangle(275, 328, 1, 8)), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(xPos - 8 * Game1.pixelZoom, yPos + 6 * Game1.pixelZoom, 8 * Game1.pixelZoom, boxHeight - 7 * Game1.pixelZoom), new Rectangle?(new Rectangle(264, 325, 8, 1)), Color.White);
            b.Draw(Game1.mouseCursors, new Rectangle(xPos + boxWidth, yPos, 7 * Game1.pixelZoom, boxHeight), new Rectangle?(new Rectangle(293, 324, 7, 1)), Color.White);
            b.Draw(Game1.mouseCursors, new Vector2((float)(xPos - 11 * Game1.pixelZoom), (float)(yPos - 7 * Game1.pixelZoom)), new Rectangle?(new Rectangle(261, 311, 14, 13)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2((float)(xPos + boxWidth - Game1.pixelZoom * 2), (float)(yPos - 7 * Game1.pixelZoom)), new Rectangle?(new Rectangle(291, 311, 12, 11)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2((float)(xPos + boxWidth - Game1.pixelZoom * 2), (float)(yPos + boxHeight - 2 * Game1.pixelZoom)), new Rectangle?(new Rectangle(291, 326, 12, 12)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.87f);
            b.Draw(Game1.mouseCursors, new Vector2((float)(xPos - 11 * Game1.pixelZoom), (float)(yPos + boxHeight - Game1.pixelZoom)), new Rectangle?(new Rectangle(261, 327, 14, 11)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.87f);
        }

        private bool shouldPortraitShake(Dialogue d)
        {
            int portraitIndex = d.getPortraitIndex();
            if (d.speaker.name.Equals("Pam") && portraitIndex == 3 || d.speaker.name.Equals("Abigail") && portraitIndex == 7 || (d.speaker.name.Equals("Haley") && portraitIndex == 5 || d.speaker.name.Equals("Maru") && portraitIndex == 9))
                return true;
            return this.newPortaitShakeTimer > 0;
        }

        public void drawPortrait(SpriteBatch b)
        {
            if (this.width < 107 * Game1.pixelZoom * 3 / 2)
                return;
            int num1 = this.x + this.width - 112 * Game1.pixelZoom + Game1.pixelZoom;
            int num2 = this.x + this.width - num1;
            b.Draw(Game1.mouseCursors, new Rectangle(num1 - 10 * Game1.pixelZoom, this.y, 9 * Game1.pixelZoom, this.height), new Rectangle?(new Rectangle(278, 324, 9, 1)), Color.White);
            b.Draw(Game1.mouseCursors, new Vector2((float)(num1 - 10 * Game1.pixelZoom), (float)(this.y - 5 * Game1.pixelZoom)), new Rectangle?(new Rectangle(278, 313, 10, 7)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.88f);
            b.Draw(Game1.mouseCursors, new Vector2((float)(num1 - 10 * Game1.pixelZoom), (float)(this.y + this.height)), new Rectangle?(new Rectangle(278, 328, 10, 8)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.88f);
            int num3 = num1 + Game1.pixelZoom * 19;
            int num4 = this.y + this.height / 2 - 74 * Game1.pixelZoom / 2 - 18 * Game1.pixelZoom / 2;
            b.Draw(Game1.mouseCursors, new Vector2((float)(num1 - 2 * Game1.pixelZoom), (float)this.y), new Rectangle?(new Rectangle(583, 411, 115, 97)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.88f);
            Rectangle rectangle = Game1.getSourceRectForStandardTileSheet(this.characterDialogue.speaker.Portrait, this.characterDialogue.getPortraitIndex(), 64, 64);
            if (!this.characterDialogue.speaker.Portrait.Bounds.Contains(rectangle))
                rectangle = new Rectangle(0, 0, 64, 64);
            int num5 = this.shouldPortraitShake(this.characterDialogue) ? Game1.random.Next(-1, 2) : 0;
            b.Draw(this.characterDialogue.speaker.Portrait, new Vector2((float)(num3 + 4 * Game1.pixelZoom + num5), (float)(num4 + 6 * Game1.pixelZoom)), new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.88f);
            SpriteText.drawStringHorizontallyCenteredAt(b, this.characterDialogue.speaker.getName(), num1 + num2 / 2, num4 + 74 * Game1.pixelZoom + 4 * Game1.pixelZoom, 999999, -1, 999999, 1f, 0.88f, false, -1);
            if (Game1.eventUp || this.friendshipJewel.Equals(Rectangle.Empty) || (this.characterDialogue == null || this.characterDialogue.speaker == null) || !Game1.player.friendships.ContainsKey(this.characterDialogue.speaker.name))
                return;
            b.Draw(Game1.mouseCursors, new Vector2((float)this.friendshipJewel.X, (float)this.friendshipJewel.Y), new Rectangle?(Game1.player.getFriendshipHeartLevelForNPC(this.characterDialogue.speaker.name) >= 10 ? new Rectangle(269, 494, 11, 11) : new Rectangle(Math.Max(140, 140 + (int)(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000.0 / 250.0) * 11), Math.Max(532, 532 + Game1.player.getFriendshipHeartLevelForNPC(this.characterDialogue.speaker.name) / 2 * 11), 11, 11)), Color.White, 0.0f, Vector2.Zero, (float)Game1.pixelZoom, SpriteEffects.None, 0.88f);
        }

        public string getCurrentString()
        {
            if (this.characterDialogue != null)
            {
                string str = this.characterDialoguesBrokenUp.Count <= 0 ? this.characterDialogue.getCurrentDialogue().Trim().Replace(Environment.NewLine, "") : this.characterDialoguesBrokenUp.Peek().Trim().Replace(Environment.NewLine, "");
                if (!Game1.options.showPortraits)
                    str = this.characterDialogue.speaker.getName() + ": " + str;
                return str;
            }
            if (this.dialogues.Count > 0)
                return this.dialogues[0].Trim().Replace(Environment.NewLine, "");
            return "";
        }

        public override void update(GameTime time)
        {
            base.update(time);
            Game1.mouseCursorTransparency = Game1.lastCursorMotionWasMouse || this.isQuestion ? 1f : 0.0f;
            if (this.gamePadIntroTimer > 0 && !this.isQuestion)
            {
                Game1.mouseCursorTransparency = 0.0f;
                this.gamePadIntroTimer -= time.ElapsedGameTime.Milliseconds;
            }
            if (this.safetyTimer > 0)
                this.safetyTimer -= time.ElapsedGameTime.Milliseconds;
            if (DialogueBox.questionFinishPauseTimer > 0)
            {
                DialogueBox.questionFinishPauseTimer -= time.ElapsedGameTime.Milliseconds;
            }
            else
            {
                if (this.transitioning)
                {
                    if (this.transitionX == -1)
                    {
                        this.transitionX = this.x + this.width / 2;
                        this.transitionY = this.y + this.height / 2;
                        this.transitionWidth = 0;
                        this.transitionHeight = 0;
                    }
                    if (this.transitioningBigger)
                    {
                        int num = this.transitionWidth;
                        this.transitionX -= (int)((double)time.ElapsedGameTime.Milliseconds * 3.0);
                        this.transitionY -= (int)((double)time.ElapsedGameTime.Milliseconds * 3.0 * ((this.isQuestion ? (double)this.heightForQuestions : (double)this.height) / (double)this.width));
                        this.transitionX = Math.Max(this.x, this.transitionX);
                        this.transitionY = Math.Max(this.isQuestion ? this.y + this.height - this.heightForQuestions : this.y, this.transitionY);
                        this.transitionWidth += (int)((double)time.ElapsedGameTime.Milliseconds * 3.0 * 2.0);
                        this.transitionHeight += (int)((double)time.ElapsedGameTime.Milliseconds * 3.0 * ((this.isQuestion ? (double)this.heightForQuestions : (double)this.height) / (double)this.width) * 2.0);
                        this.transitionWidth = Math.Min(this.width, this.transitionWidth);
                        this.transitionHeight = Math.Min(this.isQuestion ? this.heightForQuestions : this.height, this.transitionHeight);
                        if (num == 0 && this.transitionWidth > 0)
                            this.playOpeningSound();
                        if (this.transitionX == this.x && this.transitionY == (this.isQuestion ? this.y + this.height - this.heightForQuestions : this.y))
                        {
                            this.transitioning = false;
                            this.characterAdvanceTimer = 90;
                            this.setUpIcons();
                            this.transitionX = this.x;
                            this.transitionY = this.y;
                            this.transitionWidth = this.width;
                            this.transitionHeight = this.height;
                        }
                    }
                    else
                    {
                        this.transitionX += (int)((double)time.ElapsedGameTime.Milliseconds * 3.0);
                        this.transitionY += (int)((double)time.ElapsedGameTime.Milliseconds * 3.0 * ((double)this.height / (double)this.width));
                        this.transitionX = Math.Min(this.x + this.width / 2, this.transitionX);
                        this.transitionY = Math.Min(this.y + this.height / 2, this.transitionY);
                        this.transitionWidth -= (int)((double)time.ElapsedGameTime.Milliseconds * 3.0 * 2.0);
                        this.transitionHeight -= (int)((double)time.ElapsedGameTime.Milliseconds * 3.0 * ((double)this.height / (double)this.width) * 2.0);
                        this.transitionWidth = Math.Max(0, this.transitionWidth);
                        this.transitionHeight = Math.Max(0, this.transitionHeight);
                        if (this.transitionWidth == 0 && this.transitionHeight == 0)
                            this.closeDialogue();
                    }
                }
                if (!this.transitioning && this.characterIndexInDialogue < this.getCurrentString().Length)
                {
                    this.characterAdvanceTimer -= time.ElapsedGameTime.Milliseconds;
                    if (this.characterAdvanceTimer <= 0)
                    {
                        this.characterAdvanceTimer = 30;
                        int num = this.characterIndexInDialogue;
                        this.characterIndexInDialogue = Math.Min(this.characterIndexInDialogue + 1, this.getCurrentString().Length);
                        if (this.characterIndexInDialogue != num && this.characterIndexInDialogue == this.getCurrentString().Length)
                            Game1.playSound("dialogueCharacterClose");
                        if (this.characterIndexInDialogue > 1 && this.characterIndexInDialogue < this.getCurrentString().Length && Game1.options.dialogueTyping)
                            Game1.playSound("dialogueCharacter");
                    }
                }
                if (!this.transitioning && this.dialogueIcon != null)
                    this.dialogueIcon.update(time);
                if (this.transitioning || this.newPortaitShakeTimer <= 0)
                    return;
                this.newPortaitShakeTimer -= time.ElapsedGameTime.Milliseconds;
            }
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.width = 1200;
            this.height = 6 * Game1.tileSize;
            this.x = (int)Utility.getTopLeftPositionForCenteringOnScreen(this.width, this.height, 0, 0).X;
            this.y = Game1.viewport.Height - this.height - Game1.tileSize;
            this.friendshipJewel = new Rectangle(this.x + this.width - Game1.tileSize, this.y + Game1.tileSize * 4, 11 * Game1.pixelZoom, 11 * Game1.pixelZoom);
            this.setUpIcons();
        }

        public override void draw(SpriteBatch b)
        {
            if (this.width < Game1.tileSize / 4 || this.height < Game1.tileSize / 4)
                return;
            if (this.transitioning)
            {
                this.drawBox(b, this.transitionX, this.transitionY, this.transitionWidth, this.transitionHeight);
                if (this.activatedByGamePad && !Game1.lastCursorMotionWasMouse && (!this.isQuestion && !Game1.isGamePadThumbstickInMotion()) || Game1.getMouseX() == 0 && Game1.getMouseY() == 0)
                    return;
                this.drawMouse(b);
            }
            else
            {
                if (this.isQuestion)
                {
                    this.drawBox(b, this.x, this.y - (this.heightForQuestions - this.height), this.width, this.heightForQuestions);
                    SpriteText.drawString(b, this.getCurrentString(), this.x + Game1.pixelZoom * 2, this.y + Game1.pixelZoom * 3 - (this.heightForQuestions - this.height), this.characterIndexInDialogue, this.width - Game1.pixelZoom * 4, 999999, 1f, 0.88f, false, -1, "", -1);
                    if (this.characterIndexInDialogue >= this.getCurrentString().Length - 1)
                    {
                        int y = this.y - (this.heightForQuestions - this.height) + SpriteText.getHeightOfString(this.getCurrentString(), this.width - Game1.pixelZoom * 4) + Game1.pixelZoom * 12;
                        for (int index = 0; index < this.responses.Count; ++index)
                        {
                            if (index == this.selectedResponse)
                                IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(375, 357, 3, 3), this.x + Game1.pixelZoom, y - Game1.pixelZoom * 2, this.width - Game1.pixelZoom * 2, SpriteText.getHeightOfString(this.responses[index].responseText, this.width - Game1.pixelZoom * 4) + Game1.pixelZoom * 4, Color.White, (float)Game1.pixelZoom, false);
                            SpriteText.drawString(b, this.responses[index].responseText, this.x + Game1.pixelZoom * 2, y, 999999, this.width, 999999, this.selectedResponse == index ? 1f : 0.6f, 0.88f, false, -1, "", -1);
                            y += SpriteText.getHeightOfString(this.responses[index].responseText, this.width) + Game1.pixelZoom * 4;
                        }
                    }
                }
                else
                {
                    this.drawBox(b, this.x, this.y, this.width, this.height);
                    if (!this.isPortraitBox() && !this.isQuestion)
                    {
                        SpriteBatch b1 = b;
                        string currentString = this.getCurrentString();
                        int x = this.x + Game1.pixelZoom * 2;
                        int y = this.y + Game1.pixelZoom * 2;
                        int characterPosition = this.characterIndexInDialogue;
                        int width = this.width;
                        int num1 = Game1.pixelZoom;
                        int height = 999999;
                        double num2 = 1.0;
                        double num3 = 0.879999995231628;
                        int num4 = 0;
                        int drawBGScroll = -1;
                        string placeHolderScrollWidthText = "";
                        int color = -1;
                        SpriteText.drawString(b1, currentString, x, y, characterPosition, width, height, (float)num2, (float)num3, num4 != 0, drawBGScroll, placeHolderScrollWidthText, color);
                    }
                }
                if (this.isPortraitBox() && !this.isQuestion)
                {
                    this.drawPortrait(b);
                    if (!this.isQuestion)
                        SpriteText.drawString(b, this.getCurrentString(), this.x + Game1.pixelZoom * 2, this.y + Game1.pixelZoom * 2, this.characterIndexInDialogue, this.width - 115 * Game1.pixelZoom - 5 * Game1.pixelZoom, 999999, 1f, 0.88f, false, -1, "", -1);
                }
                if (this.dialogueIcon != null && this.characterIndexInDialogue >= this.getCurrentString().Length - 1)
                    this.dialogueIcon.draw(b, true, 0, 0);
                if ((!this.activatedByGamePad || Game1.lastCursorMotionWasMouse || (this.isQuestion || Game1.isGamePadThumbstickInMotion())) && (Game1.getMouseX() != 0 || Game1.getMouseY() != 0))
                    this.drawMouse(b);
                if (this.hoverText.Length <= 0)
                    return;
                SpriteText.drawStringWithScrollBackground(b, this.hoverText, this.friendshipJewel.Center.X - SpriteText.getWidthOfString(this.hoverText) / 2, this.friendshipJewel.Y - Game1.tileSize, "", 1f, -1);
            }
        }
    }
}
