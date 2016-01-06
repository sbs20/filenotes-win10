using System;
using Windows.UI.Xaml.Controls;

namespace Sbs20.Filenotes.ViewModels
{
    public class NavigationMenuItem
    {
        public string Label { get; set; }
        public Symbol Symbol { get; set; }
        public Type DestPage { get; set; }
        public object Arguments { get; set; }

        public char SymbolAsChar
        {
            get { return (char)this.Symbol; }
        }
    }
}
