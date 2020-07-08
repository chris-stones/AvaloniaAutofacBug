# Why does this crash !?
ReactiveCommands instantiated after setting up Aplat.AutoFac trigger "Call from invalid thread" excetpion.

Is this a bug in my code? Avalonia? or, Aplat.Autofac ?  

Clone this repo,  
Click "I Dont Crash" - Everything is fine.  
Click "I Do Crash" - The Action is run, but after it returns, an Invalid thread exception is raised.  

The same happens under Windows10, and ArchLinux. Both using .NET Core 3.1.  

`MainWindowViewModel.cs`
```cs
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
```

`MainWindow.axml`
```xaml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="clr-namespace:AvaloniaAutofacBug.ViewModels;assembly=AvaloniaAutofacBug"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
        x:Class="AvaloniaAutofacBug.Views.MainWindow"
        Icon="/Assets/avalonia-logo.ico"
        Title="AvaloniaAutofacBug">

    <Design.DataContext>
        <vm:MainWindowViewModel/>
    </Design.DataContext>

  <StackPanel>
    <Button Command="{Binding OnDontCrashClick}" Content="I Dont Crash"/>
    <Button Command="{Binding OnDoCrashClick}" Content="I Do Crash!"/>
  </StackPanel>
  
</Window>
```
