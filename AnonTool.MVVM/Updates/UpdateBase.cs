using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace AnonTool.MVVM.Updates
{
    public class UpdateBase : INotifyPropertyChanged
    {
        // Declare the event 
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        // Use of lambda expression to pass actual variable instead of string, useful for refactoring
        protected void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
                return;

            var body = expression.Body as MemberExpression;
            if (body != null)
                OnPropertyChanged(body.Member.Name);
        }
    }
}
