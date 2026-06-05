using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Consultorio.MAUI.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(name);
            return true;
        }

        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set => SetField(ref _isBusy, value); }

        private string _titulo = string.Empty;
        public string Titulo { get => _titulo; set => SetField(ref _titulo, value); }

        private string _mensagemErro = string.Empty;
        public string MensagemErro { get => _mensagemErro; set => SetField(ref _mensagemErro, value); }

        protected async Task ExecutarAsync(Func<Task> acao, string erroDefault = "Erro ao executar operação.")
        {
            if (IsBusy) return;
            IsBusy = true;
            MensagemErro = string.Empty;
            try { await acao(); }
            catch (Exception ex) { MensagemErro = ex.Message; }
            finally { IsBusy = false; }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;
        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
            : this(() => { execute(); return Task.CompletedTask; }, canExecute) { }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public async void Execute(object? parameter) => await _execute();
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Func<T?, Task> _execute;
        private readonly Func<T?, bool>? _canExecute;
        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Func<T?, Task> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke((T?)parameter) ?? true;
        public async void Execute(object? parameter) => await _execute((T?)parameter);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
