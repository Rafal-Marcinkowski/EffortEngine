using MahApps.Metro.IconPacks;
using SharedProject.Events;
using SharedProject.Models;
using System.Windows.Input;
using System.Windows.Threading;

namespace EffortEngine.MVVM.ViewModels;

public class InformationBarViewModel : BindableBase
{
    private readonly IEventAggregator eventAggregator;
    private DispatcherTimer timer;
    private int timeRemaining;
    private int roundsCompleted;
    private bool isBreak;

    public InformationBarViewModel(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;

        this.eventAggregator.GetEvent<ProgramAddedEvent>().Subscribe(async program => await OnProgramAdded(program));
        this.eventAggregator.GetEvent<StartWorkEvent>().Subscribe(async taskName => await OnNewsTaskStarted(taskName));

        StartPauseCommand = new DelegateCommand(OnStartPause);
        StopCommand = new DelegateCommand(OnStop);
        ResetCommand = new DelegateCommand(OnReset);

        InitializeTimer();
    }

    private async Task OnNewsTaskStarted(string taskName)
    {
        CurrentTaskText = "Praca nad: " + taskName;
    }

    private string currentTaskText = string.Empty;
    public string CurrentTaskText
    {
        get => currentTaskText;
        set => SetProperty(ref currentTaskText, value);
    }

    private string recentActionText = string.Empty;
    public string RecentActionText
    {
        get => recentActionText;
        set => SetProperty(ref recentActionText, value);
    }

    private string timerDisplay;
    public string TimerDisplay
    {
        get => timerDisplay;
        set => SetProperty(ref timerDisplay, value);
    }

    private string startPauseText;
    public string StartPauseText
    {
        get => startPauseText;
        set => SetProperty(ref startPauseText, value);
    }

    private PackIconMaterialKind startPauseIcon;
    public PackIconMaterialKind StartPauseIcon
    {
        get => startPauseIcon;
        set => SetProperty(ref startPauseIcon, value);
    }

    private bool isRunning = false;
    public bool IsRunning
    {
        get => isRunning;
        set => SetProperty(ref isRunning, value);
    }

    public ICommand StartPauseCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand ResetCommand { get; }

    private async void InitializeTimer()
    {
        timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        StartPauseText = IsRunning ? "Pause" : "Start";
        StartPauseIcon = IsRunning ? PackIconMaterialKind.Pause : PackIconMaterialKind.Play;
        timer.Tick += OnTimerTick;
    }

    private void OnStartPause()
    {
        if (!String.IsNullOrEmpty(CurrentTaskText))
        {

        }

        if (IsRunning)
        {
            timer.Stop();
        }

        else
        {
            timer.Start();
        }

        IsRunning = !IsRunning;
        StartPauseText = IsRunning ? "Pause" : "Start";
        StartPauseIcon = IsRunning ? PackIconMaterialKind.Pause : PackIconMaterialKind.Play;
    }

    private void OnStop()
    {
        timer.Stop();
        IsRunning = false;
        StartPauseText = "Start";
        StartPauseIcon = PackIconMaterialKind.Play;
    }

    private void OnReset()
    {
        timer.Stop();
        ResetTimer();
        IsRunning = false;
        StartPauseText = "Start";
        StartPauseIcon = PackIconMaterialKind.Play;
    }

    private void ResetTimer()
    {
        timeRemaining = 25 * 60;
        roundsCompleted = 0;
        isBreak = false;
        TimerDisplay = $"{(isBreak ? "Przerwa" : "Praca")}: {TimeSpan.FromSeconds(timeRemaining):mm\\:ss}";
    }

    private async void OnTimerTick(object sender, EventArgs e)
    {
        timeRemaining--;

        if (timeRemaining <= 0)
        {
            isBreak = !isBreak;
            timeRemaining = isBreak ? 5 * 60 : 25 * 60;

            if (!isBreak)
            {
                roundsCompleted++;
                if (roundsCompleted >= 4)
                {
                    OnStop();
                    ResetTimer();
                    return;
                }
            }
        }

        TimerDisplay = $"{(isBreak ? "Przerwa" : "Praca")}: {TimeSpan.FromSeconds(timeRemaining):mm\\:ss}";
    }

    private async Task OnProgramAdded(Program program)
    {
        RecentActionText = $"Dodano nowy program: {program.Name}";
    }
}




