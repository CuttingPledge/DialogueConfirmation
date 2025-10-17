using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace DialogueConfirmation
{
    internal sealed class ModEntry : Mod
    {

        private DialogueChange? diagChange;
        public ModConfig Config;
        //private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            diagChange = new DialogueChange(this);
            Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.Input.ButtonPressed += diagChange.OnButtonPressed;
            helper.Events.Display.MenuChanged += diagChange.Display_MenuChanged;
            helper.Events.Display.RenderedActiveMenu += diagChange.DrawArrow;
        }

        // Suppresses all key input passed to it except for Escape and the Controller Back Button
        public void SupressKeyInput(SButton sButton)
        {
            if (sButton != SButton.Escape || sButton != SButton.ControllerBack)
            {
                try
                {
                    this.Helper.Input.Suppress(sButton);
                }
                catch
                {
                    this.Monitor.Log($" Failed to suppress key input " + sButton, LogLevel.Error);
                }
                finally
                {

                }
            }
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            // add some config options
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("Bool.DoubleClickRegularDialogue.DisplayedName"),
                tooltip: () => this.Helper.Translation.Get("Bool.DoubleClickRegularDialogue.Tooltip"),
                getValue: () => this.Config.DoubleClickRegularDialogue,
                setValue: value => this.Config.DoubleClickRegularDialogue = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("Bool.ConfirmSleepDialogue.DisplayedName"),
                tooltip: () => this.Helper.Translation.Get("Bool.ConfirmSleepDialogue.Tooltip"),
                getValue: () => this.Config.ConfirmSleepDialogue,
                setValue: value => this.Config.ConfirmSleepDialogue = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => this.Helper.Translation.Get("Bool.ConfirmQuestionAnswer.DisplayedName"),
                tooltip: () => this.Helper.Translation.Get("Bool.ConfirmQuestionAnswer.Tooltip"),
                getValue: () => this.Config.ConfirmQuestionAnswer,
                setValue: value => this.Config.ConfirmQuestionAnswer = value
            );
        }

    }
}
