using Reactive.Bindings.Interactivity;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExplorerAddressBar2.ViewModels.EventConverters
{
    class EnterKeyDownEventToTextBoxStringConverter : ReactiveConverter<dynamic, string>
    {
        protected override IObservable<string> OnConvert(IObservable<dynamic> source)
        {
            return source
                .Where(_ => Keyboard.IsKeyDown(Key.Enter))
                .Select(_ => (this.AssociateObject as TextBox)?.Text)
                .Where(s => !string.IsNullOrEmpty(s));
        }
    }
}
