using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ISRMUL.Windows
{
    /// <summary>
    /// Логика взаимодействия для TaskInvokeForm.xaml
    /// </summary>
    /// 
    public class TaskBuffer
    {
        public static double Buffer { get; set; }
    }
    public partial class TaskInvokeForm : Window
    {
        Task invoker { get; set; }
        public TaskInvokeForm(Task task)
        {
            InitializeComponent();
            invoker = task;
            invoke();
        }

        public static double percentBuff;

        void invoke()
        {
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            timer.Tick += (a, b) => { 
                percent = Math.Round(TaskBuffer.Buffer, 2); 
                if (percent == 1) 
                { 
                    TaskBuffer.Buffer = 0;
                    timer.Stop();
                    this.Close(); 
                } 
            };
            timer.Start();

            Task t = new System.Threading.Tasks.Task(() =>
            {
                invoker.RunSynchronously();
            });
            t.Start();
        }

        #region dependency
        public static readonly DependencyProperty PercentProperty = DependencyProperty.Register("percent", typeof(double), typeof(TaskInvokeForm));
        public double percent
        {
            get
            {
                return (double)this.GetValue(PercentProperty);
            }
            set
            {
                this.SetValue(PercentProperty, value);
            }
        }
        #endregion
    }
}
