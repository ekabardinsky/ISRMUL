﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ISRMUL.Control.Editor
{
    /// <summary>
    /// Логика взаимодействия для ToolView.xaml
    /// </summary>
    public partial class ToolView : UserControl
    {
        public Operation CurrentOperation;
        public Brush SelectionBrush { get; set; }
        public Brush StandarBrush { get; set; }

        public ToolView()
        {
            InitializeComponent();

            SelectionBrush = new SolidColorBrush(Color.FromArgb(255, 153, 180, 208));
            StandarBrush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));

            CurrentOperation = Operation.None;
        }

        private void Explore_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (MissingCurrentPage()) return;
            foreach (var child in MainGrid.Children)
            {
                if (child is Button)
                {
                    Button btn = child as Button;

                    if (btn.Equals(sender))
                    {
                        if (sender.Equals(Explore))
                            CurrentOperation = Operation.Explore;
                        if (sender.Equals(Delete))
                            CurrentOperation = Operation.Delete;
                        if (sender.Equals(NewRectangle))
                            CurrentOperation = Operation.NewRectangle;
                        if (sender.Equals(Split))
                            CurrentOperation = Operation.Split;
                        if (sender.Equals(Union))
                            CurrentOperation = Operation.Union;


                        btn.Background = SelectionBrush;
                    }
                    else
                        btn.Background = StandarBrush;
                }
            }
           
        }

        private bool MissingCurrentPage()
        {
            return toolViewProject==null||toolViewProject.CurrentPage == null;
        }


        #region dependency
        public static readonly DependencyProperty toolViewProjectProperty = DependencyProperty.Register("toolViewProject", typeof(Manuscript.Project), typeof(ToolView));
        public Manuscript.Project toolViewProject
        {
            get
            {
                return this.GetValue(toolViewProjectProperty) as Manuscript.Project;
            }
            set
            {
                this.SetValue(toolViewProjectProperty, value);
            }
        }
        #endregion
    }
}
