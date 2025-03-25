using EffortEngine.LocalLibrary;
using SharedProject.Events;
using SharedProject.Models;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

public class PomodoroTimer : BindableBase, IDisposable
{
    #region Fields
    private readonly DispatcherTimer _timer;
    private readonly IEventAggregator _eventAggregator;
    private readonly TaskManager _taskManager;
    private readonly MediaPlayer _soundPlayer = new();

    private int _timeRemaining;
    private int _roundsCompleted;
    private bool _isBreak;
    private int _activeWorkTime;
    private bool _isRunning;
    private string _currentTaskText = string.Empty;
    private string _timerDisplay = "00:00";
    private string _roundCounter = "Brak sesji";
    #endregion

    #region Properties
    public decimal ActiveWorkMinutes => Math.Round(_activeWorkTime / 60m, 2);

    public bool IsRunning
    {
        get => _isRunning;
        private set => SetProperty(ref _isRunning, value);
    }

    public string CurrentTaskText
    {
        get => _currentTaskText;
        private set => SetProperty(ref _currentTaskText, value);
    }

    public string TimerDisplay
    {
        get => _timerDisplay;
        private set => SetProperty(ref _timerDisplay, value);
    }

    public string RoundCounter
    {
        get => _roundCounter;
        private set => SetProperty(ref _roundCounter, value);
    }

    public DelegateCommand StartPauseCommand { get; }
    public DelegateCommand ResetCommand { get; }
    #endregion

    #region Constructor
    public PomodoroTimer(IEventAggregator eventAggregator, TaskManager taskManager)
    {
        _eventAggregator = eventAggregator;
        _taskManager = taskManager;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += OnTimerTick;

        StartPauseCommand = new DelegateCommand(OnStartPause);
        ResetCommand = new DelegateCommand(Reset);

        SubscribeEvents();
    }
    #endregion

    #region Event Handlers
    private void SubscribeEvents()
    {
        _eventAggregator.GetEvent<CurrentTaskChangedEvent>().Subscribe(OnCurrentTaskChanged);
        _eventAggregator.GetEvent<SessionStartedEvent>().Subscribe(OnSessionStarted);
        _eventAggregator.GetEvent<SessionEndedEvent>().Subscribe(OnSessionEnded);
        _eventAggregator.GetEvent<WorkSessionInterruptedEvent>().Subscribe(OnWorkInterrupted);
    }

    private void OnCurrentTaskChanged(TaskBase task)
    {
        CurrentTaskText = task != null ? $"Praca nad: {task.Name}" : string.Empty;
    }

    private void OnSessionStarted()
    {
        if (_taskManager.CurrentTask != null)
        {
            StartNewSession();
        }
    }

    private void OnSessionEnded()
    {
        CommitWorkTimeAsync().ConfigureAwait(false);
        Reset();
    }

    private void OnWorkInterrupted()
    {
        if (IsRunning)
        {
            _timer.Stop();
            IsRunning = false;
            CommitWorkTimeAsync().ConfigureAwait(false);
        }
    }
    #endregion

    #region Public Methods
    public async Task CommitWorkTimeAsync()
    {
        if (_taskManager.CurrentTask != null && ActiveWorkMinutes > 0)
        {
            _taskManager.CurrentTask.TotalWorkTime += ActiveWorkMinutes;
            _activeWorkTime = 0;
            await Task.Delay(1); // Ensure async context
            _eventAggregator.GetEvent<WorkTimeUpdatedEvent>().Publish(_taskManager.CurrentTask.Id);
        }
    }

    public void StartNewSession()
    {
        Reset();
        _timeRemaining = 25 * 60;
        _timer.Start();
        IsRunning = true;
        RoundCounter = "Runda 1/4";
        _eventAggregator.GetEvent<PomodoroRoundStartedEvent>().Publish(1);
    }
    #endregion

    #region Private Methods
    private void OnStartPause()
    {
        if (string.IsNullOrEmpty(CurrentTaskText))
        {
            MessageBox.Show("Wybierz zadanie przed rozpoczęciem", "Uwaga",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_timeRemaining == 0)
        {
            _eventAggregator.GetEvent<SessionStartedEvent>().Publish();
            return;
        }

        IsRunning = !IsRunning;

        if (IsRunning)
        {
            _timer.Start();
            _eventAggregator.GetEvent<PomodoroResumedEvent>().Publish();
        }
        else
        {
            _timer.Stop();
            _eventAggregator.GetEvent<PomodoroPausedEvent>().Publish();
        }
    }

    private void OnTimerTick(object sender, EventArgs e)
    {
        if (_timeRemaining > 0)
        {
            _timeRemaining--;

            if (!_isBreak && IsRunning)
            {
                _activeWorkTime++;
                if (_activeWorkTime % 10 == 0) // Update every 10 seconds
                {
                    _eventAggregator.GetEvent<WorkTimeUpdatedEvent>()
                        .Publish(_taskManager.CurrentTask?.Id ?? 0);
                }
            }

            UpdateTimerDisplay();
        }

        if (_timeRemaining == 0)
        {
            HandleRoundCompletion();
        }
    }

    private void UpdateTimerDisplay()
    {
        TimerDisplay = $"{(_isBreak ? "Przerwa" : "Praca")}: " +
                     $"{TimeSpan.FromSeconds(_timeRemaining):mm\\:ss}";
    }

    private void HandleRoundCompletion()
    {
        _isBreak = !_isBreak;
        _timeRemaining = _isBreak ? 5 * 60 : 25 * 60;

        if (_isBreak)
        {
            _eventAggregator.GetEvent<BreakStartedEvent>().Publish();
            PlaySound("break_start.mp3");
        }
        else
        {
            _roundsCompleted++;
            _eventAggregator.GetEvent<BreakEndedEvent>().Publish();
            RoundCounter = $"Runda {_roundsCompleted + 1}/4";

            if (_roundsCompleted >= 4)
            {
                _eventAggregator.GetEvent<SessionEndedEvent>().Publish();
                PlaySound("session_complete.mp3");
            }
            else
            {
                PlaySound("round_complete.mp3");
            }
        }
    }

    private void PlaySound(string soundFile)
    {
        _soundPlayer.Open(new Uri($"Sounds/{soundFile}", UriKind.Relative));
        _soundPlayer.Play();
    }

    public void Reset()
    {
        _timer.Stop();
        IsRunning = false;
        _timeRemaining = 25 * 60;
        _roundsCompleted = 0;
        _activeWorkTime = 0;
        _isBreak = false;
        TimerDisplay = "00:00";
        RoundCounter = "Brak sesji";
    }

    public void Dispose()
    {
        _timer.Tick -= OnTimerTick;
        _timer.Stop();
        _soundPlayer.Close();
        GC.SuppressFinalize(this);
    }
}