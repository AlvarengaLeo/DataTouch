namespace DataTouch.Web.Services;

public class ThemeService
{
    public bool IsDarkMode { get; set; } = false;
    public event Action? OnChange;
    
    public void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
        NotifyStateChanged();
    }
    
    public void SetDarkMode(bool isDark)
    {
        IsDarkMode = isDark;
        NotifyStateChanged();
    }
    
    private void NotifyStateChanged() => OnChange?.Invoke();
}
