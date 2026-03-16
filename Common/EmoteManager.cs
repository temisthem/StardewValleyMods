using StardewValley;

namespace TemLib;

public class EmoteManager {
    public bool EmoteEnabled { get; private set; }
    public int CurrentFrame { get; private set; }
    private int _currentInterval;

    public void InitOnSaveLoaded(EmoteBubbleConfig config) {
        EmoteEnabled = !config.ToggleEmoteKey.IsBound || config.DefaultToggleOn;
    }

    public void HandleToggleInput(EmoteBubbleConfig config) {
        if (!config.Enabled) return;
        if (config.ToggleEmoteKey.JustPressed()) EmoteEnabled = !EmoteEnabled;
    }

    public void Animate(EmoteBubbleConfig config, int startFrame, int endFrame, int stillFrame)
    {
        if (!config.Enabled) return;
        
        if (config.NoAnimation)
        {
            CurrentFrame = stillFrame;
        }
        else
        {
            _currentInterval += Game1.currentGameTime.ElapsedGameTime.Milliseconds;

            if (CurrentFrame < startFrame || CurrentFrame > endFrame) CurrentFrame = startFrame;
            if (_currentInterval > config.EmoteInterval) {
                CurrentFrame = CurrentFrame < endFrame ? CurrentFrame + 1 : startFrame;
                _currentInterval = 0;
            }
        }
    }
}
