using SrtVideoPlayer.Shared.Constants;
using SrtVideoPlayer.Shared.Localization;
using SrtVideoPlayer.Shared.Logic;
using SrtVideoPlayer.Shared.Models.Enums;
using SrtVideoPlayer.Shared.PlatformServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SrtVideoPlayer.Shared.ViewModels
{
    public class PlayerViewModel : BaseViewModel
    {
        private static readonly List<char> _possibleDecimalSeparators
            = new List<char>()
            {
                LexicalSymbolsAsChar.Comma,
                LexicalSymbolsAsChar.Dot
            };

        private static readonly Dictionary<char, char> _equivalentSymbols
            = new Dictionary<char, char>()
            {
                { LexicalSymbolsAsChar.SimpleDivisionOperator, LexicalSymbolsAsChar.DivisionOperator },
                { LexicalSymbolsAsChar.SimpleMultiplicationOperator, LexicalSymbolsAsChar.MultiplicationOperator }
            };

        private readonly IAlertsService _alertsService;
        private readonly IClipboardService _clipboardService;
        private readonly ICommandFactoryService _commandFactoryService;
        private readonly INavigationService _navigationService;
        private readonly IPlatformInformationService _platformInformationService;

        private readonly Dictionary<char, decimal> _variableStorageValues
            = new Dictionary<char, decimal>();

        private bool _calculating;

        private string _lastInput;

        private NextInput _nextStroke = NextInput.DoNothing;

        public PlayerViewModel(
            IAlertsService alertsService,
            IClipboardService clipboardService,
            ICommandFactoryService commandFactoryService,
            INavigationService navigationService,
            IPlatformInformationService platformInformationService)
        {
            _alertsService = alertsService;
            _clipboardService = clipboardService;
            _commandFactoryService = commandFactoryService;
            _navigationService = navigationService;
            _platformInformationService = platformInformationService;

            AllClearCommand = _commandFactoryService.Create(AllClear);
            ClearCommand = _commandFactoryService.Create(Clear);
            DeleteCommand = _commandFactoryService.Create(Delete);
            BinaryOperatorCommand = _commandFactoryService.Create<string>((symbol) => BinaryOperator(symbol));
            UnaryOperatorCommand = _commandFactoryService.Create<string>((symbol) => UnaryOperator(symbol));
            ParenthesisCommand = _commandFactoryService.Create<string>((parenthesis) => Parenthesis(parenthesis));
            VariableStorageCommand = _commandFactoryService.Create<string>((symbol) => VariableStorage(symbol));
            NumberCommand = _commandFactoryService.Create<string>((number) => Number(number));
            DecimalCommand = _commandFactoryService.Create(Decimal);
            CalculateCommand = _commandFactoryService.Create(Calculate);
            CopyInputToClipboardCommand = _commandFactoryService.Create(CopyInputToClipboard);
            ManageInputFromHardwareCommand = _commandFactoryService.Create<char>((character) => ManageInputFromHardware(character));
            ShowHistoryCommand = _commandFactoryService.Create(ShowHistory);
            NavigateToSettingsCommand = _commandFactoryService.Create(async () => await NavigateToSettingsAsync());
            ShowAboutCommand = _commandFactoryService.Create(async () => await ShowAbout());
        }

        private string _input = string.Empty;

        public string Input
        {
            get => _input;
            set
            {
                if (_input == value)
                    return;
                _input = value;
                OnPropertyChanged();

                if (!_calculating)
                    AfterResult = false;
            }
        }

        public string DecimalSeparator => Logic.SrtVideoPlayer.DecimalSeparator;

        public bool AfterResult { get; private set; }

        public ICommand AllClearCommand { get; private set; }

        public ICommand ClearCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }

        public ICommand BinaryOperatorCommand { get; private set; }

        public ICommand UnaryOperatorCommand { get; private set; }

        public ICommand ParenthesisCommand { get; private set; }

        public ICommand VariableStorageCommand { get; private set; }

        public ICommand NumberCommand { get; private set; }

        public ICommand DecimalCommand { get; private set; }

        public ICommand CalculateCommand { get; private set; }

        public ICommand CopyInputToClipboardCommand { get; private set; }

        public ICommand ManageInputFromHardwareCommand { get; private set; }

        public ICommand ShowHistoryCommand { get; private set; }

        public ICommand NavigateToSettingsCommand { get; private set; }

        public ICommand ShowAboutCommand { get; private set; }

        private void AllClear()
        {
            // Clear user input and memory values
            Clear();
            _variableStorageValues.Clear();
        }

        private void Clear()
        {
            // Clear user input
            Input = string.Empty;
            _nextStroke = NextInput.DoNothing;
        }

        private void Delete()
        {
            // Do nothing if there is currently no input
            if (string.IsNullOrWhiteSpace(Input))
                return;
            // Clear everything if required
            if (_nextStroke != NextInput.DoNothing)
            {
                Input = string.Empty;
                _nextStroke = NextInput.DoNothing;
                return;
            }
            // Else only delete 1 character, the last one
            Input = Input[0..^1];
        }

        private void BinaryOperator(string symbol)
        {
            if (_nextStroke == NextInput.ClearAtAny)
            {
                Input = symbol;
                _nextStroke = NextInput.DoNothing;
            }
            else if (_nextStroke == NextInput.ClearAtNumber)
            {
                Input = Logic.SrtVideoPlayer.LastResult + symbol;
                _nextStroke = NextInput.DoNothing;
            }
            else
                Input += symbol;
        }

        private void UnaryOperator(string symbol)
        {
            if (_nextStroke != NextInput.DoNothing)
            {
                Input = symbol;
                _nextStroke = NextInput.DoNothing;
            }
            else
                Input += symbol;
        }

        private void Parenthesis(string parenthesis)
        {
            if (_nextStroke != NextInput.DoNothing)
            {
                Input = parenthesis;
                _nextStroke = NextInput.DoNothing;
            }
            else
                Input += parenthesis;
        }

        private void VariableStorage(string symbol)
        {
            if (_nextStroke != NextInput.DoNothing)
            {
                Input = symbol;
                _nextStroke = NextInput.DoNothing;
            }
            else
                Input += symbol;
        }

        private void Number(string number)
        {
            if (_nextStroke == NextInput.ClearAtAny
                || _nextStroke == NextInput.ClearAtNumber)
            {
                Input = number;
                _nextStroke = NextInput.DoNothing;
            }
            else
                Input += number;
        }

        private void Decimal()
        {
            if (_nextStroke != NextInput.DoNothing)
            {
                Input = Logic.SrtVideoPlayer.DecimalSeparator;
                _nextStroke = NextInput.DoNothing;
            }
            else
                Input += Logic.SrtVideoPlayer.DecimalSeparator;
        }

        private void Calculate()
        {
            // Do nothing if there is no input
            if (string.IsNullOrWhiteSpace(Input))
                return;

            // Clear input if there was no interaction after an error
            if (_nextStroke == NextInput.ClearAtAny)
            {
                Input = string.Empty;
                _nextStroke = NextInput.DoNothing;
                return;
            }

            // Use previous input if it was a valid one and there was no interaction after calculating
            var input = _nextStroke == NextInput.ClearAtNumber ? _lastInput : Input;

            // Calculate and show corresponding result
            _calculating = true;
            _lastInput = input;
            var calculationResult = Logic.SrtVideoPlayer.Calculate(input, _variableStorageValues);
            if (calculationResult != null)
                // Show result if calculation was successful
                if (calculationResult.Successful)
                {
                    var result = calculationResult.Result;
                    if (TryFormatResult(result, out var resultText))
                    {
                        AfterResult = true;

                        Input = resultText;
                        _nextStroke = NextInput.ClearAtNumber;

                        AddOrUpdateVariableStorage(Logic.SrtVideoPlayer.LastResult, result);

                        _ = Settings.Instance.ManageNewResultAsync(resultText);
                    }
                    // Show error message if result could not be formatted
                    else
                    {
                        Input = LocalizedStrings.CalculationError;
                        _nextStroke = NextInput.ClearAtAny;
                    }

                }
                // Show error message if calculation was not successful
                else
                {
                    Input = calculationResult.ErrorMessage;
                    _nextStroke = NextInput.ClearAtAny;
                }
            // It has no reason to be a null object
            else
            {
                Input = LocalizedStrings.UnexpectedError;
                _nextStroke = NextInput.ClearAtAny;
            }
            _calculating = false;
        }

        private void AddOrUpdateVariableStorage(char storage, decimal value)
        {
            // If memory value already exists, overwrite it, else, add it
            if (_variableStorageValues.TryGetValue(storage, out decimal _))
                _variableStorageValues[storage] = value;
            else
                _variableStorageValues.Add(storage, value);
        }

        private bool TryFormatResult(decimal result, out string resultText)
        {
            resultText = result.ToString();
            while (resultText.Contains(Logic.SrtVideoPlayer.DecimalSeparator)
                && (char.ToString(resultText[^1]) == Logic.SrtVideoPlayer.ZeroString
                    || char.ToString(resultText[^1]) == Logic.SrtVideoPlayer.DecimalSeparator))
                resultText = resultText[0..^1];
            return true;
        }

        private async void CopyInputToClipboard() =>
            await _clipboardService.SetTextAsync(Input);

        private void ManageInputFromHardware(char character)
        {
            var characterAsString = character.ToString();
            if (Logic.SrtVideoPlayer.VariableStorageCharacters.Contains(character))
                VariableStorage(characterAsString);
            else if (Logic.SrtVideoPlayer.Parentheses.Contains(character))
                Parenthesis(characterAsString);
            else if (Logic.SrtVideoPlayer.BinaryOperators.Contains(character))
                BinaryOperator(characterAsString);
            else if (Logic.SrtVideoPlayer.UnaryOperators.Contains(character))
                UnaryOperator(characterAsString);
            else if (Logic.SrtVideoPlayer.Numbers.Contains(character))
                Number(characterAsString);
            else if (_possibleDecimalSeparators.Contains(character))
                Decimal();
            else if (_equivalentSymbols.ContainsKey(character))
                ManageInputFromHardware(_equivalentSymbols[character]);
        }

        private async void ShowHistory()
        {
            if (Settings.Instance.GetResultsHistoryLength() == 0)
            {
                var openSettings = await _alertsService.DisplayConfirmationAsync(LocalizedStrings.Notice,
                    LocalizedStrings.DisabledResultsHistory,
                    LocalizedStrings.Settings);
                if (openSettings)
                    await NavigateToSettingsAsync();
                return;
            }

            if (!Settings.Instance.ContainsResultsHistory())
            {
                await _alertsService.DisplayAlertAsync(LocalizedStrings.Notice,
                    LocalizedStrings.EmptyResultsHistory);
                return;
            }

            var resultsHistory = await Settings.Instance.GetResultsHistoryAsync();
            resultsHistory.Reverse();
            var resultFromHistory = await _alertsService.DisplayOptionsAsync(LocalizedStrings.History,
                LocalizedStrings.ClearHistory,
                resultsHistory.ToArray());
            if (resultFromHistory != null
                && resultFromHistory != LocalizedStrings.Cancel
                && resultFromHistory != LocalizedStrings.ClearHistory)
                if (_nextStroke != NextInput.DoNothing)
                {
                    Input = resultFromHistory;
                    _nextStroke = NextInput.DoNothing;
                }
                else
                    Input += resultFromHistory;
            else if (resultFromHistory == LocalizedStrings.ClearHistory)
                Settings.Instance.ClearResultsHistory();
        }

        private async Task NavigateToSettingsAsync() =>
            await _navigationService.NavigateToAsync(Locations.SettingsPage);

        private async Task ShowAbout() =>
            await _alertsService.DisplayAlertAsync(
                LocalizedStrings.About,
                (_platformInformationService.PlatformSupportsGettingApplicationVersion() ?
                    LocalizedStrings.AppVersion
                        + Environment.NewLine
                        + _platformInformationService.GetApplicationVersion()
                        + Environment.NewLine
                        + Environment.NewLine :
                    string.Empty)
                + LocalizedStrings.AppIconAttribution);
    }
}
