using InDoOut_Core.Entities.Programs;
using InDoOut_Desktop.Programs;
using InDoOut_Desktop.UI.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace InDoOut_Desktop.UI.Controls.TaskManager
{
    public partial class TaskView : UserControl, ITaskView
    {
        private IBlockView _blockView = null;

        public IBlockView CurrentBlockView { get => _blockView; set => BlockViewChanged(value); }

        public Sidebar.Sidebar Sidebar { get; set; } = null;

        public TaskView()
        {
            InitializeComponent();
        }

        public void CreateNewTask(bool bringToFront = false)
        {
            CreateNewTask(ProgramHolder.Instance.NewProgram(), bringToFront);
        }

        public void CreateNewTask(IProgram program, bool bringToFront = false)
        {
            if (program != null)
            {
                var blockView = new BlockView.BlockView(program);
                var taskItem = new TaskItem(blockView, this);
                var fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(600)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

                _ = Wrap_Tasks.Children.Add(taskItem);

                if (bringToFront)
                {
                    BringToFront(taskItem);
                }
                else
                {
                    taskItem.BeginAnimation(OpacityProperty, fadeInAnimation);
                }
            }
        }

        public void ShowTasks()
        {
            foreach (var child in Wrap_Tasks.Children)
            {
                if (child is TaskItem taskItem && taskItem.BlockView == CurrentBlockView)
                {
                    taskItem.UpdateSnapshot();
                }
            }

            var fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };
            var fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };
            var contractAnimation = new DoubleAnimation(1, 0.1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

            fadeOutAnimation.Completed += (sender, e) => Grid_CurrentHost.Visibility = Visibility.Hidden;

            Grid_CurrentHost.BeginAnimation(OpacityProperty, fadeOutAnimation);

            Border_ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, contractAnimation);
            Border_ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, contractAnimation);

            Grid_Tasks.Visibility = Visibility.Visible;
            Grid_Tasks.BeginAnimation(OpacityProperty, fadeInAnimation);

            if (Sidebar != null)
            {
                Sidebar.BlockView = null;
            }
        }

        public void BringToFront(ITaskItem taskItem)
        {
            if (taskItem?.BlockView != null)
            {
                BringToFront(taskItem.BlockView);
            }
        }

        public void BringToFront(IBlockView blockView)
        {
            if (blockView != null && blockView is UIElement uiElement)
            {
                Grid_CurrentHost.Visibility = Visibility.Visible;

                Border_CurrentHost.Child = uiElement;

                if (_blockView != null)
                {
                    var fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };
                    var fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };
                    var expandAnimation = new DoubleAnimation(0.1, 1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

                    fadeOutAnimation.Completed += (sender, e) => Grid_Tasks.Visibility = Visibility.Hidden;

                    Border_ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, expandAnimation);
                    Border_ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, expandAnimation);
                    Grid_CurrentHost.BeginAnimation(OpacityProperty, fadeInAnimation);
                    Grid_Tasks.BeginAnimation(OpacityProperty, fadeOutAnimation);
                }
                else
                {
                    Grid_Tasks.Visibility = Visibility.Hidden;
                    Grid_CurrentHost.Opacity = 1;
                }

                _blockView = blockView;

                if (Sidebar != null)
                {
                    Sidebar.BlockView = blockView;
                }
            }
        }

        public bool RemoveTask(ITaskItem task)
        {
            if (task != null)
            {
                foreach (UIElement child in Wrap_Tasks.Children)
                {
                    if (child is ITaskItem item && item == child)
                    {
                        var program = task?.BlockView?.AssociatedProgram;
                        if (program != null)
                        {
                            program.Stop();
                        }

                        Wrap_Tasks.Children.Remove(child);

                        return true;
                    }
                }
            }

            return false;
        }

        private void BlockViewChanged(IBlockView blockView)
        {
            BringToFront(blockView);
        }

        private void Button_NewTask_Click(object sender, RoutedEventArgs e)
        {
            CreateNewTask();
        }
    }
}
