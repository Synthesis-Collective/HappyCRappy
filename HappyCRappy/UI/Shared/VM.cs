using Noggog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class VM : INotifyPropertyChanged, IDisposableDropoff
{
    private readonly CompositeDisposable _compositeDisposable = new();
    public event PropertyChangedEventHandler? PropertyChanged;

    public void Dispose() => _compositeDisposable.Dispose();

    public void Add(IDisposable disposable)
    {
        _compositeDisposable.Add(disposable);
    }
}
