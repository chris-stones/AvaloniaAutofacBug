using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

using Autofac;
using Splat.Autofac;

namespace AvaloniaAutofacBug.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> OnDontCrashClick { get; }
        public ReactiveCommand<Unit, Unit> OnDoCrashClick { get; }


        public MainWindowViewModel()
        {
            // This button Reactive command wont crash.
            //  It is instantiated BEFORE Splat.AutoFac is invoked.
            //  Note that the Autofac container is empty... it provides nothing.
            OnDontCrashClick = ReactiveCommand.Create(() => { System.Diagnostics.Debug.WriteLine("Dont Crash Clicked"); });

            // Create am empty Autofac container, and register it with Splat (The Avaolonia ServiceLocator).
            var builder = new ContainerBuilder();
            builder.UseAutofacDependencyResolver();

            // When invoked, "Do Crash Clicked" is written to debug output.
            //  When the Assosiated Action returns, an exception is thrown!
            //  Observed with .NET Core 3.1 under Windows 10, and Arch Linux.
            /*
            The thread 0x656c has exited with code 0(0x0).
            An unhandled exception of type 'System.InvalidOperationException' occurred in System.Private.CoreLib.dll
            Call from invalid thread
            */
            OnDoCrashClick = ReactiveCommand.Create(() => { System.Diagnostics.Debug.WriteLine("Do Crash Clicked"); });
        }
    }
}
