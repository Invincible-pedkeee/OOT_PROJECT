using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace Muzej

//ICommand interfejs sa metodama canexecute, execute i eventHandlerom


{
    internal class AddCommand:ICommand
    {
        private ObservableCollection<UmetnickaDela> elementi;
        public AddCommand(ObservableCollection<UmetnickaDela> lista)      
        {
            elementi = lista;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;  
        }

        public void Execute(object? parameter)
        {
          /*  var addWin = new addWindow(elementi);
            addWin.Owner = Application.Current.MainWindow;
            addWin.ShowDialog();                                    ovo nije uradjeno! */  

        }
    }
}
