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

        public void CreateNewTask()
        {
            CreateNewTask(ProgramHolder.Instance.NewProgram());
        }

        public void CreateNewTask(IProgram program)
        {
            if (program != null)
            {
                var blockView = new BlockView.BlockView(program);
                var taskItem = new TaskItem(blockView, this);

                _ = Stack_Tasks.Children.Add(taskItem);

                BringToFront(taskItem);
            }
        }

        public void ShowTasks()
        {
            foreach (var child in Stack_Tasks.Children)
            {
                if (child is TaskItem taskItem && taskItem.BlockView == CurrentBlockView)
                {
                    taskItem.UpdateSnapshot();
                }
            }

            var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };
            var fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };
            var contractAnimation = new DoubleAnimation(0.1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

            fadeOutAnimation.Completed += (sender, e) =>
            {
                Border_CurrentHost.Visibility = Visibility.Hidden;
            };

            Border_CurrentHost.BeginAnimation(OpacityProperty, fadeOutAnimation);

            Border_ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, contractAnimation);
            Border_ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, contractAnimation);

            Scroll_Tasks.Visibility = Visibility.Visible;
            Scroll_Tasks.Opacity = 0;
            Scroll_Tasks.BeginAnimation(OpacityProperty, fadeInAnimation);
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
                Border_CurrentHost.Visibility = Visibility.Visible;
                Border_CurrentHost.Child = uiElement;
                Border_CurrentHost.Opacity = 0;

                if (_blockView != null)
                {
                    var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };
                    var fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };
                    var expandAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

                    fadeOutAnimation.Completed += (sender, e) =>
                    {
                        Scroll_Tasks.Visibility = Visibility.Hidden;
                        Scroll_Tasks.Opacity = 1;

                        Border_CurrentHost.Visibility = Visibility.Visible;
                        Border_CurrentHost.Opacity = 1;
                    };

                    Border_ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, expandAnimation);
                    Border_ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, expandAnimation);
                    Border_CurrentHost.BeginAnimation(OpacityProperty, fadeInAnimation);
                    Scroll_Tasks.BeginAnimation(OpacityProperty, fadeOutAnimation);
                }
                else
                {
                    Scroll_Tasks.Visibility = Visibility.Hidden;
                    Border_CurrentHost.Opacity = 1;
                }

                _blockView = blockView;

                if (Sidebar != null)
                {
                    Sidebar.BlockView = blockView;
                }
            }
        }

        private void BlockViewChanged(IBlockView blockView)
        {
            BringToFront(blockView);
        }
    }
}
